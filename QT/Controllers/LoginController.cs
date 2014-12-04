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


using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;

namespace GetLBAMVC.Controllers
{

    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private static string DBName = "QT";
        public static string connString = "Data Source=(local);Integrated Security=True";
        //public static string connString = "Data Source='JENNY\\SQLEXPRESS';Integrated Security=True";

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
        public ActionResult RegCheck(User user, string City = "北京市", string Provice = "北京市")
        {
            //if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != register.VerificationCode.ToUpper())
            //{
            //    ModelState.AddModelError("VerificationCode", "验证码不正确");
            //    return View(register);
            //}

            if (ModelState.IsValid)
            {
                user.RegistTime = DateTime.Now;
                user.LastLogonTime = DateTime.Now;
                user.Checkstatus = commonContext.CheckStaus.待审核.ToString();
                user.Acountstatus = commonContext.AcountStatus.禁用.ToString();
                user.AcountStartTime = DateTime.Now;
                user.city = Provice + "" + City;
                user.points = 0;
                user.UserType = "普通会员";
                user.Password = Sha256(user.Password);
                CreateUser(user);
            }
            Response.Write(string.Format("<script>alert('“{0} 注册成功！请等待我们管理人员的审核!”')</script>", user.UserName));
            return View("Index");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Index", "Login");
        }
        private void CreateUser(User user)
        {
            SqlConnection sqlconn = commonContext.connectonToMSSQL();

            string sqlCommand = string.Format(@"use {0}; insert QT_USER(UserName,Password,city,QQ,MobilePhone,RegistTime,Checkstatus,LastLogonTime,Acountstatus,AcountStartTime,points,UserType) values('{1}','{2}',N'{3}',{4},{5},'{6}',N'{7}','{8}',N'{9}','{10}',{11},N'{12}');", DBName, user.UserName, user.Password, user.city, user.QQ, user.MobilePhone, user.RegistTime, user.Checkstatus, user.LastLogonTime, user.Acountstatus, user.AcountStartTime, user.points, user.UserType);
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

            string sqlCommand = string.Format(@"use {0}; select UserName,Password from QT_USER where UserName='{1}' and Password='{2}'", DBName, user.UserName,Sha256(user.Password));

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

        #region
        /// <summary>
        /// 创建验证码字符
        /// </summary>
        /// <param name="length">字符长度</param>
        /// <returns>验证码字符</returns>
        public static string CreateVerificationText(int length)
        {
            char[] _verification = new char[length];
            char[] _dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            Random _random = new Random();
            for (int i = 0; i < length; i++) { _verification[i] = _dictionary[_random.Next(_dictionary.Length - 1)]; }
            return new string(_verification);
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="verificationText">验证码字符串</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片长度</param>
        /// <returns>图片</returns>
        public static Bitmap CreateVerificationImage(string verificationText, int width, int height)
        {
            Pen _pen = new Pen(Color.Black);
            Font _font = new Font("Arial", 14, FontStyle.Bold);
            Brush _brush = null;
            Bitmap _bitmap = new Bitmap(width, height);
            Graphics _g = Graphics.FromImage(_bitmap);
            SizeF _totalSizeF = _g.MeasureString(verificationText, _font);
            SizeF _curCharSizeF;
            PointF _startPointF = new PointF((width - _totalSizeF.Width) / 2, (height - _totalSizeF.Height) / 2);
            //随机数产生器
            Random _random = new Random();
            _g.Clear(Color.White);
            for (int i = 0; i < verificationText.Length; i++)
            {
                _brush = new LinearGradientBrush(new Point(0, 0), new Point(1, 1), Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)), Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
                _g.DrawString(verificationText[i].ToString(), _font, _brush, _startPointF);
                _curCharSizeF = _g.MeasureString(verificationText[i].ToString(), _font);
                _startPointF.X += _curCharSizeF.Width;
            }
            _g.Dispose();
            return _bitmap;
        }


        /// <summary>
        /// 256位散列加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>密文</returns>
        public static string Sha256(string plainText)
        {
            SHA256Managed _sha256 = new SHA256Managed();
            byte[] _cipherText = _sha256.ComputeHash(Encoding.Default.GetBytes(plainText));
            return Convert.ToBase64String(_cipherText);
        }


        #endregion 

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult VerificationCode()
        {
            string verificationCode = CreateVerificationText(6);
            Bitmap _img = CreateVerificationImage(verificationCode, 160, 30);
            _img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            TempData["VerificationCode"] = verificationCode.ToUpper();
            return null;
        }
    }
}

