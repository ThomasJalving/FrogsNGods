using System;
using System.Collections.Generic;
using System.Speech.Recognition;

namespace Frogger.GameManagement
{
    /// <summary>
    /// Class for recognizing speech input
    /// </summary>
    public class SpeechHelper
    {
        protected SpeechRecognitionEngine engine;
        protected InputHelper input;

        protected Dictionary<string, Grammar> grammars = new Dictionary<string,Grammar>();
        protected List<string> unloading = new List<string>();
        
        /// <summary>
        /// Class that helps with speechinput
        /// </summary>
        public SpeechHelper(InputHelper input)
        {
            this.input = input;
            engine = new SpeechRecognitionEngine();
            engine.SetInputToDefaultAudioDevice();
            AddSentence("play"); //at least one grammar has to be loaded before the recognizer can start
            engine.SpeechRecognized += SpeechAccepted;
            engine.SpeechRecognitionRejected += SpeechRejected;
            engine.RecognizerUpdateReached += RecognizerSpeechUpdateReached;
            engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        //loads a grammar to the speech engine that makes it recognize 'sentence'
        public void AddSentence(string sentence)
        {
            if (unloading.Contains(sentence))
            {
                unloading.Remove(sentence);
                return;
            }
            if (grammars.ContainsKey(sentence))
                return;
            GrammarBuilder builder = new GrammarBuilder(sentence);
            builder.Culture = engine.RecognizerInfo.Culture;
            Grammar gr = new Grammar(builder);
            engine.LoadGrammarAsync(gr);
            grammars.Add(sentence, gr);
        }

        //unloads a grammar from the speech engine so that it no longer recognizes 'sentence'
        public void RemoveSentence(string sentence)
        {
            if (grammars.ContainsKey(sentence))
            {
                unloading.Add(sentence);
                engine.RequestRecognizerUpdate();
            }
        }

        void RecognizerSpeechUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            foreach(string sentence in unloading)
            {
                input.speechPrompts.RemoveAll(p => p == sentence);
                engine.UnloadGrammar(grammars[sentence]);
                grammars.Remove(sentence);
            }
            unloading.Clear();
        }

        //Method is called when the recognizer accepts the input speech
        void SpeechAccepted(object sender, SpeechRecognizedEventArgs speechEvent)
        {
            input.AddPrompt(speechEvent.Result.Text);
        }

        //Method is called when speech is not recognized
        void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs speechError)
        {
            Console.WriteLine("Speech input rejected!");
        }
    }
}
