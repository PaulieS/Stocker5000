using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stocker5000.DataDownloader;
using Stocker5000.DataSaver;
using Stocker5000.HtmlParser;

namespace Stocker5000
{
    public class Stocker5000
    {
        private readonly IDataGetter _dataGetter;
        private readonly IHtmlParser _parser;
        private readonly IDataSaver _dataSaver;

        public Stocker5000(IDataGetter dataGetter,IDataSaver dataSaver, IHtmlParser parser)
        {
            _dataGetter = dataGetter;
            _dataSaver = dataSaver;
            _parser = parser;
        }

        private async Task<bool> IsValueChanged(StockInfo data)
        {
            double oldValue;
            try
            {
                oldValue = await _dataSaver.GetLastIndexValue(data.Name);
            }
            catch (ArgumentException e)
            {
                return true;
            }
            return oldValue.CompareTo(data.Value) == 0;

        }
        public CancellationTokenSource StartSampling(int samplingInterval)
        {

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            var task = Task.Factory.StartNew(async () =>
            {

                ct.ThrowIfCancellationRequested();

                while (true)
                {

                    var data = await _dataGetter.GetDataAsync();
                    var parsedData = await _parser.ParseAsync(data);
                    foreach (var item in parsedData)
                    {
                        if (await IsValueChanged(item))
                        {
                            await _dataSaver.SaveStockInfo(item);
                        }
                    }

                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                    await Task.Delay(samplingInterval * 1000, ct);

                }
            }, tokenSource.Token);
            return tokenSource;
        }
    }
}
