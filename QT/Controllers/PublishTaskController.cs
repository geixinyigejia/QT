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
        //互斥锁
          private static object locker = new object();

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

        public ActionResult ReceviedByPepole(int p=1)
        {
            List<PublishTasks> receviedByPepole = ReceivePushlishTasksByMe();
            var pagedData = receviedByPepole.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
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


        /// <summary>
        /// 接受任务处理
        /// </summary>
        /// <param name="PublishTaskID"></param>
        /// <returns></returns>
        public ActionResult JieshouTask(string publishTaskID, string PublishUserName)
        {
            if (PublishUserName == User.Identity.Name)
            {
                //自己不能接手自己发布的任务
                return View("Index");
            }

            //check the PublishTaskID is exsits
            //check the PublishTaskID is not recevied by anyone
            int PublishTaskID=-1;
            if (int.TryParse(publishTaskID, out PublishTaskID))
            {
            }
            lock (locker)
            {
                string ReceiverName = string.Empty;

                SqlConnection sqlconn = commonContext.connectonToMSSQL();

                string sqlCommand = string.Format(@"use {0};select * from publishTasks where PublishTaskID={1} and ReceiverName is null ;", DBName, PublishTaskID);
                sqlconn.Open();

                SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
                try
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    DataRow dr = dt.Rows[0];//第一值 
                    ReceiverName = dr["ReceiverName"].ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlconn.Close();
                }
              
                // receive this task
                if (string.IsNullOrEmpty(ReceiverName))
                {
                    sqlCommand = string.Format(@"use {0};update PublishTasks set ReceiverName='{1}' where PublishTaskID={2}", DBName, User.Identity.Name, PublishTaskID);
                    //string sqlCommand = string.Format(@"use {0}; insert ReceivedTasks(links,wangwangxiaohao,Price,Point,Comment,PublishTime,PublishUserName,ReceiverName,city) values({1},'{2}',{3},{4},'{5}','{6}','{7}','{8}','{9}') select @@identity as ReceivedTaskID;", DBName, task.links, task.wangwangxiaohao, task.Price, task.Point, task.Comment, task.PublishTime, task.PublishUserName, User.Identity.Name, task.city);
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

                }
                else
                {
                    //任务已被接手
                    return View ("Index");
                }

                // return to the "我已接受的任务-〉接手任务"
                return RedirectToAction("Index", "ReceiveTask");
            }
        }

        #region 辅助方法

          private void PublishTask(PublishTasks task)
        {
            if (string.IsNullOrEmpty(task.Comment))
            {
                task.Comment = string.Empty;
            }
            task.city = GetPublisherCity(task.PublishUserName);//get publisher city

            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; insert PublishTasks(PublishUserName,links,wangwangxiaohao,TaskPrice,charges,Comment,city,PublishTime,CompleteTime,TaskType ) values('{6}',N'{1}',N'{2}','{3}','{4}',N'{5}',N'{7}','{8}','{9}',N'{10}');", DBName, task.links, task.wangwangxiaohao, task.TaskPrice, task.charges, task.Comment, task.PublishUserName, task.city, task.PublishTime, task.CompleteTime, task.TaskType);
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
            string sqlCommand = string.Format(@"use {0}; select * from publishTasks where PublishUserName=N'{1}' and ReceiverName is null  ", DBName,username);

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

            string sqlCommand = string.Format(@"use {0}; select p.ReceiverName,u.city,p.links,p.wangwangxiaohao,p.TaskPrice,p.TaskType,p.Comment,p.charges from PublishTasks p left join QT_USER u on p.ReceiverName=u.UserName where PublishUserName='{1}'and ReceiverName is not null ", DBName, User.Identity.Name);

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
