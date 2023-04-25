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
using System.Xml.Linq;

namespace SummaryAPI2.Controllers
{
    [RoutePrefix("API/TotalClientData")]
    public class ALLClientDataController : ApiController
    {
        public string goodVal, warningVal, criticalVal;
        public int goodCount, warningCount, criticalCount;
        string[] skipSDDD;
        string sms;
        [HttpPost]
        [Route("ClientData")]
        public dynamic GetTotalData(Client c)
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
            //lstclientData.RemoveAll(x => x.subDomain == "airflowcontrol" && x.domain == "dgtrak.online");
            ////lstclientData.GroupBy(x => x.subDomain).Distinct();

            //SMS Tokens
            int count = 0;
        TryAgain:
            try
            {
                HttpClient clientCall = new HttpClient();
                HttpResponseMessage responseMessage = clientCall.GetAsync("https://control.msg91.com/api/balance.php?authkey=288771Alcs1Nmue5d4be4d2&type=0").Result;
                string SmsTokens = responseMessage.Content.ReadAsStringAsync().Result;
                sms = Convert.ToDecimal(SmsTokens).ToString();
                //if (SmsTokens.All(char.IsDigit)) //SmsTokens.Contains("418")
                //{
                //    sms = SmsTokens.ToString();
                //}
                //else
                //{
                //    if (count == 3)
                //    {
                //        sms = "N.A";
                //    }
                //    else
                //    {
                //        count++;
                //        Thread.Sleep(800);
                //        goto TryAgain;
                //    }
                //}
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
                c.ErrorLogs(ex.Message,"Exception due to SMS Tokens!!!");
                ex = null;
            }
            //SSLDetails
            string[] SSLdomainNames = Convert.ToString(ConfigurationManager.AppSettings["domainsforsll"]).Split(',');
            //sslInfo _sslinfo = new sslInfo();
            List<sslDetails> ssdLst = new List<sslDetails>();
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
                    c.ErrorLogs(ex.Message, "error due to ssl certification");
                    ex = null;
                }
            }


            TotalData totalData = new TotalData();
            totalData.CData = lstclientData;
            totalData.Regions = lstRegions;
            totalData.ssl = ssdLst;
            totalData.SMSToken = sms;
            totalData.ReportTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            return totalData;

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
                c.ErrorLogs(ex.Message, "Exception near MailConfig");
                ex = null;
                return "MailConfiguration UnSuccessfull";
            }

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
        [Route("InsertLicense")]
        public dynamic InsertLicense(LicenseDetails l)
        {
            SqlHelper sH = new SqlHelper();
            try
            {
                if (!string.IsNullOrEmpty(l.LicenseKey))
                {
                    sH.InitializeDataConnecion();
                    sH.AddParameterToSQLCommand("@PersonName", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@PersonName", l.ActivatedPersonName);
                    sH.AddParameterToSQLCommand("@Organization", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@Organization",l.Organization);
                    sH.AddParameterToSQLCommand("@MacAddress", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@MacAddress", l.MacAddress);
                    sH.AddParameterToSQLCommand("@serialNumber", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@serialNumber", l.LicenseKey);
                    DataSet dsLicense = sH.GetDatasetByCommand("Update_LicenseKeyTable");
                    sH.CloseConnection();
                    //var status = Convert.ToInt16(dsLicense.Tables[0].Rows[0]["Status"]);
                    return dsLicense;
                }
                else
                {
                    sH.InitializeDataConnecion();
                    sH.AddParameterToSQLCommand("@customerMailid", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@customerMailid", l.customerMailId);
                    sH.AddParameterToSQLCommand("@customerName", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@customerName", l.customerName);
                    //sH.AddParameterToSQLCommand("@customerMobile", SqlDbType.VarChar);
                    //sH.SetSQLCommandParameterValue("@customerMobile", l.customerMobileNumber);
                    sH.AddParameterToSQLCommand("@clientId", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@clientId", l.customerId);
                    DataSet dsLicense = sH.GetDatasetByCommand("generateLicenseKey");
                    sH.CloseConnection();
                    return "updated";
                }
            }
            catch (Exception ex)
            {
                return "exception" + ex.Message;
                ex=null;
            }
        }
        [HttpPost]
        [Route("GetLicenseTable")]
        public dynamic GetLicenseTable(LicenseDetails ld)
        {
            SqlHelper sH = new SqlHelper();
            Client c = new Client();
            List<LicenseDetails> licenseDetails = new List<LicenseDetails>();
            try
            {
                sH.InitializeDataConnecion();
                sH.AddParameterToSQLCommand("@MacAddress", SqlDbType.VarChar);
                if (string.IsNullOrEmpty(ld.MacAddress))
                {
                    sH.SetSQLCommandParameterValue("@MacAddress", DBNull.Value);
                }
                else
                {
                    sH.SetSQLCommandParameterValue("@MacAddress", ld.MacAddress);
                }
                DataSet ds = sH.GetDatasetByCommand("getLicenseKeyTable");
                sH.CloseConnection();
                DataTable dt = ds.Tables[0];
                if(dt.Rows.Count > 0)
                {
                    for (int ls = 0; ls < dt.Rows.Count; ls++)
                    {
                        LicenseDetails l = new LicenseDetails();
                        try
                        {
                            l.startDate = c.epochUTCtoReadableUTC(dt.Rows[ls]["startDate"].ToString()).ToString("dd-MM-yyyy HH:mm");
                            l.endDate = c.epochUTCtoReadableUTC(dt.Rows[ls]["endDate"].ToString()).ToString("dd-MM-yyyy HH:mm");
                        }
                        catch (Exception ex)
                        {
                            ex = null;
                        }
                        l.clientId = dt.Rows[ls]["clientId"].ToString();
                        l.customerName = dt.Rows[ls]["customerName"].ToString();
                        l.customerMailId = dt.Rows[ls]["customerMail"].ToString();
                        //l.customerMobileNumber = dt.Rows[ls]["customerMobile"].ToString();
                        l.LicenseKey = dt.Rows[ls]["LicenseKey"].ToString();
                        l.Organization = dt.Rows[ls]["Organization"].ToString();
                        l.ActivatedPersonName = dt.Rows[ls]["ActivationPersonName"].ToString();
                        l.MacAddress = dt.Rows[ls]["MacAddress"].ToString();
                        licenseDetails.Add(l);
                    }
                }
            }
            catch (Exception ex)
            {
                ex = null;
            }
            return licenseDetails;
        }
        [HttpPost]
        [Route("updateLicense")]
        public dynamic updateLicense(LicenseDetails l)
        {
            SqlHelper sH = new SqlHelper();
            try
            {
                sH.InitializeDataConnecion();
                sH.AddParameterToSQLCommand("@customerMailid", SqlDbType.VarChar);
                sH.SetSQLCommandParameterValue("@customerMailid", l.customerMailId);
                sH.AddParameterToSQLCommand("@customerName", SqlDbType.VarChar);
                sH.SetSQLCommandParameterValue("@customerName", l.customerName);
                sH.AddParameterToSQLCommand("@LicenseKey", SqlDbType.VarChar);
                sH.SetSQLCommandParameterValue("@LicenseKey", l.LicenseKey);
                sH.AddParameterToSQLCommand("@fromDate", SqlDbType.VarChar);
                sH.SetSQLCommandParameterValue("@fromDate", l.endDate);
                DataSet dsLicense = sH.GetDatasetByCommand("updateLicenseKeyTableDate");
                sH.CloseConnection();
                return "updated";
            }
            catch (Exception ex)
            {
                return "exception" + ex.Message;
                ex = null;
            }
        }
    }
}
