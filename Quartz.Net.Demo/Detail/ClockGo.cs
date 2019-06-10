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

        private static int span;
        private static int SleepSpan
        {
            get
            {
                if (span == 0)
                {
                    if (int.TryParse(ConfigurationManager.ConnectionStrings["SleepSpan"].ConnectionString, out span))
                    {
                        return span;
                    }
                    return span = 300;

                }
                return span;
            }
            set { }
        }


        //private string str = HttpPost(@"http://203.160.52.218:810/log.php", "gh=320821611303161");
        public void Main()
        {
            ClockInEntity Db = new ClockInEntity();
            DateTime _now = DateTime.Now;
            string result = string.Empty;
            List<ClockModels> models = new List<ClockModels>();
            bool isSta = _now.DayOfWeek.ToString() == "Saturday";
            bool isSun = _now.DayOfWeek.ToString() == "Sunday";
            if (Int32.Parse(_now.ToString("HH")) >= 6 && Int32.Parse(_now.ToString("HH")) <= 9)
                SleepSpan = 500;
            int erro = 0;
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
                try
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
                                logger.Error($"工号id{item.CardId}打卡出错：{ex}");
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
                                Thread.Sleep(SleepSpan);
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
                                logger.Error($"工号id{item.CardId}打卡出错：{ex}");
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
                                Thread.Sleep(SleepSpan);
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
                catch (Exception ex)
                {
                    erro++;
                    logger.Error($"此次错误次数{erro}，工号id{item.CardId}，打卡出错：{ex}");
                    continue;
                }
            }
        }

        //判断当前时间是否在工作时间段内
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
            {
                logger.Error($"POST方法出错：{ex}参数:{headerDic.ToString()}");
            }

            return result;
        }
    }
}
