using EntityFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Net.Demo
{
    public class ClockReset
    {
        public void Reset()
        {
            IEnumerable<ClockModels> toUpdates = new List<ClockModels>();
            // 获取所有数据
            using (var context = new ClockInEntity())
            {
                toUpdates = context.ClockModels.ToList();
            }
            // 所有的打卡状态值都改为false
            Parallel.ForEach(toUpdates,
                (entity, state) =>
                {
                    entity.ClockStateAM = false;
                    entity.ClockStatePM = false;
                    entity.FailReason += DateTime.Now.ToString();
                });
            // 所有的过期用户flag改为False
            Parallel.ForEach(toUpdates.Where(
                r => r.CreatTime.AddDays(r.TotalDays).Subtract(DateTime.Now).Days <= 0
                ),(entity, state) => entity.flag = false);
            using (var context = new ClockInEntity())
            {
                EFBatchOperation.For(context, context.ClockModels).UpdateAll(toUpdates, x => x.ColumnsToUpdate(
                    c => c.ClockStateAM,
                    c2 => c2.ClockStatePM,
                    c3 => c3.flag
                    ));
            }
        }
    }
}
