using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace TOOL_TW_DDN
{
    public class TwcaGenerator
    {
        private readonly RichTextBox logTextBox;
        private readonly ProgressBar progressBar;
        private readonly string xmlHeader = XmlConstants.XmlHeader;
        private readonly string xmlFooter = XmlConstants.XmlFooter;

        public TwcaGenerator(RichTextBox logTextBox, ProgressBar progressBar = null)
        {
            this.logTextBox = logTextBox;
            this.progressBar = progressBar;
        }

        public void Generate(string dataPath, string xmlPath, string resultsDir, string dumpFolderPath)
        {
            string csvPath = Path.Combine(dataPath, "TWCELLDATA.csv");
            string twcaPath = Path.Combine(xmlPath, "twca");

            if (!File.Exists(csvPath))
            {
                logTextBox.AppendText($"Lỗi: Không tìm thấy file {Path.GetFileName(csvPath)}!\n");
                return;
            }

            DataTable csvData = CsvLoader.LoadCsv(csvPath, ',', logTextBox);
            if (csvData.Rows.Count == 0)
            {
                logTextBox.AppendText("Lỗi: File CSV không có dữ liệu!\n");
                return;
            }

            DisplayCsvData(csvData, "TWCELLDATA.csv");

            var uniqueMrbts = csvData.AsEnumerable()
                .Select(row => row["txtmrbts"]?.ToString()?.Trim())
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct()
                .ToList();

            if (progressBar != null)
            {
                progressBar.Maximum = uniqueMrbts.Count + csvData.Rows.Count + 4; // 124 + 702 + 4 = 830
                progressBar.Value = 0;
            }

            string[] xmlFiles = Directory.GetFiles(twcaPath, "*.xml");
            string siteTemplate = xmlFiles.FirstOrDefault(f => Path.GetFileName(f).StartsWith("site"));
            if (string.IsNullOrEmpty(siteTemplate))
            {
                logTextBox.AppendText("Lỗi: Không tìm thấy file site*.xml trong thư mục twca!\n");
                return;
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string outputXmlPath = Path.Combine(resultsDir, $"plan_twca_{timestamp}.xml");

            using (StreamWriter writer = new StreamWriter(outputXmlPath, false))
            {
                writer.WriteLine(xmlHeader);

                string siteContent = File.ReadAllText(siteTemplate);
                foreach (string mrbts in uniqueMrbts)
                {
                    writer.WriteLine(siteContent.Replace("txtmrbts", mrbts));
                    if (progressBar != null) progressBar.Value++;
                }

                foreach (DataRow row in csvData.Rows)
                {
                    string f4g = row["f4g"]?.ToString()?.Trim() ?? "";
                    string txtmrbts = row["txtmrbts"]?.ToString()?.Trim() ?? "";
                    string txtlncel = row["txtlncel"]?.ToString()?.Trim() ?? "";

                    string lncelFile = xmlFiles.FirstOrDefault(f => Path.GetFileName(f).StartsWith("lncel_") && Path.GetFileName(f).EndsWith($"_l{f4g}.xml"))
                        ?? xmlFiles.FirstOrDefault(f => Path.GetFileName(f).StartsWith("lncel_") && Path.GetFileName(f).EndsWith("_l1501.xml"));
                    if (string.IsNullOrEmpty(lncelFile))
                    {
                        logTextBox.AppendText("Lỗi: Không tìm thấy file mặc định lncel_*_l1501.xml trong thư mục twca!\n");
                        return;
                    }

                    string lncelContent = File.ReadAllText(lncelFile)
                        .Replace("txtmrbts", txtmrbts)
                        .Replace("txtlncel", txtlncel);
                    writer.WriteLine(lncelContent);
                    if (progressBar != null) progressBar.Value++;
                }

                writer.WriteLine(xmlFooter);
            }

            logTextBox.AppendText($"\nFile XML đã được tạo tại: {outputXmlPath}\n");

            var caProcessor = new CaProcessor(logTextBox);

            // Xử lý CAPR
            caProcessor.ProcessCaPairs(
                dataPath, dumpFolderPath, twcaPath, resultsDir, timestamp, "CAPR",
                "Export_CAPR__*.txt", "capr_del.xml", "capr_cre.xml",
                (row1, row2) => row1["txtmrbts"]?.ToString() == row2["txtmrbts"]?.ToString() &&
                                row1["f4g"]?.ToString() != row2["f4g"]?.ToString()
            );
            if (progressBar != null) progressBar.Value++;

            // Xử lý IRFIM
            caProcessor.ProcessCaPairs(
                dataPath, dumpFolderPath, twcaPath, resultsDir, timestamp, "IRFIM",
                "Export_IRFIM__*.txt", "irfim_del.xml", "irfim_cre_l1501.xml",
                (row1, row2) => row1["txtmrbts"]?.ToString() == row2["txtmrbts"]?.ToString() &&
                                row1["f4g"]?.ToString() != row2["f4g"]?.ToString(),
                f4g => File.Exists(Path.Combine(twcaPath, $"irfim_cre_l{f4g}.xml")) ? $"irfim_cre_l{f4g}.xml" : "irfim_cre_l1501.xml"
            );
            if (progressBar != null) progressBar.Value++;

            // Xử lý LNHOIF
            caProcessor.ProcessCaPairs(
                dataPath, dumpFolderPath, twcaPath, resultsDir, timestamp, "LNHOIF",
                "Export_LNHOIF__*.txt", "lnhoif_del.xml", "lnhoif_cre_l1501.xml",
                (row1, row2) => row1["txtmrbts"]?.ToString() == row2["txtmrbts"]?.ToString() &&
                                row1["f4g"]?.ToString() != row2["f4g"]?.ToString(),
                f4g => File.Exists(Path.Combine(twcaPath, $"lnhoif_cre_l{f4g}.xml")) ? $"lnhoif_cre_l{f4g}.xml" : "lnhoif_cre_l1501.xml",
                "lnhoif_upd_l1501.xml",
                f4g => File.Exists(Path.Combine(twcaPath, $"lnhoif_upd_l{f4g}.xml")) ? $"lnhoif_upd_l{f4g}.xml" : "lnhoif_upd_l1501.xml"
            );
            if (progressBar != null) progressBar.Value++;

            // Xử lý CAREL
            caProcessor.ProcessCaPairs(
                dataPath, dumpFolderPath, twcaPath, resultsDir, timestamp, "CAREL",
                "Export_CAREL__*.txt", "carel_del.xml", "carel_cre.xml"
            );
            if (progressBar != null) progressBar.Value++;

            if (progressBar != null) progressBar.Value = progressBar.Maximum; // Đảm bảo đạt tối đa
        }

        private void DisplayCsvData(DataTable data, string fileName)
        {
            var uniqueMrbtsCount = data.AsEnumerable()
                .Select(row => row.Field<string>("txtmrbts"))
                .Distinct()
                .Count();
            var lncelCount = data.Rows.Count;

            logTextBox.Clear();
            logTextBox.AppendText($"Dữ liệu CSV ({fileName}): {uniqueMrbtsCount} MRBTS, {lncelCount} LNCEL\n");
        }
    }
}