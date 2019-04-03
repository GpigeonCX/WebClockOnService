using System.ServiceProcess;
using Common.Logging;
using Quartz.Impl;
//using System.Configuration;

namespace Quartz.Net.Service
{
    public partial class QuartzService : ServiceBase
    {
        private readonly ILog logger;
        private IScheduler scheduler;
        //时间间隔
        //private readonly string StrCron = ConfigurationManager.AppSettings["cron"] == null ? "* 10 * * * ?" : ConfigurationManager.AppSettings["cron"];
        public QuartzService()
        {
            InitializeComponent();
            logger = LogManager.GetLogger(GetType());
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();
        }

        protected override void OnStart(string[] args)
        {
            //if (!scheduler.IsStarted)
            //{
            //    //启动调度器
            //    scheduler.Start();
            //    //新建一个任务
            //    IJobDetail job = JobBuilder.Create<AppLogJob>().WithIdentity("AppLogJob", "AppLogJobGroup").Build();
            //    //新建一个触发器
            //    ITrigger trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(StrCron).Build();
            //    //将任务与触发器关联起来放到调度器中
            //    scheduler.ScheduleJob(job, trigger);
            //    logger.Info("Quarzt 数据同步服务开启");
            //}
            scheduler.Start();
            logger.Info("ClockInService[自动打卡服务]成功启动");
        }

        protected override void OnStop()
        {
            //这里是测试这样写。传入false参数意思是强制停止，不关心任务是否结束。
            scheduler.Shutdown(false);
            logger.Info("ClockInService[自动打卡服务]成功终止");
        }

        protected override void OnPause()
        {
            scheduler.PauseAll();
        }

        protected override void OnContinue()
        {
            scheduler.ResumeAll();
        }
      
    }
}
