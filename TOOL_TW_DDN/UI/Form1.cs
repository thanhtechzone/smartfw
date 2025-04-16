using System;
using System.IO;
using System.Windows.Forms;

namespace TOOL_TW_DDN
{
    public partial class Form1 : Form
    {
        private string dumpFolderPath;

        public Form1()
        {
            InitializeComponent();

        }

        private void BtnSelectDumpFolder_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Chọn thư mục chứa file dump 4G";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    dumpFolderPath = folderDialog.SelectedPath;
                    logTextBox.AppendText($"Đã chọn thư mục dump: {dumpFolderPath}\n");
                }
                else
                {
                    logTextBox.AppendText("Chưa chọn thư mục dump!\n");
                }
            }
        }

        private void BtnOpenResultsFolder_Click(object sender, EventArgs e)
        {
            string resultsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Results");
            if (Directory.Exists(resultsDir))
            {
                System.Diagnostics.Process.Start("explorer.exe", resultsDir);
            }
            else
            {
                logTextBox.AppendText("Lỗi: Thư mục Results không tồn tại!\n");
            }
        }

        private void GenerateXml(object sender, EventArgs e)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string dataPath = Path.Combine(basePath, "DATA");
                string xmlPath = Path.Combine(basePath, "xml");
                string resultsDir = Path.Combine(basePath, "Results");

                if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
                if (!Directory.Exists(xmlPath)) Directory.CreateDirectory(xmlPath);
                if (!Directory.Exists(resultsDir)) Directory.CreateDirectory(resultsDir);

                string selectedTemplate = templateComboBox.SelectedItem.ToString();

                if (selectedTemplate.Contains("TWCA"))
                {
                    if (string.IsNullOrEmpty(dumpFolderPath) || !Directory.Exists(dumpFolderPath))
                    {
                        logTextBox.AppendText("Lỗi: Vui lòng chọn thư mục dump 4G trước khi chạy TWCA!\n");
                        return;
                    }
                    new TwcaGenerator(logTextBox, progressBar).Generate(dataPath, xmlPath, resultsDir, dumpFolderPath);
                }
                else if (selectedTemplate.Contains("AddCell"))
                {
                    new AddCellGenerator(logTextBox).Generate(selectedTemplate, dataPath, xmlPath, resultsDir);
                }
                else if (selectedTemplate.Contains("ENDC"))
                {
                    new EndcGenerator(logTextBox).Generate(selectedTemplate, dataPath, xmlPath, resultsDir);
                }
                else
                {
                    logTextBox.AppendText($"Lỗi: Template '{selectedTemplate}' chưa được hỗ trợ!\n");
                }
            }
            catch (Exception ex)
            {
                logTextBox.AppendText("Lỗi: " + ex.Message + "\n");
            }
        }

        // Giữ nguyên phần InitializeComponent() từ code của bạn
    }
}