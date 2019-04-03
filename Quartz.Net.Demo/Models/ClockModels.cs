using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Net.Demo
{
    public partial class ClockModels
    {
        /// <summary>
        /// 工号
        /// </summary>
        [Key]
        public string CardId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 上次打卡时间              默认值数据库最小时间
        /// </summary>
        public Nullable<System.DateTime> LastClockTime { get; set; }
        /// <summary>
        /// 打卡状态AM                默认值False
        /// </summary>
        public bool ClockStateAM { get; set; }
        /// <summary>
        /// 打卡状态PM                默认值False
        /// </summary>
        public bool ClockStatePM { get; set; }
        /// <summary>
        /// 需要打卡总天数
        /// </summary>
        public int TotalDays { get; set; }
        /// <summary>
        /// 创建时间            默认系统当前时间
        /// </summary>
        public System.DateTime CreatTime { get; set; }
        /// <summary>
        /// 上班打开最早时间   默认7：30
        /// </summary>
        public System.DateTime OnTimeStart { get; set; }
        /// <summary>
        /// 上班打开最晚时间        默认8：30
        /// </summary>
        public System.DateTime OnTimeEnd { get; set; }
        /// <summary>
        /// 下班打开最早时间     默认17：30
        /// </summary>
        public System.DateTime OffTimeStart { get; set; }
        /// <summary>
        /// 下班打开最晚时间     默认19：00
        /// </summary>
        public System.DateTime OffTimeEnd { get; set; }
        /// <summary>
        /// 周六需要打卡  默认false
        /// </summary>
        public bool NeedSta { get; set; }
        /// <summary>
        /// 周日需要打卡   默认false
        /// </summary>
        public bool NeedSun { get; set; }
        /// <summary>
        /// 是否有效        默认true
        /// </summary>
        public bool flag { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string FailReason { get; set; }

        //定义无参数的构造函数主要是因为在通过DbSet获取对象进行linq查询时会报错
        //The class 'EFCodeFirstModels.Student' has no parameterless constructor.
        public ClockModels()
        {

        }

    }
}
