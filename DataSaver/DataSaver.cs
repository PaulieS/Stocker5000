using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Stocker5000.HtmlParser;
[assembly: InternalsVisibleTo("Test")]
namespace Stocker5000.DataSaver
{
    public class DataSaver : IDataSaver
    {
        private StockHistoryDbContext _db;

        public DataSaver()
        {
            this._db = new StockHistoryDbContext();
        }

        internal DataSaver(StockHistoryDbContext db)
        {
            _db = db;
        }

        public async Task<double> GetLastIndexValue(string indexName)
        {
            var index = await _db.StockIndexes.FirstOrDefaultAsync(x => x.Name == indexName);
            if (index == null || !index.Values.Any())
            {
                throw new ArgumentException($"No stock values for {indexName}", indexName);
            }
            else
            {
                return await Task.Run(() =>index.Values.OrderByDescending(x => x.Time).FirstOrDefault().Value);
            }
        }

        public async Task SaveStockInfo(IEnumerable<StockInfo> data)
        {
            foreach (var item  in data)
            {
                await SaveStockInfo(item);
            }

        }

        public async Task SaveStockInfo(StockInfo data)
        {
            object asda = _db.StockIndexes.FirstOrDefault(x => x.Name == data.Name);

            StockIndex index = await _db.StockIndexes.FirstOrDefaultAsync(x => x.Name == data.Name);
                              if(index == null) index = await AddNewStockIndex(data.Name);
            _db.IndexesValues.Add(new IndexValue()
            {
                IndexId = index.Id,
                Time = DateTime.Now,
                Value = data.Value
            });
            await _db.SaveChangesAsync();
        }

        private async Task<StockIndex> AddNewStockIndex(string name)
        {
            var index = new StockIndex()
            {
                Name = name
            };
            _db.StockIndexes.Add(index);
            await _db.SaveChangesAsync();
            return index;
        }
        private bool CheckIfIndexExistsInDb(string name)
        {
            return _db.StockIndexes.FirstOrDefault(x => x.Name == name) != null;
        }
    }
}
