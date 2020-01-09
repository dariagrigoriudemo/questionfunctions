using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using QuestionFunctions;

namespace QuestionFunction
{
    public class Question
    {
        private const string FunctionQuestionSignal = "Meaning";

        private readonly IQnAService _service;

        public Question(IQnAService service)
        {
            _service = service;
        }

        [FunctionName("Question")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "questionsdb",
                collectionName: "questions",
                ConnectionStringSetting = "questionsDbConnectionString",
                SqlQuery = "SELECT * FROM questions"
            )] IEnumerable<QuestionDetail> documents,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string question = req.Query["question"];

            log.LogInformation(documents.Count().ToString());

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            question = question ?? data?.question;

            return question != null
                ? (ActionResult)new OkObjectResult(getAnswer(question, documents))
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private async Task<string> getAnswer(string question, IEnumerable<QuestionDetail> documents) {

            if (question.Contains(FunctionQuestionSignal, StringComparison.InvariantCultureIgnoreCase))
            {
                Random rnd = new Random();
                var allDocuments = documents.ToArray();
                return allDocuments[rnd.Next(allDocuments.Length)].answer_text;
            }
            else {
                return await _service.GetResponse(question);
                //throw new NotImplementedException("Generic question answering not yet supported");
            }
        }
    }
}
