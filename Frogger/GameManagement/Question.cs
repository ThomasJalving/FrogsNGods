using System;
using System.Collections.Generic;
using System.Linq;
namespace Frogger.GameManagement
{
    /// <summary>
    /// Separates a question in the questionline, the answerlines and the negative and positive responselines
    /// </summary>
    class Question
    {
        public string questionLine, responseLineRight, responseLineWrong;
        public string[] answerLines, goodAnswer;
        private int correctAnswers;

        /// <summary>
        /// Class that handles a question from a god
        /// </summary>
        public Question(string[] lines)
        {
            goodAnswer = new string[4];
            correctAnswers = 0;
            questionLine = lines[0];
            responseLineRight = lines[5];
            responseLineWrong = lines[6];
            List<string> answerLines = new List<string>();
            for(int i = 1; i < 5; i++)
            {
                processLine(lines[i], answerLines);
            }
            this.answerLines = answerLines.ToArray();
        }

        /// <summary>
        /// Checks if an answerline is labeled as a correct answer
        /// </summary>
        void processLine(string line, List<string> answerLines)
        {
            if (line.StartsWith("+"))
            {
                line = line.Replace("+", "");
                goodAnswer[correctAnswers] = line;
                correctAnswers++;
            }
            answerLines.Add(line);
        }
    }
}
