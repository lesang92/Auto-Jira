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
using System.Web;

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

        public string buildRequestBodyTestExecution(TestExecution testExecution)
        {
            FieldsTestExecution fields = new FieldsTestExecution
            {
                project = testExecution.fields.project,
                priority = testExecution.fields.priority,
                summary = testExecution.fields.summary,
                description = testExecution.fields.description,
                labels = testExecution.fields.labels,
                issuetype = new FieldByName { name = "Test Execution" },
                environment = testExecution.fields.environment,
                versions = testExecution.fields.versions
            };
            TestExecution testExecClone = new TestExecution { fields = fields };
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            
            return JsonConvert.SerializeObject(testExecClone, settings);
        }

        public string buildRequestBodySubTestExecution(SubTestExecution subTestExecution)
        {
            FieldsSubTestExecution fields = new FieldsSubTestExecution
            {
                components = subTestExecution.fields.components,
                priority = subTestExecution.fields.priority,
                parent = subTestExecution.fields.parent,
                summary = subTestExecution.fields.summary,
                description = subTestExecution.fields.description,
                labels = subTestExecution.fields.labels,
                project = subTestExecution.fields.project,
                issuetype = new FieldByName { name = "Sub Test Execution" }
            };
            SubTestExecution subTest = new SubTestExecution { fields = fields };
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return JsonConvert.SerializeObject(subTest, settings);
        }
        public ResponseInfo doRequestCreateSubTestExecution(SubTestExecution subTestExecution)
        {
            ResponseInfo responseInfo = new ResponseInfo();
            var body = buildRequestBodySubTestExecution(subTestExecution);
            var request = new RestRequest();
            var client = new RestClient(CREATE_TEST_EXECUTION_ENPOINT);
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
            else{
                JObject jObject = JObject.Parse(response.Content);
                responseInfo.isSuccess = true;
                string id = jObject.GetValue("id").ToString();
                string key = jObject.GetValue("key").ToString();
                responseInfo.id = id;
                responseInfo.key = key;
                ResponseInfo responseInfoAddTestEnvironment = doRequestAddTestEnvironment(subTestExecution.fields.project.key, id, subTestExecution.fields.testEnvironment);
                if (!responseInfoAddTestEnvironment.isSuccess)
                {
                    responseInfo.message = responseInfoAddTestEnvironment.message;
                    return responseInfo;
                }

                if (!string.IsNullOrEmpty(subTestExecution.fields.testPlanKey))
                {
                    ResponseInfo responseAddTestPlan = doRequestAddTestExecutionToTestPlan(subTestExecution.fields.testPlanKey, id);
                    if (!responseAddTestPlan.isSuccess)
                    {
                        responseInfo.message = responseAddTestPlan.message;
                    }
                }
                if (responseInfo.isSuccess)
                {
                    responseInfo.message = String.Format("Create sub test execution with key: {0} success", key);
                }
            }
            if (!responseInfo.isSuccess)
            {
                responseInfo.message = subTestExecution.fields.summary + "_" + responseInfo.message;
            }
            return responseInfo;
        }
        public ResponseInfo doRequestCreateTestExecution(TestExecution testExecution)
        {
            ResponseInfo responseInfo = new ResponseInfo();           
            var body = buildRequestBodyTestExecution(testExecution);
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
                ResponseInfo responseInfoAddTestEnvironment = doRequestAddTestEnvironment(testExecution.fields.project.key, id, testExecution.fields.testEnvironment);
                if (!responseInfoAddTestEnvironment.isSuccess)
                {
                    responseInfo.message = responseInfoAddTestEnvironment.message;
                    return responseInfo;
                }
                if (!string.IsNullOrEmpty(testExecution.fields.testPlanKey))
                {
                    ResponseInfo responseAddTestPlan = doRequestAddTestExecutionToTestPlan(testExecution.fields.testPlanKey, id);
                    if (!responseAddTestPlan.isSuccess)
                    {
                        responseInfo.message = responseAddTestPlan.message;
                    }                   
                }
                if (responseInfo.isSuccess)
                {
                    responseInfo.message = String.Format("Create test execution with key: {0} success", key);
                }
            }
            if (!responseInfo.isSuccess)
            {
                responseInfo.message = testExecution.fields.summary + "_" + responseInfo.message;
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
            arrTestExec += "\"" + testExecutionId + "\",";           
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
