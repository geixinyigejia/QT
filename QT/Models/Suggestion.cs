using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//added new using 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GetLBAMVC.Models
{
    public class Suggestion
    {
       
        /// <summary>
        /// 发布反馈建议的用户名
        /// </summary>  
        [DisplayName("发布人")]
        public string PublishUserName
        {
            get;
            set;
        }

        /// <summary>
        ///发布时间
        /// </summary>
        [DisplayName("发布时间")]
        public DateTime PublishTime
        {
            get;
            set;
        }

        [DisplayName("发布内容")]
        public string Content
        {
            get;
            set;
        }
    }
}