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
         [DisplayName("ID")]
        public int UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        [DisplayName("姓名")]
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 刷单的累积点数 
        /// </summary>
        [DisplayName("余额")]
        public int points
        {
            get;
            set;
        }

        /// <summary>
        /// 省市（IP地址）
        /// </summary>
        [DisplayName("省市")]
        public string city
        {
            get;
            set;
        }

        /// <summary>
        /// QQ号码
        /// </summary>
         [DisplayName("QQ")]
        public Int64 QQ
        {
            get;
            set;
        }

        /// <summary>
        /// 手机
        /// </summary>
         [DisplayName("手机号码")]
        public Int64 MobilePhone
        {
            get;
            set;
        }
        /// <summary>
        ///注册时间
        /// </summary>
         [DisplayName("注册时间")]
        public DateTime RegistTime
        {
            get;
            set;
        }

        /// <summary>
        /// 账号审核状态
        /// </summary>
        [DisplayName("审核状态")]
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
        [DisplayName("最后登录时间")]
        public DateTime LastLogonTime
        {
            get;
            set;
        }

        /// <summary>
        /// 账号启用状态
        /// </summary>
        [DisplayName("启用状态")]
        public string Acountstatus
        {
            get;
            set;
        }

        /// <summary>
        ///账号启用时间
        /// </summary>
        [DisplayName("启用时间")]
        public DateTime AcountStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 账户类型，是普通用户，还是管理员
        /// </summary>
        [DisplayName("账户类型")]
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

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [Display(Name = "密码")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{2}到{1}个字符")]
        [DataType(DataType.Password)]
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "两次输入的密码不一致")]
        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

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

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "验证码不正确")]
        [Display(Name = "验证码")]
        public string VerificationCode { get; set; }
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