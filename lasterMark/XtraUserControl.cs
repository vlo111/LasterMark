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

    using Newtonsoft.Json;

    public partial class XtraUserControl : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly IList<Data> _data;

        private CompetitorListApi _competitorListApi;

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

            var task = await this.GetAllCompetitorsAsync($@"http://openeventor.ru/api/event/{data.Token}/get_competitors");

            this._competitorListApi = JsonConvert.DeserializeObject<CompetitorListApi>(task);

            this.listView1.Columns.AddRange(
                new ColumnHeader[]
                    {
                        new ColumnHeader() { Text = @"Bib", Width = 50 },
                        new ColumnHeader() { Text = @"First Name", Width = 150 },
                        new ColumnHeader() { Text = @"Last Name", Width = 150 },
                        new ColumnHeader() { Text = @"Birth year", Width = 100 }
                    });

            this._competitorListApi.Competitors.ForEach(
                p =>
                    {
                        this.listView1.Items.Add(
                            new ListViewItem(new[] { p.Bib, p.FirstName, p.LastName, p.BirthYear }));
                    });
        }

        #region Searc event

        private void KeyBtns_Click(object sender, EventArgs e)
        {
            this.EventKeyClick(sender);
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
                if (string.IsNullOrEmpty(this.searchControl.Text))
                {
                    return;
                }

                this.searchControl.Text = this.searchControl.Text.Remove(this.searchControl.Text.Length - 1);
            }
            else
            {
                this.searchControl.Text = this.searchControl.Text + btn.Text;
            }

            this.listBoxControl1.Items.Clear();

            Data[] search = this._data.Where(p => p.Text.ToLower().Contains(this.searchControl.Text.Trim().ToLower()))
                .ToArray();

            this.listBoxControl1.DataSource = search;
        }

        private void EventKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                if (string.IsNullOrEmpty(this.searchControl.Text))
                {
                    return;
                }

                var search = this._data.Where(
                        p => p.Text.ToLower().Contains(
                            this.searchControl.Text.Remove(this.searchControl.Text.Length - 1).Trim().ToLower()))
                    .Select(p => p).ToArray();

                this.listBoxControl1.Items.Clear();
                this.listBoxControl1.DataSource = search;
            }
            else
            {
                if ((e.KeyChar >= 'а' && e.KeyChar <= 'я') || (e.KeyChar >= 'А' && e.KeyChar <= 'Я'))
                {
                    var search = this._data
                        .Where(p => p.Text.ToLower().Contains(this.searchControl.Text.Trim().ToLower())).ToArray();

                    this.listBoxControl1.Items.Clear();
                    this.listBoxControl1.DataSource = search;
                }
            }
        }

        #endregion
    }
}