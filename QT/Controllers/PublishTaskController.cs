﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using 
using System.Data;
using System.Data.SqlClient;
using GetLBAMVC.Models;
using System.Reflection;

using PagedList;

namespace GetLBAMVC.Controllers
{
    public class PublishTaskController : Controller
    {
          private static string DBName = "QT";
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PublishTasks publishTasks, string PublishedTasksAcount)
        {
            publishTasks.PublishUserName = User.Identity.Name;
            //get the publisher city
            //publishTasks.city = GetPublisherCity(User.Identity.Name);
            publishTasks.PublishTime = DateTime.Now;
            publishTasks.CompleteTime = DateTime.Now;

            int publishedTasksAcount = int.Parse(PublishedTasksAcount);
            if (ModelState.IsValid)
  
            
            {
                PublishTask(publishTasks, publishedTasksAcount);
            }
            return RedirectToAction("PublishedTasks");
        }

        public ActionResult PublishedTasks(int p = 1)
        {
            List<PublishTasks> PublishedTasksByMe = GetAllPublishTasksByMe(User.Identity.Name);
            var pagedData = PublishedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);            
        }

        public ActionResult ReceviedByPepole(int p=1)
        {
            List<PublishTasks> receviedByPepole = ReceivePushlishTasksByMe();
            var pagedData = receviedByPepole.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
        }

        public ActionResult WaitToBeDone(int p = 1)
        {
            List<PublishTasks> receviedByPepole = GetAllWaitToBeDoneTasksPublishedByMe(User.Identity.Name);
            var pagedData = receviedByPepole.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
        }

        public ActionResult DoneTasks(int p=1)
        {
            List<PublishTasks> receviedByPepole = GetAllCompletedTasksPublishedByMe(User.Identity.Name);
            var pagedData = receviedByPepole.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
        }

        public ActionResult DeleteTasks(int p=1)
        {
            List<PublishTasks> PublishedTasksByMe = GetAllDeletePublishTasksByMe(User.Identity.Name);
            var pagedData = PublishedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData); 
           
        }


        /// <summary>
        /// 删除自己发布的任务
        /// </summary>
        /// <param name="publishTaskID"></param>
        /// <returns></returns>
        public ActionResult shanchuTask(string publishTaskID)
        {
            int PublishTaskID = -1;
            if (int.TryParse(publishTaskID, out PublishTaskID))
            {
            }

            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0};update PublishTasks set IsDeleted=1 where PublishTaskID={1}", DBName, PublishTaskID);
          
            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }         
            return RedirectToAction("DeleteTasks");
        }
       
        public ActionResult RefuseTheRecevier(string publishTaskID)
        {
            int PublishTaskID = -1;
            if (int.TryParse(publishTaskID, out PublishTaskID))
            {
            }

            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0};update PublishTasks set ReceiverName='' where PublishTaskID={1}", DBName, PublishTaskID);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }
            return RedirectToAction("PublishedTasks");
        }
        public ActionResult ConfirmTheRecevier(string publishTaskID)
        {
            int PublishTaskID = -1;
            if (int.TryParse(publishTaskID, out PublishTaskID))
            {
            }

            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0};update PublishTasks set ReceiverConfirm=1 , CompleteStatus=N'{2}' where PublishTaskID={1}", DBName, PublishTaskID,GetLBAMVC.Models.commonContext.CompleteStatus.等待完成);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }
            return RedirectToAction("WaitToBeDone");
        }

       public ActionResult ConfrimCompleteTask(string publishTaskID )
        {
            int PublishTaskID = -1;
            if (int.TryParse(publishTaskID, out PublishTaskID))
            {
            }

            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0};update PublishTasks set CompleteStatus=N'{2}',CompleteTime=N'{3}' where PublishTaskID={1}", DBName, PublishTaskID, GetLBAMVC.Models.commonContext.CompleteStatus.已完成.ToString(),DateTime.Now);
                  
            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }

           //查出完成的这个订单的信息，从而完成扣款或偿付佣金
         
             sqlCommand = string.Format(@"use {0}; select * from publishTasks where PublishTaskID='{1}' and CompleteStatus=N'{2}'  order by PublishTime DESC ", DBName, publishTaskID,GetLBAMVC.Models.commonContext.CompleteStatus.已完成.ToString());

            sqlconn.Open();

             cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            PublishTasks task = new PublishTasks();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = task.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(task, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }               
            }
            reader.Close();
            Decimal PublishUserPoints = GetUserPointsByUserName(task.PublishUserName);
            Decimal ReceiverNamePoints = GetUserPointsByUserName(task.ReceiverName);
            //处理扣款 和偿付佣金    及记录金额的变化情况
            sqlCommand = string.Format(@"use {2};update QT_USER set points=points-{0} where UserName=N'{1}';", task.charges, task.PublishUserName,DBName);
            sqlCommand += string.Format(@"update QT_USER set points=points+{0} where UserName=N'{1}';", task.charges, task.ReceiverName);
            sqlCommand += string.Format(@"insert QT_Financial_Record(UserName,RecordTime,ChangeMethod,ChangedBeforePoints,ChangedAfterPoints,Description,ChangedPoints) values(N'{0}','{1}',N'{2}',{3},{4},'{5}',{6});", task.PublishUserName, DateTime.Now, commonContext.ChangeMethod.放单, PublishUserPoints, task.charges, "",PublishUserPoints-task.charges);
            sqlCommand += string.Format(@"insert QT_Financial_Record(UserName,RecordTime,ChangeMethod,ChangedBeforePoints,ChangedAfterPoints,Description,ChangedPoints) values(N'{0}','{1}',N'{2}',{3},{4},'{5}',{6});", task.ReceiverName, DateTime.Now, commonContext.ChangeMethod.接单, ReceiverNamePoints, task.charges, "", ReceiverNamePoints+ task.charges);

           sqlconn.Open();
           cmd = new SqlCommand(sqlCommand, sqlconn);
           try
           {
               cmd.ExecuteNonQuery();
           }
           catch (Exception ex)
           {
               throw ex;
           }
           finally
           {
               sqlconn.Close();
           }
            return RedirectToAction("DoneTasks");
        }


        #region 辅助方法

        private void PublishTask(PublishTasks task, int PublishedTasksAcount)
        {
            if (string.IsNullOrEmpty(task.Comment))
            {
                task.Comment = string.Empty;
            }
            task.city = GetPublisherCity(task.PublishUserName);//get publisher city

            // Calculation the total charge 
            task.charges = CalculationTotalcharge(task);

            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand=string.Empty;
            for (int i = 0; i < PublishedTasksAcount; i++)
            {
                sqlCommand += string.Format(@"use {0}; insert PublishTasks(PublishUserName,links,wangwangxiaohao,TaskPrice,charges,Comment,city,PublishTime,CompleteTime,TaskType,ShuadanType,payment ) values('{6}',N'{1}',N'{2}','{3}','{4}',N'{5}',N'{7}','{8}','{9}',N'{10}',N'{11}',N'{12}');", DBName, task.links, task.wangwangxiaohao, task.TaskPrice, task.charges, task.Comment, task.PublishUserName, task.city, task.PublishTime, task.CompleteTime, task.TaskType, task.ShuadanType,task.payment);

            }

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }
        }

        private List<PublishTasks> GetAllPublishTasksByMe(string username)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from publishTasks where PublishUserName=N'{1}' and IsDeleted=0 and ReceiverName='' order by PublishTime DESC ", DBName, username);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {

                PublishTasks task = new PublishTasks();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = task.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(task, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                tasks.Add(task);
            }
            reader.Close();
            return tasks;
        }

        private int CalculationTotalcharge(PublishTasks task)
        {
            int totalPayment = 0;
           
            // 快刷的基本价格
            if(task.wangwangxiaohao==GetLBAMVC.Models.commonContext.wangwangxiaohao.白号.ToString())
            {
                totalPayment = 3;
            }
            else if (task.wangwangxiaohao == GetLBAMVC.Models.commonContext.wangwangxiaohao.默认.ToString())
            {
                totalPayment = 5;
            }
            else if (task.wangwangxiaohao == GetLBAMVC.Models.commonContext.wangwangxiaohao.四星.ToString())
            {
                totalPayment = 6;
            }
            else if (task.wangwangxiaohao == GetLBAMVC.Models.commonContext.wangwangxiaohao.钻号.ToString())
            {
                totalPayment = 7;
            }

            //手机订单加1元
            if (task.TaskType == GetLBAMVC.Models.commonContext.TaskType.手机.ToString() || task.TaskType == GetLBAMVC.Models.commonContext.TaskType.电脑手机.ToString())
            {
                totalPayment += 1;
            }

            //双链接 单不是白号加1元
            if (task.links == "双链接" && task.wangwangxiaohao != GetLBAMVC.Models.commonContext.wangwangxiaohao.白号.ToString())
            {
                totalPayment += 1;
            }

            //财付通付款，超过500 加1元
            if(task.payment==GetLBAMVC.Models.commonContext.payment.财付通立返.ToString() && task.TaskPrice>=500)
            {
                totalPayment += 1;
            }

            // 慢刷加1元
            if(task.ShuadanType==GetLBAMVC.Models.commonContext.ShuadanType.慢刷.ToString())
            {
                totalPayment += 1;
            }

            return totalPayment;
        }
        private List<PublishTasks> GetAllWaitToBeDoneTasksPublishedByMe(string username)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}' and CompleteStatus=N'{2}' and  ReceiverName!='' order by PublishTime DESC ", DBName, username, GetLBAMVC.Models.commonContext.CompleteStatus.等待完成);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {

                PublishTasks task = new PublishTasks();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = task.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(task, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                tasks.Add(task);
            }
            reader.Close();
            return tasks;
        }

        private List<PublishTasks> GetAllCompletedTasksPublishedByMe(string username)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}' and CompleteStatus=N'{2}' and  ReceiverName!='' order by PublishTime DESC", DBName, username, GetLBAMVC.Models.commonContext.CompleteStatus.已完成);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {

                PublishTasks task = new PublishTasks();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = task.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(task, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                tasks.Add(task);
            }
            reader.Close();
            return tasks;
        }


        private string GetPublisherCity(string publisher)
        {
            string city = string.Empty;
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0};select city from QT_USER where UserName='{1}';",DBName,User.Identity.Name);
            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                DataRow dr = dt.Rows[0];//第一值 
                city = dr["city"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }

            return city;
        }


        private List<PublishTasks> ReceivePushlishTasksByMe()
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; select p.PublishTaskID, p.ReceiverName,u.city,p.links,p.wangwangxiaohao,p.TaskPrice,p.TaskType,p.Comment,p.charges from PublishTasks p left join QT_USER u on p.ReceiverName=u.UserName where PublishUserName='{1}'and ReceiverName!='' and ReceiverConfirm=0 order by PublishTime DESC ", DBName, User.Identity.Name);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {

                PublishTasks task = new PublishTasks();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = task.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(task, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                tasks.Add(task);
            }
            reader.Close();
            return tasks;
        }


        private void ChangeCompleteStatusTOComplete(int publishTaskID)
        {           
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; update  PublishTasks set CompleteStatus=N'{2}' where PublishTaskID={1};", DBName, publishTaskID,GetLBAMVC.Models.commonContext.CompleteStatus.已完成);
            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }
        }


        private List<PublishTasks> GetAllDeletePublishTasksByMe(string publisher)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from publishTasks where PublishUserName=N'{1}' and IsDeleted=1 order by PublishTime DESC ", DBName, publisher);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {

                PublishTasks task = new PublishTasks();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = task.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(task, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                tasks.Add(task);
            }
            reader.Close();
            return tasks;
        }

        private Decimal GetUserPointsByUserName(string userName)
        {
            Decimal points = 0;
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; select points from QT_USER where UserName='{1}'", DBName, userName);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);           

            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                DataRow dr = dt.Rows[0];//第一值 
                points = Decimal.Parse(dr["points"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlconn.Close();
            }
            return points;
        }

        #endregion 
    }
}
