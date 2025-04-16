using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace TOOL_TW_DDN
{
    public class CaProcessor
    {
        private readonly RichTextBox logTextBox;
        private readonly ProgressBar progressBar; // Thêm ProgressBar
        private readonly string xmlHeader = XmlConstants.XmlHeader; // Sử dụng XmlConstants
        private readonly string xmlFooter = XmlConstants.XmlFooter;
        private readonly Dictionary<string, DataTable> dumpCache = new Dictionary<string, DataTable>();

        public CaProcessor(RichTextBox logTextBox)
        {
            this.logTextBox = logTextBox;
        }

        public void ProcessCaPairs(
            string dataPath,
            string dumpPath,
            string templatePath,
            string resultsDir,
            string timestamp,
            string pairType,
            string dumpFilePattern,
            string delTemplateFile,
            string creTemplateFile,
            Func<DataRow, DataRow, bool> pairCondition = null,
            Func<string, string> creTemplateSelector = null,
            string updTemplateFile = null,
            Func<string, string> updTemplateSelector = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string logOutput = "";

            string csvPath = Path.Combine(dataPath, "TWCELLDATA.csv");
            if (!File.Exists(csvPath))
            {
                logOutput += $"Lỗi: Không tìm thấy file {Path.GetFileName(csvPath)}!\n";
                logTextBox.AppendText(logOutput);
                return;
            }
            DataTable csvData = CsvLoader.LoadCsv(csvPath, ',', logTextBox);
            if (csvData.Rows.Count == 0)
            {
                logOutput += "Lỗi: File TWCELLDATA.csv không có dữ liệu!\n";
                logTextBox.AppendText(logOutput);
                return;
            }

            string dumpFile = Directory.GetFiles(dumpPath, dumpFilePattern).FirstOrDefault();
            if (string.IsNullOrEmpty(dumpFile))
            {
                logOutput += $"Cảnh báo: Không tìm thấy file {dumpFilePattern}, bỏ qua xử lý {pairType}!\n";
                logTextBox.AppendText(logOutput);
                return;
            }

            DataTable dumpData;
            if (!dumpCache.TryGetValue(dumpFile, out dumpData))
            {
                dumpData = CsvLoader.LoadCsv(dumpFile, '\t', logTextBox);
                dumpCache[dumpFile] = dumpData;
            }
            if (dumpData.Rows.Count == 0)
            {
                logOutput += $"Lỗi: File dump {Path.GetFileName(dumpFile)} không có dữ liệu!\n";
                logTextBox.AppendText(logOutput);
                return;
            }

            // Thêm dữ liệu LNCEL_FDD để kiểm tra dlMimoMode (chỉ cho CAREL)
            DataTable lncelFddDumpData = null;
            if (pairType == "CAREL")
            {
                string lncelFddDumpFile = Directory.GetFiles(dumpPath, "Export_LNCEL_FDD__*.txt").FirstOrDefault();
                if (string.IsNullOrEmpty(lncelFddDumpFile))
                {
                    logOutput += $"Cảnh báo: Không tìm thấy file Export_LNCEL_FDD__*.txt, bỏ qua kiểm tra dlMimoMode!\n";
                }
                else
                {
                    lncelFddDumpData = CsvLoader.LoadCsv(lncelFddDumpFile, '\t');
                    if (lncelFddDumpData.Rows.Count == 0)
                    {
                        logOutput += $"Lỗi: File Export_LNCEL_FDD__*.txt không có dữ liệu!\n";
                        logTextBox.AppendText(logOutput);
                        return;
                    }
                }
            }

            string delTemplatePath = Path.Combine(templatePath, delTemplateFile);
            if (!File.Exists(delTemplatePath))
            {
                logOutput += $"Lỗi: Không tìm thấy file {delTemplateFile} trong {templatePath}!\n";
                logTextBox.AppendText(logOutput);
                return;
            }
            string delTemplate = File.ReadAllText(delTemplatePath);

            string caDelPath = Path.Combine(resultsDir, $"{pairType.ToLower()}_del_{timestamp}.xml");
            string caCrePath = Path.Combine(resultsDir, $"{pairType.ToLower()}_cre_{timestamp}.xml");
            string caUpdPath = updTemplateFile != null ? Path.Combine(resultsDir, $"{pairType.ToLower()}_upd_{timestamp}.xml") : null;

            using (StreamWriter delWriter = new StreamWriter(caDelPath, false))
            using (StreamWriter creWriter = new StreamWriter(caCrePath, false))
            using (var updWriter = caUpdPath != null ? new StreamWriter(caUpdPath, false) : null)
            {
                delWriter.WriteLine(xmlHeader);
                creWriter.WriteLine(xmlHeader);
                if (updWriter != null) updWriter.WriteLine(xmlHeader);

                var deletedMOs = new HashSet<string>();
                var createdPairs = new HashSet<string>();
                var updatedPairs = new HashSet<string>();
                var usedIndicesByPrefix = new Dictionary<string, HashSet<int>>();

                var csvPairs = csvData.AsEnumerable()
                    .Select(r => (txtmrbts: r.Field<string>("txtmrbts"), txtlncel: r.Field<string>("txtlncel"), f4gSource: r.Field<string>("f4g")))
                    .ToList();

                var caPairs = pairType == "CAREL"
                    ? new PairGenerator().GenerateCarelPairs(csvData, lncelFddDumpData)
                        .Select(p => (txtmrbts: p.txtmrbts, txtlncel: "", txtlncelSource: p.txtlncelSource, f4gSource: csvPairs.FirstOrDefault(cp => cp.txtmrbts == p.txtmrbts && cp.txtlncel == p.txtlncelSource).f4gSource ?? "", f4gTarget: p.txtlncelTarget))
                        .ToList()
                    : pairType == "LNREL"
                        ? new PairGenerator().GenerateLnrelPairs(csvData)
                            .Select(p => (txtmrbts: p.txtmrbts, txtlncel: "", txtlncelSource: p.txtlncelSource, f4gSource: "", f4gTarget: p.txtlncelTarget))
                            .ToList()
                        : new PairGenerator().GeneratePairs(csvData, pairCondition)
                            .Select(p => (txtmrbts: p.txtmrbts, txtlncel: p.txtlncel, txtlncelSource: "", f4gSource: p.f4gSource, f4gTarget: p.f4gTarget))
                            .ToList();

                var allDumpPairs = dumpData.AsEnumerable()
                    .Where(r => r["MO"]?.ToString()?.Contains($"/{pairType}-") == true)
                    .Select(r =>
                    {
                        string mo = r["MO"].ToString();
                        string[] parts = mo.Split('/');
                        string txtMrbts = parts.FirstOrDefault(p => p.StartsWith("MRBTS-"))?.Replace("MRBTS-", "") ?? "Unknown";
                        string txtLncel = parts.FirstOrDefault(p => p.StartsWith("LNCEL-"))?.Replace("LNCEL-", "") ?? "Unknown";
                        return new
                        {
                            MO = mo,
                            Freq = pairType == "CAREL" ? r.Field<string>("lcrId") : r.Field<string>(GetFreqColumn(pairType)),
                            TxtMrbts = txtMrbts,
                            TxtLncel = txtLncel
                        };
                    })
                    .ToList();

                int createCount = 0, deleteCount = 0, updateCount = 0;

                // Tạo dictionary dlMimoMode từ lncelFddDumpData
                // Trong ProcessCaPairs, đoạn tạo mimoModes:
                var mimoModes = lncelFddDumpData?.AsEnumerable()
                    .Where(r => r["MO"]?.ToString()?.Contains("/LNCEL_FDD-") == true)
                    .GroupBy(
                        r => r["MO"].ToString().Split('/').FirstOrDefault(p => p.StartsWith("LNCEL-"))?.Replace("LNCEL-", "") ?? "",
                        r => r["dlMimoMode"]?.ToString()?.Trim() ?? "")
                    .ToDictionary(
                        g => g.Key,
                        g => g.First() // Lấy giá trị đầu tiên nếu có trùng
                    ) ?? new Dictionary<string, string>();

                foreach (var pair in caPairs)
                {
                    string moPrefix = pairType == "CAREL"
                        ? $"PLMN-PLMN/MRBTS-{pair.txtmrbts}/LNBTS-{pair.txtmrbts}/LNCEL-{pair.txtlncelSource}/{pairType}-"
                        : $"PLMN-PLMN/MRBTS-{pair.txtmrbts}/LNBTS-{pair.txtmrbts}/LNCEL-{pair.txtlncel}/{pairType}-";
                    var existingPairs = dumpData.AsEnumerable()
                        .Where(r => r["MO"]?.ToString()?.StartsWith(moPrefix) == true)
                        .Select(r => new { MO = r["MO"].ToString(), Freq = pairType == "CAREL" ? r.Field<string>("lcrId") : r.Field<string>(GetFreqColumn(pairType)) })
                        .ToList();

                    foreach (var existing in existingPairs)
                    {
                        bool isValid = pairType == "CAREL"
                            ? caPairs.Any(p => p.txtmrbts == pair.txtmrbts && p.txtlncelSource == pair.txtlncelSource && p.f4gTarget == existing.Freq)
                            : existing.Freq != pair.f4gSource;

                        // Kiểm tra dlMimoMode cho CAREL hiện hữu
                        if (pairType == "CAREL" && lncelFddDumpData != null)
                        {
                            string lncelSource = pair.txtlncelSource;
                            string lncelTarget = existing.Freq; // lcrId là txtlncelTarget
                            string mimoSource = mimoModes.ContainsKey(lncelSource) ? mimoModes[lncelSource] : "";
                            string mimoTarget = mimoModes.ContainsKey(lncelTarget) ? mimoModes[lncelTarget] : "";

                            if (!string.IsNullOrEmpty(mimoSource) && !string.IsNullOrEmpty(mimoTarget) && !IsValidMimoPair(mimoSource, mimoTarget))
                            {
                                isValid = false; // Nếu không thỏa mãn dlMimoMode, đánh dấu để xóa
                            }
                        }

                        if (!isValid && !deletedMOs.Contains(existing.MO))
                        {
                            string delContent = delTemplate.Replace($"txtmo{pairType.ToLower()}", existing.MO);
                            delWriter.WriteLine(delContent);
                            deletedMOs.Add(existing.MO);
                            deleteCount++;
                        }
                    }

                    bool pairExists = existingPairs.Any(p => p.Freq == pair.f4gTarget);
                    string pairKey = pairType == "CAREL"
                        ? $"{pair.txtmrbts}-{pair.txtlncelSource}-{pair.f4gTarget}"
                        : $"{pair.txtmrbts}-{pair.txtlncel}-{pair.f4gTarget}";

                    if (!pairExists && !createdPairs.Contains(pairKey))
                    {
                        string creTemplatePath = creTemplateSelector != null
                            ? Path.Combine(templatePath, creTemplateSelector(pair.f4gTarget))
                            : Path.Combine(templatePath, creTemplateFile);
                        if (!File.Exists(creTemplatePath))
                        {
                            logOutput += $"Lỗi: Không tìm thấy file {Path.GetFileName(creTemplatePath)}!\n";
                            logTextBox.AppendText(logOutput);
                            return;
                        }
                        string creTemplate = File.ReadAllText(creTemplatePath);

                        string txtprio = "2";
                        if (pairType == "CAREL")
                        {
                            if (!int.TryParse(pair.txtlncelSource, out int lncelSource) || !int.TryParse(pair.f4gTarget, out int lncelTarget))
                            {
                                logOutput += $"Lỗi: Không thể parse txtlncelSource={pair.txtlncelSource} hoặc txtlncelTarget={pair.f4gTarget} thành số!\n";
                                logTextBox.AppendText(logOutput);
                                return;
                            }
                            int diff = lncelTarget - lncelSource;
                            txtprio = (diff % 30 == 0) ? "1" : "2";
                        }

                        if (!usedIndicesByPrefix.ContainsKey(moPrefix))
                        {
                            usedIndicesByPrefix[moPrefix] = existingPairs
                                .Select(p => int.TryParse(p.MO.Split('-').Last(), out int index) ? index : -1)
                                .Where(i => i > 0)
                                .ToHashSet();
                        }
                        var usedIndices = usedIndicesByPrefix[moPrefix];
                        int newIndex = 1;
                        while (usedIndices.Contains(newIndex))
                        {
                            newIndex++;
                        }
                        usedIndices.Add(newIndex);

                        string creContent = creTemplate
                            .Replace("txtmrbts", pair.txtmrbts)
                            .Replace("txtlncel", pairType == "CAREL" ? pair.txtlncelSource : pair.txtlncel)
                            .Replace("txtfca", pair.f4gTarget) // CAPR
                            .Replace("txtdlcarfrqeut", pair.f4gTarget) // IRFIM
                            .Replace("txteutracarrierinfo", pair.f4gTarget) // LNHOIF
                            .Replace("txtlcrid", pair.f4gTarget) // CAREL
                            .Replace("txtprio", txtprio) // CAREL
                            .Replace($"{pairType}-*", $"{pairType}-{newIndex}");
                        creWriter.WriteLine(creContent);
                        createdPairs.Add(pairKey);
                        createCount++;
                    }
                    if (progressBar != null) progressBar.Value++; // Cập nhật tiến trình
                }

                if (pairType == "LNHOIF" && updWriter != null)
                {
                    var validMrbts = csvData.AsEnumerable().Select(r => r.Field<string>("txtmrbts")).Distinct().ToHashSet();
                    foreach (var dumpPair in allDumpPairs)
                    {
                        if (validMrbts.Contains(dumpPair.TxtMrbts) && !deletedMOs.Contains(dumpPair.MO))
                        {
                            string pairKey = $"{dumpPair.TxtMrbts}-{dumpPair.TxtLncel}-{dumpPair.Freq}";
                            if (!updatedPairs.Contains(pairKey))
                            {
                                string updTemplatePath = updTemplateSelector != null
                                    ? Path.Combine(templatePath, updTemplateSelector(dumpPair.Freq))
                                    : Path.Combine(templatePath, "lnhoif_upd_default.xml");
                                if (!File.Exists(updTemplatePath))
                                {
                                    updTemplatePath = Path.Combine(templatePath, "lnhoif_upd_l1501.xml");
                                    if (!File.Exists(updTemplatePath))
                                    {
                                        logOutput += $"Lỗi: Không tìm thấy file mặc định lnhoif_upd_l1501.xml trong {templatePath}!\n";
                                        logTextBox.AppendText(logOutput);
                                        return;
                                    }
                                }
                                string updTemplate = File.ReadAllText(updTemplatePath);
                                string updContent = updTemplate
                                    .Replace("txtmolnhoif", dumpPair.MO)
                                    .Replace("txteutracarrierinfo", dumpPair.Freq);
                                updWriter.WriteLine(updContent);
                                updatedPairs.Add(pairKey);
                                updateCount++;
                            }
                        }
                    }
                }

                delWriter.WriteLine(xmlFooter);
                creWriter.WriteLine(xmlFooter);
                if (updWriter != null) updWriter.WriteLine(xmlFooter);

                stopwatch.Stop();
                logOutput += $"{pairType}: Xử lý {allDumpPairs.Count} MO (Tạo: {createCount}, Xóa: {deleteCount}{(pairType == "LNHOIF" ? $", Cập nhật: {updateCount}" : "")}), Thời gian: {stopwatch.Elapsed.TotalSeconds:F2}s\n";
            }

            logOutput += $"File xóa {pairType}: {caDelPath}\n";
            logOutput += $"File tạo {pairType}: {caCrePath}\n";
            if (caUpdPath != null) logOutput += $"File cập nhật {pairType}: {caUpdPath}\n";
            logTextBox.AppendText(logOutput);
        }

        // Hàm kiểm tra cặp dlMimoMode hợp lệ (thêm vào lớp)
        private bool IsValidMimoPair(string mimo1, string mimo2)
        {
            if (string.IsNullOrEmpty(mimo1) || string.IsNullOrEmpty(mimo2))
                return false;

            mimo1 = mimo1.Trim();
            mimo2 = mimo2.Trim();

            if (mimo1 == "Closed Loop MIMO (4x4)" && mimo2 == "Closed Loop MIMO (4x4)") return true;
            if (mimo1 == "Closed Loop Mimo" && mimo2 == "Closed Loop Mimo") return true;
            if (mimo1 == "SingleTX" && mimo2 == "SingleTX") return true;
            if (mimo1 == "Closed Loop MIMO (4x4)" && mimo2 == "Closed Loop Mimo") return true;
            if (mimo1 == "Closed Loop Mimo" && mimo2 == "Closed Loop MIMO (4x4)") return true;

            return false;
        }
        private string GetFreqColumn(string pairType)
        {
            switch (pairType)
            {
                case "CAPR": return "earfcnDL";
                case "IRFIM": return "dlCarFrqEut";
                case "LNHOIF": return "eutraCarrierInfo";
                default: throw new ArgumentException($"Unknown pair type: {pairType}");
            }
        }
    }
}