using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace GetLBAMVC.Ajax
{
    /// <summary>
    /// Summary description for GetLanguage
    /// </summary>
    public class GetLanguage : IHttpHandler, IRequiresSessionState
    {
        private static string DBName = "QT";
        //public static string connString = "Data Source=(local);Integrated Security=True";
        public static string connString = "Data Source='JENNY\\SQLEXPRESS';Integrated Security=True";

        private HttpRequest request = null;
        private HttpResponse response = null;
        private HttpSessionState session = null;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            request = context.Request;
            response = context.Response;
            session = context.Session;
            string action = context.Request.Form["action"];

            switch (action)
            {
                case "GetLanguage": getLanguage(); break;
            }
        }

        private void getLanguage()
        {
             

            string result = string.Empty;
            string comName = request.Form["ComName"];
            List<String> languageList = new List<string>();

            if (!string.IsNullOrEmpty(comName))
            {
                string sqlCommand = string.Format(@"use {0}; select CityName from TB_City where ProvinceID in (select ID from TB_Province where ProvinceName like   N'{1}')", DBName, comName);
                SqlConnection sqlconn =   new SqlConnection(connString);
                sqlconn.Open();
                SqlCommand cmd = new SqlCommand(sqlCommand, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    languageList.Add(reader["CityName"].ToString());
                }
                reader.Close();
            }
            for (int i = 0; i < languageList.Count; i++)
            {
                if (i != 0)
                {
                    result += "|" + languageList[i];
                }
                else
                {
                    result += languageList[i];
                }
            }
            response.Write(result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}