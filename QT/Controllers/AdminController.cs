using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//added using 
using System.Data;
using System.Data.SqlClient;
using GetLBAMVC.Models;
using System.Reflection;

using PagedList;
using System.Text;

namespace GetLBAMVC.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private static string DBName = "QT";

        public ActionResult UserManagment(string UsersID, int p = 1)
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

            List<User> usersQuery = new List<User>();

            if (string.IsNullOrEmpty(UsersID))
            {
                sqlCommand = string.Format(@"use {0}; select * from QT_USER where Checkstatus=N'{1}'and UserName not in (select UserName from QT_USER where UserType like   N'管理员')", DBName, commonContext.CheckStaus.待审核.ToString());
            }
            else
            {
                sqlCommand = string.Format(@"use {0}; select * from QT_USER  where UserName='{1}' Checkstatus=N'{2}'and UserName not in (select UserName from QT_USER where UserType like   N'管理员')", DBName,UsersID, commonContext.CheckStaus.待审核.ToString());
            }
                sqlconn.Open();
                cmd = new SqlCommand(sqlCommand, sqlconn);
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    User user = new User();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo property = user.GetType().GetProperty(reader.GetName(i));
                        property.SetValue(user, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                    }
                    usersQuery.Add(user);
                }
                reader.Close();

                var pagedData = usersQuery.ToPagedList(pageNumber: p, pageSize: 20);
                return View(pagedData);
            }          
            
 

        public ActionResult ChargeManagment(string UsersID, int p = 1)
        {
            List<string> users = new List<string>();

            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0}; select UserName from QT_USER ", DBName);

            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                users.Add(reader["UserName"].ToString());
            }
            reader.Close();
            ViewBag.SelectedUsers = users;

            if (!string.IsNullOrEmpty(UsersID))
            {
                ViewBag.SelectedUsers = UsersID;
            }

            List<FinancialRecord> financialRecords = GetFinancialRecordsByUsersNames(UsersID);
            var pagedData = financialRecords.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);

        }

        public ActionResult UserQueryOperation(string UsersID,int p=1)
        {
            UsersID = "admin";
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

            List<User> usersQuery=new List<User>();
            if (string.IsNullOrEmpty(UsersID))
            {
               
            }
            else
            {
                 sqlCommand = string.Format(@"use {0}; select * from QT_USER  where UserName='{1}' ", DBName, UsersID);
                sqlconn.Open();
                 cmd = new SqlCommand(sqlCommand, sqlconn);
                 reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);               
                while (reader.Read())
                {
                    User user = new User();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        PropertyInfo property = user.GetType().GetProperty(reader.GetName(i));
                        property.SetValue(user, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                    }
                    usersQuery.Add(user);
                }
                reader.Close();               
            }

            var pagedData = usersQuery.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
            

        }

        public ActionResult ChargeQuery(string UsersID, int p = 1)
        {
            List<string> users = new List<string>();

            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0}; select UserName from QT_USER ", DBName);

            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                users.Add(reader["UserName"].ToString());
            }
            reader.Close();           
            ViewBag.SelectedUsers = users;

            if (!string.IsNullOrEmpty(UsersID))
            {
                ViewBag.SelectedUsers = UsersID;
            }

            List<FinancialRecord> financialRecords = GetFinancialRecordsByUsersNames(UsersID);
            var pagedData = financialRecords.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
           
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

        #region 辅助方法
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

        private List<FinancialRecord> GetFinancialRecordsByUsersNames(string username)
        {
            List<FinancialRecord> financialRecords = new List<FinancialRecord>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0};select * from QT_Financial_Record where 1=1  ", DBName );

            if (string.IsNullOrEmpty(username))
            {
            }
            else
            {
                string[] usernames = username.Split(',');

                if (usernames.Count() == 1)
                {
                    sqlCommand += string.Format(" and UserName='{0}'", username);
                }

                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("'" + usernames[0] + "'");
                    for (int i = 1; i < usernames.Count(); i++)
                    {

                        sb.Append("," + string.Format((" '{0}' "), usernames[i]));
                    }

                    sqlCommand += string.Format(" and UserName in ({0})", sb.ToString());
                }
            }

            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
          
            while (reader.Read())
            {
                FinancialRecord financialRecord = new FinancialRecord();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = financialRecord.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(financialRecord, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                financialRecords.Add(financialRecord);
            }
            reader.Close();
            return financialRecords;
        }

        #endregion 

    }
}
