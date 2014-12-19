using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//added using 
using GetLBAMVC.Models;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace GetLBAMVC.Controllers
{
    public class HomeController : Controller
    {
        private static string DBName = "QT";

        public ActionResult Index()
        {
            Session["UserName"] = User.Identity.Name;
            User currentUser=GetUserByUserName(User.Identity.Name);
            //ViewBag.points = currentUser.points;
            Session["points"] = currentUser.points;
            ViewBag.UserType = currentUser.UserType;
            Session["UserType"]=currentUser.UserType;

            Session["PublishTaskCount"] = GetPublishTaskByMonth(User.Identity.Name);
            Session["ReceivedTaskCount"] = 0;
            return View();
        }

        private User GetUserByUserName(string userName)
        {
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; select * from QT_USER where UserName='{1}'", DBName, userName);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            User personal = new User();
            while (reader.Read())
            {              

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = personal.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(personal, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
              
            }
            reader.Close();
            return personal;
        }

        public JsonResult GetTestData()
        {
            Models.User  user= GetUserByUserName(User.Identity.Name);
            Session["points"] = user.points;
            return Json(
                new { points=user.points }
            );
        }


        private int GetPublishTaskByMonth(string userName)
        {
            int result = 0;
         
            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0};select PublishTasks.PublishUserName,count(PublishTaskID) as PublishTaskCount from dbo.PublishTasks where PublishUserName=N'{1}' and YEAR(PublishTime)=YEAR(GETDATE()) and MONTH(PublishTime)=MONTH(GETDATE()) group by PublishTasks.PublishUserName,YEAR(PublishTime),MONTH(PublishTime)",DBName,userName);
            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
               result=int.Parse(reader["PublishTaskCount"].ToString());
            }
            reader.Close();
            
            return result;

            
        }

        private int GetReceviedTaskByMonth(string userName)
        {
            int result = -1;
           
            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0};select PublishTasks.PublishUserName,count(PublishTaskID) as PublishTaskCount from dbo.PublishTasks where PublishUserName=N'{1}' and YEAR(PublishTime)=YEAR(GETDATE()) and MONTH(PublishTime)=MONTH(GETDATE()) group by PublishTasks.PublishUserName,YEAR(PublishTime),MONTH(PublishTime)", DBName, userName);
            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                result = int.Parse(reader["PublishTaskCount"].ToString());
            }
            reader.Close();

            return result;
        }
    }
}
