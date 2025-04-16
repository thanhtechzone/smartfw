using System;
using System.Data;
using System.IO;

namespace TOOL_TW_DDN
{
    public static class CsvLoader
    {
        /// <summary>
        /// Loads a CSV file into a DataTable.
        /// </summary>
        /// <param name="filePath">Path to the CSV file.</param>
        /// <param name="delimiter">Character used to separate columns (default: ',').</param>
        /// <param name="logTextBox">RichTextBox to log warnings (optional).</param>
        /// <returns>DataTable containing CSV data.</returns>
        public static DataTable LoadCsv(string filePath, char delimiter = ',', RichTextBox logTextBox = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using StreamReader sr = new StreamReader(filePath);
                string? headerLine = sr.ReadLine();
                if (string.IsNullOrEmpty(headerLine)) return dt;

                string[] headers = headerLine.Split(delimiter);
                foreach (string header in headers)
                {
                    dt.Columns.Add(header.Trim());
                }

                int lineNumber = 1;
                while (!sr.EndOfStream)
                {
                    lineNumber++;
                    string? line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] rows = line.Split(delimiter);
                        if (rows.Length == headers.Length)
                        {
                            dt.Rows.Add(rows);
                        }
                        else if (logTextBox != null)
                        {
                            logTextBox.AppendText($"Cảnh báo: Dòng {lineNumber} trong {Path.GetFileName(filePath)} có {rows.Length} cột, không khớp với header ({headers.Length} cột). Dòng bị bỏ qua.\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading CSV file {Path.GetFileName(filePath)}: {ex.Message}");
            }
            return dt;
        }
    }
}