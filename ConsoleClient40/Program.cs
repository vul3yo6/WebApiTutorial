using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using WebApiTutorial.Models;

namespace ConsoleClient40
{
    class Program
    {
        //private readonly static string _url = "http://localhost:64486/api/";
        private readonly static string _url = "http://140.116.234.21/WebApiTurtorial/api/";

        static void Main(string[] args)
        {
            try
            {
                //TestRestClient();
                TestHttpClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private static void TestRestClient()
        {
            // http://restsharp.org/
            var client = new RestClient(_url);

            // GET
            var request = new RestRequest("Customers", Method.GET);
            IRestResponse<List<Customer>> response = client.Execute<List<Customer>>(request);
            //var custList = response.Data;

            //foreach (var cust in custList)
            //{
            //    Console.WriteLine(cust.ToString());
            //}

            //Console.WriteLine("--------------------------------------");

            //// GET
            //request = new RestRequest("Customers/{id}", Method.GET);
            //request.AddUrlSegment("id", "9");
            //IRestResponse response2 = client.Execute(request);
            //var content = response2.Content; // raw content as string
            //Console.WriteLine(content);

            //Console.WriteLine("--------------------------------------");
            //Console.ReadKey();

            //// POST
            //request = new RestRequest("Customers", Method.POST);
            //request.AddJsonBody(new Customer() {Id = 99, Name = "TEST", Age = 30, IsMarried = true});
            //IRestResponse response3 = client.Execute(request);
            //Console.WriteLine(response3.Content);

            Console.WriteLine("--------------------------------------");

            request = new RestRequest("Files", Method.POST);
            request.AddHeader("content-type", "multipart/form-data;");
            request.AddFile("file", @"C:\Users\kentseng\Dropbox\Public\images\S__87482823.jpg");
            IRestResponse response4 = client.Execute(request);
            Console.WriteLine(response4.Content);

            Console.WriteLine("--------------------------------------");

            request = new RestRequest("Files", Method.GET);
            request.AddHeader("content-type", "multipart/form-data;");
            request.AddParameter("fileName", "20211130143339989.jpg");
            IRestResponse response5 = client.Execute(request);
            Console.WriteLine(response5.Content);
        }

        private static void TestHttpClient()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            client.BaseAddress = new Uri(_url);

            using (var content = new MultipartFormDataContent())
            {
                using (var fs = File.OpenRead(@"C:\Users\kentseng\Dropbox\Public\images\S__87482823.jpg"))
                {
                    using (var sc = new StreamContent(fs))
                    {
                        content.Add(sc, "file", "S__87482823.jpg");

                        HttpResponseMessage response = client.PostAsync("Files", content).Result;
                        response.EnsureSuccessStatusCode();
                        Console.WriteLine($"{response.StatusCode}: {response.Content}");
                    }
                }
            }

            Console.WriteLine("--------------------------------------");

            HttpResponseMessage response2 = client.GetAsync("Files?fileName=S__87482823_2021120111_5208551.jpg").Result;
            response2.EnsureSuccessStatusCode();
            using (var stream = response2.Content.ReadAsStreamAsync().Result)
            using (var fs = new FileStream("S__87482823_2021120111_5208551.jpg", FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fs);
            }

            Console.WriteLine("--------------------------------------");
        }
    }
}
