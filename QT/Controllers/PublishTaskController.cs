using System;
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
        public ActionResult Index(PublishTasks publishTasks)
        {
            publishTasks.PublishUserName = User.Identity.Name;
            //get the publisher city
            //publishTasks.city = GetPublisherCity(User.Identity.Name);
            publishTasks.PublishTime = DateTime.Now;
            publishTasks.CompleteTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                PublishTask(publishTasks);
            }          
            return View();
        }

        public ActionResult PublishedTasks(int p = 1)
        {
            List<PublishTasks> PublishedTasksByMe = GetAllPublishTasksByMe(User.Identity.Name);
            var pagedData = PublishedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);            
        }

        public ActionResult ReceviedByPepole()
        {
            return View();
        }

        public ActionResult WaitToBeDone()
        {
            return View();
        }

        public ActionResult DoneTasks()
        {
            return View();
        }

        public ActionResult DeleteTasks()
        {
            return View();
        }


        #region 辅助方法

          private void PublishTask(PublishTasks task)
        {
            if (string.IsNullOrEmpty(task.Comment))
            {
                task.Comment = string.Empty;
            }
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; insert PublishTasks(PublishUserName,links,wangwangxiaohao,TaskPrice,charges,Comment,city,PublishTime,CompleteTime,TaskType ) values('{6}','{1}','{2}',{3},{4},'{5}','{7}','{8}','{9}','{10}');", DBName, task.links, task.wangwangxiaohao, task.TaskPrice, task.charges, task.Comment, task.PublishUserName, task.city, task.PublishTime, task.CompleteTime, task.TaskType);
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
            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks ", DBName);

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

            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}' and CompleteStatus=1 and  ReceiverName is not null", DBName, username);

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

            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'and ReceiverName is not null", DBName, User.Identity.Name);

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

            string sqlCommand = string.Format(@"use {0}; update  PublishTasks set CompleteStatus=1 where PublishTaskID={1};", DBName, publishTaskID);
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

        #endregion 
    }
}
