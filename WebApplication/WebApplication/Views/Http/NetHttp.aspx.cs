using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

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
        /// <param name="methodType"></param>
        /// <param name="headerKey"></param>
        /// <param name="headerValue"></param>
        private void WebRequest(string url, Dictionary<string, string> headers, string request, MethodType methodType = MethodType.Post, ContentType contentType = Http.ContentType.Json) 
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
                //System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)768 | (System.Net.SecurityProtocolType)3072;    // Framework 2.0
                //System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;   // Framework 4.0

                // get 방식
                if (methodType.Equals(MethodType.Get))
                {
                    if (!string.IsNullOrEmpty(request))
                    {
                        url = string.Format("{0}{1}{2}", url, request.IndexOf("?") == -1 ? "?" : "&", request);
                    }
                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    httpWebRequest.Method = WebRequestMethods.Http.Get;
                }
                else
                {
                    bytes = Encoding.UTF8.GetBytes(request);
                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.ContentLength = bytes.Length;

                    switch (methodType)
                    {
                        case MethodType.Post:
                            httpWebRequest.Method = WebRequestMethods.Http.Post;
                            break;
                        case MethodType.Put:
                            httpWebRequest.Method = WebRequestMethods.Http.Put;
                            break;
                        case MethodType.Delete:
                            httpWebRequest.Method = "DELETE";
                            break;
                        default:
                            httpWebRequest.Method = WebRequestMethods.Http.Post;
                            break;
                    }
                }

                // method 에 따른 contentType 설정
                httpWebRequest.ContentType = GetEnumDescription(contentType);

                // header 설정
                if (headers.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in headers)
                    {
                        //httpWebRequest.Headers.Add(kvp.Key, kvp.Value);
                        httpWebRequest.Headers[kvp.Key] = kvp.Value;
                    }
                }

                if (methodType.Equals("post") || methodType.Equals("put"))
                {
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
                returnCode = 998;
                returnMessage = webEx.Message;

                using (Stream data = webEx.Response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    response = reader.ReadToEnd();
                }
            }
            catch(Exception ex)
            {
                returnCode = 999;
                returnMessage = ex.Message;
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

        /// <summary>
        /// enum description 조회
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        /// <summary>
        /// object 를 string 으로 변환
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="isUrlencode"></param>
        /// <returns></returns>
        private static string ConvertObjectToString(object data, string separator, bool isUrlencode)
        {
            string strData = string.Empty;
            string strName = string.Empty;
            string strValue = string.Empty;

            StringBuilder sb = null;
            Type objType = null;

            try
            {
                sb = new StringBuilder();
                objType = data.GetType();

                foreach (PropertyInfo propertyInfo in objType.GetProperties())
                {
                    strName = propertyInfo.Name;
                    strValue = isUrlencode ? HttpUtility.UrlEncode(propertyInfo.GetValue(data, null).ToString()) : propertyInfo.GetValue(data, null).ToString();

                    if (sb.Length.Equals(0))
                    {
                        sb.AppendFormat("{0}={1}", strName, strValue);
                    }
                    else
                    {
                        sb.AppendFormat("{0}{1}={2}", separator, strName, strValue);
                    }
                }

                strData = sb.ToString();
            }
            catch (Exception ex)
            {
                strData = ex.Message;
            }
            return strData;
        }
    }


    #region Method Type
    public enum MethodType
    {
        Get,
        Post,
        Put,
        Delete
    }
    #endregion

    #region Content Type
    public enum ContentType
    {
        [Description("")]
        None,
        [Description("application/x-www-form-urlencoded")]
        Form,
        [Description("application/json")]
        Json,
        [Description("application/xml")]
        Xml,
        [Description("text/plain")]
        Text,
        [Description("text/html")]
        Html,
        [Description("application/soap+xml; charset=utf-8")]
        Soap
    }
    #endregion
}