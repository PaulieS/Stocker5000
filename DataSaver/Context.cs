using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocker5000.DataSaver
{
    public class StockHistoryDbContext :DbContext
    {   
        public DbSet<IndexValue> IndexesValues { get; set; }
        public DbSet<StockIndex> StockIndexes { get; set; }

        public StockHistoryDbContext() : base("DefaultConnection")
        {
            Database.SetInitializer<StockHistoryDbContext>(null);
        }
    }
   [Table("IndexesValues")]
    public class IndexValue
    {
        public Int64 Id { get; set; }
        [ForeignKey("Index")]
        public Int64 IndexId { get; set; }
        public double Value { get; set; }
        public DateTime Time { get; set; }
        public virtual StockIndex Index { get; set; }
    }
    [Table("StockIndexes")]
    public class StockIndex
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<IndexValue> Values { get; set; }
    }
}
