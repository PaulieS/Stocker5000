using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Stocker5000.HtmlParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        #region TestInputData
        private const string CorrectInputData1 =
            @"function pp_m_(a){if(typeof(pp_m)=='function')pp_m(a);}document.write('<table border=0 cellpadding=1 cellspacing=1 id=pp_t><tr id=pp_n><td colspan=4 id=pp_h nowrap>&nbsp;Notowania GPW</td></tr><tbody align=right><tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=wig id=pp_s onclick=pp_m_(this)>WIG</a></td><td id=pp_v>45770.4</td><td id=pp_cu nowrap>+0.60%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=wig20 id=pp_s onclick=pp_m_(this)>WIG20</a></td><td id=pp_v>1844.6</td><td id=pp_cu nowrap>+0.35%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=fw20 id=pp_s onclick=pp_m_(this)>WIG20 Fut</a></td><td id=pp_v>1842.0</td><td id=pp_cd nowrap>-0.05%</td><td id=pp_d nowrap>17:03</td></tr><tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=wig20usd id=pp_s onclick=pp_m_(this)>WIG20USD</a></td><td id=pp_v>461.8</td><td id=pp_cd nowrap>-0.67%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=mwig40 id=pp_s onclick=pp_m_(this)>mWIG40</a></td><td id=pp_v>3478.6</td><td id=pp_cu nowrap>+1.32%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=swig80 id=pp_s onclick=pp_m_(this)>sWIG80</a></td><td id=pp_v>12709.2</td><td id=pp_cu nowrap>+0.62%</td><td id=pp_d nowrap>17:15</td></tr></tbody></table>');";

        private const string ExtractedTableBody =
            @"<tbody align=right><tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=wig id=pp_s onclick=pp_m_(this)>WIG</a></td><td id=pp_v>45770.4</td><td id=pp_cu nowrap>+0.60%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=wig20 id=pp_s onclick=pp_m_(this)>WIG20</a></td><td id=pp_v>1844.6</td><td id=pp_cu nowrap>+0.35%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=fw20 id=pp_s onclick=pp_m_(this)>WIG20 Fut</a></td><td id=pp_v>1842.0</td><td id=pp_cd nowrap>-0.05%</td><td id=pp_d nowrap>17:03</td></tr><tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=wig20usd id=pp_s onclick=pp_m_(this)>WIG20USD</a></td><td id=pp_v>461.8</td><td id=pp_cd nowrap>-0.67%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=mwig40 id=pp_s onclick=pp_m_(this)>mWIG40</a></td><td id=pp_v>3478.6</td><td id=pp_cu nowrap>+1.32%</td><td id=pp_d nowrap>17:15</td></tr><tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=swig80 id=pp_s onclick=pp_m_(this)>sWIG80</a></td><td id=pp_v>12709.2</td><td id=pp_cu nowrap>+0.62%</td><td id=pp_d nowrap>17:15</td></tr></tbody>";
        private readonly string[] TableRows =  {
            @"<tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=wig id=pp_s onclick=pp_m_(this)>WIG</a></td><td id=pp_v>45770.4</td><td id=pp_cu nowrap>+0.60%</td><td id=pp_d nowrap>17:15</td></tr>",
            @"<tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=wig20 id=pp_s onclick=pp_m_(this)>WIG20</a></td><td id=pp_v>1844.6</td><td id=pp_cu nowrap>+0.35%</td><td id=pp_d nowrap>17:15</td></tr>",
            @"<tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=fw20 id=pp_s onclick=pp_m_(this)>WIG20 Fut</a></td><td id=pp_v>1842.0</td><td id=pp_cd nowrap>-0.05%</td><td id=pp_d nowrap>17:03</td></tr>",
            @"<tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=wig20usd id=pp_s onclick=pp_m_(this)>WIG20USD</a></td><td id=pp_v>461.8</td><td id=pp_cd nowrap>-0.67%</td><td id=pp_d nowrap>17:15</td></tr>",
            @"<tr id=pp_r1><td align=left nowrap><a href=http://stooq.pl/q/?s=mwig40 id=pp_s onclick=pp_m_(this)>mWIG40</a></td><td id=pp_v>3478.6</td><td id=pp_cu nowrap>+1.32%</td><td id=pp_d nowrap>17:15</td></tr>",
            @"<tr id=pp_r2><td align=left nowrap><a href=http://stooq.pl/q/?s=swig80 id=pp_s onclick=pp_m_(this)>sWIG80</a></td><td id=pp_v>12709.2</td><td id=pp_cu nowrap>+0.62%</td><td id=pp_d nowrap>17:15</td></tr>" };
        #endregion

        [TestMethod]
        public void ExctractTableBody()
        {
            Regex extractTableBodyRegex = new Regex(@"<tbody.+>.+<\/tbody>");
            var extractedTableBody = extractTableBodyRegex.Match(CorrectInputData1);
            Assert.AreEqual(extractedTableBody.Value, ExtractedTableBody);
        }

        [TestMethod]
        public void ExctractTableRows()
        {
            Regex extractTableRowsRegex = new Regex(@"(<tr[^>]+>)(.*?)(<\/tr>)");
            var matches = extractTableRowsRegex.Matches(ExtractedTableBody);
            string[] tableRows = new string[matches.Count];
            tableRows = matches.OfType<Match>().Select(x=>x.Value).ToArray();
            Assert.IsTrue(tableRows.SequenceEqual(TableRows));
        }

        [TestMethod]
        public void ExctractStockInfo()
        {
            Regex nameExctracRegex = new Regex(@"(?:<a[^>]+id=pp_s[^>]+>)([^<^>]+)(?:<\/a>)");
            Regex valueExtractRegex = new Regex(@"(?:<td[^>]*id=pp_v[^>]*>)([^<^>]+)(?:<\/td>)");
            ICollection<StockInfo> stockInfos = (from row in TableRows
                let name = nameExctracRegex.Match(row).Groups[1].Value
                let value = valueExtractRegex.Match(row).Groups[1].Value
                select new StockInfo()
                {
                    Name = name, Value = Double.Parse(value, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo),
                }).ToList();
            Assert.IsTrue(stockInfos.Count == TableRows.Length);
        }

        [TestMethod]
        public void DeciamlParseTest()
        {
            const string value = "12.34";
            Debug.WriteLine(Decimal.Parse(value, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo));
        }

        [TestMethod]
        public void TestValidInput()
        {
            HtmlParser parser = new HtmlParser();
            var output = parser.Parse(CorrectInputData1);
            Assert.IsTrue(output.Count()==6);
            Assert.AreEqual(output.FirstOrDefault(x=>x.Name == "WIG20").Value, 1844.60);


        }
    }
}
