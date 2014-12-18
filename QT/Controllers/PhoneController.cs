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
    public class PhoneController : Controller
    {
        private static string DBName = "QT";

        public ActionResult Index(int p = 1)
        {
            List<PublishTasks> PublishedTasksByMe = GetAllPublishTasksByPhone();
            var pagedData = PublishedTasksByMe.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
        }



        private List<PublishTasks> GetAllPublishTasksByPhone()
        {
            List<PublishTasks> tasks = new List<PublishTasks>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            //string sqlCommand = string.Format(@"use {0}; select * from PublishTasks where PublishUserName='{1}'", DBName, username);
            string sqlCommand = string.Format(@"use {0}; select * from publishTasks where TaskType=N'{1}'  and IsDeleted=0 and ReceiverName='' order by PublishTime DESC ", DBName, GetLBAMVC.Models.commonContext.TaskType.手机);

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
