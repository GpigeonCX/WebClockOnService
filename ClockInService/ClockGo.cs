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
using System.Threading;
using System.Threading.Tasks;

namespace ClockInService
{
    public class ClockGo
    {
        private static readonly string PostURL = ConfigurationManager.ConnectionStrings["PostURL"].ConnectionString;
        //private string str = HttpPost(@"http://203.160.52.218:810/log.php", "gh=320821611303161");
        public void Main()
        {
            ClockInEntity Db = new ClockInEntity();
            DateTime _now = DateTime.Now;
            string result = string.Empty;
            List<ClockModels> models = new List<ClockModels>();
            bool isSta = _now.DayOfWeek.ToString() == "Saturday";
            bool isSun = _now.DayOfWeek.ToString() == "Sunday";

            var data1 = from r in Db.ClockModels.ToList()
                        where (r.flag == true && (r.ClockStateAM == false || r.ClockStatePM == false))
                        select new
                        {
                            r.CardId,//工号
                            r.LastClockTime,
                            r.ClockStateAM,//打卡状态
                            r.ClockStatePM,
                            OnTimeStart = r.OnTimeStart.ToShortTimeString(),//上班打卡开始时间
                            OnTimeEnd = r.OnTimeEnd.ToShortTimeString(),//上班打卡结束时间
                            OffTimeStart = r.OffTimeStart.ToShortTimeString(),//下班打卡开始时间
                            OffTimeEnd = r.OffTimeEnd.ToShortTimeString(),//下班打卡结束时间
                            r.NeedSta,//周六打卡
                            r.NeedSun//周日打卡
                        };
            foreach (var item in data1.ToList().OrderBy(p => Guid.NewGuid()))
            {
                //判断当前用户“今天”需不需要打卡
                if ((item.NeedSta && isSta) || (item.NeedSun && isSun) || (!item.NeedSta && !item.NeedSun && !isSta && !isSun))
                {
                    //判断“现在”是不是该用户打上班卡的时间
                    if (getTimeSpan(item.OnTimeStart, item.OnTimeEnd) && !item.ClockStateAM)
                    {
                        result = HttpPost(PostURL, "gh=" + item.CardId);
                        if (result.Contains("true"))
                        {
                            var data = from r in Db.ClockModels
                                       where r.CardId == item.CardId
                                       select r;
                            ClockModels Cloc = data.FirstOrDefault<ClockModels>();
                            Cloc.LastClockTime = DateTime.Now;
                            Cloc.ClockStateAM = true;
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            //一次性 生成sql语句到数据库执行            
                            Db.SaveChanges();
                            Thread.Sleep(1000);
                        }
                    }
                    //判断“现在”是不是该用户打下班卡的时间
                    if (getTimeSpan(item.OffTimeStart, item.OffTimeEnd) && !item.ClockStatePM)
                    {
                        result = HttpPost(PostURL, "gh=" + item.CardId);
                        if (result.Contains("true"))
                        {
                            var data = from r in Db.ClockModels
                                       where r.CardId == item.CardId
                                       select r;
                            ClockModels Cloc = data.FirstOrDefault<ClockModels>();
                            Cloc.LastClockTime = DateTime.Now;
                            Cloc.ClockStatePM = true;
                            Db.Entry<ClockModels>(Cloc).State = System.Data.Entity.EntityState.Modified;
                            //一次性 生成sql语句到数据库执行            
                            Db.SaveChanges();
                            Thread.Sleep(1000);
                        }
                    }
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
