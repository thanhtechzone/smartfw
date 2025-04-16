using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace TOOL_TW_DDN
{
    public class AddCellGenerator
    {
        private readonly RichTextBox logTextBox;
        private readonly string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                                            "<!DOCTYPE raml SYSTEM 'raml20.dtd'>\n" +
                                            "<raml version=\"2.0\" xmlns=\"raml20.xsd\">\n" +
                                            "<cmData type=\"plan\">\n" +
                                            "<header>\n <log dateTime=\"\" action=\"created\" appInfo=\"PlanExporter\">UIValues are used</log>\n</header>";
        private readonly string xmlFooter = "</cmData>\n</raml>";

        public AddCellGenerator(RichTextBox logTextBox)
        {
            this.logTextBox = logTextBox;
        }

        public void Generate(string selectedTemplate, string dataPath, string xmlPath, string resultsDir)
        {
            string csvPath = Path.Combine(dataPath, "CELLDATA.csv");
            string xmlTemplatePath;

            switch (selectedTemplate)
            {
                case "AddCell_L1874":
                    xmlTemplatePath = Path.Combine(xmlPath, "addcell", "l1874.xml");
                    break;
                case "AddCell_L1874-IFHO":
                    xmlTemplatePath = Path.Combine(xmlPath, "addcell", "l1874_ifho.xml");
                    break;
                case "AddCell_L900":
                    xmlTemplatePath = Path.Combine(xmlPath, "addcell", "l900.xml");
                    break;
                case "AddCell_L900-IFHO":
                    xmlTemplatePath = Path.Combine(xmlPath, "addcell", "l900_ifho.xml");
                    break;
                default:
                    logTextBox.AppendText($"Lỗi: Template '{selectedTemplate}' không được hỗ trợ trong module AddCell!\n");
                    return;
            }

            if (!File.Exists(csvPath))
            {
                logTextBox.AppendText($"Lỗi: Không tìm thấy file {Path.GetFileName(csvPath)}!\n");
                return;
            }
            if (!File.Exists(xmlTemplatePath))
            {
                logTextBox.AppendText($"Lỗi: Không tìm thấy file mẫu {Path.GetFileName(xmlTemplatePath)}!\n");
                return;
            }

            DataTable csvData = CsvLoader.LoadCsv(csvPath);
            if (csvData.Rows.Count == 0)
            {
                logTextBox.AppendText("Lỗi: File CSV không có dữ liệu!\n");
                return;
            }

            DisplayCsvData(csvData, "CELLDATA.csv");

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string templateFileName = Path.GetFileNameWithoutExtension(xmlTemplatePath);
            string outputXmlPath = Path.Combine(resultsDir, $"plan_add_cell_{templateFileName}_{timestamp}.xml");

            using (StreamWriter writer = new StreamWriter(outputXmlPath, false))
            {
                writer.WriteLine(xmlHeader);
                string xmlContent = File.ReadAllText(xmlTemplatePath);

                foreach (DataRow row in csvData.Rows)
                {
                    string modifiedXmlContent = ReplaceXmlValues(xmlContent, row);
                    writer.WriteLine(modifiedXmlContent);
                }

                writer.WriteLine(xmlFooter);
            }

            logTextBox.AppendText($"\nFile XML đã được tạo tại: {outputXmlPath}\n");
        }

        private void DisplayCsvData(DataTable data, string fileName)
        {
            string[] columnNames = data.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
            int[] columnWidths = CalculateColumnWidths(columnNames, data);
            logTextBox.Clear();
            logTextBox.AppendText($"Dữ liệu CSV ({fileName}):\n");
            string header = FormatRowAsTable(columnNames, columnWidths);
            logTextBox.AppendText(header + "\n");
            logTextBox.AppendText(new string('-', header.Length - 4) + "\n");

            foreach (DataRow row in data.Rows)
            {
                string[] rowData = row.ItemArray.Select(item => item?.ToString() ?? "").ToArray();
                logTextBox.AppendText($"{FormatRowAsTable(rowData, columnWidths)}\n");
            }
        }

        private static string ReplaceXmlValues(string xmlContent, DataRow row)
        {
            string[] keys = { "txtmrbts", "txtlncel", "txtcellname", "txtpci", "txtroot", "txttac", "txtrmodantl1", "txtrmodantl2", "txtenbid", "txtgnbid", "txtip5g" };
            foreach (string key in keys)
            {
                if (row.Table.Columns.Contains(key))
                {
                    string value = row[key]?.ToString()?.Trim() ?? "";
                    xmlContent = xmlContent.Replace(key, value);
                }
            }
            return xmlContent;
        }

        private int[] CalculateColumnWidths(string[] columnNames, DataTable data)
        {
            int[] widths = new int[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                widths[i] = columnNames[i].Length;
            }

            foreach (DataRow row in data.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    string value = row.ItemArray[i]?.ToString() ?? "";
                    widths[i] = Math.Max(widths[i], value.Length);
                }
            }

            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = Math.Min(25, widths[i]);
            }

            return widths;
        }

        private string FormatRowAsTable(string[] data, int[] columnWidths)
        {
            string[] paddedData = new string[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                string value = data[i].Length > columnWidths[i] ? data[i].Substring(0, columnWidths[i]) : data[i];
                paddedData[i] = value.PadRight(columnWidths[i]);
            }
            return "| " + string.Join(" | ", paddedData) + " |";
        }
    }
}