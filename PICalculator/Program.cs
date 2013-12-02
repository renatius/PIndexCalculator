using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using PICalculator.Model.Application;
using DevExpress.Skins;
using DevExpress.UserSkins;

namespace PICalculator
{
    static class Program
    {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SkinManager.EnableFormSkins();
            BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            var theApp = new CalculatorApplication();
            Application.Run(new MainForm(theApp));
        }
    }
}
