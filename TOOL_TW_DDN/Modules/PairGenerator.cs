using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace TOOL_TW_DDN
{
    public class PairGenerator
    {
        public List<(string txtmrbts, string txtlncel, string f4gSource, string f4gTarget)> GeneratePairs(
            DataTable csvData,
            Func<DataRow, DataRow, bool> pairCondition)
        {
            var pairs = new HashSet<(string txtmrbts, string txtlncel, string f4gSource, string f4gTarget)>();
            var groupedRows = csvData.AsEnumerable()
                .GroupBy(row => row["txtmrbts"]?.ToString()?.Trim() ?? "");

            foreach (var group in groupedRows)
            {
                var rows = group.ToList();
                for (int i = 0; i < rows.Count; i++)
                {
                    for (int j = 0; j < rows.Count; j++)
                    {
                        if (pairCondition(rows[i], rows[j]))
                        {
                            string txtmrbts = rows[i]["txtmrbts"]?.ToString()?.Trim() ?? "";
                            string txtlncel = rows[i]["txtlncel"]?.ToString()?.Trim() ?? "";
                            string f4gSource = rows[i]["f4g"]?.ToString()?.Trim() ?? "";
                            string f4gTarget = rows[j]["f4g"]?.ToString()?.Trim() ?? "";
                            pairs.Add((txtmrbts, txtlncel, f4gSource, f4gTarget));
                        }
                    }
                }
            }
            return pairs.ToList();
        }

        // Sinh cặp cho CAREL với điều kiện dlMimoMode, chỉ xét khi cùng MRBTS
        public List<(string txtmrbts, string txtlncelSource, string txtlncelTarget)> GenerateCarelPairs(
            DataTable csvData,
            DataTable dumpData)
        {
            var pairs = new HashSet<(string txtmrbts, string txtlncelSource, string txtlncelTarget)>();
            var rows = csvData.AsEnumerable().ToList();

            // Xử lý trùng key bằng cách nhóm và lấy giá trị đầu tiên từ dump
            var mimoModes = dumpData?.AsEnumerable()
                .Where(r => r["MO"]?.ToString()?.Contains("/LNCEL_FDD-") == true)
                .GroupBy(
                    r => r["MO"].ToString().Split('/').FirstOrDefault(p => p.StartsWith("LNCEL-"))?.Replace("LNCEL-", "") ?? "",
                    r => r["dlMimoMode"]?.ToString()?.Trim() ?? "")
                .ToDictionary(
                    g => g.Key,
                    g => g.First() // Lấy giá trị đầu tiên nếu có nhiều dòng trùng LNCEL
                ) ?? new Dictionary<string, string>();

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    string mrbts1 = rows[i]["txtmrbts"]?.ToString()?.Trim() ?? "";
                    string mrbts2 = rows[j]["txtmrbts"]?.ToString()?.Trim() ?? "";
                    string lncel1 = rows[i]["txtlncel"]?.ToString()?.Trim() ?? "";
                    string lncel2 = rows[j]["txtlncel"]?.ToString()?.Trim() ?? "";
                    string f4g1 = rows[i]["f4g"]?.ToString()?.Trim() ?? "";
                    string f4g2 = rows[j]["f4g"]?.ToString()?.Trim() ?? "";

                    // Chỉ xét các cặp cùng MRBTS
                    if (mrbts1 == mrbts2 && lncel1 != lncel2 && f4g1 != f4g2)
                    {
                        // Lấy dlMimoMode từ dump cho cặp cùng MRBTS
                        string mimoSource = mimoModes.ContainsKey(lncel1) ? mimoModes[lncel1] : "";
                        string mimoTarget = mimoModes.ContainsKey(lncel2) ? mimoModes[lncel2] : "";

                        // Kiểm tra điều kiện dlMimoMode chỉ khi cùng MRBTS
                        if (IsValidMimoPair(mimoSource, mimoTarget))
                        {
                            pairs.Add((mrbts1, lncel1, lncel2));
                        }
                    }
                }
            }
            return pairs.ToList();
        }

        // Hàm kiểm tra cặp dlMimoMode hợp lệ
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
        public List<(string txtmrbts, string txtlncelSource, string txtlncelTarget)> GenerateLnrelPairs(DataTable csvData)
        {
            var pairs = new HashSet<(string txtmrbts, string txtlncelSource, string txtlncelTarget)>();
            var rows = csvData.AsEnumerable().ToList();

            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    string mrbts1 = rows[i]["txtmrbts"]?.ToString()?.Trim() ?? "";
                    string mrbts2 = rows[j]["txtmrbts"]?.ToString()?.Trim() ?? "";
                    string lncel1 = rows[i]["txtlncel"]?.ToString()?.Trim() ?? "";
                    string lncel2 = rows[j]["txtlncel"]?.ToString()?.Trim() ?? "";
                    string f4g1 = rows[i]["f4g"]?.ToString()?.Trim() ?? "";
                    string f4g2 = rows[j]["f4g"]?.ToString()?.Trim() ?? "";

                    // Chỉ xét các cặp cùng MRBTS, khác LNCEL và khác f4g
                    if (mrbts1 == mrbts2 && lncel1 != lncel2 && f4g1 != f4g2)
                    {
                        pairs.Add((mrbts1, lncel1, lncel2));
                    }
                }
            }
            return pairs.ToList();
        }
    }
}