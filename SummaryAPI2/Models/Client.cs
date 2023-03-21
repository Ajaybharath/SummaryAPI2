using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SummaryAPI2.Models
{
    
    public class sslDetails
    {
        public string domain { get; set; }
        public string expTime { get; set; }
        public string severity { get; set; }
    }
    public class Client
    {
        public string uid { get; set; }
        public string pwd { get; set; }
        public void ErrorLogs(string excepData,string exceptionAt)
        {

            //Exception Logins
            string exceptionCon = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString1"]).Replace("IoTMainData", "CentralizedDB");
            SqlConnection exceptionsqlConnection = new SqlConnection(exceptionCon);
            exceptionsqlConnection.Open();
            using (SqlCommand command = new SqlCommand("proc_ExceptionDatainAPI", exceptionsqlConnection))
            {
                command.Parameters.Add("@Exception", SqlDbType.VarChar).Value = excepData;
                command.Parameters.Add("@ExceptionAt", SqlDbType.VarChar).Value = exceptionAt;
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
            exceptionsqlConnection.Close();
        }
        public DateTime epochUTCtoReadableUTC(string epochUTC)
        {
            if (epochUTC.Length > 10)
            {
                epochUTC = epochUTC.Substring(0, 10);
            }
            long epoch = Convert.ToInt64(epochUTC) + 19800;// + 19800; //indian time + 330 mins
                                                           //long epoch = Convert.ToInt64(epochUTC);

            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return UnixEpoch + TimeSpan.FromMilliseconds((epoch) * 1000);

        }
    }
    public class UserDetails
    {
        public string UserName { get; set; }
    }
    public class reportTime
    {
        public string reportAt { get; set; }
    }

    public class clientResp
    {
        public List<clientDetails> details { get; set; }
    }
    public class CPU_Load
    {
        public string DomainName { get; set; }
        public string cpu_used { get; set; }
        public string memoryused { get; set; }
        public string timestamp { get; set; }
    }
    public class clientDetails
    {
        public string domain { get; set; }
        public string subDomain { get; set; }
        public string totalDevice { get; set; }
        public string reporting { get; set; }
        public string notReporting { get; set; }
        public string portal { get; set; }
    }

    public class region
    {
        public string name { get; set; }
        public string location { get; set; }
        //public string longitude { get; set; }
    }
    public class clientData
    {
        public string C_CID { get; set; }
        public string domain { get; set; }
        public string subDomain { get; set; }
        public string totalDevice { get; set; }
        public string reporting { get; set; }
        public string ReportingTot { get; set; }
        public string notReporting { get; set; }
        public string portal { get; set; }
        public string Good { get; set; }
        public string Warning { get; set; }
        public string Critical { get; set; }
        public string Devices { get; set; }
        public string ReportingPercent { get; set; }
    }
    public class MailInput
    {
        //public string MailIds { get; set; }
        public string Message { get; set; }
        public string Filename { get; set; }
    }
    public class LicenseDetails
    {
        public string customerId { get; set; }
        public string customerName { get; set; }
        public string customerMailId { get; set; }
        //public string customerMobileNumber { get; set; }
        public string LicenseKey { get; set; }
        public string Organization { get; set; }
        public string ActivatedPersonName { get; set; }
        public string MacAddress { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string clientId { get; set; }
    }
    public class MailConfig
    {
        public string Mails { get; set; }
        public TimeSpan Time { get; set; }
        
    }
    public class Login
    {
        public string Password { get; set; }
        public string UserId { get; set; }
    }
    public class TotalData
    {
        public string ReportTime { get; set; }
        public List<clientData> CData { get; set; }
        public List<sslDetails> ssl { get; set; }
        public List<region> Regions { get; set; }
        public string SMSToken { get; set; }
    }

}