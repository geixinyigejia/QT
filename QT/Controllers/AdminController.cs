using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//added using 
using System.Data;
using System.Data.SqlClient;
using GetLBAMVC.Models;

namespace GetLBAMVC.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private static string DBName = "QT";       

        public ActionResult UserManagment()
        {
            List<string> users = new List<string>();

            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0}; select UserName from QT_USER where Checkstatus=N'{1}'and UserName not in (select UserName from QT_USER where UserType like   N'管理员')", DBName, commonContext.CheckStaus.待审核.ToString());
          
            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                users.Add(reader["UserName"].ToString());
            }
            reader.Close();
            ViewBag.Allusers = users;

            ViewBag.Verify = true;
            ViewBag.Grant = true;
            return View();
        }

    }
}
