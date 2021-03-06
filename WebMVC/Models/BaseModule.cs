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
    
    public partial class BaseModule
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string FullName { get; set; }
        public string Category { get; set; }
        public string ImageIndex { get; set; }
        public string SelectedImageIndex { get; set; }
        public string NavigateUrl { get; set; }
        public string Target { get; set; }
        public string FormName { get; set; }
        public string AssemblyName { get; set; }
        public Nullable<int> IsMenu { get; set; }
        public Nullable<int> SortCode { get; set; }
        public Nullable<int> IsPublic { get; set; }
        public Nullable<int> Enabled { get; set; }
        public Nullable<int> DeletionStateCode { get; set; }
        public Nullable<int> AllowEdit { get; set; }
        public Nullable<int> AllowDelete { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreateOn { get; set; }
        public Nullable<int> CreateUserId { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public string ModifiedBy { get; set; }
    }
}
