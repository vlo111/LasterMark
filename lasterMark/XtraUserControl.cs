using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using lasterMark.ApiModel;

namespace lasterMark
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using DevExpress.Utils.Extensions;

    using Newtonsoft.Json;

    public partial class XtraUserControl : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly IList<Data> _data;

        private CompetitorListApi _competitorListApi;

        // event true
        // competitor false
        private bool eventOrCompetitorSearch = true;

        public XtraUserControl(EventorApi api)
        {
            this.InitializeComponent();

            this._data = api.Events.Select(p => new Data { Token = p.Token, Value = p.Id, Text = p.Name }).ToList();

            if (api.Events != null)
            {
                this.listBoxControl1.DataSource = this._data;
                this.listBoxControl1.DisplayMember = "Text";
            }
        }

        #region Get all Competitors

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(responseStream ?? throw new InvalidOperationException()))
                {
                    string strContent = sr.ReadToEnd();
                    return strContent;
                }
            }
        }

        private async Task<string> GetAllCompetitorsAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return await task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        #endregion

        private async void SubmitEventBtn_Click(object sender, EventArgs e)
        {
            var data = (Data)this.listBoxControl1.SelectedItem;

            if (data != null)
            {
                var task = await this.GetAllCompetitorsAsync(
                               $@"http://openeventor.ru/api/event/{data.Token}/get_competitors");

                this._competitorListApi = JsonConvert.DeserializeObject<CompetitorListApi>(task);

                this.selectetEventLbl.Text = data.Text;
            }
        }

        #region Searc event

        private void KeyBtns_Click(object sender, EventArgs e)
        {
            if (this.eventOrCompetitorSearch)
            {
                this.EventKeyClick(sender);
            }
            else
            {
                var btn = (DevExpress.XtraEditors.SimpleButton)sender;
                if (btn.Text == "<")
                {
                    if (string.IsNullOrEmpty(this.searchEventControl.Text))
                    {
                        return;
                    }

                    this.searchControlCompetitor.Text = this.searchControlCompetitor.Text.Remove(this.searchControlCompetitor.Text.Length - 1);
                }
                else
                {
                    this.searchControlCompetitor.Text = this.searchControlCompetitor.Text + btn.Text;
                }
            }
        }

        private void SearchControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.EventKeyPress(e);
        }

        private void EventKeyClick(object sender)
        {
            var btn = (DevExpress.XtraEditors.SimpleButton)sender;
            if (btn.Text == "<")
            {
                if (string.IsNullOrEmpty(this.searchEventControl.Text))
                {
                    return;
                }

                this.searchEventControl.Text = this.searchEventControl.Text.Remove(this.searchEventControl.Text.Length - 1);
            }
            else
            {
                this.searchEventControl.Text = this.searchEventControl.Text + btn.Text;
            }

            this.listBoxControl1.Items.Clear();

            Data[] search = this._data.Where(p => p.Text.ToLower().Contains(this.searchEventControl.Text.Trim().ToLower()))
                .ToArray();

            this.listBoxControl1.DataSource = search;
        }

        private void EventKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                if (string.IsNullOrEmpty(this.searchEventControl.Text))
                {
                    return;
                }

                var search = this._data.Where(
                        p => p.Text.ToLower().Contains(
                            this.searchEventControl.Text.Remove(this.searchEventControl.Text.Length - 1).Trim().ToLower()))
                    .Select(p => p).ToArray();

                this.listBoxControl1.Items.Clear();
                this.listBoxControl1.DataSource = search;
            }
            else
            {
                if ((e.KeyChar >= 'а' && e.KeyChar <= 'я') || (e.KeyChar >= 'А' && e.KeyChar <= 'Я'))
                {
                    var search = this._data
                        .Where(p => p.Text.ToLower().Contains(this.searchEventControl.Text.Trim().ToLower())).ToArray();

                    this.listBoxControl1.Items.Clear();
                    this.listBoxControl1.DataSource = search;
                }
            }
        }

        #endregion

        private void GetCompetitorBtn_Click(object sender, EventArgs e)
        {
            var search = this.searchControlCompetitor.Text;

            if (string.IsNullOrEmpty(search))
            {
                return;
            }

            this.listView1.Items.Clear();

            this._competitorListApi.Competitors.Where(
                p => p.Bib == search 
                     || (p.FirstName != null && p.FirstName.ToLower().Contains(search.Trim().ToLower()))
                     || (p.LastName != null && p.LastName.ToLower().Contains(search.Trim().ToLower()))
                     || (p.BirthYear != null && p.BirthYear.ToLower().Contains(search.Trim().ToLower()))).ForEach(
                p =>
                    {
                        this.listView1.Items.Add(
                            new ListViewItem(new[] { p.Bib, p.FirstName, p.LastName, p.BirthYear }));
                    });
        }

        private void XtraUserControl_Load(object sender, EventArgs e)
        {
            this.listView1.Columns.AddRange(
                new ColumnHeader[]
                    {
                        new ColumnHeader() { Text = @"Bib", Width = 50 },
                        new ColumnHeader() { Text = @"First Name", Width = 150 },
                        new ColumnHeader() { Text = @"Last Name", Width = 150 },
                        new ColumnHeader() { Text = @"Birth year", Width = 100 }
                    });
        }

        private void ConfirmBtn_Click(object sender, EventArgs e)
        {
            var item = this.listView1.SelectedItems[0];
        }

        private void SearchEventControl_Enter(object sender, EventArgs e)
        {
            this.eventOrCompetitorSearch = true;
        }

        private void SearchControlCompetitor_Enter(object sender, EventArgs e)
        {
            this.eventOrCompetitorSearch = false;
        }
    }
}