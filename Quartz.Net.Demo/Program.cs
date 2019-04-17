using Quartz.Net.Demo.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Net.Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {

            new ClockGo().Main();
            //new ClockBatchDetail().ClockBatchGo();
            }
            catch (Exception ex )
            {
                Console.ReadKey();
                throw;
            }
        }
    }
}
