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
    public class User
    {
        /// <summary>
        /// 用户id（QT编号）
        /// </summary>
        public int UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 刷单的累积点数 
        /// </summary>
        public int points
        {
            get;
            set;
        }

        /// <summary>
        /// 省市（IP地址）
        /// </summary>
        public string city
        {
            get;
            set;
        }

        /// <summary>
        /// QQ号码
        /// </summary>
        public Int64 QQ
        {
            get;
            set;
        }

        /// <summary>
        /// 手机
        /// </summary>
        public Int64 MobilePhone
        {
            get;
            set;
        }
        /// <summary>
        ///注册时间
        /// </summary>
        public DateTime RegistTime
        {
            get;
            set;
        }

        /// <summary>
        /// 账号审核状态
        /// </summary>
        public string Checkstatus
        {
            get;
            set;
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        ///最后登录时间
        /// </summary>
        public DateTime LastLogonTime
        {
            get;
            set;
        }

        /// <summary>
        /// 账号启用状态
        /// </summary>
        public string Acountstatus
        {
            get;
            set;
        }

        /// <summary>
        ///账号启用时间
        /// </summary>
        public DateTime AcountStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 账户类型，是普通用户，还是管理员
        /// </summary>
        public string UserType
        {
            get;
            set;
        }

    }

    public class Register
    {
        [Required]
        [DisplayName("用户名")]
        public string UserName
        {
            get;
            set;
        }

        [Required]
        [DisplayName("密码")]
        [DataType(DataType.Password)]
        public string Password
        {
            get;
            set;
        }

        [Required]
        [DisplayName("城市")]
        public string city
        {
            get;
            set;
        }

        [Required]
        public string QQ
        {
            get;
            set;
        }

        [Required]
        [DisplayName("手机")]
        public string MobilePhone
        {
            get;
            set;
        }
    }


    public class Logon
    {
        [Required]
        [DisplayName("用户名")]
        public string UserName
        {
            get;
            set;
        }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password
        {
            get;
            set;
        }
    }

}