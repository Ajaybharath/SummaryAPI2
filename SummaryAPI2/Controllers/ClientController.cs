using SummaryAPI2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Net.Mail;
using System.IO;
using System.Management;
using Amazon.CloudWatch;
using Amazon;
using Amazon.CloudWatch.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http.Cors;

namespace SummaryAPI2.Controllers
{
    //    [EnableCors("*", "*", "*")]
    [RoutePrefix("API/Client")]
    public class ClientController : ApiController
    {
        public string goodVal, warningVal, criticalVal;
        public int goodCount, warningCount, criticalCount;
        string[] skipSD;
        string[] skipSDDD;
        //[HttpPost]
        //[Route("Details")]
        //public dynamic get_clientDetails(Client c)
        //{

        //    List<clientDetails> lstClients = new List<clientDetails>();
        //    string[] skipSD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');

        //    try
        //    {
        //        if (c.uid == "idea" && c.pwd == "bytes")
        //        {


        //            string conSqlMain = string.Empty;
        //            string conSqlSub = string.Empty;
        //            string subDomain = string.Empty;
        //            string clientDetails = string.Empty;

        //            for (int i = 1; i < 10; i++)
        //            {
        //                try
        //                {

        //                    conSqlMain = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);
        //                    // clientDetails = "uid=sa;pwd=Ide@123;database=AB;server=AJAYBHARATH\\SQLEXPRESS";
        //                    DataSet dsClientData = new DataSet();

        //                    using (SqlConnection cnMain = new SqlConnection(conSqlMain))
        //                    {
        //                        SqlDataAdapter da = new SqlDataAdapter("select DomainName,IoTDomain from clientdetails", cnMain);

        //                        da.Fill(dsClientData);
        //                    }


        //                    for (int sd = 0; sd < dsClientData.Tables[0].Rows.Count; sd++)
        //                    {
        //                        clientDetails cD = new clientDetails();

        //                        subDomain = Convert.ToString(dsClientData.Tables[0].Rows[sd]["DomainName"]);

        //                        conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);


        //                        cD.domain = Convert.ToString(dsClientData.Tables[0].Rows[sd]["IoTDomain"]);
        //                        cD.subDomain = subDomain;

        //                        if (Array.IndexOf(skipSD, subDomain) == -1)
        //                        {
        //                            cD.reporting = "0";
        //                            cD.notReporting = "0";

        //                            try
        //                            {
        //                                DataSet dsReporting = new DataSet();
        //                                DataSet dsNotReporting = new DataSet();
        //                                DataSet dsRegions = new DataSet();

        //                                using (SqlConnection cnMain = new SqlConnection(conSqlSub))
        //                                {
        //                                    SqlDataAdapter da = new SqlDataAdapter("select count(*) as notReporting from sensordetails where isnull(isreporting, '0') = '0'", cnMain);

        //                                    da.Fill(dsNotReporting);
        //                                }

        //                                using (SqlConnection cnMain = new SqlConnection(conSqlSub))
        //                                {
        //                                    SqlDataAdapter da = new SqlDataAdapter("select count(*) as Reporting from sensordetails where isreporting = '1'", cnMain);

        //                                    da.Fill(dsReporting);
        //                                }


        //                                cD.reporting = Convert.ToString(dsReporting.Tables[0].Rows[0]["Reporting"]);
        //                                cD.notReporting = Convert.ToString(dsNotReporting.Tables[0].Rows[0]["notReporting"]);


        //                                if (cD.subDomain == "vignaninstruments")
        //                                {
        //                                    cD.portal = "https://web.vlogdata.net/";
        //                                }
        //                                else
        //                                {
        //                                    cD.portal = "https://" + cD.subDomain + "." + cD.domain;
        //                                }
        //                            }
        //                            catch (Exception exI)
        //                            {
        //                                exI = null;
        //                            }

        //                            cD.totalDevice = (Convert.ToInt32(cD.reporting) + Convert.ToInt32(cD.notReporting)).ToString();

        //                            lstClients.Add(cD);
        //                        }
        //                    }

        //                }
        //                catch (Exception exFor)
        //                {
        //                    exFor = null;

        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex = null;
        //    }


        //    return lstClients;

        //}

