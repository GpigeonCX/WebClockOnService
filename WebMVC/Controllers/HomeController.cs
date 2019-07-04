using Common;
using EntityFramework.Utilities;
using MvcUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        ClockInEntity Db = new ClockInEntity();
        public ActionResult GetEmployee()
        {
            try
            {
                int pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
                int pageSize = Request["rows"] == null ? 50 : int.Parse(Request["rows"]);
                var data1 = from r in Db.ClockModels.ToList()
                            select new //待优化，格式化放到前台去处理，速度会快
                            {
                                CardId = r.CardId,//工号
                                EmployeeName = r.EmployeeName,//名称
                                LastClockTime = r.LastClockTime.ToString(),//上次打卡时间
                                ClockStateAM = r.ClockStateAM,//打卡状态
                                ClockStatePM = r.ClockStatePM,
                                TotalDays = r.TotalDays,//打卡天数
                                SurplusDay = r.CreatTime.AddDays(r.TotalDays).Subtract(DateTime.Now).Days,//剩余天数
                                OnTimeStart = r.OnTimeStart.ToString(),//上班打卡开始时间
                                OnTimeEnd = r.OnTimeEnd.ToString(),//上班打卡结束时间
                                OffTimeStart = r.OffTimeStart.ToString(),//下班打卡开始时间
                                OffTimeEnd = r.OffTimeEnd.ToString(),//下班打卡结束时间
                                NeedSta = r.NeedSta,//周六打卡
                                NeedSun = r.NeedSun,//周日打卡
                                r.flag,
                                r.PassWord,
                            };
                // ).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                if (bool.TryParse((Request["Stop"]), out bool isStop))
                    data1 = isStop ? data1.Where(r => r.flag == false) : data1;//没有选中给有所数据
                if (!string.IsNullOrEmpty(Request["SuplDay"]) && int.TryParse(Request["SuplDay"], out int sday))
                {
                    data1 = isStop ? data1 : data1.Where(r => r.flag == true);
                    sday = sday < 0 ? 0 : (sday > 30 ? 30 : sday);
                    data1 = data1.Where(r => r.SurplusDay <= sday).OrderBy(da => da.SurplusDay);
                }
                if (!string.IsNullOrEmpty(Request["EmployeeName"]))
                    data1 = data1.Where(r => r.EmployeeName.Contains(Request["EmployeeName"].ToString().Trim()));
                if (!string.IsNullOrEmpty(Request["CardId"]))
                    data1 = data1.Where(r => r.CardId.Equals(Request["CardId"].ToString().Trim()));

                int total = data1.Count();//总条数
                                          //构造成Json的格式传递

                var result = new { total, rows = data1.Skip(pageSize * (pageIndex - 1)).Take(pageSize) };

                return Json(result, JsonRequestBehavior.AllowGet);
                #region 遗弃代码
                //if (!string.IsNullOrEmpty(Request["EmployeeName"]) && !string.IsNullOrEmpty(Request["CardId"]))
                //{
                //    EmployeeName = Request["EmployeeName"].ToString();
                //    CardId = Request["CardId"].ToString();

                //    var data1 = (from r in Db.ClockModels.ToList()
                //                 where r.EmployeeName.Contains(EmployeeName) && r.CardId.Equals(CardId)
                //                 select new
                //                 {
                //                     CardId = r.CardId,//工号
                //                     EmployeeName = r.EmployeeName,//名称
                //                     LastClockTime = r.LastClockTime.ToString().CompareTo("2010-1-1 0:00:00") > 0 ? r.LastClockTime.ToString() : "",//上次打卡时间
                //                     ClockStateAM = r.ClockStateAM,//打卡状态
                //                     ClockStatePM = r.ClockStatePM,
                //                     TotalDays = r.TotalDays,//打卡天数
                //                     SurplusDay = r.CreatTime.AddDays(r.TotalDays).Subtract(DateTime.Now).Days,//剩余天数
                //                     OnTimeStart = r.OnTimeStart.ToString(),//上班打卡开始时间
                //                     OnTimeEnd = r.OnTimeEnd.ToString(),//上班打卡结束时间
                //                     OffTimeStart = r.OffTimeStart.ToString(),//下班打卡开始时间
                //                     OffTimeEnd = r.OffTimeEnd.ToString(),//下班打卡结束时间
                //                     NeedSta = r.NeedSta,//周六打卡
                //                     NeedSun = r.NeedSun,//周日打卡
                //                     r.flag,
                //                     r.PassWord,
                //                 }
                //                  )
                //                  .Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                //    int total = Db.ClockModels.Count();//总条数
                //                                       //构造成Json的格式传递
                //    var result = new { total = total, rows = data1 };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //else if (!string.IsNullOrEmpty(Request["UserName"]) && string.IsNullOrEmpty(Request["CardId"]))
                //{
                //    EmployeeName = Request["UserName"].ToString();

                //    var data1 = (from r in Db.ClockModels.ToList()
                //                 where r.EmployeeName.Contains(EmployeeName)
                //                 select new
                //                 {
                //                     CardId = r.CardId,//工号
                //                     EmployeeName = r.EmployeeName,//名称
                //                     LastClockTime = r.LastClockTime.ToString().CompareTo("2010-1-1 0:00:00") > 0 ? r.LastClockTime.ToString() : "",//上次打卡时间
                //                     ClockStateAM = r.ClockStateAM,//打卡状态
                //                     ClockStatePM = r.ClockStatePM,
                //                     TotalDays = r.TotalDays,//打卡天数
                //                     SurplusDay = r.CreatTime.AddDays(r.TotalDays).Subtract(DateTime.Now).Days,//剩余天数
                //                     OnTimeStart = r.OnTimeStart.ToString(),//上班打卡开始时间
                //                     OnTimeEnd = r.OnTimeEnd.ToString(),//上班打卡结束时间
                //                     OffTimeStart = r.OffTimeStart.ToString(),//下班打卡开始时间
                //                     OffTimeEnd = r.OffTimeEnd.ToString(),//下班打卡结束时间
                //                     NeedSta = r.NeedSta,//周六打卡
                //                     NeedSun = r.NeedSun,//周日打卡
                //                     r.flag,
                //                     r.PassWord,
                //                 }
                //                  )
                //                  .Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                //    int total = Db.ClockModels.Count();//总条数
                //                                       //构造成Json的格式传递
                //    var result = new { total = total, rows = data1 };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //else if (string.IsNullOrEmpty(Request["UserName"]) && !string.IsNullOrEmpty(Request["CardId"]))
                //{
                //    CardId = Request["CardId"].ToString();
                //    var data1 = (from r in Db.ClockModels.ToList()
                //                 where r.CardId.Equals(CardId)
                //                 select new
                //                 {
                //                     CardId = r.CardId,//工号
                //                     EmployeeName = r.EmployeeName,//名称
                //                     LastClockTime = r.LastClockTime.ToString().CompareTo("2010-1-1 0:00:00") > 0 ? r.LastClockTime.ToString() : "",//上次打卡时间
                //                     ClockStateAM = r.ClockStateAM,//打卡状态
                //                     ClockStatePM = r.ClockStatePM,
                //                     SurplusDay = r.CreatTime.AddDays(r.TotalDays).Subtract(DateTime.Now).Days,//剩余天数
                //                     OnTimeStart = r.OnTimeStart.ToString(),//上班打卡开始时间
                //                     OnTimeEnd = r.OnTimeEnd.ToString(),//上班打卡结束时间
                //                     OffTimeStart = r.OffTimeStart.ToString(),//下班打卡开始时间
                //                     OffTimeEnd = r.OffTimeEnd.ToString(),//下班打卡结束时间
                //                     NeedSta = r.NeedSta,//周六打卡
                //                     NeedSun = r.NeedSun,//周日打卡
                //                     r.flag,
                //                     r.PassWord,
                //                 }
                //                  )
                //                  .Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                //    int total = Db.ClockModels.Count();//总条数
                //                                       //构造成Json的格式传递
                //    var result = new { total = total, rows = data1 };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var data1 = (from r in Db.ClockModels.ToList()
                //                 select new
                //                 {
                //                     CardId = r.CardId,//工号
                //                     EmployeeName = r.EmployeeName,//名称
                //                     LastClockTime = r.LastClockTime.ToString().CompareTo("2010-1-1 0:00:00") > 0 ? r.LastClockTime.ToString() : "",//上次打卡时间
                //                     ClockStateAM = r.ClockStateAM,//打卡状态
                //                     ClockStatePM = r.ClockStatePM,
                //                     TotalDays = r.TotalDays,//打卡天数
                //                     SurplusDay = r.CreatTime.AddDays(r.TotalDays).Subtract(DateTime.Now).Days,//剩余天数
                //                     OnTimeStart = r.OnTimeStart.ToString(),//上班打卡开始时间
                //                     OnTimeEnd = r.OnTimeEnd.ToString(),//上班打卡结束时间
                //                     OffTimeStart = r.OffTimeStart.ToString(),//下班打卡开始时间
                //                     OffTimeEnd = r.OffTimeEnd.ToString(),//下班打卡结束时间
                //                     NeedSta = r.NeedSta,//周六打卡
                //                     NeedSun = r.NeedSun,//周日打卡
                //                     r.flag,
                //                     r.PassWord,
                //                 }
                //  )
                //  .Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                //    int total = Db.ClockModels.Count();//总条数
                //                                       //构造成Json的格式传递
                //    var result = new { total = total, rows = data1 };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Add(ClockModels model)
        {
            try
            {
                model.CardId = model.CardId.Trim();
                model.PassWord = model.PassWord.Trim();
                model.CreatTime = DateTime.Now;
                model.flag = true;
                model.ClockStateAM = false;
                model.ClockStatePM = false;
                model.LastClockTime = Convert.ToDateTime("2000-1-1 00:00:00");
                Db.ClockModels.Add(model);
                Db.Entry<ClockModels>(model).State = EntityState.Added;
                if (Db.SaveChanges() > 0)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("添加失败");
                }
            }
            catch (Exception ex)
            {
                return Json($"添加失败,请检查数据格式以及工号是否重复！异常信息：{ex}");
            }

        }
        [HttpPost]
        public ActionResult Edit(ClockModels model)
        {
            try
            {
                model.PassWord = model.PassWord.Trim();
                Db.ClockModels.Attach(model);
                Db.Entry<ClockModels>(model).State = EntityState.Modified;
                Db.Entry(model).Property("LastClockTime").IsModified = false;
                Db.Entry(model).Property("ClockStateAM").IsModified = false;
                Db.Entry(model).Property("ClockStatePM").IsModified = false;
                Db.Entry(model).Property("CreatTime").IsModified = false;
                Db.Entry(model).Property("flag").IsModified = false;
                if (Db.SaveChanges() > 0)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("修改失败");
                }
            }
            catch (Exception ex)
            {
                return Json(string.Format("添加失败,请检查数据格式输入！异常信息：{0}", ex));
            }

        }
        public ActionResult Delete(string Id)
        {
            try
            {
                string[] ids = Id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ids == null) return View("Index");
                foreach (var id in ids)
                {
                    var model1 = new ClockModels() { CardId = id };
                    Db.ClockModels.Attach(model1);
                    Db.Entry<ClockModels>(model1).State = EntityState.Deleted;
                }
                Db.SaveChanges();
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(string.Format("删除失败！异常信息：{0}", ex));
            }
        }
        public ActionResult btnhandle(string Id)
        {
            try
            {
                var now = DateTime.Now;
                string[] ids = Id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ids == null) return View("Index");
                foreach (var id in ids)
                {
                    var model1 = (from r in Db.ClockModels
                                  where r.CardId == id
                                  select r).FirstOrDefault();
                    if (model1.CreatTime.AddDays(model1.TotalDays).Subtract(now).Days <= 0)
                        continue;
                    model1.flag = model1.flag ? false : true;//暂停或者恢复正常
                    model1.FailReason += (model1.flag ? "启用" : "禁用") + DateTime.Now.ToString();
                    Db.ClockModels.Attach(model1);
                    Db.Entry<ClockModels>(model1).State = EntityState.Modified;
                }
                Db.SaveChanges();
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(string.Format("操作失败！异常信息：{0}", ex));
            }
        }

        public ActionResult UploadFileExcel()
        {
            int count = 0;
            StringBuilder failCadID = new StringBuilder();
            StringBuilder errorMsg = new StringBuilder(); // 错误信息
            try
            {
                #region 1.获取Excel文件并转换为一个List集合

                // 1.1存放Excel文件到本地服务器
                HttpPostedFile filePost = System.Web.HttpContext.Current.Request.Files[0];
                string filePath = ExcelHelper.SaveExcelFile(filePost); // 保存文件并获取文件路径

                // 单元格抬头
                // key：实体对象属性名称，可通过反射获取值
                // value：属性对应的中文注解
                Dictionary<string, string> cellheader = new Dictionary<string, string> {
                    { "CardId", "工号" },
                    { "PassWord", "密码" },
                    { "EmployeeName", "名称" },
                    { "TotalDays","需要打卡总天数"},
                    { "OnTimeStart","OnStart"},
                    { "OnTimeEnd","OnEnd"},
                    { "OffTimeStart","OffStart"},
                    { "OffTimeEnd","OffEnd"},
                    { "NeedSta","周六"},
                    { "NeedSun","周日"}
                };

                // 1.2解析文件，存放到一个List集合里
                List<ClockModels> enlist = ExcelHelper.ExcelToEntityList<ClockModels>(cellheader, filePath, out errorMsg);
                if (errorMsg.Length != 0) return Json($"excel格式不对，请使用模板格式导入！！《预期内的错误》");
                #endregion
                // 获取所有数据
                IEnumerable<ClockModels> allModels = Db.ClockModels.ToList();
                for (int i = 0; i < enlist.Count; i++)
                {
                    ClockModels toupdateitem = (from r in allModels
                                                where r.CardId == enlist[i].CardId
                                                select r).FirstOrDefault();
                    if (toupdateitem != null)
                    {
                        toupdateitem.CardId = enlist[i].CardId.Trim();
                        toupdateitem.PassWord = enlist[i].PassWord.Trim();
                        toupdateitem.EmployeeName = enlist[i].EmployeeName;
                        toupdateitem.TotalDays = enlist[i].TotalDays;
                        toupdateitem.OnTimeStart = enlist[i].OnTimeStart;
                        toupdateitem.OnTimeEnd = enlist[i].OnTimeEnd;
                        toupdateitem.OffTimeStart = enlist[i].OffTimeStart;
                        toupdateitem.OffTimeEnd = enlist[i].OffTimeEnd;
                        toupdateitem.NeedSta = enlist[i].NeedSta;
                        toupdateitem.NeedSun = enlist[i].NeedSun;

                        toupdateitem.CreatTime = DateTime.Now;
                        toupdateitem.LastClockTime = Convert.ToDateTime("2000-1-1 00:00");
                        toupdateitem.ClockStateAM = false;
                        toupdateitem.ClockStatePM = false;
                        toupdateitem.flag = true;

                        enlist[i] = null;

                        Db.ClockModels.Attach(toupdateitem);
                        Db.Entry<ClockModels>(toupdateitem).State = EntityState.Modified;
                        try
                        {
                            count += Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            if (toupdateitem.CardId != null)
                                failCadID.AppendLine(toupdateitem.CardId);
                            continue;
                        }

                    }
                }
                foreach (var model in enlist)
                {
                    if (model == null) continue;
                    //默认数据
                    model.CardId = model.CardId.Trim();
                    model.CreatTime = DateTime.Now;
                    model.LastClockTime = Convert.ToDateTime("2000-1-1 00:00");
                    model.ClockStateAM = false;
                    model.ClockStatePM = false;
                    model.flag = true;
                    Db.ClockModels.Add(model);
                    Db.Entry<ClockModels>(model).State = EntityState.Added;
                    try
                    {
                        count += Db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        if (model.CardId != null)
                            failCadID.AppendLine(model.CardId);
                        continue;
                    }
                }
                return Json($"成功导入{count}条数据！异常的工号“{failCadID.ToString()}”");
            }
            catch (Exception ex)
            {
                return Json($"导入出错,已成功{count}条《预期外的错误》");
            }
        }
        public ActionResult ExportFileExcel()
        {
            string id = Request["cardids"];
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<ClockModels> data = Db.ClockModels.Where(x => ids.Contains(x.CardId)).ToList();
            // 2.设置单元格抬头
            // key：实体对象属性名称，可通过反射获取值
            // value：Excel列的名称
            Dictionary<string, string> cellheader = new Dictionary<string, string> {
                    { "CardId", "工号" },
                    { "PassWord", "密码" },
                    { "EmployeeName", "名称" },
                    { "LastClockTime", "上次打卡时间" },
                    { "ClockStateAM", "上午打卡状态" },
                    { "ClockStatePM", "下午打卡状态" },
                    { "TotalDays","需要打卡总天数"},
                    { "CreatTime","创建时间"},
                    { "OnTimeStart","OnStart"},
                    { "OnTimeEnd","OnEnd"},
                    { "OffTimeStart","OffStart"},
                    { "OffTimeEnd","OffEnd"},
                    { "NeedSta","周六"},
                    { "NeedSun","周日"}
                };
            string urlPath = ExcelHelper.EntityListToExcel2003(cellheader, data, "员工信息");
            // return File(urlPath, "text/plain");
            return Json(urlPath, JsonRequestBehavior.AllowGet); //File(urlPath, "text/plain", "ceshi.xls"); //welcome.txt是客户端保存的名字
        }
        #region 遗弃的txt导入导出
        [HttpPost]
        public ActionResult UploadFile()
        {
            HttpPostedFile file = System.Web.HttpContext.Current.Request.Files[0];
            if (file == null)
                return Json("文件上传失败!");//验证文件

            string FileType = file.FileName.Split('.').Last().ToUpper();
            if (FileType != "TXT")
                return Json("文件上传失败,请上传模板格式为txt的文件");//验证文件
            try
            {
                Stream fileStream = file.InputStream;
                StreamReader reader = new StreamReader(fileStream, Encoding.Default);
                string[] StrDat = reader.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var data1 = from r in Db.ClockModels select new { r.CardId };

                foreach (var item in StrDat.Skip(1))
                {
                    var line = item.Split('-');
                    ClockModels model = new ClockModels()
                    {
                        CardId = line[0],
                        EmployeeName = line[1],
                        TotalDays = Convert.ToInt32(line[2]),
                        OnTimeStart = Convert.ToDateTime(line[3]),
                        OnTimeEnd = Convert.ToDateTime(line[4]),
                        OffTimeStart = Convert.ToDateTime(line[5]),
                        OffTimeEnd = Convert.ToDateTime(line[6]),
                        NeedSta = line[7] == "0" ? false : true,
                        NeedSun = line[8] == "0" ? false : true,
                        //默认数据
                        CreatTime = DateTime.Now,
                        LastClockTime = Convert.ToDateTime("2000-1-1 00:00"),
                        ClockStateAM = false,
                        ClockStatePM = false,
                        flag = true,
                    };
                    Db.ClockModels.Add(model);
                    Db.Entry<ClockModels>(model).State = EntityState.Added;
                }
                int countNum = Db.SaveChanges();
                return countNum > 0 ? Json($"成功导入{countNum}条数据!") : Json("添加失败,请检查数据格式及工号是否重复！");
            }
            catch (Exception ex)
            {
                return Json($"添加失败,请检查数据格式及工号是否重复！异常信息：{ex}");
            }
        }
        public ActionResult ExportFile()
        {
            string id = Request["cardids"];
            string[] ids = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<ClockModels> data = Db.ClockModels.Where(x => ids.Contains(x.CardId)).ToList();
            StringBuilder result = new StringBuilder();
            result.Append("工号-名称-最后打卡时间-上午打卡状态-下午打卡状态-总天数-剩余天数-OnStart-OnEnd-OffStart-OffEnd-周六-周日\r\n");
            foreach (var model in data)
            {
                result.Append(model.CardId + "-"
                            + model.EmployeeName + "-"
                            + model.LastClockTime + "-"
                            + model.ClockStateAM + "-"
                            + model.ClockStatePM + "-"
                            + model.TotalDays + "-"
                            + model.CreatTime.AddDays(model.TotalDays).Subtract(DateTime.Now).Days + "-"
                            + model.OnTimeStart.ToString() + "-"
                            + model.OnTimeEnd.ToString() + "-"
                            + model.OffTimeStart.ToString() + "-"
                            + model.OffTimeEnd.ToString() + "-"
                            + model.NeedSta + "-"
                            + model.NeedSun + "\r\n"
                            );

            }
            byte[] bytearry = Encoding.UTF8.GetBytes(result.ToString());
            return File(bytearry, "text/plain", string.Format("导出数据{0}.txt", DateTime.Now));
        }
        #endregion
    }
}