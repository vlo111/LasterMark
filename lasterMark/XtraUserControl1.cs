namespace lasterMark
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows.Forms;

    using lasterMark.Model;

    using Newtonsoft.Json;

    public class Data
    {
        public int Value { get; set; }

        public string Token { get; set; }

        public string Text { get; set; }
    }

    /// <summary>
    /// The xtra user control 1.
    /// </summary>
    public partial class XtraUserControl1 : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly IList<Data> _data;

        public XtraUserControl1(EventorDto dto)
        {
            this.InitializeComponent();

            this._data = dto.Events.Select(p => new Data { Token = p.Token, Value = p.Id, Text = p.Name }).ToList();

            if (dto.Events != null)
            {
                this.listBoxControl1.DataSource = this._data;
                this.listBoxControl1.DisplayMember = "Text";
            }
        }

        private void KeyBtns_Click(object sender, EventArgs e)
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

            Data[] search = this._data.Where(p => p.Text.ToLower().Contains(this.searchControl.Text.Trim().ToLower())).ToArray();

            this.listBoxControl1.Items.AddRange(search);
        }

        private void searchControl_KeyPress(object sender, KeyPressEventArgs e)
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
                this.listBoxControl1.Items.AddRange(search);
            }
            else
            {
                if ((e.KeyChar >= 'а' && e.KeyChar <= 'я') || (e.KeyChar >= 'А' && e.KeyChar <= 'Я'))
                {
                    var search = this._data.Where(p => p.Text.ToLower().Contains(this.searchControl.Text.Trim().ToLower()))
                        .ToArray();

                    this.listBoxControl1.Items.Clear();
                    this.listBoxControl1.Items.AddRange(search);
                }
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            var s = this.listBoxControl1.SelectedItem;

            CompetitorListDto result = new CompetitorListDto();

            var uri = @"http://openeventor.ru/api/get_events";

            // Create a request for the URL.
            var request = WebRequest.Create(uri);

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var response = request.GetResponse())
            {
                var status = ((HttpWebResponse)response).StatusDescription;

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream ?? throw new InvalidOperationException());

                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    // the content response.
                    result = JsonConvert.DeserializeObject<CompetitorListDto>(responseFromServer);
                }
            }

        }
    }
}