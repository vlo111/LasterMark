namespace lasterMark
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using DataAccess;
    using DataAccess.DTOs;
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

        private readonly string currentPath = Application.StartupPath;

        private Image _ezdOriginalImage;

        private int _ezdOriginalImageMaxSize;

        private Image _bgOriginalImage;

        private int _bgOriginalImageMaxSize;

        private bool scale_bg = false;

        private UserFileRepository userFileRepository;

        private User currentUser;

        private UserFiles currentUserFile;

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

        #region Login
        
        private async void LogInBtn_Click(object sender, EventArgs e)
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
                try
                {
                    var login = this.loginInput.Text;

                    var password = this.passwordInput.Text;

                    if (currentUser !=null)
                    {
                        if(currentUser.Password == password && currentUser.Login == login)
                        {
                            XtraMessageBox.Show($@"Вы уже вошли в систему как - {login}", "Information", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    // var user = new User();

                    var user = UserRepository.GetUser(this.loginInput.Text, this.passwordInput.Text);

                    if (user == null)
                    {
                        user = new User
                        {
                            Login = this.loginInput.Text,
                            Password = this.passwordInput.Text
                        };

                        UserRepository.Insert(user);

                        user = UserRepository.GetUser(this.loginInput.Text, this.passwordInput.Text);
                    }

                    currentUser = user;

                    XtraMessageBox.Show("Вы успешно вошли в систему", "Success", MessageBoxButtons.OK);

                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Неверный логин или пароль", "Error", MessageBoxButtons.OK);
                }
            }
        }

        #endregion

        #region Update ezd

        private async void UpdateEzdOpenDialogBtn_Click(object sender, EventArgs e)
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
                    if (currentUser != null)
                    {
                        // Get all events
                        var base64HeaderValue = Convert.ToBase64String(
                            System.Text.Encoding.UTF8.GetBytes($@"{this.loginInput.Text}:{this.passwordInput.Text}"));

                        const string Url = @"http://openeventor.ru/api/get_events";

                        var task = await RequestData.GetAllEventsAsync(Url, base64HeaderValue);

                        var result = JsonConvert.DeserializeObject<EventorApi>(task);

                        // it will inaitailize this variable {CompetitorApi Competitor}
                        CustomFlyoutDialog.ShowForm(this, null, new XtraUserControl(result));

                        // Update via this.Competitor
                        this.UpdateImage();
                    }
                    else
                    {
                        XtraMessageBox.Show("Пожалуйста войдите в систему", "Error", MessageBoxButtons.OK);
                    }

                }
            }
            else
            {
                XtraMessageBox.Show("Пожалуйста, выберите ezd файл", "Error", MessageBoxButtons.OK);
            }
        }

        private async void UpdateUrl_Click(object sender, EventArgs e)
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
                        if (currentUser != null)
                        {
                            var url =
                                @"http://openeventor.ru/api/event/262a9ea6af4e459c92656a62b3db5cb4/engraver/get?bib=123";

                            var task = await RequestData.GetRequestAsync(url);

                            var result = JsonConvert.DeserializeObject<CurrentCompotitorApi>(task);

                            // Update via CurrentCompotitorApi
                            UpdateImage(result);
                        }
                        else
                        {
                            XtraMessageBox.Show("Пожалуйста войдите в систему", "Error", MessageBoxButtons.OK);
                        }
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

        #endregion

        private void saveDataBtn_Click(object sender, EventArgs e)
        {
            if (currentUser != null)
            {
                Bitmap bm = this.PanelToImage(this.panelControl1);

                currentUserFile.BackgroundImageData = ImageToByteArray(this._bgOriginalImage);

                currentUserFile.EzdImageData = ImageToByteArray(this._ezdOriginalImage);

                currentUserFile.ReadyMadeImageData = ImageToByteArray(bm);

                userFileRepository.Insert(currentUserFile);

                UpdateImage();
            }
            else
            {
                XtraMessageBox.Show("Сначала войдите в систему", "Error", MessageBoxButtons.OK);
            }
        }

        #region CustomMethods

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
                    }
                }
            }
        }

        private void LoadImage(string fileName)
        {
            // load ezd
            JczLmc.LoadEzdFile(fileName);

            // get image from sdk
            var img = JczLmc.GetCurPreviewImage(this.foregroundPictureBox.Width, this.foregroundPictureBox.Height);

            img = SetImageTransparent(img);


            this.foregroundPictureBox.Image = img;

            this.foregroundPictureBox.Width = img.Width;
            this.foregroundPictureBox.Height = img.Height;

            this._ezdOriginalImage = img;

            this.ezdFileLbl.Text = Path.GetFileName(fileName);
        }

        private void UpdateImage()
        {
            if (Competitor != null)
            {
                UpdateImage(new CurrentCompotitorApi
                {
                    Competitor = new CompetitorApi
                    {
                        FirstName = Competitor.FirstName,
                        LastName = Competitor.LastName,
                        TimeOfDistance = Competitor.TimeOfDistance,
                        Distance = Competitor.Distance
                    }
                });
            }

            Competitor = null;
        }

        private void UpdateImage(CurrentCompotitorApi compotitorApi)
        {
            var compotitor = compotitorApi.Competitor;

            if (!string.IsNullOrEmpty(compotitor.FirstName))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(0), compotitor.FirstName);
            }

            if (!string.IsNullOrEmpty(compotitor.LastName))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(1), compotitor.LastName);
            }

            if (!string.IsNullOrEmpty(compotitor.TimeOfDistance))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(2), compotitor.TimeOfDistance);
            }

            if (!string.IsNullOrEmpty(compotitor.Distance))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(3), compotitor.Distance);
            }

            var img = JczLmc.GetCurPreviewImage(this.foregroundPictureBox.Width, this.foregroundPictureBox.Height);

            img = SetImageTransparent(img);

            this.currentUserFile = new UserFiles
            {
                UserId = this.currentUser.Id,
                EzdImageData = ImageToByteArray(img),
                ReadyMadeImageData = ImageToByteArray(PanelToImage(this.panelControl1))
            };

            this.foregroundPictureBox.Image = img;
        }

        private Bitmap SetImageTransparent(Image image)
        {
            var bitmap = (Bitmap)image;

            bitmap.MakeTransparent();

            return bitmap;
        }

        private byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private Bitmap PanelToImage(PanelControl control)
        {
            int width = control.Size.Width;
            int height = control.Size.Height;

            var bmp = new Bitmap(width, height);
            control.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));

            return bmp;
        }

        #endregion
    }
}