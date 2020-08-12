using Nancy.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    public class FieldsPojo
    {
        public String summary { get; set; }
        public String description { get; set; }
        public Field issuetype { get; set; }
        public Field project { get; set; }
        public Field priority { get; set; }
        public Field parent { get; set; }
        public Field component { get; set; }
        public Field environment { get; set; }
        public Field versions { get; set; }
        public String[] labels { get; set; }        
    }

    public class Field
    {
        public String id { get; set; }
        public String key { get; set; }
        public String name { get; set; }
    }
   
    public enum IssueType
    {
        [Description("Test Execution")]
        TestExecution,
        [Description("Sub Test Execution")]
        SubTestExecution
    }
}
