using System;
using System.Threading.Tasks;

namespace Stocker5000.DataDownloader
{
    public interface IDataGetter
    {
        Uri Target { get; set; }
        Task<string> GetDataAsync();
    }
}