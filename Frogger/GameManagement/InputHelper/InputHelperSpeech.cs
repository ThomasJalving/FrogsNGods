using System;
using System.Collections.Generic;
namespace Frogger.GameManagement
{
    public partial class InputHelper
    {
        public List<String> speechPrompts; //list of recognized speech inputs that still need to be processed
        public SpeechHelper Speech { get; private set; } //the class that handles the speech recognition

        //Checks if the user has said 'promptText' and removes the value once if it is true
        public bool CheckPrompt(string promptText)
        {
            if (speechPrompts.Contains(promptText))
            {
                foreach(String s in speechPrompts)
                {
                    Console.WriteLine(s);
                }
                speechPrompts.RemoveAll(prompt => prompt == promptText);
                return true;
            }
            return false;
        }

        //adds an entry to the speechPrompts list, used by SpeechHelper class
        public void AddPrompt(string promptText)
        {
            speechPrompts.Add(promptText);
        }
    }
}