        [HttpPost]
        [Route("regions")]
        public dynamic getRions(Client c)
        {

            List<region> lstRegions = new List<region>();
            string[] skipSD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');

            try
            {
                if (c.uid == "idea" && c.pwd == "bytes")
                {
                    string conSqlMain = string.Empty;
                    string conSqlSub = string.Empty;
                    string subDomain = string.Empty;


                    for (int i = 1; i < 10; i++)
                    {
                        try
                        {

                            conSqlMain = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);

                            DataSet dssubDomains = new DataSet();

                            using (SqlConnection cnMain = new SqlConnection(conSqlMain))
                            {
                                SqlDataAdapter da = new SqlDataAdapter("select DomainName,IoTDomain from clientdetails", cnMain);

                                da.Fill(dssubDomains);
                            }


                            for (int sd = 0; sd < dssubDomains.Tables[0].Rows.Count; sd++)
                            {
                                region cD = new region();

                                subDomain = Convert.ToString(dssubDomains.Tables[0].Rows[sd]["DomainName"]);
                                if (subDomain == "vignaninstruments")
                                {
                                    conSqlSub = conSqlMain.Replace("IoTMainData", "vignaninstruments_live");
                                }
                                else
                                {
                                    conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);
                                }

                                //conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);



                                if (Array.IndexOf(skipSD, subDomain) == -1)
                                {
                                    try
                                    {

                                        DataSet dsRegions = new DataSet();

                                        //regions
                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select region_name as Region,latitude,longitude from regiondetails", cnMain);

                                            da.Fill(dsRegions);
                                        }



                                        for (int ri = 0; ri < dsRegions.Tables[0].Rows.Count; ri++)
                                        {
                                            region r = new region();

                                            r.name = Convert.ToString(dsRegions.Tables[0].Rows[ri]["Region"]);
                                            r.location = Convert.ToString(dsRegions.Tables[0].Rows[ri]["latitude"]) + "," + Convert.ToString(dsRegions.Tables[0].Rows[ri]["longitude"]);


                                            lstRegions.Add(r);
                                        }
                                    }
                                    catch (Exception exFor)
                                    {
                                        exFor = null;

                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception exx)
                        {
                            exx = null;
                        }
                    }
                }

            }
            catch (Exception exxx)
            {
                exxx = null;
            }
            return lstRegions;

        }

        [HttpPost]
        [Route("getDateTime")]
        public dynamic getDateTime()
        {
            reportTime rt = new reportTime();


            rt.reportAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm");

            return rt;
        }

        [HttpPost]
        [Route("Access")]
        public dynamic Login()
        {
            Login login = new Login();
            try
            {
                string con = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "CentralizedDB");
                SqlDataAdapter da1; DataSet ds;
                SqlConnection cn = new SqlConnection(con);
                da1 = new SqlDataAdapter("select top 1 * from AccessKeyTable order by slno desc", cn);
                ds = new DataSet();
                da1.Fill(ds);
                string accesskey = ds.Tables[0].AsEnumerable().Select(x => x["access"].ToString()).FirstOrDefault();
                login.Password = accesskey;
                da1 = new SqlDataAdapter("select * from Portal_UserId", cn);
                ds = new DataSet();
                da1.Fill(ds);
                string UserId = ds.Tables[0].AsEnumerable().Select(x => x["UserId"].ToString()).FirstOrDefault();
                login.UserId = UserId;
            }
            catch (Exception ex)
            {
                ex = null;
            }
            return login;
        }

