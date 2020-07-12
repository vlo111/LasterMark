namespace lasterMark
{
    using System.Drawing;

    public class PictureControl
    {
        public static int ColculateTrackBarMaxSize(int width, int height)
        {
            var maxWidth = 0;
            var maxHeight = 0;

            for (int i = 0; i < width; i++)
            {
                if (width - (width * i / 100) == 0)
                {
                    maxWidth = i;
                }
            }

            for (int i = 0; i < height; i++)
            {
                if (height - (height * i / 100) == 0)
                {
                    maxHeight = i;
                }
            }

            return maxWidth >= maxHeight ? maxWidth - 1 : maxHeight - 1;
        }

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