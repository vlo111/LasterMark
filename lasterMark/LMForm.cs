namespace lasterMark
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows.Forms;

    using DevExpress.XtraEditors;

    using lasterMark.Api;
    using lasterMark.ApiModel;

    using Newtonsoft.Json;

    /// <summary>
    /// The lm form.
    /// </summary>
    public partial class LMForm : XtraForm
    {
        #region Fields

        public static CompetitorApi Competitor;

        // @"C:\Users\Lifebeget\source\repos\lasterMark\lasterMark\bin\Debug";
        private readonly string currentPath = Application.StartupPath;

        private Image _ezdOriginalImage;

        private int _ezdOriginalImageMaxSize;

        private Image _bgOriginalImage;

        private int _bgOriginalImageMaxSize;

        private bool scale_bg = false;

        private enum UploadType
        {
            /// <summary>
            /// The image.
            /// </summary>
            image = 1,

            /// <summary>
            /// The ezd filter.
            /// </summary>
            Ezd
        }

        #endregion

        #region Inits

        public LMForm()
        {
            this.InitializeComponent();
        }

        private void LMForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Drag and drop
                this.foregroundPictureBox.AllowDrop = true;

                // Drop Down
                var index = 0;

                // Init forground image parent
                this.foregroundPictureBox.Parent = this.backgroundPictureBox;

                // Connect sdk
                var err = JczLmc.Initialize(this.currentPath, true);

                this.backgroundPictureBox.MaximumSize = new Size(ClientRectangle.Width, ClientRectangle.Height);
                this.foregroundPictureBox.MaximumSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

                if (err != 0)
                {
                    XtraMessageBox.Show($@"При инициализации возникла ошибка - {err}", "Error", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        #endregion

        #region Upload file

        private void EZDFileUpload_Click(object sender, EventArgs e)
        {
            this.Upload(UploadType.Ezd);
        }

        private void UploadBGBtn_Click(object sender, EventArgs e)
        {
            this.Upload(UploadType.image);
        }

        #endregion

        #region Drag and drop

        private void PictureBoxFG_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);

            if (data == null)
            {
                return;
            }

            if (data is string[] fileNames && fileNames.Length > 0)
            {
                this.LoadImage(fileNames[0]);
            }
        }

        private void PictureBoxFG_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        #endregion

        #region Login/Save

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // var showForm = new Engrave();

            // showForm.Show();
        }

        private async void LogInBtn_Click(object sender, EventArgs e)
        {
            if (this._ezdOriginalImage != null)
            {
                var first_name = JczLmc.GetEntityNameByIndex(0);

                if (string.IsNullOrEmpty(first_name))
                {
                    XtraMessageBox.Show(
                        "Пожалуйста, выберите соответствующий ezd файл, для изменение например таких полей:(first_name, last_name, time_on_distance, distance_name)",
                        "Error",
                        MessageBoxButtons.OK);
                }
                else
                {
                    if (string.IsNullOrEmpty(this.loginInput.Text))
                    {
                        XtraMessageBox.Show(
                            "The login and password field is required, please enter login",
                            "Warning",
                            MessageBoxButtons.OK);
                    }
                    else if (string.IsNullOrEmpty(this.passwordInput.Text))
                    {
                        XtraMessageBox.Show(
                            "The login and password field is required, please enter password",
                            "Warning",
                            MessageBoxButtons.OK);
                    }
                    else
                    {
                        var base64HeaderValue = Convert.ToBase64String(
                            System.Text.Encoding.UTF8.GetBytes("ya@lenev.ru:poster86"));

                        const string Url = @"http://openeventor.ru/api/get_events";

                        var task = await RequestData.GetAllEventsAsync(Url, base64HeaderValue);

                        var result = JsonConvert.DeserializeObject<EventorApi>(task);

                        CustomFlyoutDialog.ShowForm(this, null, new XtraUserControl(result));

                        this.UpdateImage();
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Пожалуйста, выберите ezd файл", "Error", MessageBoxButtons.OK);
            }
        }

        #endregion

        private void FastenBackgrountCheckEdit_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;

            if (check.Checked)
            {
                this.backgroundPictureBox.Dock = DockStyle.Fill;
            }
            else
            {
                this.backgroundPictureBox.Dock = DockStyle.None;
            }
        }

        private async void SubmitUrl_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._ezdOriginalImage != null)
                {
                    var first_name = JczLmc.GetEntityNameByIndex(0);

                    if (string.IsNullOrEmpty(first_name))
                    {
                        XtraMessageBox.Show(
                            "Пожалуйста, выберите соответствующий ezd файл, для изменение например таких полей:(first_name, last_name, time_on_distance, distance_name)",
                            "Error",
                            MessageBoxButtons.OK);
                    }
                    else
                    {
                        var url =
                            @"http://openeventor.ru/api/event/262a9ea6af4e459c92656a62b3db5cb4/engraver/get?bib=123";

                        var task = await RequestData.GetRequestAsync(url);

                        var result = JsonConvert.DeserializeObject<CurrentCompotitorApi>(task);

                        var firstObj = JczLmc.GetEntityNameByIndex(0);

                        var secondObj= JczLmc.GetEntityNameByIndex(1);

                        var thirdObj= JczLmc.GetEntityNameByIndex(2);

                        var lastObj = JczLmc.GetEntityNameByIndex(3);

                        if (!string.IsNullOrEmpty(result.Competitor.FirstName))
                        {
                            JczLmc.ChangeTextByName(firstObj, result.Competitor.FirstName);
                        }

                        if (!string.IsNullOrEmpty(result.Competitor.FirstName))
                        {
                            JczLmc.ChangeTextByName(secondObj, result.Competitor.LastName);
                        }

                        if (!string.IsNullOrEmpty(result.Competitor.TimeOfDistance))
                        {
                            JczLmc.ChangeTextByName(thirdObj, result.Competitor.TimeOfDistance);
                        }

                        if (!string.IsNullOrEmpty(result.Competitor.Distance))
                        {
                            JczLmc.ChangeTextByName(lastObj, result.Competitor.Distance);
                        }

                        this.foregroundPictureBox.Image = JczLmc.GetCurPreviewImage(this.foregroundPictureBox.Width, this.foregroundPictureBox.Height);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Пожалуйста, выберите ezd файл", "Error", MessageBoxButtons.OK);
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void Upload(UploadType type)
        {
            // take filter type
            var filter = type == UploadType.Ezd
                             ? @"EZD file (*.ezd) | *.ezd"
                             : @"Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            using (var ofd = new OpenFileDialog { Multiselect = false, ValidateNames = true, Filter = filter })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // take BG|FG image from fileName
                    if (type == UploadType.Ezd)
                    {
                        try
                        {
                            this.LoadImage(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        var img = Image.FromFile(ofd.FileName);

                        if (img.Width > ClientRectangle.Width)
                        {
                            var residue = img.Width - ClientRectangle.Width;

                            int percent = (int)(decimal.Divide(residue, img.Width) * 100);

                            img = PictureControl.Scale(img, new Size(percent, percent));

                            this.scale_bg = true;
                        }
                        else
                        {
                            this.scale_bg = false;
                        }

                        if (img.Height > ClientRectangle.Height)
                        {
                            var residue = img.Height - ClientRectangle.Height;

                            var percent = (int)(decimal.Divide(residue, img.Height) * 100);

                            img = PictureControl.Scale(img, new Size(percent, percent));

                            this.scale_bg = true;
                        }

                        this.backgroundPictureBox.Image = img;

                        this.backgroundPictureBox.Width = img.Width;
                        this.backgroundPictureBox.Height = img.Height;

                        this._bgOriginalImage = img;

                        this.bgFileLbl.Text = Path.GetFileName(ofd.FileName);

                        this.InitialBGImage();
                    }
                }
            }
        }

        private void InitialFGImage()
        {
            this._ezdOriginalImageMaxSize = PictureControl.ColculateTrackBarMaxSize(
                this.foregroundPictureBox.Width,
                this.foregroundPictureBox.Height);
        }

        private void InitialBGImage()
        {
            this._bgOriginalImageMaxSize = PictureControl.ColculateTrackBarMaxSize(
                this.backgroundPictureBox.Width,
                this.backgroundPictureBox.Height);
        }

        private void LoadImage(string fileName)
        {
            // load ezd
            JczLmc.LoadEzdFile(fileName);

            // get image from sdk
            var img = JczLmc.GetCurPreviewImage(this.foregroundPictureBox.Width, this.foregroundPictureBox.Height);

            // convert to bitmap for rransparent
            var bitMap = (Bitmap)img;

            bitMap.MakeTransparent();

            this.foregroundPictureBox.Image = bitMap;

            this.foregroundPictureBox.Width = bitMap.Width;
            this.foregroundPictureBox.Height = bitMap.Height;

            this._ezdOriginalImage = bitMap;

            this.InitialFGImage();

            this.ezdFileLbl.Text = Path.GetFileName(fileName);
        }

        private void UpdateImage()
        {
            if (Competitor != null)
            {
            }

            Competitor = null;
        }
    }
}