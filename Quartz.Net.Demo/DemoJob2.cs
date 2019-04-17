using Quartz.Net.Demo.Detail;
using System;

namespace Quartz.Net.Demo
{
    /// <summary>
    /// 实现IJob接口
    /// </summary>
    public class DemoJob2 : IJob
    {
        //使用log4net.dll日志接口实现日志记录
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                logger.Info("开始  每天定时（默认0点）清理所有打卡状态数据及过期用户！！！ ClockReset().Reset() "+ DateTime.Now);
                new ClockReset().Reset();
                logger.Info("成功！每天定时（默认0点）清理所有打卡状态数据及过期用户完成！！！ ClockReset().Reset()"+ DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.Fatal("每天定时（默认0点）清理所有打卡状态数据及过期用户   运行异常 ClockReset().Reset()"+ DateTime.Now, ex);
            }

        }

        #endregion
    }
}
