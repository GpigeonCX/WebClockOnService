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
    
    public partial class BaseRelation
    {
        public int Id { get; set; }
        public string FormTable { get; set; }
        public Nullable<int> FormId { get; set; }
        public string ToTable { get; set; }
        public Nullable<int> ToId { get; set; }
    }
}