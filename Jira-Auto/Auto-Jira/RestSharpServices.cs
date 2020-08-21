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
        private static String TEST_ENVIRONMENT_ENDPOINT = XRAY_BASE_URL + "api/internal/testExec";
        private static String TGET_PROJECT_ID_ENDPOINT = JIRA_BASE_URL + "rest/api/2/project/";
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
            testExecution.fields.issuetype = new Field { name = "Test Execution" };
            request.AddJsonBody(testExecution.toJson());
            IRestResponse response = client.Post(request);
            testExecution.isSuccessfully = response.IsSuccessful;
            if (response.IsSuccessful)
            {
                JObject jObject = JObject.Parse(response.Content);
                testExecution.key = jObject.GetValue("key").ToString();
                testExecution.id = jObject.GetValue("id").ToString();
                testExecution.summary = testExecution.fields.summary;
            }
            else
            {
                testExecution.errorMessage = response.Content;
            }
            return testExecution;
        }

        public void doRequestAddTestEnvironment(String projectKey, String testExecutionId, String testenvironment)
        {
            String projectId = doRequestGetProjectId(projectKey);
            String enpoint = TEST_ENVIRONMENT_ENDPOINT +"/"+ testExecutionId + "/testEnvironments";
            String body = "{\"testEnvironments\":[\"" + testenvironment + "\"],\"projectId\":\"" + projectId + "\"}"; 
            var client = new RestClient(enpoint);
            var request = new RestRequest();
            String xrayToken = doRequestGeneraterXrayToken();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("X-acpt", xrayToken);
            request.AddJsonBody(body);
            IRestResponse response = client.Post(request);
        }
        public SubTestExecutionInfo doRequestCreateSubTestExecution(SubTestExecutionInfo subTestExecution)
        {
            var client = new RestClient(CREATE_TESTEXECUTION_ENPOINT);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("Accept", ContentType.Json);
            request.AddHeader("Authorization", JIRA_AUTHORIZATION);
            request.Method = Method.POST;
            subTestExecution.fields.issuetype = new Field { name = "Sub Test Execution" };
            request.AddJsonBody(subTestExecution.toJson());
            IRestResponse response = client.Post(request);
            subTestExecution.isSuccessfully = response.IsSuccessful;
            if (response.IsSuccessful)
            {
                JObject jObject = JObject.Parse(response.Content);
                subTestExecution.key = jObject.GetValue("key").ToString();
                subTestExecution.id = jObject.GetValue("id").ToString();
            }
            return subTestExecution;
        }

        private String buildBodyTestExecution(IssueInfo testExecution)
        {
            string startupPath = System.IO.Directory.GetCurrentDirectory() + "\\Template\\CreateTestExecution.json";
            var jsonString = File.ReadAllText(startupPath);
            FieldsPojo fields = testExecution.fields;
           
            return jsonString;
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


        public String doRequestGetProjectId(String projectKey)
        {
            String endpoint = TGET_PROJECT_ID_ENDPOINT + projectKey;
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
