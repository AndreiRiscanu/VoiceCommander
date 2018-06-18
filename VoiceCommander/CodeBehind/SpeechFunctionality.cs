using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoiceCommander.CommandLine;
using VoiceCommander.ViewModels;

namespace VoiceCommander.CodeBehind
{
    class SpeechFunctionality
    {
        public static SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

        public static void InitializeSpeechFunctionality()
        {
            Choices Command = new Choices();
            Command.Add(new string[] { "count", "back", "clear", "enter", "fontsize", "info" });
            Command.Add(PopulateWithNumbers(100));

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder CommandsGrammarBuilder = new GrammarBuilder(Command, 0, 4);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar CommandsGrammar = new Grammar(CommandsGrammarBuilder);
            recognizer.LoadGrammar(CommandsGrammar);

            // Register a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized += sre_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
        
        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.8)
                return;
            
            string[] Words = e.Result.Text.Split(' ');
            
            switch(Words[0])
            {
                case "count":
                    Command.Execute("count", null);
                    break;
                case "back":
                    Command.Execute("cd", new string[] { ".." });
                    break;
                case "clear":
                    Command.Execute("clear", null);
                    break;
                case "enter":
                    if (Words.Length > 1)
                    {
                        try
                        {
                            var Element = Int32.Parse(Words[1]);

                            if (Element < DirectoryStructure.numberOfItems)
                                Command.Execute("cd", new string[] { DirectoryStructure.getElementAtPosition(Element) });
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case "fontsize":
                    if (Words.Length > 1)
                    {
                        Command.Execute("fontsize", new string[] { Words[1] });
                    }
                    break;
                case "info":
                    if (Words.Length > 1)
                    {
                        try
                        {
                            var Element = Int32.Parse(Words[1]);

                            if (Element < DirectoryStructure.numberOfItems)
                                Command.Execute("info", new string[] { DirectoryStructure.getElementAtPosition(Element) });
                        }
                        catch (Exception)
                        { }
                    }
                    break;
            }
        }
        
        private static string[] PopulateWithNumbers(int size)
        {
            string[] Numbers = new string[size + 1];

            for (int i = 0; i <= size; ++i)
            {
                Numbers[i] = i.ToString();
            }

            return Numbers;
        }
    }
}
