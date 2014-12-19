using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//
using PagedList;
using System.Data;
using System.Data.SqlClient;
using GetLBAMVC.Models;
using System.Reflection;

namespace GetLBAMVC.Controllers
{
    public class PhoneComputerController : Controller
    {
        private static string DBName = "QT";
        //互斥锁
        private static object locker = new object();

        public ActionResult Index(int p = 1)
        {
            List<PublishTasks> PublishedTasksByMe = GetAllPublishTasksByComputerPhone();
            var pagedData = PublishedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
        }



        private List<PublishTasks> GetAllPublishTasksByComputerPhone()
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from publishTasks where TaskType=N'{1}'  and IsDeleted=0 and ReceiverName='' order by PublishTime DESC ", DBName, GetLBAMVC.Models.commonContext.TaskType.电脑手机);

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
            int PublishTaskID = -1;
            if (int.TryParse(publishTaskID, out PublishTaskID))
            {
            }
            lock (locker)
            {
                string ReceiverName = string.Empty;

                SqlConnection sqlconn = commonContext.connectonToMSSQL();

                string sqlCommand = string.Format(@"use {0};select * from publishTasks where PublishTaskID={1} and IsDeleted=0 and ReceiverName=''  ;", DBName, PublishTaskID);
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
                    return View("Index");
                }

                // return to the "我已接受的任务-〉接手任务"
                return RedirectToAction("Index", "ReceiveTask");
            }
        }

    }
}
