using Nancy.Json;
using Nancy.Json.Simple;
using Newtonsoft.Json;
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
    public class RestShapUtils
    {
        private static String JIRA_BASE_URL = ConfigurationManager.AppSettings["jira.baseUrl"];
        private static String XRAY_BASE_URL = ConfigurationManager.AppSettings["xray.baseUrl"];
        private static String CREATE_TEST_EXECUTION_ENPOINT = JIRA_BASE_URL + "rest/api/2/issue/";
        private static String GEN_TOKEN_XRAY_ENPOINT = JIRA_BASE_URL + "plugins/servlet/ac/com.xpandit.plugins.xray/issue-picker-dialog";
        private static String GET_JIRA_ISSUE_INFO_ENPOINT = JIRA_BASE_URL + "rest/api/2/issue/";
        private static String JIRA_AUTHORIZATION = "Basic " + ConfigurationManager.AppSettings["jira.apiToken"];
        private static String TEST_ENVIRONMENT_ENDPOINT = XRAY_BASE_URL + "api/internal/testExec";
        private static String TGET_PROJECT_ID_ENDPOINT = JIRA_BASE_URL + "rest/api/2/project/";
        private static RestShapUtils instance = new RestShapUtils();
        public static RestShapUtils getInstance()
        {
            return instance;
        }

        public string buildRequestBody(Issue issue, IssueType issueType, string projectId)
        {
            Issue newIssue = issue;
            newIssue.fields.testEnvironment = "";
            newIssue.fields.testPlanKey = "";
            newIssue.fields.project.id = projectId;
            newIssue.fields.issuetype.name = issueType.ToString();
            var deserializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(newIssue);
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var newSerializedObject = JsonConvert.SerializeObject(deserializedObject, serializerSettings);
            return newSerializedObject.ToString();
        }


        public ResponseInfo doRequestCreateIssue(Issue issue, IssueType issueType, string projectKey)
        {
            string projectId = doRequestGetProjectId(projectKey);
            ResponseInfo responseInfo = new ResponseInfo();
            if (projectId == null)
            {
                return new ResponseInfo
                {
                    isSuccess = false,
                    message = String.Format("Project key: {0} invalid", projectKey)
                };
            }
            var body = buildRequestBody(issue, issueType, projectId);
            var client = new RestClient(CREATE_TEST_EXECUTION_ENPOINT);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("Accept", ContentType.Json);
            request.AddHeader("Authorization", JIRA_AUTHORIZATION);
            request.Method = Method.POST;
            request.AddJsonBody(body);
            IRestResponse response = client.Post(request);
            if (!response.IsSuccessful)
            {
                responseInfo.isSuccess = false;
                responseInfo.message = response.Content;
            }
            else
            {
                JObject jObject = JObject.Parse(response.Content);
                responseInfo.isSuccess = true;
                string id = jObject.GetValue("id").ToString();
                string key = jObject.GetValue("key").ToString();
                responseInfo.id = id;
                responseInfo.key = key;
                ResponseInfo responseInfoAddTestEnvironment = doRequestAddTestEnvironment(projectKey, id, issue.fields.testEnvironment);
                if (!responseInfoAddTestEnvironment.isSuccess)
                {
                    responseInfo.message = responseInfoAddTestEnvironment.message;
                    return responseInfo;
                }
                ResponseInfo responseAddTestPlan = doRequestAddTestExecutionToTestPlan(issue.fields.testPlanKey, id);
                if (!responseAddTestPlan.isSuccess)
                {
                    responseInfo.message = responseAddTestPlan.message;
                }
                if (responseInfo.isSuccess)
                {
                    responseInfo.message = String.Format("Create test execution with key: {0} success", key);
                }
            }
            return responseInfo;
        }

        public ResponseInfo doRequestAddTestEnvironment(String projectKey, String testExecutionId, String testenvironment)
        {
            ResponseInfo responseInfo = new ResponseInfo();
            String projectId = doRequestGetProjectId(projectKey);
            String enpoint = TEST_ENVIRONMENT_ENDPOINT + "/" + testExecutionId + "/testEnvironments";
            String body = "{\"testEnvironments\":[\"" + testenvironment + "\"],\"projectId\":\"" + projectId + "\"}";
            var client = new RestClient(enpoint);
            var request = new RestRequest();
            String xrayToken = doRequestGeneraterXrayToken();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("X-acpt", xrayToken);
            request.AddJsonBody(body);
            IRestResponse response = client.Post(request);
            responseInfo.isSuccess = response.IsSuccessful;
            responseInfo.message = response.Content;
            return responseInfo;
        }

        public string doRequestGetProjectId(string projectKey)
        {
            String endpoint = TGET_PROJECT_ID_ENDPOINT + projectKey;
            var client = new RestClient(endpoint);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("Authorization", JIRA_AUTHORIZATION);
            IRestResponse response = client.Get(request);
            return response.IsSuccessful ? JObject.Parse(response.Content).GetValue("id").ToString() : null;
        }

        public ResponseInfo doRequestAddTestExecutionToTestPlan(String testPlanKey, String testExecutionId)
        {
            ResponseInfo responseInfo = new ResponseInfo();
            String testPlanId = doRequestGetIssueId(testPlanKey);
            String enpoint = XRAY_BASE_URL + "api/internal/testplan/" + testPlanId + "/addTestExecs";
            String xrayToken = doRequestGeneraterXrayToken();
            var client = new RestClient(enpoint);
            var request = new RestRequest();
            request.AddHeader("Content-Type", ContentType.Json);
            request.AddHeader("X-acpt", xrayToken);
            String arrTestExec = "";
            arrTestExec += "[\"" + testExecutionId + "]\",";           
            String arrTestExecBody = "[" + arrTestExec.Substring(0, arrTestExec.Length - 1) + "]";
            request.AddParameter("application/json", arrTestExecBody, ParameterType.RequestBody);         
            IRestResponse response = client.Post(request);
            responseInfo.isSuccess = response.IsSuccessful;
            responseInfo.message = response.Content;
            return responseInfo;
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

    }
}
