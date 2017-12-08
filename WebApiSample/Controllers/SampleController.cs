using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiSample.Controllers
{
    public class SampleController : ApiController
    {
        /// <summary>
        /// 取得範例資料清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 取得一筆範例資料
        /// </summary>
        /// <param name="id">鍵值</param>
        /// <returns></returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 新增範例資料
        /// </summary>
        /// <param name="value">字串</param>
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// 更新範例資料
        /// </summary>
        /// <param name="id">鍵值</param>
        /// <param name="value">字串</param>
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// 刪除範例資料
        /// </summary>
        /// <param name="id">鍵值</param>
        public void Delete(int id)
        {
        }
    }
}
