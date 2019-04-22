using Newtonsoft.Json.Linq;
using Quartz.Net.Demo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Quartz.Net.Demo
{
    public class ClockGo
    {
        private static readonly string PostURL = ConfigurationManager.ConnectionStrings["PostURL"].ConnectionString;
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private string str = HttpPost(@"http://203.160.52.218:810/log.php", "gh=320821611303161");
        public void Main()
        {
            ClockInEntity Db = new ClockInEntity();
            DateTime _now = DateTime.Now;
            string result = string.Empty;
            List<ClockModels> models = new List<ClockModels>();
            bool isSta = _now.DayOfWeek.ToString() == "Saturday";
            bool isSun = _now.DayOfWeek.ToString() == "Sunday";
            JObject StrJson;
            IEnumerable<JToken> JsonListcode;
            IEnumerable<JToken> JsonListmsg;

            var data1 = from r in Db.ClockModels.ToList()
                        where (r.flag == true && (r.ClockStateAM == false || r.ClockStatePM == false))
                        select new
                        {
                            r.CardId,//工号
                            r.PassWord,
                            r.LastClockTime,
                            r.ClockStateAM,//打卡状态
                            r.ClockStatePM,
                            OnTimeStart = r.OnTimeStart.ToShortTimeString(),//上班打卡开始时间
                            OnTimeEnd = r.OnTimeEnd.ToShortTimeString(),//上班打卡结束时间
                            OffTimeStart = r.OffTimeStart.ToShortTimeString(),//下班打卡开始时间
                            OffTimeEnd = r.OffTimeEnd.ToShortTimeString(),//下班打卡结束时间
                            r.NeedSta,//周六打卡
                            r.NeedSun,//周日打卡
                            r.FailReason
                        };
            foreach (var item in data1.OrderBy(p => Guid.NewGuid()))
            {
                //判断当前用户“今天”需不需要打卡
                if ((item.NeedSta && isSta) || (item.NeedSun && isSun) || (!isSta && !isSun))
                {
                    //判断“现在”是不是该用户打上班卡的时间
                    if (getTimeSpan(item.OnTimeStart, item.OnTimeEnd) && !item.ClockStateAM)
                    {
                        result = HttpPost(PostURL, "user=" + item.CardId + "&pass=" + item.PassWord);

                        var Cloc = (from r in Db.ClockModels
                                    where r.CardId == item.CardId
                                    select r).FirstOrDefault();
                        //网站有返回不规范情况
                        try
                        {
                            StrJson = JObject.Parse(result);
                            JsonListcode = StrJson["code"].AsEnumerable();
                            JsonListmsg = StrJson["msg"].AsEnumerable();
                        }
                        catch (Exception ex)
                        {
                            Cloc.FailReason += Unicode2String(result).ToString() + "JSON解析错误" + DateTime.Now.ToString();
                            if (Unicode2String(result).Contains("0"))   //网站是通过CODE=0 来判断是否成功
                            {
                                Cloc.LastClockTime = DateTime.Now;
                                Cloc.ClockStateAM = true;
                            }
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            Db.SaveChanges();
                            continue;
                        }
                        if (JsonListcode.ToString().Contains("0"))
                        {
                            Cloc.LastClockTime = DateTime.Now;
                            Cloc.ClockStateAM = true;
                            Cloc.FailReason = null;
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            //一次性 生成sql语句到数据库执行            
                            Db.SaveChanges();
                            Thread.Sleep(300);
                        }
                        else
                        {
                            Cloc.FailReason += StrJson.ToString() + DateTime.Now.ToString();
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            Db.SaveChanges();
                        }
                    }
                    //判断“现在”是不是该用户打下班卡的时间
                    if (getTimeSpan(item.OffTimeStart, item.OffTimeEnd) && !item.ClockStatePM)
                    {
                        result = HttpPost(PostURL, "user=" + item.CardId + "&pass=" + item.PassWord);
                        var Cloc = (from r in Db.ClockModels
                                    where r.CardId == item.CardId
                                    select r).FirstOrDefault();
                        //网站有返回不规范情况
                        try
                        {
                            StrJson = JObject.Parse(result);
                            JsonListcode = StrJson["code"].AsEnumerable();
                            JsonListmsg = StrJson["msg"].AsEnumerable();
                        }
                        catch (Exception ex)
                        {
                            Cloc.FailReason += Unicode2String(result).ToString() + "JSON解析错误" + DateTime.Now.ToString();
                            if (Unicode2String(result).Contains("0"))   //网站是通过CODE=0 来判断是否成功
                            {
                                Cloc.LastClockTime = DateTime.Now;
                                Cloc.ClockStatePM = true;
                            }
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;   
                            Db.SaveChanges();
                            continue;
                        }
                        if (JsonListcode.ToString().Contains("0"))
                        {
                            Cloc.LastClockTime = DateTime.Now;
                            Cloc.ClockStatePM = true;
                            Cloc.FailReason = null;
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            Db.SaveChanges();
                            Thread.Sleep(300);
                        }
                        else
                        {
                            Cloc.FailReason += StrJson.ToString() + DateTime.Now.ToString();
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            Db.SaveChanges();
                        }
                    }
                }

            }
        }
        //判断当前时间是否在工作时间段内
        public void backup()
        {
            //_dicCookie.Clear();
            //_dicClassID.Clear();
            //_dicSucClass.Clear();
            //ClockInEntity Db = new ClockInEntity();
            //IEnumerable<ClockBatch> allData = (from r in Db.ClockBatch
            //                                   where r.flag == true
            //                                   select r).ToList();
            ////.Where(r => _now.Subtract(Convert.ToDateTime(r.LastClockTime)).TotalHours > 1);
            ////.Where(r => _now.TimeOfDay.TotalHours-  Convert.ToDateTime(Convert.ToDateTime(r.LastClockTime).ToShortTimeString()).TimeOfDay.TotalHours > 1);
            //var ClassName = "";
            //JObject studentsJson = null;
            //string strResult1 = "";
            //string strResult2 = "";
            //IEnumerable<JToken> studentsList;
            ////获取所有班级
            //var temp = allData.GroupBy(x => x.ClassName).Select(g => new { g.Key }).ToList();
            //List<string> allClass = new List<string>();
            //temp.ForEach(x => _dicSucClass.TryAdd(x.Key, 2));
            //int thread = int.TryParse(ThreadCount, out int count) ? count : 3;
            //try
            //{
            //    Parallel.ForEach(allData.OrderBy(P => Guid.NewGuid()), new ParallelOptions() { MaxDegreeOfParallelism = thread }, (model, state) =>
            //    {
            //        try
            //        {
            //            //该班失败的次数大于成功的次数 会停止该班的打卡
            //            _dicSucClass.TryGetValue(model.ClassName, out int b);
            //            if (b <= 0) return;
            //            if (model.StartClockTime == null || DateTime.Now.Subtract(Convert.ToDateTime(model.LastClockTime)).TotalHours < 1)
            //                return;
            //            if (DateTime.Parse(Convert.ToDateTime(model.StartClockTime.ToString()).ToShortTimeString()).TimeOfDay > DateTime.Now.TimeOfDay)
            //                return;
            //            ClassName = System.Text.RegularExpressions.Regex.Replace(model.ClassName, @"[^0-9]+", "");
            //            var response1 = HttpPost(posturl1, "idcard=" + model.CardId);
            //            strResult1 = Unicode2String(response1);//正常字符串结果
            //            if (strResult1.Contains("第一步成功"))
            //            {
            //                try
            //                {
            //                    studentsJson = JObject.Parse(response1);
            //                }
            //                catch (Exception ex)
            //                {
            //                    logger.Warn($"第一步网站JSON返回解析错误{strResult1}");
            //                    model.ClockState = false;
            //                    model.FailedReason += strResult1 + DateTime.Now;
            //                    Db.ClockBatch.Attach(model);
            //                    Db.Entry<ClockBatch>(model).State = EntityState.Modified;
            //                    Db.SaveChanges();
            //                    _dicSucClass[model.ClassName]--;
            //                    return;
            //                }
            //                studentsList = studentsJson["data"]["classes"].AsEnumerable();
            //                foreach (var item in studentsList)
            //                {
            //                    //找到班级名称对应的班级ID 放入班级ID字典  删除“2019”再做数字匹配
            //                    if (ClassName.Equals(Regex.Replace(item["tname"].ToString().Replace("2019", ""), @"[^0-9]+", "")))
            //                    {
            //                        if (!_dicClassID.ContainsKey(model.ClassName))
            //                        {
            //                            _dicClassID.TryAdd(model.ClassName, item["id"].ToString());//key用model.ClassName是因为有：不同老师的班，但是数字相同的情况
            //                            break;
            //                        }
            //                    };
            //                }
            //                if (!_dicClassID.ContainsKey(model.ClassName))
            //                {
            //                    model.FailedReason += $"打卡失败，请该工号{model.CardId}是否在该班级";
            //                    Db.ClockBatch.Attach(model);
            //                    Db.Entry<ClockBatch>(model).State = EntityState.Modified;
            //                    Db.SaveChanges();
            //                    _dicSucClass[model.ClassName]--;
            //                    return;
            //                }

            //                var response2 = HttpPost(posturl2, "class_id=" + _dicClassID[model.ClassName]
            //                 , new Dictionary<string, string>()
            //                 {
            //                { "cookie", _dicCookie[string.Format("idcard={0}",model.CardId)] }
            //                 });
            //                try
            //                {
            //                    strResult2 = JObject.Parse(response2).ToString();
            //                }
            //                catch (Exception)
            //                {
            //                    logger.Warn($"第二步网站JSON返回解析错误{Unicode2String(response2)}");
            //                    _dicSucClass[model.ClassName]--;
            //                    return;
            //                }
            //                if (strResult2.Contains("成功"))
            //                {
            //                    model.LastClockTime = DateTime.Now;
            //                    model.ClockState = true;
            //                    model.FailedReason = "";
            //                    model.Times++;
            //                    Db.ClockBatch.Attach(model);
            //                    Db.Entry<ClockBatch>(model).State = EntityState.Modified;
            //                    Db.SaveChanges();
            //                    _dicSucClass[model.ClassName]++; //此次操作成功次数
            //                    Thread.Sleep(new Random().Next(2000, 5000));
            //                }
            //                else if ((strResult2.Contains("已签")))
            //                {
            //                    if (DateTime.Now.Subtract(Convert.ToDateTime(model.LastClockTime)).TotalHours > 0.5)
            //                        model.Times++;
            //                    model.LastClockTime = DateTime.Now;
            //                    model.ClockState = true;
            //                    model.FailedReason += JObject.Parse(response2)["msg"].ToString() + DateTime.Now.ToString();
            //                    Db.ClockBatch.Attach(model);
            //                    Db.Entry<ClockBatch>(model).State = EntityState.Modified;
            //                    Db.SaveChanges();
            //                    _dicSucClass[model.ClassName]++; //此次操作成功次数
            //                }
            //                else
            //                {
            //                    model.ClockState = false;
            //                    model.FailedReason += JObject.Parse(response2)["msg"].ToString() + DateTime.Now.ToString();
            //                    Db.ClockBatch.Attach(model);
            //                    Db.Entry<ClockBatch>(model).State = EntityState.Modified;
            //                    Db.SaveChanges();
            //                    //_dicSucClass.TryRemove(model.ClassName, out int aa);
            //                    _dicSucClass[model.ClassName]--;

            //                }
            //            }
            //            else
            //            {
            //                model.ClockState = false;
            //                model.FailedReason += strResult1 + DateTime.Now;
            //                Db.ClockBatch.Attach(model);
            //                Db.Entry<ClockBatch>(model).State = EntityState.Modified;
            //                Db.SaveChanges();
            //                //_dicSucClass.TryRemove(model.ClassName, out bool bb);
            //                _dicSucClass[model.ClassName]--;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            logger.Fatal(ex);
            //            return;
            //        }

            //    });
            //}
            //catch (Exception ex)
            //{
            //    logger.Fatal(ex);
            //}
        }
        protected bool getTimeSpan(string _strWorkingDayAM, string _strWorkingDayPM)
        {
            TimeSpan dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            TimeSpan dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;

            DateTime t1 = DateTime.Now;

            TimeSpan dspNow = t1.TimeOfDay;
            if (dspNow > dspWorkingDayAM && dspNow < dspWorkingDayPM)
            {
                return true;
            }
            return false;
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
                }
            }
            catch (Exception ex)
            { }

            return result;
        }
    }
}
