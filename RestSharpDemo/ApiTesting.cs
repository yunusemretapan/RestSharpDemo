using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;
using RestSharpDemo.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestSharpDemo
{
    [TestFixture]
    public class ApiTesting
    {
        readonly RestClient restClient;
        public ApiTesting()
        {
            restClient = new RestClient("http://localhost:3000/");
        }

        [Test]
        public void GetTestMethod()
        {
            var request = new RestRequest("posts/{postid}", Method.GET);
            request.AddUrlSegment("postid", 1);

            var response = restClient.Execute(request);

            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<object, object>>(response);
            //var result = output["author"];

            var responseBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            var result = responseBody["author"];

            Assert.That(result, Is.EqualTo("typicode"), "Author is not correct");
        }

        [Test]
        [Obsolete]
        public void PostWithAnonymousBody()
        {
            RestRequest request = new RestRequest("posts/{postid}/profile", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(new { name = "Yunus" });

            request.AddUrlSegment("postid", 1);

            var response = restClient.Execute(request);

            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            var result = output["name"];

            Assert.That(result, Is.EqualTo("Yunus"), "Author is not correct");
        }

        [Test]
        [Obsolete]
        public void PostWithTypeClassBody()
        {
            RestRequest request = new RestRequest("posts", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(new Posts { id = "16", author = "Execute Automation", title = "RestSharp Demo" });

            var response = restClient.Execute<Posts>(request);

            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            Assert.That(response.Data.author, Is.EqualTo("Execute Automation"), "Author is not correct");
        }

        [Test]
        [Obsolete]
        public void PostWithAsync()
        {
            RestRequest request = new RestRequest("posts", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddBody(new Posts { id = "18", author = "Execute Automation", title = "RestSharp Demo" });

            //var response = client.Execute<Posts>(request);

            var response = ExecuteAsyncRequest<Posts>(restClient, request).GetAwaiter().GetResult();
            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            Assert.That(response.Data.author, Is.EqualTo("Execute Automation"), "Author is not correct");
        }

        [Obsolete]
        private async Task<IRestResponse<T>> ExecuteAsyncRequest<T>(RestClient client, IRestRequest request) where T : class, new()
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, restResponse =>
            {
                if (restResponse.ErrorException != null)
                {
                    const string message = "Error retrieving response.";
                    throw new ApplicationException(message, restResponse.ErrorException);
                }
                taskCompletionSource.SetResult(restResponse);
            });

            return await taskCompletionSource.Task;
        }
    }
}
