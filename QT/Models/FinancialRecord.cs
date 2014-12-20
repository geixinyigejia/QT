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
    public class FinancialRecord
    {
        /// <summary>
        /// 资金的用户名
        /// </summary>  
        [DisplayName("用户名")]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        ///记录时间
        /// </summary>
        [DisplayName("记录时间")]
        public DateTime RecordTime
        {
            get;
            set;
        }

        /// <summary>
        ///记录时间
        /// </summary>
        [DisplayName("描述")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        ///金额变化方式
        /// </summary>
        [DisplayName("变化方式")]
        public string ChangeMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 变化前的金额
        /// </summary>
        [DisplayName("变化前的金额")]
        public Decimal ChangedBeforePoints
        {
            get;
            set;
        }

        /// <summary>
        /// 变化前的金额
        /// </summary>
        [DisplayName("变化的金额")]
        public Decimal ChangedPoints
        {
            get;
            set;
        }

        /// <summary>
        /// 变化后的金额 
        /// </summary>
        [DisplayName("变化后的金额")]
        public Decimal ChangedAfterPoints
        {
            get;
            set;
        }

        /// <summary>
        /// 是否确认 
        /// </summary>
        [DisplayName("是否确认过？")]
        public Boolean IsConfirmed
        {
            get;
            set;
        }
    }
}