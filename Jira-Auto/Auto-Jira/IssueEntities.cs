using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Jira
{
    class IssueEntities
    {
    }

    public class TestExecution: ICloneable
    {
        public FieldsTestExecution fields { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class TestExecutionCreate
    {
        public FieldsTestExecutionCreate fields { get; set; }
    }

    public class SubTestExecution: ICloneable
    {
        public FieldsSubTestExecution fields { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class SubTestExecutionCreate
    {
        public FieldsSubTestExecutionCreate fields { get; set; }
    }
    public class FieldsTestExecution
    {
        public FieldByKey project { get; set; }
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

    public class FieldsTestExecutionCreate
    {
        public FieldByKey project { get; set; }
        public string summary { get; set; }
        public FieldByName issuetype { get; set; }
        public FieldByName priority { get; set; }
        public string[] labels { get; set; }
        public List<FieldByName> versions { get; set; }
        public string environment { get; set; }
        public string description { get; set; }
        public List<FieldByName> components { get; set; }
        public string testPlanKey { get; set; }
    }

    public class FieldsSubTestExecution
    {
        public FieldByKey project { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public FieldByName issuetype { get; set; }
        public string testEnvironment { get; set; }
        public FieldByKey parent { get; set; }
        public string[] labels { get; set; }
        public List<FieldByName> components { get; set; }
        public FieldByName priority { get; set; }
        public string testPlanKey { get; set; }
    }

    public class FieldsSubTestExecutionCreate
    {
        public FieldByKey project { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public FieldByName issuetype { get; set; }
        public FieldByKey parent { get; set; }
        public string[] labels { get; set; }
        public List<FieldByName> components { get; set; }
        public FieldByName priority { get; set; }
    }
    public class FieldByName
    {
        public string name { get; set; }
    }

    public class FieldById
    {
        public string id { get; set; }
    }

    public class FieldByKey
    {
        public string key { get; set; }
    }


    public class ResponseInfo
    {
        public string id { get; set; }
        public string key { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
    }

    public enum IssueType
    {
        [Description("Test Execution")]
        TestExecution,
        [Description("Sub Test Execution")]
        SubTestExecution
    }
}
