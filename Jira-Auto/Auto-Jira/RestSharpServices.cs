using Nancy.Json;
using Nancy.Json.Simple;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis.TokenSeparatorHandlers;
using RestSharp;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Jira
{
    public class RestSharpServices
    {
        private static RestSharpServices instance = new RestSharpServices();

        private static String JIRA_BASE_URL = ConfigurationManager.AppSettings["jira.baseUrl"];
        private static String XRAY_BASE_URL = ConfigurationManager.AppSettings["xray.baseUrl"];
        private static String CREATE_TESTEXECUTION_ENPOINT = JIRA_BASE_URL + "rest/api/2/issue/";
        private static String GEN_TOKEN_XRAY_ENPOINT = JIRA_BASE_URL + "plugins/servlet/ac/com.xpandit.plugins.xray/issue-picker-dialog";
        private static String GET_JIRA_ISSUE_INFO_ENPOINT = JIRA_BASE_URL + "rest/api/2/issue/";
        private static String JIRA_AUTHORIZATION = "Basic " + ConfigurationManager.AppSettings["jira.apiToken"];
        public static RestSharpServices getInstance()
        {
            return instance;
        }
        public IssueInfo doRequestCreateTestExecution(IssueInfo testExecution)
        {
            var client = new RestClient(CREATE_TESTEXECUTION_ENPOINT);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("Accept", ContentType.Json);
            request.AddHeader("Authorization", JIRA_AUTHORIZATION);
            request.Method = Method.POST;
            String body = buildBodyTestExecution(testExecution);
            request.AddJsonBody(body);
            IRestResponse response = client.Post(request);
            testExecution.isSuccessfully = response.IsSuccessful;
            if (response.IsSuccessful)
            {
                JObject jObject = JObject.Parse(response.Content);
                testExecution.key = jObject.GetValue("key").ToString();
                testExecution.id = jObject.GetValue("id").ToString();
                testExecution.summary = testExecution.fields.summary;
            }
            return testExecution;
        }

        private String buildBodyTestExecution(IssueInfo testExecution)
        {
            string startupPath = System.IO.Directory.GetCurrentDirectory() + "\\Template\\CreateTestExecution.json";
            var jsonString = File.ReadAllText(startupPath);
            JObject obj = JObject.Parse(jsonString);
            obj["fields"]["project"]["key"] = testExecution.fields.project.key;
            obj["fields"]["summary"] = testExecution.fields.summary;
            obj["fields"]["description"] = testExecution.fields.description;
            obj["fields"]["summary"] = testExecution.fields.summary;
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
            return result;
        }

        public String doRequestGeneraterXrayToken()
        {
            var client = new RestClient(GEN_TOKEN_XRAY_ENPOINT);
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Authorization", JIRA_AUTHORIZATION);
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddParameter("classifier", "json");
            IRestResponse response = client.Post(request);
            String token = JObject.Parse(response.Content).GetValue("contextJwt").ToString();
            return token;
        } 

        public String doRequestGetIssueId(String issueKey)
        {
            String endpoint = GET_JIRA_ISSUE_INFO_ENPOINT + issueKey;
            var client = new RestClient(endpoint);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("Authorization", JIRA_AUTHORIZATION);
            IRestResponse response = client.Get(request);
            String id = JObject.Parse(response.Content).GetValue("id").ToString();
            return id;
        }

        public IRestResponse doRequestAddTestExecutionToTestPlan(String testPlanKey, List<String> testExecutionId)
        {
            String testPlanId = doRequestGetIssueId(testPlanKey);
            String enpoint = XRAY_BASE_URL + "api/internal/testplan/" + testPlanId + "/addTestExecs";
            String xrayToken = doRequestGeneraterXrayToken();
            var client = new RestClient(enpoint);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("X-acpt", xrayToken);
            String arrTestExec = "";
            foreach(var p in testExecutionId)
            {
                arrTestExec += "\""+ p +"\",";
            }
            String arrTestExecBody = "[" + arrTestExec.Substring(0, arrTestExec.Length - 1) + "]";           
            request.AddParameter("application/json", arrTestExecBody, ParameterType.RequestBody);
            return client.Post(request);
        }
    }
}
