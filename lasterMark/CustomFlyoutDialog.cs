namespace lasterMark
{
    using System.Windows.Forms;

    using DevExpress.XtraBars.Docking2010.Customization;
    using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
    
    class CustomFlyoutDialog : FlyoutDialog
    {
        public CustomFlyoutDialog(Form owner, FlyoutAction actions, Control userControlToShowControl)
            : base(owner, actions)
        {
            this.Properties.HeaderOffset = 0;
            this.Properties.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.Properties.Style = FlyoutStyle.Popup;
            this.FlyoutControl = userControlToShowControl;
        }

        public static DialogResult ShowForm(Form owner, FlyoutAction actions, Control userControlToShowControl)
        {
            CustomFlyoutDialog customFlyoutDialog = new CustomFlyoutDialog(owner, actions, userControlToShowControl);
            return customFlyoutDialog.ShowDialog();
        }
    }
}