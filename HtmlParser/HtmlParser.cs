using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stocker5000.HtmlParser
{
    public class HtmlParser : IHtmlParser
    {
        #region Regexes
        readonly Regex _extractTableBodyRegex = new Regex(@"<tbody.+>.+<\/tbody>");
        readonly Regex _extractTableRowsRegex = new Regex(@"(<tr[^>]+>)(.*?)(<\/tr>)");
        readonly Regex _nameExctracRegex = new Regex(@"(?:<a[^>]+id=pp_s[^>]+>)([^<^>]+)(?:<\/a>)");
        readonly Regex _valueExtractRegex = new Regex(@"(?:<td[^>]*id=pp_v[^>]*>)([^<^>]+)(?:<\/td>)");
        #endregion

        #region Private Methods
        private string ExctractTableBody(string input)
        {
            var extractedTableBody = _extractTableBodyRegex.Match(input);
            return extractedTableBody.Value;
        }

        private string[] ExctractTableRows(string extractedTableBody)
        {
            var matches = _extractTableRowsRegex.Matches(extractedTableBody);
            string[] tableRows = new string[matches.Count];
            return matches.OfType<Match>().Select(x => x.Value).ToArray();
        }
   

        private IEnumerable<StockInfo> ExctractStockInfo(string[] tableRows)
        {
            var stockInfos = (from row in tableRows
                                                 let name = _nameExctracRegex.Match(row).Groups[1].Value
                                                 let value = _valueExtractRegex.Match(row).Groups[1].Value
                                                 select new StockInfo()
                                                 {
                                                     Name = name,
                                                     Value = Double.Parse(value, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo),
                                                 }).ToList();
            return stockInfos.AsEnumerable();
        }
        #endregion

        public async Task<IEnumerable<StockInfo>> ParseAsync(string data)
        {
            return await Task.Run(() => Parse(data));
        }

        public IEnumerable<StockInfo> Parse(string data)
        {
            var extractedTableBody = ExctractTableBody(data);
            var tableRows = ExctractTableRows(extractedTableBody);
            var stockInfos = ExctractStockInfo(tableRows);
            return stockInfos;
        } 
    }
}
