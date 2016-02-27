using System;
using System.Net;
using System.Threading.Tasks;

namespace Stocker5000.DataDownloader
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".

    public class DataGetter : IDataGetter
    {
       
        private WebClient _webClient;

        public Uri Target { get; set; }

        public DataGetter(Uri target, WebClient webClient)
        {
            _webClient = webClient;
            Target = target;
        }

        public async Task<string> GetDataAsync()
        {
            return await _webClient.DownloadStringTaskAsync(Target);
        }

    }
}
