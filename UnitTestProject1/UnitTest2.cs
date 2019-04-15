using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Quartz.Net.Demo;
using Quartz.Net.Demo.Models;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest2
    {
        private static ConcurrentDictionary<string, string> _dicCookie = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> _dicClassID = new ConcurrentDictionary<string, string>();
        private static System.Collections.Concurrent.ConcurrentDictionary<string, Boolean> _dicSucClass = new System.Collections.Concurrent.ConcurrentDictionary<string, Boolean>();
        private static readonly string posturl1 = ConfigurationManager.ConnectionStrings["PostUrl1"].ConnectionString;
        private static readonly string posturl2 = ConfigurationManager.ConnectionStrings["PostUrl2"].ConnectionString;
        [TestMethod]
        public void TestMethod1()
        {
            ClockBatchGo111111111111();
        }

        [TestMethod]
        public void TestMethod222()
        {
            new ClockGo().Main();
        }
        

        public void ClockBatchGo111111111111()
        {

            DateTime _now = DateTime.Now;
            ClockInEntity Db = new ClockInEntity();
            IEnumerable<ClockBatch> allData = (from r in Db.ClockBatch
                                               where r.flag == true
                                               select r).ToList();
            //.Where(r => _now.Subtract(Convert.ToDateTime(r.LastClockTime)).TotalHours > 1);
            //.Where(r => _now.TimeOfDay.TotalHours-  Convert.ToDateTime(Convert.ToDateTime(r.LastClockTime).ToShortTimeString()).TimeOfDay.TotalHours > 1);
            var ClassName = "";
            JObject studentsJson = null;
            string strResult = "";
            IEnumerable<JToken> studentsList;
            //获取所有班级
            var temp = allData.GroupBy(x => x.ClassName).Select(g => new { g.Key }).ToList();
            List<string> allClass = new List<string>();
            temp.ForEach(x => _dicSucClass.TryAdd(x.Key, true));
            try
            {
                foreach (var model in allData.OrderBy(P => Guid.NewGuid()))
                {
                    _dicSucClass.TryGetValue(model.ClassName, out bool b);
                    if (!b) continue;
                    if (model.StartClockTime == null || _now.Subtract(Convert.ToDateTime(model.LastClockTime)).TotalHours < 1)
                        continue;
                    if (DateTime.Parse(Convert.ToDateTime(model.StartClockTime.ToString()).ToShortTimeString()).TimeOfDay > _now.TimeOfDay)
                        continue;

                    ClassName = System.Text.RegularExpressions.Regex.Replace(model.ClassName, @"[^0-9]+", "");
                    var response1 = HttpPost(posturl1, "idcard=" + model.CardId);
                    strResult = Unicode2String(response1);//正常字符串结果
                    if (strResult.Contains("第一步成功"))
                    {
                        try
                        {
                            studentsJson = JObject.Parse(response1);
                        }
                        catch (Exception)
                        {
                            model.ClockState = false;
                            model.FailedReason = strResult;
                            Db.ClockBatch.Attach(model);
                            Db.Entry<ClockBatch>(model).State = EntityState.Modified;
                            Db.SaveChanges();
                            continue;
                        }
                        studentsList = studentsJson["data"]["classes"].AsEnumerable();
                        foreach (var item in studentsList)
                        {
                            //找到班级名称对应的班级ID 放入班级ID字典  从右边取10个字符，然后正则取纯数字。OS：我是被逼的
                            if (ClassName.Equals(System.Text.RegularExpressions.Regex.Replace(item["tname"].ToString().Remove(0, item["tname"].ToString().Length - 10), @"[^0-9]+", "")))
                            {
                                if (!_dicClassID.ContainsKey(model.ClassName))
                                {
                                    _dicClassID.TryAdd(ClassName, item["id"].ToString());
                                    break;
                                }
                            };
                        }
                        if (!_dicClassID.ContainsKey(ClassName))
                        {
                            //logger.Warn($"打卡失败，请该工号{model.CardId}是否在该班级");
                            continue;
                        }

                        var response2 = HttpPost(posturl2, "class_id=" + _dicClassID[ClassName]
                         , new Dictionary<string, string>()
                         {
                            { "cookie", _dicCookie[string.Format("idcard={0}",model.CardId)] }
                         });
                        if (JObject.Parse(response2).ToString().Contains("成功"))
                        {
                            model.LastClockTime = DateTime.Now;
                            model.ClockState = true;
                            model.FailedReason = "";
                            model.Times++;
                            Db.ClockBatch.Attach(model);
                            Db.Entry<ClockBatch>(model).State = EntityState.Modified;
                            Db.SaveChanges();
                            allClass.Remove(model.ClassName);
                            Thread.Sleep(new Random().Next(3000, 6000));
                        }
                        else if ((JObject.Parse(response2).ToString().Contains("已签到")))
                        {
                            model.ClockState = false;
                            model.FailedReason = JObject.Parse(response2)["msg"].ToString() + DateTime.Now.ToString();
                            Db.ClockBatch.Attach(model);
                            Db.Entry<ClockBatch>(model).State = EntityState.Modified;
                            Db.SaveChanges();
                        }
                        else
                        {
                            model.ClockState = false;
                            model.FailedReason = JObject.Parse(response2)["msg"].ToString() + DateTime.Now.ToString();
                            Db.ClockBatch.Attach(model);
                            Db.Entry<ClockBatch>(model).State = EntityState.Modified;
                            Db.SaveChanges();
                            _dicSucClass.TryRemove(model.ClassName, out bool aa);

                        }
                    }
                    else
                    {
                        model.ClockState = false;
                        model.FailedReason = strResult;
                        Db.ClockBatch.Attach(model);
                        Db.Entry<ClockBatch>(model).State = EntityState.Modified;
                        Db.SaveChanges();
                        _dicSucClass.TryRemove(model.ClassName, out bool bb);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// <summary>
        /// 字符串转Unicode
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>Unicode编码后的字符串</returns>
        public static string String2Unicode(string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        public static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
        public static string HttpPost(string url, string paramData, Dictionary<string, string> headerDic = null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Method = "POST";
                wbRequest.ContentType = "application/x-www-form-urlencoded";
                wbRequest.ContentLength = Encoding.UTF8.GetByteCount(paramData);
                if (headerDic != null && headerDic.Count > 0)
                {
                    foreach (var item in headerDic)
                    {
                        wbRequest.Headers.Add(item.Key, item.Value);
                    }
                }
                using (Stream requestStream = wbRequest.GetRequestStream())
                {
                    using (StreamWriter swrite = new StreamWriter(requestStream))
                    {
                        swrite.Write(paramData);
                    }
                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sread = new StreamReader(responseStream))
                    {
                        result = sread.ReadToEnd();
                    }
                    if (_dicCookie.ContainsKey(paramData))
                    {
                        _dicCookie.TryRemove(paramData, out string val);
                        _dicCookie.TryAdd(paramData, wbResponse.GetResponseHeader("Set-Cookie"));
                    }
                    _dicCookie.TryAdd(paramData, wbResponse.GetResponseHeader("Set-Cookie"));
                }
            }
            catch (Exception ex)
            { }

            return result;
        }

    }
}
