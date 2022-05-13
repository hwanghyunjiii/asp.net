using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebApplication.Views.Http
{
    public partial class NetHttp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// HttpWebRequest
        /// </summary>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <param name="headerKey"></param>
        /// <param name="headerValue"></param>
        private void WebRequest(string url, string request, string method, string headerKey, string headerValue) 
        {
            HttpWebRequest httpWebRequest   = null;
            HttpWebResponse httpWebResponse = null;
            StreamReader streamReader       = null;
            Stream stream                   = null;

            string response     = string.Empty;
            string errMessage   = string.Empty;

            byte[] bytes = null;

            int returnCode       = 0;
            string returnMessage = string.Empty;

            try
            {
                // get 방식
                if (method.ToLower().Equals("get"))
                {
                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url + "?" + request);
                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                    httpWebRequest.ContentType = "application/json";
                    if (!string.IsNullOrEmpty(headerKey))
                    {
                        // header 추가
                        httpWebRequest.Headers.Add(headerKey, headerValue);
                    }
                }
                else
                {
                    bytes = Encoding.UTF8.GetBytes(request);

                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Method = WebRequestMethods.Http.Post;
                    httpWebRequest.ContentLength = bytes.Length;

                    if (!string.IsNullOrEmpty(headerKey))
                    {
                        // header 추가
                        httpWebRequest.Headers.Add(headerKey, headerValue);
                    }

                    // method 에 따른 contentType 지정
                    // contentType 도 파라미터로 받아서 처리하도록 수정 필요
                    if (method.ToLower().Equals("post"))
                    {
                        httpWebRequest.ContentType = "application/json";
                    }
                    else if (method.ToLower().Equals("soap"))
                    {
                        httpWebRequest.ContentType = "application/soap+xml; charset=utf-8";
                    }
                    else
                    {
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    }

                    // get 방식에서는 GetRequestStream() 호출하지 않음
                    stream = httpWebRequest.GetRequestStream(); 
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }

                httpWebRequest.Timeout = 60000;

                // response 받기
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                // http status 확인
                int httpStatus = httpWebResponse.StatusCode.GetHashCode();
                if( httpStatus != (int)HttpStatusCode.OK)
                {
                    returnCode = httpStatus;
                    returnMessage = httpWebResponse.StatusDescription;
                }
                else
                {
                    streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                    response = streamReader.ReadToEnd();
                    streamReader.Close();
                }

                httpWebResponse.Close();

            }
            catch(WebException webEx)
            {
                returnCode = 999;
                returnMessage = webEx.Message;

                using (Stream data = webEx.Response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    response = reader.ReadToEnd();
                }
            }
            finally
            {
                httpWebRequest = null;
                httpWebResponse = null;
                streamReader = null;
                stream = null;
            }

        }

        private void WebClient()
        {

        }

        private void HttpClient()
        {

        }
    }
}