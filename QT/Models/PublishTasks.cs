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
    public class PublishTasks
    {

        /// <summary>
        /// 发布任务的ID
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int PublishTaskID
        {
            get;
            set;
        }
        /// <summary>
        /// 发布任务的用户名
        /// </summary>       
        [Key]
        [DisplayName("发布人")]
        public string PublishUserName
        {
            get;
            set;
        }

        [DisplayName("省市")]
        public string city
        {
            get;
            set;
        }

        /// <summary>
        /// 订单是单链接还是双链接 以及多链接
        /// </summary>
        [Required]
        [DisplayName("链接")]
        public string links
        {
            get;
            set;
        }

        /// <summary>
        /// 小号要求
        /// </summary>
        [Required]
        [DisplayName("小号")]
        public string wangwangxiaohao
        {
            get;
            set;
        }
        /// <summary>
        /// 订单金额
        /// </summary>
        [Required]
        [DisplayName("价格")]
        public Decimal TaskPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 点数或是钱数
        /// </summary>
        [Required]
        [DisplayName("任务佣金")]
        public int charges
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("要求")]
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// 任务的类别：电脑单，手机单，手机电脑单
        /// </summary>
        [DisplayName("任务类别")]
        public string TaskType
        {
            get;
            set;
        }

        /// <summary>
        /// 接手ID
        /// </summary> 
        [DisplayName("接手人")]
        public string ReceiverName
        {
            get;
            set;
        }
        /// <summary>
        /// 发布任务时间
        /// </summary>
        private DateTime publishtime = DateTime.Now;
        [DisplayName("任务发布时间")]
        public DateTime PublishTime
        {
            get
            {
                return publishtime;
            }
            set
            {
                publishtime = value;
            }
        }
        /// <summary>
        /// 完成状态
        /// </summary>  
        [DisplayName("完成状态")]
        public string CompleteStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        private DateTime ompletetime = DateTime.Now;
        [DisplayName("任务完成时间")]
        public DateTime CompleteTime
        {
            get
            {
                return ompletetime;
            }
            set
            {
                ompletetime = value;
            }
        }

    }
}