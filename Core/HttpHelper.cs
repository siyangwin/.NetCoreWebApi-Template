using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using Newtonsoft.Json;

namespace Core
{
    /// <summary>
    /// Http连接操作帮助类（基于 HttpClientFactory 实现，线程安全）
    /// </summary>
    public class HttpHelper
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpHelper(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        #region Public
        /// <summary>
        /// 生成请求数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public byte[] GetPostDate(object param)
        {
            var jsonData = JsonConvert.SerializeObject(param);
            return Encoding.Default.GetBytes(jsonData);
        }

        /// <summary>
        /// 为string格式的字符串生成请求数据（XML可用）
        /// </summary>
        /// <returns></returns>
        public byte[] GetPostDateOther(string param)
        {
            return Encoding.UTF8.GetBytes(param);
        }

        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="item">参数类对象</param>
        /// <returns>返回HttpResult类型</returns>
        public HttpResult GetHtml(HttpItem item, HttpContext httpContext = null)
        {
            //返回参数
            HttpResult result = new HttpResult();

            try
            {
                //需要自定义 handler 配置（代理/证书/Cookie 容器）时单独构建，否则复用 HttpClientFactory 的连接池
                bool needCustomHandler = !string.IsNullOrWhiteSpace(item.ProxyIp)
                    || item.WebProxy != null
                    || !string.IsNullOrWhiteSpace(item.CerPath)
                    || item.ClentCertificates != null && item.ClentCertificates.Count > 0
                    || item.ResultCookieType == ResultCookieType.CookieCollection;

                HttpClient client;
                HttpClientHandler handler = null;
                if (needCustomHandler)
                {
                    handler = BuildHandler(item);
                    client = new HttpClient(handler);
                }
                else
                {
                    client = httpClientFactory.CreateClient();
                }

                using (client)
                using (handler)
                {
                    client.Timeout = TimeSpan.FromMilliseconds(item.Timeout);

                    using (HttpRequestMessage request = BuildRequest(item))
                    using (HttpResponseMessage response = client.Send(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        ReadResponse(item, response, result);
                    }
                }

                if (item.IsToLower) result.Html = result.Html.ToLower();
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
                result.StatusDescription = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 构建 HttpClientHandler（代理/证书/Cookie/自动重定向）
        /// </summary>
        private HttpClientHandler BuildHandler(HttpItem item)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = item.Allowautoredirect,
                UseCookies = item.ResultCookieType == ResultCookieType.CookieCollection
            };

            if (item.MaximumAutomaticRedirections > 0)
            {
                handler.MaxAutomaticRedirections = item.MaximumAutomaticRedirections;
            }

            //证书校验：仅当证书链校验通过时才信任
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => errors == SslPolicyErrors.None;

            //客户端证书
            if (!string.IsNullOrWhiteSpace(item.CerPath))
            {
                handler.ClientCertificates.Add(new X509Certificate(item.CerPath));
            }
            if (item.ClentCertificates != null)
            {
                foreach (X509Certificate c in item.ClentCertificates)
                {
                    handler.ClientCertificates.Add(c);
                }
            }

            //代理
            if (!string.IsNullOrWhiteSpace(item.ProxyIp) && !item.ProxyIp.ToLower().Contains("ieproxy"))
            {
                if (item.ProxyIp.Contains(":"))
                {
                    string[] plist = item.ProxyIp.Split(':');
                    handler.Proxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                }
                else
                {
                    handler.Proxy = new WebProxy(item.ProxyIp, false);
                }
                if (!string.IsNullOrEmpty(item.ProxyUserName))
                {
                    handler.Proxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                }
            }
            else if (item.WebProxy != null)
            {
                handler.Proxy = item.WebProxy;
            }

            //Cookie容器
            if (item.ResultCookieType == ResultCookieType.CookieCollection)
            {
                handler.CookieContainer = new CookieContainer();
                if (item.CookieCollection != null)
                {
                    handler.CookieContainer.Add(item.CookieCollection);
                }
            }

            return handler;
        }

        /// <summary>
        /// 构建 HttpRequestMessage
        /// </summary>
        private HttpRequestMessage BuildRequest(HttpItem item)
        {
            var request = new HttpRequestMessage(new HttpMethod(item.Method.ToUpper()), item.URL);

            //请求头
            if (!string.IsNullOrWhiteSpace(item.Host)) request.Headers.Host = item.Host;
            if (!string.IsNullOrWhiteSpace(item.Accept)) request.Headers.TryAddWithoutValidation("Accept", item.Accept);
            if (!string.IsNullOrWhiteSpace(item.UserAgent)) request.Headers.TryAddWithoutValidation("User-Agent", item.UserAgent);
            if (!string.IsNullOrWhiteSpace(item.Referer)) request.Headers.TryAddWithoutValidation("Referer", item.Referer);
            if (item.IfModifiedSince != null) request.Headers.IfModifiedSince = item.IfModifiedSince;
            if (!string.IsNullOrWhiteSpace(item.Cookie)) request.Headers.TryAddWithoutValidation("Cookie", item.Cookie);

            //自定义Header
            if (item.Header != null && item.Header.Count > 0)
            {
                foreach (string key in item.Header.AllKeys)
                {
                    if (string.IsNullOrWhiteSpace(key)) continue;
                    //受限头（Host/Content-Length 等）跳过
                    if (key.Equals("Host", StringComparison.OrdinalIgnoreCase)
                        || key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    request.Headers.TryAddWithoutValidation(key, item.Header[key]);
                }
            }

            //POST数据
            if (!request.Method.Equals(HttpMethod.Get) && !request.Method.Equals(HttpMethod.Head))
            {
                Encoding postencoding = item.PostEncoding ?? Encoding.Default;
                if (item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
                {
                    request.Content = new ByteArrayContent(item.PostdataByte);
                }
                else if (item.PostDataType == PostDataType.FilePath && !string.IsNullOrWhiteSpace(item.Postdata))
                {
                    request.Content = new StreamContent(File.OpenRead(item.Postdata));
                }
                else if (!string.IsNullOrWhiteSpace(item.Postdata))
                {
                    request.Content = new StringContent(item.Postdata, postencoding, item.ContentType ?? "text/html");
                }

                if (request.Content != null && !string.IsNullOrWhiteSpace(item.ContentType) && item.PostDataType != PostDataType.String)
                {
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(item.ContentType);
                }
            }

            return request;
        }

        /// <summary>
        /// 读取响应并填充 HttpResult
        /// </summary>
        private void ReadResponse(HttpItem item, HttpResponseMessage response, HttpResult result)
        {
            result.StatusCode = response.StatusCode;
            result.StatusDescription = response.ReasonPhrase;
            result.ResponseUri = response.RequestMessage?.RequestUri?.ToString();

            //Headers
            result.Header = new WebHeaderCollection();
            foreach (var header in response.Headers)
            {
                result.Header[header.Key] = string.Join(",", header.Value);
            }
            foreach (var header in response.Content.Headers)
            {
                result.Header[header.Key] = string.Join(",", header.Value);
            }

            //set-cookie
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                result.Cookie = string.Join(",", setCookies);
            }

            //响应字节
            byte[] ResponseByte = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            if (ResponseByte != null && ResponseByte.Length > 0)
            {
                //编码识别：优先 item.Encoding，其次响应 ContentType 的 charset
                Encoding encoding = item.Encoding;
                if (encoding == null)
                {
                    string contentType = response.Content.Headers.ContentType?.CharSet;
                    if (!string.IsNullOrWhiteSpace(contentType))
                    {
                        try { encoding = Encoding.GetEncoding(contentType); } catch { encoding = Encoding.UTF8; }
                    }
                    else
                    {
                        encoding = Encoding.UTF8;
                    }
                }

                if (item.ResultType == ResultType.Byte)
                {
                    result.ResultByte = ResponseByte;
                }
                result.Html = encoding.GetString(ResponseByte);
            }
            else
            {
                result.Html = string.Empty;
            }
        }
        #endregion
    }

    #region public calss
    /// <summary>
    /// Http请求参考类
    /// </summary>
    public class HttpItem
    {
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL { get; set; }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 100000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 30000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        /// <summary>
        /// 设置Host的标头信息
        /// </summary>
        public string Host { get; set; }
        Boolean _KeepAlive = true;
        /// <summary>
        ///  获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。
        /// </summary>
        public Boolean KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        string _Accept = "text/html, application/json, */*";
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "text/html";
        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get; set; }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata { get; set; }
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; }
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath { get; set; }
        /// <summary>
        /// 设置代理对象，不想使用IE默认配置就设置为Null，而且不要设置ProxyIp
        /// </summary>
        public WebProxy WebProxy { get; set; }
        private Boolean isToLower = false;
        /// <summary>
        /// 是否设置为全文小写，默认为不转化
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        private Boolean allowautoredirect = false;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd { get; set; }
        /// <summary>
        /// 代理 服务IP,如果要使用IE代理就设置为ieproxy
        /// </summary>
        public string ProxyIp { get; set; }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        /// <summary>
        //     获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion { get; set; }
        private Boolean _expect100continue = true;
        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }
        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding { get; set; }
        private ResultCookieType _ResultCookieType = ResultCookieType.String;
        /// <summary>
        /// Cookie返回类型,默认的是只返回字符串类型
        /// </summary>
        public ResultCookieType ResultCookieType
        {
            get { return _ResultCookieType; }
            set { _ResultCookieType = value; }
        }
        private ICredentials _ICredentials = CredentialCache.DefaultCredentials;
        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        public ICredentials ICredentials
        {
            get { return _ICredentials; }
            set { _ICredentials = value; }
        }
        /// <summary>
        /// 设置请求将跟随的重定向的最大数目
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }
        private DateTime? _IfModifiedSince = null;
        /// <summary>
        /// 获取和设置IfModifiedSince，默认为当前日期和时间
        /// </summary>
        public DateTime? IfModifiedSince
        {
            get { return _IfModifiedSince; }
            set { _IfModifiedSince = value; }
        }

    }
    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        private string _html = string.Empty;
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html
        {
            get { return _html; }
            set { _html = value; }
        }
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte { get; set; }
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; }
        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 最后访问的URl
        /// </summary>
        public string ResponseUri { get; set; }
        /// <summary>
        /// 获取重定向的URl
        /// </summary>
        public string RedirectUrl
        {
            get
            {
                try
                {
                    if (Header != null && Header.Count > 0)
                    {
                        if (Header.AllKeys.Any(k => k.ToLower().Contains("location")))
                        {
                            string locationurl = Header["location"].ToString().ToLower();

                            if (!string.IsNullOrWhiteSpace(locationurl))
                            {
                                bool b = locationurl.StartsWith("http://") || locationurl.StartsWith("https://");
                                if (!b)
                                {
                                    locationurl = new Uri(new Uri(ResponseUri), locationurl).AbsoluteUri;
                                }
                            }
                            return locationurl;
                        }
                    }
                }
                catch { }
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 表示只返回字符串 只有Html有数据
        /// </summary>
        String,
        /// <summary>
        /// 表示返回字符串和字节流 ResultByte和Html都有数据返回
        /// </summary>
        Byte
    }
    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串类型，这时编码Encoding可不设置
        /// </summary>
        String,
        /// <summary>
        /// Byte类型，需要设置PostdataByte参数的值编码Encoding可设置为空
        /// </summary>
        Byte,
        /// <summary>
        /// 传文件，Postdata必须设置为文件的绝对路径，必须设置Encoding的值
        /// </summary>
        FilePath
    }
    /// <summary>
    /// Cookie返回类型
    /// </summary>
    public enum ResultCookieType
    {
        /// <summary>
        /// 只返回字符串类型的Cookie
        /// </summary>
        String,
        /// <summary>
        /// CookieCollection格式的Cookie集合同时也返回String类型的cookie
        /// </summary>
        CookieCollection
    }
    #endregion
}
