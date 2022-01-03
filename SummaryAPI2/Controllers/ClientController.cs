using SummaryAPI2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SummaryAPI2.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("API/Client")]
    public class ClientController : ApiController
    {
        public string goodVal,warningVal,criticalVal;
        public int goodCount, warningCount, criticalCount;
        [HttpPost]
        [Route("Details")]
        public dynamic get_clientDetails(Client c)
        {

            List<clientDetails> lstClients = new List<clientDetails>();
            string[] skipSD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');

            try
            {
                if (c.uid == "idea" && c.pwd == "bytes")
                {


                    string conSqlMain = string.Empty;
                    string conSqlSub = string.Empty;
                    string subDomain = string.Empty;
                    string clientDetails = string.Empty;

                    for (int i = 1; i < 10; i++)
                    {
                        try
                        {

                            conSqlMain = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);
                            // clientDetails = "uid=sa;pwd=Ide@123;database=AB;server=AJAYBHARATH\\SQLEXPRESS";
                            DataSet dsClientData = new DataSet();

                            using (SqlConnection cnMain = new SqlConnection(conSqlMain))
                            {
                                SqlDataAdapter da = new SqlDataAdapter("select DomainName,IoTDomain from clientdetails", cnMain);

                                da.Fill(dsClientData);
                            }


                            for (int sd = 0; sd < dsClientData.Tables[0].Rows.Count; sd++)
                            {
                                clientDetails cD = new clientDetails();

                                subDomain = Convert.ToString(dsClientData.Tables[0].Rows[sd]["DomainName"]);

                                conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);


                                cD.domain = Convert.ToString(dsClientData.Tables[0].Rows[sd]["IoTDomain"]);
                                cD.subDomain = subDomain;

                                if (Array.IndexOf(skipSD, subDomain) == -1)
                                {
                                    cD.reporting = "0";
                                    cD.notReporting = "0";

                                    try
                                    {
                                        DataSet dsReporting = new DataSet();
                                        DataSet dsNotReporting = new DataSet();
                                        DataSet dsRegions = new DataSet();

                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select count(*) as notReporting from sensordetails where isnull(isreporting, '0') = '0'", cnMain);

                                            da.Fill(dsNotReporting);
                                        }

                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select count(*) as Reporting from sensordetails where isreporting = '1'", cnMain);

                                            da.Fill(dsReporting);
                                        }


                                        cD.reporting = Convert.ToString(dsReporting.Tables[0].Rows[0]["Reporting"]);
                                        cD.notReporting = Convert.ToString(dsNotReporting.Tables[0].Rows[0]["notReporting"]);


                                        if (cD.subDomain == "vignaninstruments")
                                        {
                                            cD.portal = "https://web.vlogdata.net/";
                                        }
                                        else
                                        {
                                            cD.portal = "https://" + cD.subDomain + "." + cD.domain;
                                        }
                                    }
                                    catch (Exception exI)
                                    {
                                        exI = null;
                                    }

                                    cD.totalDevice = (Convert.ToInt32(cD.reporting) + Convert.ToInt32(cD.notReporting)).ToString();

                                    lstClients.Add(cD);
                                }
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
            catch (Exception ex)
            {
                ex = null;
            }


            return lstClients;

        }

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

                                conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);



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
        [Route("ClientData")]
        public dynamic getClientData(Client c)
        {
            List<clientData> lstclientData = new List<clientData>();
           
            try
            {
                string[] skipSD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');
                if (c.uid == "idea" && c.pwd == "bytes")
                {
                    SqlConnection cn = new SqlConnection("uid=sa;pwd=Ide@123;database=AB;server=AJAYBHARATH\\SQLEXPRESS");
                    SqlDataAdapter da1 = new SqlDataAdapter("select * from centralcontrol", cn);
                    DataSet ds = new DataSet();
                    da1.Fill(ds);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        clientData clientData = new clientData();
                        clientData.domain = ds.Tables[0].Rows[i]["Domain"].ToString();
                        clientData.subDomain = ds.Tables[0].Rows[i]["Subdomain"].ToString();
                        clientData.portal = ds.Tables[0].Rows[i]["PortalURL"].ToString();
                        lstclientData.Add(clientData);
                    }
                    //reporting
                    string conSqlMain = string.Empty;
                    string conSqlSub = string.Empty;
                    string subDomain = string.Empty;
                    string clientDetails = string.Empty;
                    //string goodVal, warningVal, criticalVal;
                    for (int i = 1; i < 10; i++)
                    {
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

                                conSqlSub = conSqlMain.Replace("IoTMainData", subDomain);

                                if (Array.IndexOf(skipSD, subDomain) == -1)
                                {
                                    try
                                    {
                                        DataSet dsReporting = new DataSet();
                                        DataSet dsNotReporting = new DataSet();
                                        DataSet dsRegions = new DataSet();
                                        DataSet dsLoginId = new DataSet();
                                        DataSet dsState = new DataSet();
                                       
                                        using (SqlConnection cnMain = new SqlConnection(conSqlSub))
                                        {
                                            SqlDataAdapter da = new SqlDataAdapter("select count(*) as notReporting from sensordetails where isnull(isreporting, '0') = '0'", cnMain);

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
                                            }
                                        }
                                    }
                                    catch (Exception exI)
                                    {
                                        exI = null;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex = null;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                ex = null;
            }
            return lstclientData;

        }

    }
}
