using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocker5000.HtmlParser
{
    public interface IHtmlParser
    {
        IEnumerable<StockInfo> Parse(string data);
        Task<IEnumerable<StockInfo>> ParseAsync(string data);
    }
}