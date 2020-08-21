using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Jira
{
    class IssueEntities
    {
    }

    public class Issue
    {
        public FieldsIssue fields { get; set; }
    }
    public class FieldsIssue
    {
        public FieldById project { get; set; }
        public string summary { get; set; }
        public FieldByName issuetype { get; set; }
        public FieldByName priority { get; set; }
        public string[] labels { get; set; }
        public List<FieldByName> versions { get; set; }
        public string environment { get; set; }
        public string description { get; set; }
        public List<FieldByName> components { get; set; }
        public string testEnvironment { get; set; }
        public string testPlanKey { get; set; }
    }
    public class FieldByName
    {
        public string name { get; set; }
    }

    public class FieldById
    {
        public string id { get; set; }
    }

    public class ResponseInfo
    {
        public string id { get; set; }
        public string key { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
    }
}