        [HttpPost]
        [Route("ClientData")]
        public dynamic getClientData(Client c)
        {
            //GettingRegions

            List<region> lstRegions = new List<region>();
            string[] skipSD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');

            try
            {
                if (c.uid == "idea" && c.pwd == "bytes")
                {
                    string conSqlMain = string.Empty;
                    string conSqlSub = string.Empty;
                    string subDomain = string.Empty;


                    for (int i = 1; i < 10; i++)
                    {
                        try
                        {

                            conSqlMain = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);

                            DataSet dssubDomains = new DataSet();

                            using (SqlConnection cnMain = new SqlConnection(conSqlMain))
                            {
                                SqlDataAdapter da = new SqlDataAdapter("select DomainName,IoTDomain from clientdetails", cnMain);

                                da.Fill(dssubDomains);
                            }


                            for (int sd = 0; sd < dssubDomains.Tables[0].Rows.Count; sd++)
                            {
                                region cD = new region();

                                subDomain = Convert.ToString(dssubDomains.Tables[0].Rows[sd]["DomainName"]);
                                if (subDomain == "vignaninstruments")
                                {
                                    conSqlSub = conSqlMain.Replace("IoTMainData", "vignaninstruments_live");
                                }
                                else
                                {
                                    conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);
                                }

                                //conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);



                                if (Array.IndexOf(skipSD, subDomain) == -1)
                                {
                                    try
                                    {

                                        DataSet dsRegions = new DataSet();

                                        //regions
                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select region_name as Region,latitude,longitude from regiondetails", cnMain);

                                            da.Fill(dsRegions);
                                        }



                                        for (int ri = 0; ri < dsRegions.Tables[0].Rows.Count; ri++)
                                        {
                                            region r = new region();

                                            r.name = Convert.ToString(dsRegions.Tables[0].Rows[ri]["Region"]);
                                            r.location = Convert.ToString(dsRegions.Tables[0].Rows[ri]["latitude"]) + "," + Convert.ToString(dsRegions.Tables[0].Rows[ri]["longitude"]);


                                            lstRegions.Add(r);
                                        }
                                    }
                                    catch (Exception exFor)
                                    {
                                        exFor = null;

                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception exx)
                        {
                            exx = null;
                            break;
                        }
                    }
                }

            }
            catch (Exception exxx)
            {
                exxx = null;
            }



            //ClientData

            skipSDDD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');
            string conSqlMain1 = string.Empty;
            //string conSqlCentral = string.Empty;
            string subDomain1 = string.Empty;
            List<string> existingClients = new List<string>();
            //deleting subdomains which doesn't exist
            for (int i = 1; i < 10; i++)
            {
                try
                {
                    // delete not existing Subdomains
                    conSqlMain1 = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);
                    DataSet clientsData = new DataSet();
                    using (SqlConnection cnMain = new SqlConnection(conSqlMain1))
                    {
                        SqlDataAdapter da = new SqlDataAdapter("select DomainName,IoTDomain from clientdetails", cnMain);
                        da.Fill(clientsData);
                    }
                    existingClients.AddRange(clientsData.Tables[0].AsEnumerable().Select(x => x[0].ToString()).ToList());
                }
                catch (Exception ex)
                {
                    if (conSqlMain1 != "")
                    {
                        c.ErrorLogs(ex.Message, "Inserting Clientsdetails Data into CentralizedDB if not Exists" + conSqlMain1);
                        ex = null;
                    }
                    else if (conSqlMain1 == "")
                    {
                        c.ErrorLogs(ex.Message, "At Inserting Clientsdetails Data into CentralizedDB if not Exists due to Empty connection string" + conSqlMain1);
                        ex = null;
                        break;
                    }

                }
            }
            try
            {
                string con = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "CentralizedDB");
                //string con = "uid=sa;pwd=Ide@123;database=AB;server=DESKTOP-FMJB5MP";
                using (SqlConnection cn = new SqlConnection(con))
                {
                    SqlDataAdapter da1 = new SqlDataAdapter("select * from centralcontrol", cn);
                    DataSet ds = new DataSet();
                    da1.Fill(ds);
                    List<string> deletingClients = ds.Tables[0].AsEnumerable().Select(x => x["Subdomain"].ToString()).ToList();
                    var results = deletingClients.Where(m => !existingClients.Contains(m));
                    //string str = string.Join(",", results);
                    string deleteClients = "'" + String.Join("','", results) + "'";
                    deleteClients = "delete from centralcontrol where Subdomain in " + "(" + deleteClients + ")";
                    cn.Open();
                    SqlCommand sqlCommand = new SqlCommand(deleteClients, cn);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception exL)
            {
                c.ErrorLogs(exL.Message, "From CentralizedDataBase while  Deleting");
                exL = null;
            }



            //data Inserting into central DashBoard
            for (int i = 1; i < 10; i++)
            {
                //Inserting Clientsdetails Data into CentralizedDB if not Exists
                try
                {
                    // delete not existing Subdomains
                    conSqlMain1 = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);
                    DataSet clientsData = new DataSet();
                    using (SqlConnection cnMain = new SqlConnection(conSqlMain1))
                    {
                        SqlDataAdapter da = new SqlDataAdapter("select * from clientdetails", cnMain);
                        da.Fill(clientsData);
                    }
                    string con = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "CentralizedDB");
                    SqlConnection connection = new SqlConnection(con);//"uid=sa;pwd=Ide@123;database=AB;server=DESKTOP-FMJB5MP"



