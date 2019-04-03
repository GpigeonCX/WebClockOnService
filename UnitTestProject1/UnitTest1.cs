using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Quartz.Net.Demo;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Wocao()
        {
            List<User> list = new List<User>()
        {
            new User(){CredentialsID=3,UserID=12,CredentialsRankID=2,ProfessionID=1},
            new User(){CredentialsID=4,UserID=12,CredentialsRankID=2,ProfessionID=2},
            new User(){CredentialsID=5,UserID=12,CredentialsRankID=3,ProfessionID=1},
            new User(){CredentialsID=6,UserID=1,CredentialsRankID=2,ProfessionID=1},
            new User(){CredentialsID=13,UserID=9,CredentialsRankID=2,ProfessionID=1},
            new User(){CredentialsID=17,UserID=12,CredentialsRankID=3,ProfessionID=2},
        };
            var a = list.GroupBy(x => x.CredentialsRankID).Select(g => new { g.Key }).ToList();
            //bool b = a.Contains('2');

            //a.Remove(2);

            var b = a;
            Console.ReadKey();
            //new ClockReset().Reset();
            //new ClockGo().Main();
        }
        [TestMethod]
        public void EFgroupby() {
            List<User> list = new List<User>()
        {
            new User(){CredentialsID=3,UserID=12,CredentialsRankID=2,ProfessionID=1},
            new User(){CredentialsID=4,UserID=12,CredentialsRankID=2,ProfessionID=2},
            new User(){CredentialsID=5,UserID=12,CredentialsRankID=3,ProfessionID=1},
            new User(){CredentialsID=6,UserID=1,CredentialsRankID=2,ProfessionID=1},
            new User(){CredentialsID=13,UserID=9,CredentialsRankID=2,ProfessionID=1},
            new User(){CredentialsID=17,UserID=12,CredentialsRankID=3,ProfessionID=2},
        };
            var a = list.GroupBy(x => x.CredentialsRankID).Select(g => new { g });
            var b = a;
        }
        class User
        {
            public int CredentialsID{get;set;}

            public int UserID { get; set; }
            public int CredentialsRankID { get; set; }
            public int ProfessionID { get; set; }
        }
        [TestMethod]
        public void TestMethod2()
        {
            DateTime OnTimeStart = Convert.ToDateTime("2000-1-1 18:30:00");
            DateTime OnTimeEnd = Convert.ToDateTime("2019-1-1 20:30:00");
            string nowww = DateTime.Now.ToShortTimeString();
            bool b = nowww.CompareTo(OnTimeStart.ToShortTimeString()) >= 0 &&
                nowww.CompareTo(OnTimeEnd.ToShortTimeString()) <= 0;
        }
        /// <summary> 
        /// 判断某个日期是否在某段日期范围内，返回布尔值
        /// </summary> 
        /// <param name="dt">要判断的日期</param> 
        /// <param name="dt1">开始日期</param> 
        /// <param name="dt2">结束日期</param> 
        /// <returns></returns>  
        private bool IsInDate(DateTime dt, DateTime dt1, DateTime dt2)
        {
            return dt.CompareTo(dt1) >= 0 && dt.CompareTo(dt2) <= 0;
        }
        [TestMethod]
        public void TestMethod1()
        {
           string result1 = HttpPost(@"http:// /run.php", "user=" + "11" + "&pass=" + "666666AB");
            string result2 = HttpPost(@"http:// /run.php", "user=" + "51011100004990" + "&pass=" + "111");
            string result3 = HttpPost(@"http:// /run.php", "user=" + "11" );
            string result4 = HttpPost(@"http:// /run.php", "pass=" + "11");
            string result5 = HttpPost(@"http:// /run.php", "");
            var studentsJson = JObject.Parse(result4);
            var studentsList = studentsJson["code"].AsEnumerable();
            bool a= studentsList.ToString().Contains("0");
            Console.WriteLine(result4);
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
