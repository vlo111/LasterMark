namespace lasterMark
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    class CustomPictureBox : DevExpress.XtraEditors.PictureEdit
    {
        public CustomPictureBox(IContainer container)
        {
            container.Add(this);
        }

        Point point;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.point = e.Location;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - this.point.X;
                this.Top += e.Y - this.point.Y;
            }

            base.OnMouseMove(e);
        }

    }
}
