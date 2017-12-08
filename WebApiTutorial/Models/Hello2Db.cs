using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApiTutorial.Models
{
    public class Hello2Db
    {
        private string _filePath = @"D:\temp\Hello2Db.txt";

        private Dictionary<int, string> GetDataFromFile()
        {
            if (File.Exists(_filePath) == false)
            {
                FileStream fs = File.Create(_filePath);
                fs.Close();
            }

            Dictionary<int, string> _database = new Dictionary<int, string>();
            using (var sr = new StreamReader(_filePath, Encoding.UTF8))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    string[] aaa = line.Split(',');
                    int id = Convert.ToInt32(aaa[0]);
                    string value = aaa[1];
                    _database[id] = value;
                }
            }

            return _database;
        }

        public IEnumerable<string> Get()
        {
            Dictionary<int, string> _database = GetDataFromFile();
            return _database.Values;
        }

        public string Get(int id)
        {
            Dictionary<int, string> _database = GetDataFromFile();

            if (_database.ContainsKey(id) == false)
            {
                return string.Empty;
            }
            return _database[id];
        }

        public void Post(string value)
        {
            Dictionary<int, string> _database = GetDataFromFile();
            int maxId = 0;
            if (_database.Count > 0)
            {
                maxId = _database.Keys.Max();
            }

            using (var sw = new StreamWriter(_filePath, true, Encoding.UTF8))
            {
                string txt = string.Format("{0},{1}", maxId + 1, value.Trim());
                sw.WriteLine(txt);
            }
        }

        public void Put(int id, string value)
        {
            Dictionary<int, string> _database = GetDataFromFile();

            if (_database.ContainsKey(id) == false)
            {
                return;
            }
            _database[id] = value.Trim();

            using (var sw = new StreamWriter(_filePath, false, Encoding.UTF8))
            {
                foreach (var kvp in _database)
                {
                    string txt = string.Format("{0},{1}", kvp.Key, kvp.Value);
                    sw.WriteLine(txt);
                }
            }
        }

        public void Delete(int id)
        {
            Dictionary<int, string> _database = GetDataFromFile();

            if (_database.ContainsKey(id) == false)
            {
                return;
            }
            _database.Remove(id);

            using (var sw = new StreamWriter(_filePath, false, Encoding.UTF8))
            {
                foreach (var kvp in _database)
                {
                    string txt = string.Format("{0},{1}", kvp.Key, kvp.Value);
                    sw.WriteLine(txt);
                }
            }
        }
    }
}