using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTutorial.Models;

namespace WebApiTutorial.Controllers
{
    public class CustomersController : ApiController
    {
        private List<Customer> _database;

        public CustomersController()
        {
            _database = new List<Customer>();
            for (int i = 0; i < 10; i++)
            {
                var cust = new Customer
                {
                    Id = i,
                    Name = "Name_" + i.ToString(),
                    Age = 30 + i,
                    IsMarried = Convert.ToBoolean(i % 2),
                };
                _database.Add(cust);
            }
        }

        // GET: api/Customers
        public IEnumerable<Customer> Get()
        {
            return _database;
        }

        // GET: api/Customers/5
        public Customer Get(int id)
        {
            return _database.FirstOrDefault(x => x.Id == id);
        }
        public Customer Get(int id, string name)
        {
            return _database.FirstOrDefault(x => x.Id == id && x.Name == name);
        }
        public Customer Get(int id, int age)
        {
            return _database.FirstOrDefault(x => x.Id == id && x.Age >= age);
        }

        // POST: api/Customers
        public void Post([FromBody]Customer customer)
        {
            _database.Add(customer);
        }

        // PUT: api/Customers/5
        public void Put(int id, [FromBody]Customer customer)
        {
            Customer cust = Get(id);
            cust = customer;
        }

        // DELETE: api/Customers/5
        public void Delete(int id)
        {
            _database.RemoveAll(x => x.Id == id);
        }
    }
}
