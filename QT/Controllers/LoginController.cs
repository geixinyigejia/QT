using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//USING
using System.Data.SqlClient;
using System.Data;
using GetLBAMVC.Models;

using System.Reflection;
using System.Web.Security;


namespace GetLBAMVC.Controllers
{

    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private static string DBName = "QT";
        public static string connString = "Data Source=(local);Integrated Security=True";

        public static SqlConnection connectonToMSSQL()
        {

            SqlConnection Sqlconn = new SqlConnection(connString);

            return Sqlconn;

        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Logon user)
        {

            if (ModelState.IsValid)
            {
                if (CheckUserLogonStatus(user))
                {
                    //跳转到所有任务页面
                    FormsAuthentication.SetAuthCookie(user.UserName, createPersistentCookie: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //跳转到注册页面
                    return RedirectToAction("Register");
                }

            }
            return View();
        }

        public ActionResult Register(string name, FormCollection form, string Provice = "北京市")
        {
            List<string> Provices = new List<string>();
            List<string> Citys = new List<string>();

            string sqlCommand = string.Format(@"use {0}; select * from TB_Province", DBName);
            SqlConnection sqlconn = connectonToMSSQL();
            sqlconn.Open();
            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                Provices.Add(reader["ProvinceName"].ToString());
            }
            reader.Close();
            ViewBag.Provice = Provices;

            if (!string.IsNullOrEmpty(Provice))
            {
                sqlCommand = string.Format(@"use {0}; select CityName from TB_City where ProvinceID in (select ID from TB_Province where ProvinceName like   N'{1}')", DBName, Provice);
                sqlconn = connectonToMSSQL();
                sqlconn.Open();
                cmd = new SqlCommand(sqlCommand, sqlconn);
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    Provices.Add(reader["CityName"].ToString());
                }
                reader.Close();
            }
            ViewBag.City = Citys;

            return View();
        }

        [HttpPost]
        //public ActionResult RegCheck(string name, string password, string Phone, string QQ, string City = "北京市", string Provice = "北京市")
        public ActionResult RegCheck(User user,string City = "北京市", string Provice = "北京市")
        {
            if (ModelState.IsValid)
            {
                user.RegistTime = DateTime.Now;
                user.LastLogonTime = DateTime.Now;
                user.Checkstatus = CheckStaus.待审核.ToString();
                user.Acountstatus = AcountStatus.禁用.ToString();
                user.AcountStartTime = DateTime.Now;
                user.city = Provice +""+ City;
                user.points = 0;

                CreateUser(user);
            }
            Response.Write(string.Format("<script>alert('“{0} 注册成功！请等待我们管理人员的审核!”')</script>", user.UserName));
            return View("Index");
        }

        private void CreateUser(User user)
        {
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; insert QT_USER(UserName,Password,city,QQ,MobilePhone,RegistTime,Checkstatus,LastLogonTime,Acountstatus,AcountStartTime,points) values('{1}','{2}',N'{3}',{4},{5},'{6}',N'{7}','{8}',N'{9}','{10}',{11});", DBName, user.UserName, user.Password, user.city, user.QQ, user.MobilePhone,user.RegistTime,user.Checkstatus,user.LastLogonTime,user.Acountstatus,user.AcountStartTime,user.points);
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
        
        private bool CheckUserLogonStatus(Logon user)
        {
            List<Logon> users = new List<Logon>();
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; select UserName,Password from QT_USER where UserName='{1}' and Password='{2}'", DBName, user.UserName, user.Password);

            sqlconn.Open();

            SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);

            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {

                Logon personal = new Logon();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = personal.GetType().GetProperty(reader.GetName(i));
                    property.SetValue(personal, reader.IsDBNull(i) ? "[null]" : reader.GetValue(i), null);
                }
                users.Add(personal);
            }
            reader.Close();
            if (users.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        enum CheckStaus
        {
            待审核,
           审核通过
        };

    enum AcountStatus
        {
            禁用,
           正常
        } ;
    }
}
