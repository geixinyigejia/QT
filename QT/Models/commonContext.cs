using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//using 
using System.Data.SqlClient;

namespace GetLBAMVC.Models
{
    public class commonContext
    {
        //public static string connString = "Data Source=(local);Integrated Security=True";
        public static string connString = "Data Source='JENNY\\SQLEXPRESS';Integrated Security=True";
     
        public static SqlConnection connectonToMSSQL()
        {

            SqlConnection Sqlconn = new SqlConnection(connString);

            return Sqlconn;

        }

     public    enum CheckStaus
        {
            待审核,
           审核通过
        };

   public  enum AcountStatus
        {
            禁用,
           正常
        } ;
    }
}
    