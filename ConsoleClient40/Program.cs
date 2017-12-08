using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApiTutorial.Models;

namespace ConsoleClient40
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://restsharp.org/
            var client = new RestClient("http://localhost:64486/api/");

            // GET
            var request = new RestRequest("Customers", Method.GET);
            IRestResponse<List<Customer>> response = client.Execute<List<Customer>>(request);
            var custList = response.Data;

            foreach (var cust in custList)
            {
                Console.WriteLine(cust.ToString());
            }

            Console.WriteLine("--------------------------------------");

            // GET
            request = new RestRequest("Customers/{id}", Method.GET);
            request.AddUrlSegment("id", "9");
            IRestResponse response2 = client.Execute(request);
            var content = response2.Content; // raw content as string
            Console.WriteLine(content);

            Console.WriteLine("--------------------------------------");
            Console.ReadKey();

            // POST
            request = new RestRequest("Customers", Method.POST);
            request.AddJsonBody(new Customer() {Id = 99, Name = "TEST", Age = 30, IsMarried = true});
            IRestResponse response3 = client.Execute(request);
            Console.WriteLine(response3.Content);

            Console.WriteLine("--------------------------------------");

            Console.ReadKey();
        }
    }
}
