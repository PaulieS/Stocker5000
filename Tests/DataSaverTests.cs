using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stocker5000.DataSaver;
using Stocker5000.HtmlParser;
using DataSaver =Stocker5000.DataSaver.DataSaver;


namespace Tests 
{
    [TestClass]
    public class DataSaverTests
    {
        [TestMethod]
        public void AddDataToNewStockIndex()
        {
            StockHistoryDbContext context = new StockHistoryDbContext();
            context.IndexesValues.RemoveRange(context.IndexesValues);
            context.StockIndexes.RemoveRange(context.StockIndexes);
            context.SaveChanges();
            var dataSaver =  new DataSaver();
            var data = new StockInfo()
            {
                Name = "WIG20",
                Value = 123.45
            };
           Task.Run(async ()=> await dataSaver.SaveStockInfo(data)).GetAwaiter().GetResult();
            var index = context.StockIndexes.FirstOrDefault(x => x.Name == data.Name);

            Assert.IsTrue(index!=null);
            Assert.IsTrue(index.Values.Any());

        }
        [TestMethod]
        public void AddDataToNewStockIndexTwoTimes()
        {
            StockHistoryDbContext context = new StockHistoryDbContext();
            context.IndexesValues.RemoveRange(context.IndexesValues);
            context.StockIndexes.RemoveRange(context.StockIndexes);
            context.SaveChanges();
            var dataSaver = new DataSaver();
            var data = new StockInfo()
            {
                Name = "WIG20",
                Value = 123.45
            };
            Task.Run(async () => await dataSaver.SaveStockInfo(data)).GetAwaiter().GetResult();
            Task.Run(async () => await dataSaver.SaveStockInfo(data)).GetAwaiter().GetResult();
            var index = context.StockIndexes.FirstOrDefault(x => x.Name == data.Name);

            Assert.IsTrue(index != null);
            Assert.IsTrue(index.Values.Count() ==2 );
        }
    }
}
