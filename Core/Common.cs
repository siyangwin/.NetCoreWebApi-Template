using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Core
{
    public static class Common
    {
        public static Dictionary<string, string> areaDc = new Dictionary<string, string>() { { "852", "0" }, { "86", "1" }, { "853", "2" } };
        public static Dictionary<string, string> areaDc2 = new Dictionary<string, string>() { { "0", "852" }, { "1", "86" }, { "2", "853" } };

        //地球半徑，單位米
        private const double EARTH_RADIUS = 6378137;

        /// <summary>
        /// 計算兩點位置的距離，返回兩點的距離，單位：米
        /// 該公式為GOOGLE提供，誤差小于0.2米
        /// </summary>
        /// <param name="lng1">第一點經度</param>
        /// <param name="lat1">第一點緯度</param>        
        /// <param name="lng2">第二點經度</param>
        /// <param name="lat2">第二點緯度</param>
        /// <returns></returns>
        public static double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return Math.Round(result, 2);
        }
        /// <summary>
        /// 經緯度轉化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }
        public static string GetAreaId(string areaCode)
        {
            if (areaDc.ContainsKey(areaCode))
            {
                return areaDc[areaCode];
            }
            return "";
        }
        public static string GetAreaNum(string areaCode)
        {
            if (areaDc2.ContainsKey(areaCode))
            {
                return areaDc2[areaCode];
            }
            return "";
        }
        public static string GetWeekName(string dt)
        {
            string week = string.Empty;
            switch (dt)
            {
                case "Monday":
                    week = "(星期一)";
                    break;
                case "Tuesday":
                    week = "(星期二)";
                    break;
                case "Wednesday":
                    week = "(星期三)";
                    break;
                case "Thursday":
                    week = "(星期四)";
                    break;
                case "Friday":
                    week = "(星期五)";
                    break;
                case "Saturday":
                    week = "(星期六)";
                    break;
                case "Sunday":
                    week = "(星期日)";
                    break;
            }
            return week;
        }

        /// <summary>
        /// 獲的十位隨機數
        /// </summary>
        /// <returns></returns>
        public static string GetRandom(int length = 8)
        {
            var resultStr = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                resultStr.Append(r.Next(0, 10));
            }
            return "89" + resultStr.ToString();
        }



        //生成纯字母随机数
        public static string Str_char(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        /// <summary>
        /// 返回SHA1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSha1(string key)
        {
            //SHA1加密方法
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] str01 = Encoding.Default.GetBytes(key);
            byte[] str02 = sha1.ComputeHash(str01);
            var result = BitConverter.ToString(str02).Replace("-", "");
            return result;
        }


        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="key">传入参数</param>
        /// <returns></returns>
        public static string GetMD5(string key)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                var strResult = BitConverter.ToString(result);
                string result3 = strResult.Replace("-", "");
                return result3;
            }
        }
        #endregion

        #region 参数按照ASCII码从小到大排序（字典序）
        /// <summary>
        /// 参数按照ASCII码从小到大排序（字典序）
        /// </summary>
        /// <param name="paramsMap">参数</param>
        /// <returns></returns>
        public static string getParamSrc(Dictionary<string, string> paramsMap)
        {
            //排序
            var vDic = (from objDic in paramsMap orderby objDic.Key ascending select objDic);
            StringBuilder str = new StringBuilder();
            //拼接
            foreach (KeyValuePair<string, string> kv in vDic)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value;
                str.Append(pkey + "=" + pvalue + "&");
            }
            //去尾
            string result = str.ToString().Substring(0, str.ToString().Length - 1);
            return result;
        }
        #endregion

        #region DES对称加密解密
        /// <summary> 加密字符串
        /// </summary> 
        /// <param name="strText">需被加密的字符串</param> 
        /// <param name="strEncrKey">密钥</param> 
        /// <returns></returns> 
        public static string DesEncrypt(string strText, string strEncrKey)
        {
            try
            {
                byte[] byKey = null;
                //byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                byte[] IV = Encoding.UTF8.GetBytes("f1cb2d32-8cd7-441e-aea0-b5109c78f62f");
                byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        /// <summary> 解密字符串
        /// </summary> 
        /// <param name="strText">需被解密的字符串</param> 
        /// <param name="sDecrKey">密钥</param> 
        /// <returns></returns> 
        public static string DesDecrypt(string strText, string sDecrKey)
        {
            try
            {
                byte[] byKey = null;
                //byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                byte[] IV = Encoding.UTF8.GetBytes("f1cb2d32-8cd7-441e-aea0-b5109c78f62f");
                byte[] inputByteArray = new Byte[strText.Length];

                byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = new UTF8Encoding();
                return encoding.GetString(ms.ToArray());
            }
            catch
            {
                return null;
            }
        }

        #endregion


        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }


        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        //保存上传的文件
        public static string Upload(IFormFile formFile, string savePath)
        {
            string newFileName = "";
            try
            {
                var resName = Guid.NewGuid().ToString("N");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                newFileName = resName + Path.GetExtension(formFile.FileName);
                using (FileStream fs = System.IO.File.Create(savePath + "//" + newFileName))
                {
                    formFile.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return newFileName;
        }
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256EncryptString(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="StrIn">待加密字符串</param>
        /// <returns>加密数组</returns>
        public static Byte[] SHA256EncryptByte(string StrIn)
        {
            var sha256 = new SHA256Managed();
            var Asc = new ASCIIEncoding();
            var tmpByte = Asc.GetBytes(StrIn);
            var EncryptBytes = sha256.ComputeHash(tmpByte);
            sha256.Clear();
            return EncryptBytes;
        }

        /// <summary>
        /// List<T> 轉DataTable
        /// </summary>
        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns></returns>
        public static string getGUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static M ToModel<T, M>(this T t, LanguageEnum lan) where T : class where M : class
        {
            return ToModel<T, M>(t, lan.ToString());
        }

        public static M ToModel<T, M>(this T t, string lan = "") where T : class where M : class
        {
            M model = Activator.CreateInstance<M>();//创建泛型对象
            if (t == null) return null;
            PropertyInfo[] mPros = model.GetType().GetProperties(), tPros = t.GetType().GetProperties();
            Dictionary<string, PropertyInfo> diM = new Dictionary<string, PropertyInfo>(), diT = new Dictionary<string, PropertyInfo>();
            foreach (var item in mPros) if (!diM.ContainsKey(GetPropName(item.Name, lan))) diM.Add(GetPropName(item.Name, lan), item);
            foreach (var item in tPros) if (!diT.ContainsKey(GetPropName(item.Name, lan))) diT.Add(GetPropName(item.Name, lan), item);
            foreach (var item in diM) if (diT.Keys.Contains(item.Key)) item.Value.SetValue(model, diT[item.Key].GetValue(t));
            return model;
        }

        public static List<M> ToModel<T, M>(this IEnumerable<T> t, string lan = "") where T : class where M : class
        {
            return t.Select(model => model.ToModel<T, M>(lan)).ToList();
        }

        public static List<M> ToModel<T, M>(this IEnumerable<T> t, LanguageEnum lan) where T : class where M : class
        {
            return t.Select(model => model.ToModel<T, M>(lan.ToString())).ToList();
        }

        public static T FormatUrl<T>(this T model, string addUrl, string pattam = "ImageUrl") where T : class
        {
            if (model == null) return null;
            PropertyInfo[] tPros = model.GetType().GetProperties();
            foreach (var item in tPros) if (GetPropName(item.Name.ToLower()).Contains(pattam.ToLower())) item.SetValue(model, $"{addUrl}{item.GetValue(model)}");
            return model;
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static T Clone<T>(this T model) where T : class
        {
            if (model == null) return null;
            T modelN = Activator.CreateInstance<T>();//创建泛型对象
            PropertyInfo[] tPros = modelN.GetType().GetProperties();
            foreach (var item in tPros) item.SetValue(modelN, item.GetValue(model));
            return modelN;
        }

        public static int ToInt(this string v)
        {
            return string.IsNullOrEmpty(v) ? 0 : Convert.ToInt32(v);
        }

        public static string ToStr(this object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        public static decimal ToDecimal(this object v)
        {
            return string.IsNullOrEmpty(v.ToString()) ? 0 : Convert.ToDecimal(v);
        }

        public static string GetPropName(string name, string lan = "")
        {
            return string.IsNullOrEmpty(lan) ? name.Replace("_", "").ToLower() : name.Replace(lan, "").Replace("_", "").ToLower();
        }

        /// <summary>
        /// 字符模板格式化对象
        /// </summary>
        /// <param name="input"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FormatString(string input, object obj)
        {
            Type type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                string placeholder = $"{{{property.Name}}}";
                object value = property.GetValue(obj);

                if (value != null)
                {
                    input = input.Replace(placeholder, value.ToString());
                }
            }
            return input;
        }

        /// <summary>
        /// DataTable转对象List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            List<T> modelList = new List<T>();
            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();
                foreach (DataColumn col in dataTable.Columns)
                {
                    string propertyName = col.ColumnName;
                    var property = typeof(T).GetProperty(propertyName);
                    object propertyValue = row[col];
                    // 使用反射设置属性的值
                    property?.SetValue(obj, Convert.ChangeType(propertyValue, property.PropertyType));
                }

                modelList.Add(obj);
            }
            return modelList;
        }

        public static T IfNull<T>(this object obj, Func<object, T> funcSuccess, Func<object, T> funcFail)
        {
            return obj == null ? funcSuccess(obj) : funcFail(obj);
        }

    }
}
