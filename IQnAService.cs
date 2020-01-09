using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuestionFunctions
{
    public interface IQnAService
    {
        /// <summary>
        /// get answer to a question supported by a QnA service
        /// </summary>
        /// <param name="question">question text</param>
        /// <returns>answer</returns>
        Task<string> GetResponse(string question);
    }
}
