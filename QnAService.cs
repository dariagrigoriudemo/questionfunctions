using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuestionFunctions
{
    public class QnAService : IQnAService
    {
        public QnAService()
        {
            
        }
        
        public async Task<string> GetResponse(string question)
        {
            var kbId = GetEnvironmentVariable("kbid");
            var endpointhostName = GetEnvironmentVariable("QNAMAKER_ENDPOINT_HOSTNAME");
            var endpointKey = GetEnvironmentVariable("QNAMAKER_ENDPOINT_SECRET");
            var uri = $"https://{endpointhostName}.azurewebsites.net/qnamaker/knowledgebases/{kbId}/generateAnswer";

            // JSON format for passing question to service
            string questionRequest = @"{'question': '"+question+"','top': 1}";

            // Create http client
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // POST method
                request.Method = HttpMethod.Post;

                // Add host + service to get full URI
                request.RequestUri = new Uri(uri);

                // Set question
                request.Content = new StringContent(questionRequest, Encoding.UTF8, "application/json");

                // Set authorization
                request.Headers.Add("Authorization", "EndpointKey " + endpointKey);

                // Send request to Azure service, get response
                var response = client.SendAsync(request).Result;
                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                var allAnswers = JsonConvert.DeserializeObject<Answers>(jsonResponse);
                foreach (var answer in allAnswers.answers)
                {
                    return answer.answer;
                }
            }
            
            return "Not everything in life is black and white, I have not decided on an answer.";
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        public class Answers { 
            public Answer[] answers { get; set; }
        }
        public class Answer { 
            public string[] questions { get; set; }
            public string answer { get; set; }
        }
    }
}
