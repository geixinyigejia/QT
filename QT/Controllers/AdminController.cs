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


        public ActionResult Chongzhi(string ChangedPoints, string alpayUserName, string alpayAcount)
        {
            Decimal changePoints = 0;
            if (!Decimal.TryParse(ChangedPoints, out changePoints))
            {
                return View();
            }

            string comment = string.Format(@"支付宝用户：{0},支付宝账号：{1}", alpayUserName, alpayAcount);

            string operationType = GetLBAMVC.Models.commonContext.ChangeMethod.充值.ToString();
            AlPayOperation(User.Identity.Name, operationType, changePoints, comment);

            return RedirectToAction("TaskRecords", "User");
        }

        public ActionResult Tixian(string ChangedPoints,string alpayUserName,string alpayAcount)
        {
            Decimal changePoints = 0;
            if (!Decimal.TryParse(ChangedPoints, out changePoints))
            {
                return View();
            }

            string comment = string.Format(@"支付宝用户：{0},支付宝账号：{1}",alpayUserName,alpayAcount);

            string operationType = GetLBAMVC.Models.commonContext.ChangeMethod.提现.ToString();
            AlPayOperation(User.Identity.Name,operationType,changePoints,comment);

            return RedirectToAction("TaskRecords", "User");
        }


        private void AlPayOperation(string userName, string operationType, Decimal ChangedPoints,string comment)
        {
            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0};insert QT_Financial_Record(UserName,RecordTime,ChangeMethod,ChangedBeforePoints,ChangedAfterPoints,Description,ChangedPoints) values(N'{1}','{2}',N'{3}',{4},{5},'{6}',{7});", DBName, User.Identity.Name, DateTime.Now, operationType, 0, 0, comment, ChangedPoints);
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
    }
}
