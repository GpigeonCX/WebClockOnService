using Quartz.Net.Demo.Detail;
using System;

namespace Quartz.Net.Demo
{
    /// <summary>
    /// 实现IJob接口
    /// </summary>
    public class DemoJob1 : IJob
    {
        //使用Common.Logging.dll日志接口实现日志记录
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                logger.Info("开始检查是否有需要打卡的用户，并准备开始打卡！ ClockGo().Main()"+DateTime.Now.ToString());

                new ClockGo().Main();

                logger.Info("已完成当前时间需要打卡的所有用户的打卡操作，等待下一次检查！ ClockGo().Main() "+DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                logger.Fatal("检查打卡用并执行打卡操作时发生异常！！！！！ ClockGo().Main()"+ DateTime.Now, ex);
            }

        }

        #endregion
    }
}
