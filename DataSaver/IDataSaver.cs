using System.Collections.Generic;
using System.Threading.Tasks;
using Stocker5000.HtmlParser;

namespace Stocker5000.DataSaver
{
    public interface IDataSaver
    {
        Task<double> GetLastIndexValue(string indexName);
        Task SaveStockInfo(IEnumerable<StockInfo> data);
        Task SaveStockInfo(StockInfo data);
    }
}