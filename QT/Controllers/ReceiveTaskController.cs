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
    public class ReceiveTaskController : Controller
    {
        private static string DBName = "QT";

        public ActionResult Index(int p=1)
        {
            List<PublishTasks> ReceivedTasksByMe = GetAllReceviedTasksByMe(User.Identity.Name);
            var pagedData = ReceivedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);   
        }   

        public ActionResult WaitToBeDone(int p=1)
        {
            List<PublishTasks> ReceivedTasksByMe = GetAllReceviedToBeDoneTasksByMe(User.Identity.Name);
            var pagedData = ReceivedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);   
        }

        public ActionResult DoneTasks(int p=1)
        {
            List<PublishTasks> ReceivedTasksByMe = GetAllDoneReceviedTasksByMe(User.Identity.Name);
            var pagedData = ReceivedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);   
        }

        private List<PublishTasks> GetAllReceviedTasksByMe(string username)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where  ReceiverName='{1}' order by PublishTime DESC ", DBName, username);

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

        private List<PublishTasks> GetAllReceviedToBeDoneTasksByMe(string username)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where  ReceiverName='{1}' order by PublishTime DESC ", DBName, username);

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

        private List<PublishTasks> GetAllDoneReceviedTasksByMe(string username)
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where  ReceiverName='{1}' order by PublishTime DESC ", DBName, username);

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
    }
}
