using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace QuestionFunction
{
    public static class Question
    {
        private const string FunctionQuestionSignal = "Meaning";

        private static readonly string[] GenericFactAboutAzureFunctions = 
            new string[] {
                "Serverless technologies may not be the meaning of life, but they can make your life more meaningful and more productive!",
                "With great functions comes great productivity!",
                "Serverless is so 2020!",
                "To be or not to be serverless!"
        };

        [FunctionName("Answer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string question = req.Query["question"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            question = question ?? data?.question;

            return question != null
                ? (ActionResult)new OkObjectResult(getAnswer(question))
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static string getAnswer(string question) {

            if (question.Contains(FunctionQuestionSignal, StringComparison.InvariantCultureIgnoreCase))
            {
                Random rnd = new Random();
                return GenericFactAboutAzureFunctions[rnd.Next(GenericFactAboutAzureFunctions.Length)];
            }
            else {
                throw new NotImplementedException("Generic question answering not yet supported");
            }
        }
    }
}
