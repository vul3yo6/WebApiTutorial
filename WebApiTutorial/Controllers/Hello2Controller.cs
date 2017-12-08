using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTutorial.Models;

namespace WebApiTutorial.Controllers
{
    public class Hello2Controller : ApiController
    {
        private Hello2Db _db;

        public Hello2Controller()
        {
            _db = new Hello2Db();
        }

        // GET: api/Hello2
        public IEnumerable<string> Get()
        {
            return _db.Get();
        }

        // GET: api/Hello2/5
        public string Get(int id)
        {
            return _db.Get(id);
        }

        // POST: api/Hello2
        public void Post([FromBody]string value)
        {
            _db.Post(value);
        }

        // PUT: api/Hello2/5
        public void Put(int id, [FromBody]string value)
        {
            _db.Put(id, value);
        }

        // DELETE: api/Hello2/5
        public void Delete(int id)
        {
            _db.Delete(id);
        }
    }
}
