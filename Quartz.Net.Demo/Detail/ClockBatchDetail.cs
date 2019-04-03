using Newtonsoft.Json.Linq;
using Quartz.Net.Demo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Net.Demo.Detail
{
   public class ClockBatchDetail
    {
        private static  Dictionary<string, string> _dicCookie = new Dictionary<string, string>();
        private static Dictionary<string, string> _dicClassID = new Dictionary<string, string>();
        private static readonly string posturl1 = ConfigurationManager.ConnectionStrings["PostUrl1"].ConnectionString;
        private static readonly string posturl2 = ConfigurationManager.ConnectionStrings["PostUrl2"].ConnectionString;
        public void ClockBatchGo()
        {
            DateTime _now = DateTime.Now;
            ClockInEntity Db = new ClockInEntity();
            IEnumerable<ClockBatch> allData = (from r in Db.ClockBatch
                                               where r.flag == true
                                               select r).ToList();
            //.Where(r => _now.Subtract(Convert.ToDateTime(r.LastClockTime)).TotalHours > 1);
            //.Where(r => _now.TimeOfDay.TotalHours-  Convert.ToDateTime(Convert.ToDateTime(r.LastClockTime).ToShortTimeString()).TimeOfDay.TotalHours > 1);
            var ClassName = "";
            JObject studentsJson;
            IEnumerable<JToken> studentsList;
            foreach (var model in allData.OrderBy(P => Guid.NewGuid()))
            {
                if (model.StartClockTime == null || _now.Subtract(Convert.ToDateTime(model.LastClockTime)).TotalHours < 1)
                    continue;
                if (DateTime.Parse(Convert.ToDateTime(model.StartClockTime.ToString()).ToShortTimeString()).TimeOfDay > _now.TimeOfDay)
                    continue;
                ClassName = System.Text.RegularExpressions.Regex.Replace(model.ClassName, @"[^0-9]+", "");
                var response1 = HttpPost(posturl1, "idcard=" + model.CardId);
                studentsJson = JObject.Parse(response1);
                if (studentsJson.ToString().Contains("第一步成功"))
                {
                    studentsList = studentsJson["data"]["classes"].AsEnumerable();
                    foreach (var item in studentsList)
                    {
                        //找到班级名称对应的班级ID 放入班级ID字典  从右边取10个字符，然后正则取纯数字。OS：我是被逼的
                        if (ClassName.Equals(System.Text.RegularExpressions.Regex.Replace(item["tname"].ToString().Remove(0, item["tname"].ToString().Length - 10), @"[^0-9]+", "")))
                        {
                            if (!_dicClassID.ContainsKey(model.ClassName))
                            {
                                _dicClassID.Add(ClassName, item["id"].ToString());
                                break;
                            }
                        };
                    }
                    if (!_dicClassID.ContainsKey(ClassName))
                    {
                        //logger.Error($"打卡失败，请该工号{model.CardId}是否在该班级");
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
                        //if (allData.IndexOf(model) == 0)
                        //    break;
                    }
                }
                else
                {
                    model.ClockState = false;
                    model.FailedReason = JObject.Parse(response1)["msg"].ToString() + DateTime.Now.ToString();
                    Db.ClockBatch.Attach(model);
                    Db.Entry<ClockBatch>(model).State = EntityState.Modified;
                    Db.SaveChanges();
                    //if (allData.IndexOf(model) == 0)
                    //    break;
                }

            }
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
                        _dicCookie.Remove(paramData);
                        _dicCookie.Add(paramData, wbResponse.GetResponseHeader("Set-Cookie"));
                    }
                    _dicCookie.Add(paramData, wbResponse.GetResponseHeader("Set-Cookie"));
                }
            }
            catch (Exception ex)
            { }

            return result;
        }
    }
}
