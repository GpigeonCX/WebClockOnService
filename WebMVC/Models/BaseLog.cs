//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BaseLog
    {
        public int logId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string UserRealName { get; set; }
        public string ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string MethodId { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public string UrlReferrer { get; set; }
        public string IPAddress { get; set; }
        public string WebUrl { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreateOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
