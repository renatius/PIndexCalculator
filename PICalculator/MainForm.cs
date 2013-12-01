using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using FileHelpers;
using System.Diagnostics;
using PIndexCalculator.Model.Application;
using PIndexCalculator.Model.Exceptions;
using PIndexCalculator.Model.Output;

namespace PICalculator
{
    public partial class MainForm : XtraForm
    {
        private ICalculatorApplication theApp;

        public MainForm(ICalculatorApplication theApp) {
            InitializeComponent();

            this.theApp = theApp;

            string startingDir =
                Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            openFileDialog.InitialDirectory = startingDir;
            saveFileDialog.InitialDirectory = startingDir;
        }

        private void LoadPanel(string filename) {
            Cursor = Cursors.WaitCursor;
            try {
                BeforeLoadingPanelData();
                theApp.LoadCsvFile(filename);
                OnLoadPanelData();
            }
            catch (BusinessException exc) {
                XtraMessageBox.Show(
                    exc.Message,
                    "Error Loading File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally {
                Cursor = Cursors.Default;
            }
        }

        private void BeforeLoadingPanelData() {
            ClearControls();
        }

        private void ClearControls() {
            gridControlInputData.DataSource = null;
            gridControlPeople.DataSource = null;
            gridControlPovertyPersistanceRatios.DataSource = null;
            gridControlDatasetErrors.DataSource = null;
            gridControlPovertIndexes.DataSource = null;
        }

        private void OnLoadPanelData() {
            gridControlInputData.DataSource = theApp.Observations;
            gridControlPeople.DataSource = theApp.People;
            gridControlPovertyPersistanceRatios.DataSource = theApp.PovertyPersistenceRatios;
            gridControlDatasetErrors.DataSource = theApp.Errors;

            if (theApp.Errors.Count() > 0) {
                dockPanelErrors.Visibility = DockVisibility.Visible;

                XtraMessageBox.Show(
                    "The panel is not valid. Please fix errors in file and load it again",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else {
                dockPanelErrors.Visibility = DockVisibility.Hidden;
                LoadPovertyIndexes();
            }
        }

        private void LoadPovertyIndexes() {

            /* •—————————————————————————————————————————————————————————————————————————————————————————————————•
               | if (panelData.IsValid) {                                                              |
               |     double[] alphaValues = { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 }; |
               |     foreach (var alpha in alphaValues) {                                              |
               |         indexes.AddRange(panelData.CalculatePovertyIndex(alpha));                     |
               |     }                                                                                 |
               |     gridControlPovertIndexes.DataSource =                                             |
               |         indexes;                                                                      |
               | }                                                                                     |
               •———————————————————————————————————————————————————————————————————————————————————————• */
        }

        private void biExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            Close();
        }

        private void biLoadFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                LoadPanel(openFileDialog.FileName);
            }
        }

        private void biViewPanelErrors_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            dockPanelErrors.Visibility = DockVisibility.Visible;
        }

        private void biSavePovertyPersistenceRatios_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            saveFileDialog.FileName = "PPProbs.txt";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
                var writer = new FileHelperEngine<PovertyPersistenceRatio>();
                writer.WriteFile(saveFileDialog.FileName, theApp.PovertyPersistenceRatios);
            }
        }

        private void biSavePovertyIndexes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            /* •————————————————————————————————————————————————————————————————————————•
               | saveFileDialog.FileName = "PIndices.txt";                    |
               | if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {    |
               |     var writer = new FileHelperEngine<PovertyIndexResult>(); |
               |     writer.WriteFile(saveFileDialog.FileName, indexes);      |
               | }                                                            |
               •——————————————————————————————————————————————————————————————• */
        }

        private void menuExport_Popup(object sender, EventArgs e) {
            biSavePovertyIndexes.Enabled = false;
            biSavePovertyPersistenceRatios.Enabled = theApp.PovertyPersistenceRatios.Count() > 0;
        }

        private void biHelpAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            using (AboutBox aboutBox = new AboutBox()) {
                aboutBox.ShowDialog(this);
            }
        }

        private void biHelpReadme_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var dirname = Path.GetDirectoryName(Application.ExecutablePath);
            var readme = Path.Combine(dirname, @"Resources\readme.pdf");

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = readme;

            process.Start();
        }
    }
}