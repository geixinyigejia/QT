using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//USING
using System.Data.SqlClient;
using System.Data;


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
        public ActionResult RegCheck(string name, string password, string Phone, string QQ, string City = "北京市", string Provice = "北京市")
        {
            if (ModelState.IsValid)
            {

            }
            Response.Write(string.Format("<script>alert('“{0} 注册成功！请等待我们管理人员的审核!”')</script>", name));
            return View("Index");
        }
    }
}