                    SqlCommand sqlCommand = new SqlCommand("Insert_centralcontrol", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    for (int cd = 0; cd < clientsData.Tables[0].Rows.Count; cd++)
                    {
                        connection.Open();
                        sqlCommand.Parameters.Clear();
                        subDomain1 = Convert.ToString(clientsData.Tables[0].Rows[cd]["DomainName"]);
                        if (Array.IndexOf(skipSDDD, subDomain1) == -1)
                        {
                            sqlCommand.Parameters.Add("Name", SqlDbType.VarChar).Value = clientsData.Tables[0].Rows[cd]["ClientName"];
                            //sqlCommand.Parameters.Add("Subdomain",SqlDbType.VarChar).Value = subDomain1.ToString() == "vignaninstruments" ? "web" : clientsData.Tables[0].Rows[cd]["DomainName"];
                            sqlCommand.Parameters.Add("Subdomain", SqlDbType.VarChar).Value = clientsData.Tables[0].Rows[cd]["DomainName"];
                            sqlCommand.Parameters.Add("domain", SqlDbType.VarChar).Value = clientsData.Tables[0].Rows[cd]["IoTDomain"];
                            sqlCommand.Parameters.Add("APIListener", SqlDbType.VarChar).Value = clientsData.Tables[0].Rows[cd]["ListenerURL"];
                            sqlCommand.Parameters.Add("MQTTListenerTopic", SqlDbType.VarChar).Value = clientsData.Tables[0].Rows[cd]["mqtt_topic"];
                            sqlCommand.Parameters.Add("portalurl", SqlDbType.VarChar).Value = clientsData.Tables[0].Rows[cd]["DomainURL"];
                            sqlCommand.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (conSqlMain1 != "")
                    {
                        c.ErrorLogs(ex.Message, "Inserting Clientsdetails Data into CentralizedDB if not Exists" + conSqlMain1);
                        ex = null;
                    }
                    else if (conSqlMain1 == "")
                    {
                        c.ErrorLogs(ex.Message, "At Inserting Clientsdetails Data into CentralizedDB if not Exists due to Empty connection string" + conSqlMain1);
                        ex = null;
                        break;
                    }

                }
            }


            List<clientData> lstclientData = new List<clientData>();
            try
            {
                //getting summary details from centralizedDB
                string[] skipSDD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');
                if (c.uid == "idea" && c.pwd == "bytes")
                {
                    //CentralizedDB
                    try
                    {
                        string con = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "CentralizedDB");
                        //string con = "uid=sa;pwd=Ide@123;database=AB;server=DESKTOP-FMJB5MP";
                        SqlConnection cn = new SqlConnection(con);
                        SqlDataAdapter da1 = new SqlDataAdapter("select * from centralcontrol", cn);
                        DataSet ds = new DataSet();
                        da1.Fill(ds);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            clientData clientData = new clientData();
                            clientData.C_CID = ds.Tables[0].Rows[i]["C_CID"].ToString();
                            clientData.domain = ds.Tables[0].Rows[i]["Domain"].ToString();
                            clientData.subDomain = ds.Tables[0].Rows[i]["Subdomain"].ToString();
                            clientData.portal = ds.Tables[0].Rows[i]["PortalURL"].ToString();
                            lstclientData.Add(clientData);
                        }
                    }
                    catch (Exception exL)
                    {
                        c.ErrorLogs(exL.Message, "From CentralizedDataBase");
                        exL = null;
                    }
                    //reporting
                    string conSqlMain = string.Empty;
                    string conSqlSub = string.Empty;
                    string subDomain = string.Empty;
                    string clientDetails = string.Empty;
                    //string goodVal, warningVal, criticalVal;
                    //select count(*) as notReporting from sensordetails where isnull(isreporting, '0') = '0'
                    //select count(*) as notReporting from sensordetails where isreporting = '0'"
                    for (int i = 1; i < 10; i++)
                    {
                        //DomainName,IoTDomain from clientdetails
                        try
                        {

                            List<clientDetails> lstClients = new List<clientDetails>();
                            conSqlMain = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);
                            DataSet dsClientData = new DataSet();

                            using (SqlConnection cnMain = new SqlConnection(conSqlMain))
                            {
                                SqlDataAdapter da = new SqlDataAdapter("select DomainName,IoTDomain from clientdetails", cnMain);
                                da.Fill(dsClientData);
                            }
                            for (int sd = 0; sd < dsClientData.Tables[0].Rows.Count; sd++)
                            {
                                subDomain = Convert.ToString(dsClientData.Tables[0].Rows[sd]["DomainName"]);
                                if (subDomain == "vignaninstruments")
                                {
                                    conSqlSub = conSqlMain.Replace("IoTMainData", "vignaninstruments_live");
                                }
                                else
                                {
                                    conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);
                                }
                                //string newConn = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionStringSplit"]);
                                //string[] newConnection = Convert.ToString(ConfigurationManager.AppSettings["changedDataBase"]).Split(',');
                                //if (Array.IndexOf(newConnection, subDomain) != -1)
                                //{
                                //    conSqlSub = newConn.Replace("IoTMainData", subDomain);
                                //    //if (subDomain == "vignaninstruments")
                                //    //{
                                //    //    conSqlSub = newConn.Replace("IoTMainData", "vignaninstruments_live");
                                //    //}
                                //    //else
                                //    //{
                                //    //    conSqlSub = newConn.Replace("IoTMainData", subDomain);
                                //    //}
                                //}

                                if (Array.IndexOf(skipSDD, subDomain) == -1)
                                {
                                    //Getting data from Each SubDomain dsNotReporting,dsReporting,dsLoginId
                                    //select count(*) as notReporting from sensordetails where isnull(isreporting, '0') = '0'
                                    try
                                    {
                                        DataSet dsReporting = new DataSet();
                                        DataSet dsNotReporting = new DataSet();
                                        DataSet dsRegions = new DataSet();
                                        DataSet dsLoginId = new DataSet();
                                        DataSet dsState = new DataSet();

                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select count(*) as notReporting from sensordetails where isreporting = '0'", cnMain);

                                            da.Fill(dsNotReporting);
                                        }

                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select count(*) as Reporting from sensordetails where isreporting = '1'", cnMain);

                                            da.Fill(dsReporting);
                                        }
                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select userid from UserDetails where RoleId is null and not UserName is null", cnMain);
                                            da.Fill(dsLoginId);
                                        }

                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            //getting Data from Each SubDomain Good,Warning and Critical
                                            try
                                            {
                                                SqlDataAdapter da = new SqlDataAdapter("GET_LiveData_New", cnMain);
                                                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                                                da.SelectCommand.Parameters.AddWithValue("@LoginId", new Guid(dsLoginId.Tables[0].Rows[0]["userid"].ToString()));
                                                da.Fill(dsState);
                                                DataTable stateTable = new DataTable();
                                                stateTable = dsState.Tables[3];
                                                if (stateTable.Rows.Count > 0)
                                                {
                                                    List<string> deviceLst = new List<string>();
                                                    for (int sT = 0; sT < stateTable.Rows.Count; sT++)
                                                    {
                                                        deviceLst.Add(stateTable.Rows[sT]["Deviceid"].ToString());
                                                    }
                                                    List<string> deviceLst1 = new List<string>();
                                                    deviceLst1 = deviceLst.Distinct().ToList();
                                                    goodCount = 0; warningCount = 0; criticalCount = 0;
                                                    for (int dLst = 0; dLst < deviceLst1.Count; dLst++)
                                                    {
                                                        List<string> stateLst = new List<string>();
                                                        for (int sT = 0; sT < stateTable.Rows.Count; sT++)
                                                        {
                                                            if (stateTable.Rows[sT]["Deviceid"].ToString() == deviceLst1[dLst])
                                                            {
                                                                stateLst.Add(stateTable.Rows[sT]["State"].ToString());
                                                            }
                                                        }
                                                        if (stateLst.Contains("Critical"))
                                                        {
                                                            criticalCount = criticalCount + 1;
                                                        }
                                                        else if (stateLst.Contains("Warning"))
                                                        {
                                                            warningCount = warningCount + 1;
                                                        }
                                                        else
                                                        {
                                                            goodCount = goodCount + 1;
                                                        }
                                                    }
                                                    goodVal = goodCount.ToString(); criticalVal = criticalCount.ToString(); warningVal = warningCount.ToString();
                                                }
                                                else
                                                {
                                                    goodVal = "0"; criticalVal = "0"; warningVal = "0";
                                                }
                                            }
                                            catch (Exception exe)
                                            {
                                                c.ErrorLogs(exe.Message, "getting Data from Each SubDomain Good,Warning and Critical" + conSqlSub);
                                                exe = null;
                                            }
                                        }
                                        for (int pd = 0; pd < lstclientData.Count; pd++)
                                        {
                                            if (subDomain == lstclientData[pd].subDomain.ToString())
                                            {
                                                lstclientData[pd].reporting = Convert.ToString(dsReporting.Tables[0].Rows[0]["Reporting"]);
                                                lstclientData[pd].notReporting = Convert.ToString(dsNotReporting.Tables[0].Rows[0]["notReporting"]);
                                                lstclientData[pd].totalDevice = Convert.ToString(Convert.ToInt32(dsReporting.Tables[0].Rows[0]["Reporting"].ToString()) + Convert.ToInt32(dsNotReporting.Tables[0].Rows[0]["notReporting"].ToString()));
                                                lstclientData[pd].Good = goodVal;
                                                lstclientData[pd].Warning = warningVal;
                                                lstclientData[pd].Critical = criticalVal;
                                                string reportPercent;
                                                double report = Convert.ToDouble(dsReporting.Tables[0].Rows[0]["Reporting"]);
                                                double tot = Convert.ToDouble(dsReporting.Tables[0].Rows[0]["Reporting"]) + Convert.ToDouble(dsNotReporting.Tables[0].Rows[0]["notReporting"]);
                                                if (Convert.ToInt32(dsReporting.Tables[0].Rows[0]["Reporting"]) == 0 || Convert.ToInt32(dsReporting.Tables[0].Rows[0]["Reporting"]) + Convert.ToInt32(dsNotReporting.Tables[0].Rows[0]["notReporting"]) == 0)
                                                {
                                                    lstclientData[pd].ReportingPercent = "0";
                                                    reportPercent = "0";
                                                }
                                                else
                                                {
                                                    lstclientData[pd].ReportingPercent = Math.Round((report / tot) * 100).ToString();
                                                    reportPercent = Math.Round((report / tot) * 100).ToString();
                                                }
                                                lstclientData[pd].Devices = $"{report} / {tot}";
                                                lstclientData[pd].ReportingTot = $"{report} / {tot} ({reportPercent}%)";
                                            }
                                        }
                                    }
                                    catch (Exception exI)
                                    {
                                        c.ErrorLogs(exI.Message, "Getting data from Each SubDomain dsNotReporting,dsReporting,dsLoginId" + conSqlSub);
                                        exI = null;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (conSqlMain != "")
                            {
                                c.ErrorLogs(ex.Message, "DomainName,IoTDomain from clientdetails" + conSqlMain);
                                ex = null;
                            }
                            else
                            {
                                c.ErrorLogs(ex.Message, "At DomainName,IoTDomain from clientdetails due to Empty connection string" + conSqlMain);
                                ex = null;
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                c.ErrorLogs(ex.Message, "getting summary details from centralizedDB");
                ex = null;
            }
            lstclientData.RemoveAll(x => x.totalDevice == null || x.totalDevice == string.Empty || x.subDomain == "airflowcontrol" && x.domain == "dgtrak.online");

            return lstclientData;

        }
        [HttpPost]
        [Route("SendMail")]
        public string Mail(MailInput v)
        {
            try
            {
                string con = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "ajaybharath");
                SqlConnection cn = new SqlConnection(con);
                SqlDataAdapter da1 = new SqlDataAdapter("select * from Mails", cn);
                DataSet ds = new DataSet();
                da1.Fill(ds);
                string[] mails = ds.Tables[0].AsEnumerable().Select(x => x[1].ToString()).ToArray();
                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new MailAddress(ConfigurationManager.AppSettings["emailId"].ToString(), "IB IoT");
                foreach (string mail in mails)
                {
                    mailMsg.To.Add(new MailAddress(mail));
                }
                mailMsg.Subject = "Testing Mail";
                mailMsg.Body = v.Message;
                string filename = v.Filename;
                Thread.Sleep(10000);
                string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string filePath = $"{userPath}/Downloads/" + filename;
                string fileName = filePath.Split('/')[filePath.Split('/').Length - 1];
                byte[] bytes = File.ReadAllBytes(filePath);
                mailMsg.Attachments.Add(new Attachment(new MemoryStream(bytes), fileName));
                mailMsg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailId"].ToString(), ConfigurationManager.AppSettings["emailPwd"].ToString());
                smtp.EnableSsl = true;
                smtp.Send(mailMsg);
                return "mail sent";
            }
            catch (Exception ex)
            {
                return "mail not sent!!!" + ex.Message;
            }
        }
        [HttpPost]
        [Route("MailConfig")]
        public string SaveMail(MailConfig m)
        {
            Client c = new Client();
            try
            {
                string sql = "update mails set MailId=@mailid,Timestamp=@timestamp";
                //string sql = "insert into mails(MailId,Timestamp) values(@mailid,@timestamp)";
                string con = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "CentralizedDB");
                SqlConnection cn = new SqlConnection(con);
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@mailid", SqlDbType.VarChar).Value = m.Mails;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Time).Value = m.Time;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                return "MailConfiguration Successfull!!!";
            }
            catch (Exception ex)
            {
                c.ErrorLogs(ex.Message, "exception near Mail config");
                ex = null;
                return "MailConfiguration UnSuccessfull!!!";
            }

        }
        [Route("memory")]
        [HttpGet]
        [Obsolete]
        public dynamic Memorymethod()
        {
            try
            {
                DataSet dsClientData = new DataSet();
                using (SqlConnection cnMain = new SqlConnection("uid=sa;pwd=Ide@123;database=AB;server=DESKTOP-FMJB5MP"))
                {
                    SqlDataAdapter da = new SqlDataAdapter("select * from AWSDetails", cnMain);
                    da.Fill(dsClientData);
                }
                List<CPU_Load> cPU_Loads = new List<CPU_Load>();
                for (int sd = 0; sd < dsClientData.Tables[0].Rows.Count; sd++)
                {
                    CPU_Load d = new CPU_Load();
                    try
                    {
                        d.DomainName = dsClientData.Tables[0].Rows[sd]["domain"].ToString();
                        string AWSAccessKey, AWSSecretKey;
                        if (d.DomainName == "subzeroiot.com")
                        {
                            AWSAccessKey = "AKIAXJT3T2E745CX6S2Q";//AKIATOOKBXDCYTSXOREY    aYjk4VPBFeEMikmZP1WM2sxjNaliX6fMaeJqRGVC
                            AWSSecretKey = "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";
                        }
                        else
                        {

                            AWSAccessKey = "AKIATOOKBXDCYTSXOREY";//AKIATOOKBXDCYTSXOREY    aYjk4VPBFeEMikmZP1WM2sxjNaliX6fMaeJqRGVC
                            AWSSecretKey = "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";
                        }
                        // Access Key ID: AKIAXJT3T2E745CX6S2Q.Access Key: VEjIH1Jo + rHz5VPig1aq2wNO1rqdGVeKD6vxG//k
                        var newRegion = RegionEndpoint.GetBySystemName(dsClientData.Tables[0].Rows[sd]["region"].ToString());
                        IAmazonCloudWatch cw = Amazon.AWSClientFactory.CreateAmazonCloudWatchClient(AWSAccessKey, AWSSecretKey, newRegion);
                        try
                        {
                        First:
                            Dimension dim = new Dimension() { Name = "InstanceId", Value = dsClientData.Tables[0].Rows[sd]["instanceid"].ToString() };
                            System.Collections.Generic.List<Dimension> dimensions = new List<Dimension>() { dim };
                            string startTime1 = DateTimeOffset.Parse(DateTime.Now.AddMinutes(-2).ToString()).ToUniversalTime().ToString("s");
                            GetMetricStatisticsRequest reg = new GetMetricStatisticsRequest()
                            {
                                MetricName = "CPUUtilization",
                                Period = 60,
                                Statistics = new System.Collections.Generic.List<string>() { "Average" },
                                Dimensions = dimensions,
                                Namespace = "AWS/EC2",
                                EndTime = DateTime.Now,
                                StartTime = Convert.ToDateTime(startTime1)
                            };
                            var points = cw.GetMetricStatistics(reg).GetMetricStatisticsResult.Datapoints.OrderBy(p => p.Timestamp);
                            if (points.Count() == 0)
                            {
                                goto First;
                            }
                            foreach (var p in points)
                            {
                                d.cpu_used = Math.Round(p.Average, 2).ToString();
                                d.timestamp = (p.Timestamp).ToString();
                            }
                        }
                        catch (Amazon.CloudWatch.AmazonCloudWatchException ex)
                        {
                            if (ex.ErrorCode.Equals("OptInRequired"))
                                throw new Exception("You are not signed in for Amazon EC2.");
                            else
                                throw;
                        }

                        //PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                        //dynamic firstValue = cpuCounter.NextValue();
                        //System.Threading.Thread.Sleep(500);
                        //firstValue = cpuCounter.NextValue();

                        //d.cpu_used = Math.Round(firstValue, 0).ToString();
                        //d.timestamp = DateTime.Now.ToString();

                        //PerformanceCounter ramCounter;
                        //ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                        //dynamic memory = ramCounter.NextValue();
                        //d.memoryused = (8 - Math.Round((memory / 1000), 1)).ToString() + "GB";

                        //var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

                        //var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                        //{
                        //    FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
                        //    TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
                        //}).FirstOrDefault();

                        //if (memoryValues != null)
                        //{
                        //    var percent = ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
                        //    d.memoryused = Math.Round(percent, 2).ToString();

                        //}
                        //RamDetails rd = new RamDetails();
                        string jsonString = "";
                        //if ()
                        //{

                        //}
                        // string URL = $"https://adminiot.{d.DomainName}/RamUsage_API/RamUsage/Server_Ram";
                        string URL = "https://adminiot.iotsolution.net/RamUsage_API/RamUsage/Server_Ram";
                        //string URL = "https://localhost:44389/Ram_Usage/memory";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                        request.Method = "GET";
                        request.ContentType = "application/json";
                        WebResponse response = request.GetResponse();
                        StreamReader sr = new StreamReader(response.GetResponseStream());
                        jsonString = sr.ReadToEnd();
                        sr.Close();
                        dynamic stuff = JsonConvert.DeserializeObject(jsonString);
                        //dynamic stuff = JsonConvert.DeserializeObject<RamDetails>(jsonString);
                        var ram = stuff.RamUsage.Value;
                        d.memoryused = ram;
                        cPU_Loads.Add(d);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                return cPU_Loads;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        [Route("SMSTOKEN")]
        public dynamic smstokens()
        {
            int count = 0; string sms = string.Empty;
        TryAgain:
            try
            {
                HttpClient clientCall = new HttpClient();
                HttpResponseMessage responseMessage = clientCall.GetAsync("https://control.msg91.com/api/balance.php?authkey=288771Alcs1Nmue5d4be4d2&type=0").Result;
                string SmsTokens = responseMessage.Content.ReadAsStringAsync().Result;
                sms = Convert.ToDecimal(SmsTokens).ToString();
            }
            catch (Exception ex)
            {
                if (count == 3)
                {
                    sms = "N.A";
                }
                else
                {
                    count++;
                    Thread.Sleep(800);
                    goto TryAgain;
                }
                ex = null;
            }
            return sms;
        //    int count = 0;
        //TryAgain:
        //    HttpClient clientCall = new HttpClient();
        //    HttpResponseMessage responseMessage = clientCall.GetAsync("https://control.msg91.com/api/balance.php?authkey=288771Alcs1Nmue5d4be4d2&type=0").Result;
        //    string SmsTokens = responseMessage.Content.ReadAsStringAsync().Result;
        //    if (SmsTokens.All(char.IsDigit)) //SmsTokens.Contains("418")
        //    {
        //        return SmsTokens;
        //    }
        //    else
        //    {
        //        if (count == 3)
        //        {
        //            return "N.A";
        //        }
        //        count++;
        //        Thread.Sleep(800);
        //        goto TryAgain;
        //    }

        }

        [HttpGet]
        [Route("SSLExpDate")]
        public dynamic getSSLExpDate()
        {
            string[] SSLdomainNames = Convert.ToString(ConfigurationManager.AppSettings["domainsforsll"]).Split(',');
            //sslInfo _sslinfo = new sslInfo();
            List<sslDetails> ssdLst = new List<sslDetails>();
            Client c = new Client();

            for (int i = 0; i < SSLdomainNames.Length; i++)
            {
                try
                {
                    if (SSLdomainNames[i] != "")
                    {

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + SSLdomainNames[i] + "/");
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        response.Close(); //retrieve the ssl cert and assign it to an X509Certificate object
                        X509Certificate cert = request.ServicePoint.Certificate; //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
                        X509Certificate2 cert2 = new X509Certificate2(cert); //string cn = cert2.GetIssuerName();
                        string cedate = Convert.ToDateTime(cert2.GetExpirationDateString()).ToString("dd-MM-yyyy HH:mm");
                        string cpub = cert2.GetPublicKeyString(); TimeSpan ts = new TimeSpan(); ts = Convert.ToDateTime(cert2.GetExpirationDateString()) - DateTime.Now; sslDetails sslD = new sslDetails(); sslD.domain = SSLdomainNames[i].Split('.')[1] + "." + SSLdomainNames[i].Split('.')[2];
                        sslD.expTime = cedate;
                        //ts = Convert.ToDateTime(cert2.GetExpirationDateString()) -Convert.ToDateTime("3/21/2022 09:30:00");
                        if (ts.Days > 10)
                        {
                            sslD.severity = "low";
                        }
                        else if (ts.Days > 5 && ts.Days <= 10)
                        {
                            sslD.severity = "Medium";
                        }
                        else
                        {
                            sslD.severity = "High";
                        }
                        ssdLst.Add(sslD);
                    }
                }
                catch (Exception ex)
                {
                    c.ErrorLogs(ex.Message, "Exception near sslcertifications");
                    ex = null;
                }
            }
            

            //_sslinfo.sslInformation = new List<sslDetails>();
            //_sslinfo.sslInformation = ssdLst; return _sslinfo;
            return ssdLst;
        }
    }
}