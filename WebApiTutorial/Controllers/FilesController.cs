using AisysWinApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        //public ApiResult Post(HttpRequestMessage request)
        public double Post(HttpRequestMessage request)
        {
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["file"];
            
            if (postedFile.ContentLength > 0)
            {
                // 記得先建立資料夾
                //string filePath = HttpContext.Current.Server.MapPath(@"~\upload\" + postedFile.FileName);
                string extensionName = Path.GetExtension(postedFile.FileName);
                string filePath = HttpContext.Current.Server.MapPath(@"~\upload\" + DateTime.Now.ToString("yyyyMMddHHmmsssfff") + extensionName);
                postedFile.SaveAs(filePath);

                // vision verify
                using (var aisys = new AisysVision())
                {
                    using (var matchAngle = new AisysMatchAngle())
                    {
                        //matchAngle.Learn(ArrowType.A, HttpContext.Current.Server.MapPath(@"~\upload\model_A.bmp"));
                        //matchAngle.Learn(ArrowType.B, HttpContext.Current.Server.MapPath(@"~\upload\model_B.bmp"));
                        //matchAngle.Learn(ArrowType.C, HttpContext.Current.Server.MapPath(@"~\upload\model_C.bmp"));
                        matchAngle.Learn();

                        var result = matchAngle.Match(filePath);
                        //return new ApiResult(result);
                        return result?.TrunAngleIn180 ?? double.NaN;
                    }
                }
            }
            
            //return new ApiResult(null);
            return double.NaN;
        }

        public class ApiResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }

            public ApiResult(bool success, string message, object data)
            {
                Success = success;
                Message = message;
                Data = data;
            }

            public ApiResult(object arrowResult)
            {
                if (arrowResult == null)
                {
                    Success = false;
                    Message = "Fail";
                }
                else
                {
                    Success = true;
                    Message = "Success";
                }

                Data = arrowResult;
            }
        }
    }
}
