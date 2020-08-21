using Nancy.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Jira
{
    public class IssueInfo
    {
        public bool isSuccessfully { get; set; }
        public String key { get; set; }
        public String id { get; set; }
        public String summary { get; set; }

        public String comment { get; set; }

        public String errorMessage { get; set; }
        public FieldsPojo fields { get; set; }
        public String toJson()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return json.ToString();
        }

        public String toString()
        {
            return String.Format("Key: {0} , Id: {1}", this.key, this.id);
        }
    }

    public class SubTestExecutionInfo
    {
        public bool isSuccessfully { get; set; }
        public String key { get; set; }
        public String id { get; set; }
        public String summary { get; set; }

        public FieldSubTestExecution fields { get; set; }

        public String toJson()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return json.ToString();
        }

        public String toString()
        {
            return String.Format("Key: {0} , Id: {1}", this.key, this.id);
        }

    }


    public class FieldSubTestExecution
    {
        public Field issuetype { get; set; }
        public Parrent parent { get; set; }
        public Project project { get; set; }
        public String summary { get; set; }
    }

    public class FieldsPojo
    {
        public String summary { get; set; }
        public String description { get; set; }
        public Field issuetype { get; set; }
        public Project project { get; set; }
        public Field priority { get; set; }
        public Parrent parent { get; set; }
        public ICollection<Field> components { get; set; }
        public String environment { get; set; }
        public ICollection<Field> versions { get; set; }
        public String[] labels { get; set; }
    }

    public class ExcelData{
        public string projectKey { get; set; }
        public string priority { get; set; }
        public string labels { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string components { get; set; }
        public string versions { get; set; }
        public string environment { get; set; }
        public string testPlanKey { get; set; }
        public string testEnvironment { get; set; }
    }
    public class Field
    {
        public String name { get; set; }
    }

    public class Project
    {
        public String key { get; set; }
    }

    public class Parrent
    {
        public String id { get; set; }
    }


    public enum IssueType
    {
        [Description("Test Execution")]
        TestExecution,
        [Description("Sub Test Execution")]
        SubTestExecution
    }
}
