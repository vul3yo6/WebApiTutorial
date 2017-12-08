using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiTutorial.Models
{
    public class Customer
    {
        //public Guid Id { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsMarried { get; set; }

        public override string ToString()
        {
            string txt = string.Format("Id: {0}, Name: {1}, Age: {2}, IsMarried: {3}", Id, Name, Age, IsMarried);
            return txt;
        }
    }
}