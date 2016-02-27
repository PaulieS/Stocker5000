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
    class Program
    {
        private const string sourceUrl = "http://s.stooq.pl/pp/g.js";
        private static Stocker5000 _stocker5000 { get; set; }

        public static void Main(string[] args)
        {
            _stocker5000 = new Stocker5000(new DataGetter(new Uri(sourceUrl),new WebClient() ),new DataSaver.DataSaver(), new HtmlParser.HtmlParser());
            Console.WriteLine("HELLO! HERE IS STOCKET5000");
            int samplingInterval = 0;
            do
            {
                Console.WriteLine("Please set sampling interval higher than in full seconds");
                var input = Console.ReadLine();
                int.TryParse(input, out samplingInterval);
            } while (samplingInterval==0);
            bool stop = false;
            var task =_stocker5000.StartSampling(samplingInterval);
            do
            {
                Console.WriteLine("Sampling in proges. Press q to stop.");
                var input = Console.ReadLine();
                if (input.ToLower()=="q")
                {
                    stop = true;
                    task.Cancel();
                }
            } while (!stop);

        }

   


    }

   
}
