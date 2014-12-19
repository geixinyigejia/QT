using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//
using System.Data;
using System.Data.SqlClient;
using GetLBAMVC.Models;
using System.Reflection;
using PagedList;

namespace GetLBAMVC.Controllers
{
    public class UserController : Controller
    {
        private static string DBName = "QT";

        public ActionResult Index()
        {
            GetLBAMVC.Models.User userInfo = GetcurrentUserInfo(User.Identity.Name);
            return View(userInfo);
        }

        public ActionResult TaskRecords()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult Suggestions( int p = 1)
        {           
            List<Suggestion> suggesions = GetAllSuggestions();
            var pagedData = suggesions.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData);
        }
        
        [HttpPost]
        public ActionResult Suggestions(string Comment,int p=1)
        {
            if(!string.IsNullOrEmpty(Comment))
            {
                InsertSuggestionToDB(Comment);
            }
            List<Suggestion> suggesions = GetAllSuggestions();
            var pagedData = suggesions.ToPagedList(pageNumber: p, pageSize: 20);
            return View(pagedData); 
        }

        #region 辅助方法
        private User GetcurrentUserInfo(string username)
        {
            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0}; select * from QT_USER  where UserName='{1}' ", DBName, username);
            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            User user = new User();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = user.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(user, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
            }
            reader.Close();
            return user;
        }

        private List<Suggestion> GetAllSuggestions()
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0}; select  * from QT_Suggestion order by PublishTime;", DBName);
            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
           
            while (reader.Read())
            {
                Suggestion suggestion = new Suggestion();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = suggestion.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(suggestion, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);                   
                }
                suggestions.Add(suggestion);
            }
            reader.Close();
            return suggestions;
        }

        private void InsertSuggestionToDB(string  comment)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            SqlConnection sqlconn = commonContext.connectonToMSSQL();
            string sqlCommand = string.Format(@"use {0}; insert QT_Suggestion(PublishUserName,PublishTime,Content) values(N'{1}','{2}',N'{3}');", DBName,User.Identity.Name,DateTime.Now,comment);
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
