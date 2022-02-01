using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SummaryAPI2.Models
{
    public class Client
    {
        public string uid { get; set; }
        public string pwd { get; set; }
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
        public string notReporting { get; set; }
        public string portal { get; set; }
        public string Good { get; set; }
        public string Warning { get; set; }
        public string Critical { get; set; }
    }
    public class MailInput
    {
        //public string MailIds { get; set; }
        public string Message { get; set; }
        public string Filename { get; set; }
    }
    public class MailConfig
    {
        public string Mails { get; set; }
        public TimeSpan Time { get; set; }
    }
}