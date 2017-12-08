using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace WebApiTutorial.Controllers
{
    public class FilesController : ApiController
    {
        public HttpResponseMessage Get(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath(@"~\upload\" + fileName);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;

            //var path = @"C:\Temp\test.exe";
            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");
            //return result;
        }

        // POST: api/File
        //public void Post([FromBody]string value)
        //public void Post(HttpPostedFileBase file)
        public void Post(HttpRequestMessage request)
        {
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["file"];

            if (postedFile.ContentLength > 0)
            {
                // 記得先建立資料夾
                string filePath = HttpContext.Current.Server.MapPath(@"~\upload\" + postedFile.FileName);
                postedFile.SaveAs(filePath);
            }
        }
    }
}
