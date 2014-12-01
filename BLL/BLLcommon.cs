using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added new using 
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace BLL
{
    public class BLLcommon
    {
        public static string DBName = "CTPIntlTest";
        public static string defaultSchemaName = "dbo.";

        static List<string> skipThoseFoldersEMS = new List<string> { "ar-ploc", "az", "az-cyrl", "iu-latn", "ja-ploc", "mn", "nb", "pt-br" };
        static List<string> skipThoseFoldersForPCS = new List<string> { "ar-ploc", "az", "az-cyrl", "iu-latn", "ja-ploc", "mn" };


        public static string GetLBAFullPath(string LBAPath, string componetName, bool isOld)
        {
            string fullPath = string.Empty;
            if (componetName.ToUpper().Contains("CMAT"))
            {
                if (isOld)
                {
                    fullPath = LBAPath + string.Format(@"\private\SCS\dev\LBA");
                }
                else
                {
                    fullPath = LBAPath + @"\private\SCS\dev\LBA";
                }
            }
            else
            {
                if (isOld)
                {
                    fullPath = LBAPath + string.Format(@"\private\pcs\LBA");
                }
                else
                {
                    fullPath = LBAPath + @"\private\pcs\LBA";
                }
            }
            if (!System.IO.Directory.Exists(fullPath))
            {
                fullPath = string.Empty;
            }
            return fullPath;
        }


        /// <summary>
        /// if the DB exists,it will return true, else return false
        /// </summary>
        /// <returns></returns>
        public static bool CheckDBExistsOrNot()
        {
            SqlConnection sqlconn = DAL.DALCommon.connectonToMSSQL();
            string sqlCommand = string.Format("use master;SELECT name FROM sysdatabases where name='{0}'", DBName);
            SqlDataAdapter sda = new SqlDataAdapter(sqlCommand, sqlconn);
            sqlconn.Close();

            DataSet ds = new DataSet();
            sda.Fill(ds, "CheckResult");
            DataTable dt = ds.Tables[0];
            if (ds == null || dt.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// check the table exists or not ,database name like CTPIntlTest
        /// </summary>
        /// <param name="DataBaseName"> table name like CTP.PCS</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool CheckTableExistsOrNot(string tableName)
        {
            SqlConnection sqlconn = DAL.DALCommon.connectonToMSSQL();
            string sqlCommand = string.Format("use {0}; SELECT TABLE_SCHEMA+'.'+TABLE_NAME as TABLE_NAEM FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'and TABLE_SCHEMA+'.'+TABLE_NAME='{1}'", DBName, defaultSchemaName + tableName);
            SqlDataAdapter sda = new SqlDataAdapter(sqlCommand, sqlconn);
            sqlconn.Close();

            DataSet ds = new DataSet();
            sda.Fill(ds, "CheckResult");
            DataTable dt = ds.Tables[0];
            if (ds == null || dt.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// intial the MSSQL database name
        /// </summary>
        /// <param name="LBAPath">the path value like this @"D:\CTP\csSCS.mainbackup\private\SCS\dev\LBA"</param>
        /// <param name="DatabaseName">the database name</param>
        /// <returns></returns>
        public static bool intialTestDB(string LBAPath, string compontName, bool isOld)
        {
            //List<string> LBA_Folders = GetAllSubFolders(@"D:\CTP\csSCS.mainbackup\private\SCS\dev\LBA");
            List<string> LBA_Folders = GetAllSubFolders(LBAPath, compontName, isOld);
            DataTable dt = GetTableSchema();

            foreach (string temp in LBA_Folders)
            {
                string filepath = null;
                string Namespace = null;
                //ingore the default folder
                if (temp.ToLower().Contains("default"))
                {
                    continue;
                }
                if (compontName == "CMAT" || compontName == "CMAT_BACKUP")
                {
                    filepath = string.Format(@"{0}\spsxr3lang\spsxr3lang.lspkg", temp);
                    Namespace = "/ns:LocPackage/ns:FileDataList/ns:FileData[@FileId='2.lcl']/lcx:LCX/lcx:Item";
                }
                if (compontName == "PCS" || compontName == "PCS_BACKUP")
                {
                    filepath = string.Format(@"{0}\Pcsv2\PCSV2.lspkg", temp);
                    Namespace = "/ns:LocPackage/ns:FileDataList/ns:FileData[@FileId='1.lcl']/lcx:LCX/lcx:Item/lcx:Item";
                }
                if (!System.IO.File.Exists(filepath))
                {
                    continue;
                }
                List<string> result = GetAllItemIDResource(filepath, Namespace);
                // 拆分数据           
                string itemID = string.Empty;
                string resource = string.Empty;
                string languageID = string.Empty;
                string translation = string.Empty;
                int len = result.Count;
                int count = len / 4;

                for (int i = 0; i < len; i = i + 4)
                {
                    itemID = result[i];
                    resource = result[i + 1];
                    languageID = result[i + 2];
                    translation = result[i + 3];

                    DataRow r = dt.NewRow();
                    r[0] = itemID;
                    r[1] = resource;
                    r[2] = languageID;
                    r[3] = translation;
                    dt.Rows.Add(r);
                }
            }
            return TableValueToDB(dt, compontName);

        }


        /// <summary>
        /// the folder like D:\CTP\CSSCS\private\SCS\dev\LBA which sub folder contains all languages resource
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static List<string> GetAllSubFolders(string folderPath, string componetName, bool isOld)
        {
            string fullLBAPath = GetLBAFullPath(folderPath, componetName, isOld);
            List<string> result = new List<string>();
            string[] lan_folders = Directory.GetDirectories(fullLBAPath);
            string[] splitChar = new string[] { "\\" };

            bool canadded = false;
            if (lan_folders[0].ToLower().Contains("pcs"))
            {

                for (int i = 0; i < lan_folders.Length; i++)
                {
                    canadded = true;

                    foreach (string temp in skipThoseFoldersForPCS)
                    {
                        int len = lan_folders[i].Split(splitChar, StringSplitOptions.None).Length;
                        if (lan_folders[i].Split(splitChar, StringSplitOptions.None)[len - 1].ToLower() == temp)
                        {
                            skipThoseFoldersForPCS.Remove(temp);
                            canadded = false;
                            break;
                        }
                    }
                    if (canadded == true)
                    {
                        result.Add(lan_folders[i]);
                    }
                }

            }
            else
            {
                for (int i = 0; i < lan_folders.Length; i++)
                {
                    canadded = true;

                    foreach (string temp in skipThoseFoldersEMS)
                    {
                        int len = lan_folders[i].Split(splitChar, StringSplitOptions.None).Length;
                        if (lan_folders[i].Split(splitChar, StringSplitOptions.None)[len - 1].ToLower() == temp)
                        {
                            skipThoseFoldersEMS.Remove(temp);
                            canadded = false;
                            break;
                        }
                    }
                    if (canadded == true)
                    {
                        result.Add(lan_folders[i]);
                    }
                }

            }

            return result;
        }


        public static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("IteamID", typeof(string)), new DataColumn("Resource", typeof(string)), new DataColumn("LanguageID", typeof(string)), new DataColumn("Translation", typeof(string)) });
            return dt;
        }

        public static List<string> GetAllItemIDResource(string filePath, string Namespace)
        {
            List<string> result = new List<string>();
            string[] splitChar = new string[] { "\\" };
            string[] filePathList = filePath.Split(splitChar, StringSplitOptions.None);
            string LanguageID = filePathList[filePathList.Length - 3].ToString();

            XmlDocument doc = new XmlDocument();
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("ns", "http://schemas.microsoft.com/locstudio/2006/6/lspackage");
            nsMgr.AddNamespace("lcx", "http://schemas.microsoft.com/locstudio/2006/6/lcx");


            doc.Load(filePath);

            //XNamespace d=@"http://schemas.microsoft.com/locstudio/2006/6/lspackage";
            XmlNodeList notes = doc.SelectSingleNode(Namespace, nsMgr).ChildNodes;

            #region 遍历结点

            foreach (XmlNode node in notes)
            {
                if (node.Name == "Item")
                {
                    bool havaValue = false;
                    bool havsTgt = false;

                    XmlNodeList subNodes = node.ChildNodes;
                    foreach (XmlElement subnode in subNodes)
                    {
                        if (subnode.Name == "Str")
                        {
                            string value = string.Empty;
                            string tgt = string.Empty;

                            XmlNodeList leafNodes = subnode.ChildNodes;
                            foreach (XmlElement leaf in leafNodes)
                            {

                                if (leaf.Name == "Val")
                                {
                                    havaValue = true;
                                    value = leaf.InnerText;
                                }
                                if (leaf.Name == "Tgt")
                                {
                                    XmlNodeList sNodes = leaf.ChildNodes;
                                    foreach (XmlElement s in sNodes)
                                    {
                                        if (s.Name == "Val")
                                        {
                                            havsTgt = true;
                                            tgt = leaf.InnerText;
                                        }
                                    }
                                }
                                if (havaValue && havsTgt)
                                {
                                    result.Add(node.Attributes["ItemId"].Value);
                                    result.Add(value);
                                    result.Add(LanguageID);
                                    result.Add(tgt);
                                    havaValue = false;
                                    havsTgt = false;

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return result;
        }

        public static bool TableValueToDB(DataTable dt, string tableName)
        {
            SqlConnection sqlConn = DAL.DALCommon.connectonToMSSQL();
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
            //bulkCopy.DestinationTableName = "Twan.dbo.EMS_BACKUP";
            bulkCopy.DestinationTableName = DBName + "." + defaultSchemaName + tableName;
            bulkCopy.BatchSize = dt.Rows.Count;

            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                {
                    bulkCopy.Close();
                }
            }
        }


    }
}
