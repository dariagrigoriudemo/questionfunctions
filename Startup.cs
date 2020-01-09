using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuestionFunctions;

[assembly: FunctionsStartup(typeof(QuestionFunctions.Startup))]

namespace QuestionFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IQnAService, QnAService>();
        }
    }
}