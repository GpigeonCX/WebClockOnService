using System;
using System.Diagnostics;
using System.ServiceProcess;
namespace Quartz.Net.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static void Main(string[] args)
        {
            {
                //如果传递了参数 s 就启动服务
                if (args.Length > 0 && args[0] == "s")
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new QuartzService() };
                    ServiceBase.Run(ServicesToRun);
                }
                else
                {
                    Console.WriteLine("这是Windows应用程序");
                    Console.WriteLine("请选择，[1]安装服务 [2]卸载服务 [3]退出");
                    var rs = int.Parse(Console.ReadLine());
                    string strServiceName = "ClockInService[自动打卡服务]";
                    switch (rs)
                    {
                        case 1:
                            //取当前可执行文件路径，加上"s"参数，证明是从windows服务启动该程序
                            var path = Process.GetCurrentProcess().MainModule.FileName + " s";
                            Process.Start("sc", "create " + strServiceName + " binpath= \"" + path + "\" displayName= " + strServiceName + " start= auto");
                            Console.WriteLine("安装成功");
                            Console.Read();
                            break;
                        case 2:
                            Process.Start("sc", "delete " + strServiceName + "");
                            Console.WriteLine("卸载成功");
                            Console.Read();
                            break;
                        case 3: break;
                    }

                }


            }
        }
    }
}

          

//HostFactory.Run(x => //1.我们用HostFactory.Run来设置一个宿主主机。我们初始化一个新的lambda表达式X，来显示这个宿主主机的全部配置。
//{

//x.Service<TownCrier>(s => //2.告诉Topshelf ，有一个类型为“TownCrier服务”,通过定义的lambda 表达式的方式，配置相关的参数。
//{

//    s.ConstructUsing(name =>
//        new TownCrier()); //3.告诉Topshelf如何创建这个服务的实例，目前的方式是通过new 的方式，但是也可以通过Ioc 容器的方式：getInstance<towncrier>()。

//    s.WhenStarted(tc => tc.Start()); //4.开始 Topshelf 服务。

//    s.WhenStopped(tc => tc.Stop()); //5.停止 Topshelf 服务。

//});

//x.RunAsLocalSystem(); //6.这里使用RunAsLocalSystem() 的方式运行，也可以使用命令行(RunAsPrompt())等方式运行。

//x.SetDescription("Sample Topshelf Host"); //7.设置towncrier服务在服务监控中的描述。

//x.SetDisplayName("Stuff"); //8.设置towncrier服务在服务监控中的显示名字。

//x.SetServiceName("Stuff"); //9.设置towncrier服务在服务监控中的服务名字。

//});