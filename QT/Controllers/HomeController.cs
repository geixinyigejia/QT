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
            ViewBag.CurrentUser = User.Identity.Name;
            User currentUser=GetUserByUserName(User.Identity.Name);
            ViewBag.points = currentUser.points;
            ViewBag.UserType = currentUser.UserType;

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
        
    }
}
