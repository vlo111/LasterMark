namespace lasterMark
{
    using System.Drawing;

    public class PictureControl
    {
        // The Scale. Reduce image size
        public static Image Scale(Image img, Size size)
        {
            int width = img.Width - (img.Width * size.Width / 100);
            int heigth = img.Height - (img.Height * size.Height / 100);

            Bitmap bmp = new Bitmap(img, width, heigth);

            Graphics graphics = Graphics.FromImage(bmp);

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            return bmp;
        }

        public static Image Zoom(Image img, Size size)
        {
            int width = img.Width + (img.Width * size.Width / 100);
            int heigth = img.Height + (img.Height * size.Height / 100);

            Bitmap bmp = new Bitmap(img, width, heigth);

            Graphics graphics = Graphics.FromImage(bmp);

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            return bmp;
        }
    }
}