using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebMVC.Models.BaseTB
{
    public class UserModels
    {
        [Key]
        public string UserName { get; set; }

        public string UserPwd { get; set; }

        //public int Age { get; set; }

        public UserModels(string name, string pwd)
        {
            this.UserName = name;
            this.UserPwd = pwd;
        }
        //定义无参数的构造函数主要是因为在通过DbSet获取对象进行linq查询时会报错
        //The class 'EFCodeFirstModels.Student' has no parameterless constructor.
        public UserModels() { }
    }
}