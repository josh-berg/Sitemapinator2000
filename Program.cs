using System;
using System.Xml;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sitemapinator2000
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        public async static Task Main(string[] args)
        {
            await RegenerateAndCache();
        }

        private static async Task RegenerateAndCache()
        {
            Console.Write("Enter Sleep Duration In MilliSeconds:");
            var sleep = Console.ReadLine();


            var urls = GetSitemapUrls();
            await RunCacheUpdater(urls, sleep);
        }

        private static async Task RunCacheUpdater(List<string> urls, string sleep)
        {
            Console.WriteLine($"===========Regenerating {urls.Count} URLs===========\n");

            var index = 1;
            var errorUrls = new List<string>();

            foreach(var url in urls)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var response = await client.GetAsync(url);
                stopwatch.Stop();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{index} - OK - {url} - {stopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    Console.WriteLine($"{index} - ERROR - {url} - Code: {response.StatusCode}");
                    errorUrls.Add($"{url} :: {response.StatusCode}");
                }

                stopwatch.Reset();
                index++;

                await Task.Delay(int.Parse(sleep));
            }

            Console.WriteLine($"\n\n============COMPLETE!============\n\n{errorUrls.Count} ERRORS:");
            foreach(var error in errorUrls)
            {
                Console.WriteLine(error);
            }
        }

        private static List<string> GetSitemapUrls()
        {
            Console.Write("Enter Sitemap Index Url (include http/https) ->  ");
            var index = Console.ReadLine();

            XmlReader doc = XmlReader.Create(index);

            var urls = new List<string>();

            while (doc.Read())
            {
                if (doc.NodeType == XmlNodeType.Text && doc.Value.StartsWith("http"))
                {
                    urls.Add(doc.Value);
                }
            }
            return urls;
        }
    }
}
