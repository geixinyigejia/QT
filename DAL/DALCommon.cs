using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added new using 
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace DAL
{
    public class DALCommon
    {
        public static bool createdDB = false;
        public static string DBName = "CTPIntlTest";
        public static string defaultSchemaName = "dbo.";
       public static  string connString = "Data Source=(local);Integrated Security=True";

        public static SqlConnection connectonToMSSQL()
        {           
            SqlConnection Sqlconn = new SqlConnection(connString);
            return Sqlconn;
        }


        /// <summary>
        /// database name,like CTPIntlTest
        /// </summary>
        /// <param name="databaeName"></param>
        public static void CreateNewDataBase()
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Format(@"create database {0}", DBName);
            sb.Append(sql);

            string mdfPath = "\\DATA\\CTPIntlTest_data.mdf";
            string ldfpath = @"\\DATA\CTPIntlTest_log.ldf";
            sql = string.Format(" on primary(name =N'CTPIntlTest_data',filename='{0}',size=6120MB,maxsize=6500MB,FILEGROWTH=10%)", GetSQLServerInstallPath() + mdfPath);
            sb.Append(sql);

            sql = string.Format("  LOG ON(name=N'CTPIntlTest_log',filename='{0}',size=314MB,maxsize=500MB,FILEGROWTH=1);", GetSQLServerInstallPath() + ldfpath);
            sb.Append(sql);

            ExecuteSql(connString, "master", sb.ToString());
            createdDB = true;
        }

        public static void CreateDBTable(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("USE CTPIntlTest;  ");
            //sb.Append("CREATE SCHEMA CTP AUTHORIZATION dbo; ");  //这一条要单独运行 先运行  
            sb.Append(string.Format("create table {0}(IteamID		 NVARCHAR(1000)  null,Resource	 NVARCHAR(4000)  null,LanguageID	 NVARCHAR(15)   null,Translation	 NVARCHAR(4000) NULL);", tableName));
            //sb.Append("create table CTP.PCS(IteamID		 NVARCHAR(100)  NOT NULL PRIMARY KEY,Resource	 NVARCHAR(200)  NOT NULL,LanguageID	 NVARCHAR(15)   NOT NULL,Translation	 NVARCHAR(4000) NULL)");

            SqlConnection sqlconn = connectonToMSSQL();
            SqlCommand sqlcommmand = new SqlCommand(sb.ToString(), sqlconn);
            sqlcommmand.Connection.Open();
            try
            {
                sqlcommmand.ExecuteNonQuery();
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


        public static void ExecuteSql(string conn, string database, string sqlCommand)
        {
            SqlConnection sqlConn = new SqlConnection(conn);
            SqlCommand command = new SqlCommand(sqlCommand, sqlConn);
            command.Connection.Open();
            command.Connection.ChangeDatabase(database);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        #region operation regist: to check installed the SQL Server
        /// <summary>
        /// return the value like this: C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL
        /// </summary>
        /// <returns></returns>
        public static string GetSQLServerInstallPath()
        {
            string SQLPath = string.Empty;

            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey MSSQLServerKey = hkml.OpenSubKey(@"SOFTWARE\MICROSOFT\MSSQLServer");
            if (MSSQLServerKey != null)
            {
                string[] keys = MSSQLServerKey.GetSubKeyNames();
                RegistryKey setupKey = MSSQLServerKey.OpenSubKey("Setup");
                if (setupKey != null)
                {
                    SQLPath = setupKey.GetValue("SQLPath").ToString();
                }
                else
                {
                    SQLPath = Get64BitRegistryKey(RegistryHive.LocalMachine, @"SOFTWARE\MICROSOFT\MSSQLServer\Setup", "SQLPath", RegistryView.Registry64);
                }
            }
            return SQLPath;
        }

        /// <summary>
        /// this method is use for the 32appliction to get the 64bit regestkey
        /// </summary>
        /// <param name="parentKeyName"></param>
        /// <param name="subKeyName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string Get64BitRegistryKey(RegistryHive hive, string keyName, string valueName, RegistryView view)
        {
            Microsoft.Win32.SafeHandles.SafeRegistryHandle handle = new Microsoft.Win32.SafeHandles.SafeRegistryHandle(GetHiveHandle(hive), true);
            RegistryKey subkey = RegistryKey.FromHandle(handle, view).OpenSubKey(keyName);
            RegistryKey key = RegistryKey.FromHandle(subkey.Handle, view);
            return key.GetValue(valueName).ToString();

        }

        static IntPtr GetHiveHandle(RegistryHive hive)
        {
            IntPtr preexistingHandle = IntPtr.Zero;

            IntPtr HKEY_CLASSES_ROOT = new IntPtr(-2147483648);
            IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
            IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
            IntPtr HKEY_USERS = new IntPtr(-2147483645);
            IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);
            IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);
            IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);
            switch (hive)
            {
                case RegistryHive.ClassesRoot: preexistingHandle = HKEY_CLASSES_ROOT; break;
                case RegistryHive.CurrentUser: preexistingHandle = HKEY_CURRENT_USER; break;
                case RegistryHive.LocalMachine: preexistingHandle = HKEY_LOCAL_MACHINE; break;
                case RegistryHive.Users: preexistingHandle = HKEY_USERS; break;
                case RegistryHive.PerformanceData: preexistingHandle = HKEY_PERFORMANCE_DATA; break;
                case RegistryHive.CurrentConfig: preexistingHandle = HKEY_CURRENT_CONFIG; break;
                case RegistryHive.DynData: preexistingHandle = HKEY_DYN_DATA; break;
            }
            return preexistingHandle;
        }

        #endregion 



    }
}
