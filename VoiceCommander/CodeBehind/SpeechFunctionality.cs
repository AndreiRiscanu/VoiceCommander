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
            Choices command = new Choices();
            command.Add(new string[] { "count", "back", "clear", "enter", "fontsize", "info" });
            command.Add(PopulateWithNumbers(100));

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder commandsGrammarBuilder = new GrammarBuilder(command, 0, 4);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar commandsGrammar = new Grammar(commandsGrammarBuilder);
            recognizer.LoadGrammar(commandsGrammar);

            // Register a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized += sre_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        // Create a simple handler for the SpeechRecognized event.
        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.8)
                return;
            
            string[] words = e.Result.Text.Split(' ');
            
            switch(words[0])
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
                    if (words.Length > 1)
                    {
                        try
                        {
                            var element = Int32.Parse(words[1]);

                            if (element < DirectoryStructure.numberOfItems)
                                Command.Execute("cd", new string[] { DirectoryStructure.getElementAtPosition(element) });
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case "fontsize":
                    if (words.Length > 1)
                    {
                        Command.Execute("fontsize", new string[] { words[1] });
                    }
                    break;
                case "info":
                    if (words.Length > 1)
                    {
                        try
                        {
                            var element = Int32.Parse(words[1]);

                            if (element < DirectoryStructure.numberOfItems)
                                Command.Execute("info", new string[] { DirectoryStructure.getElementAtPosition(element) });
                        }
                        catch (Exception)
                        { }
                    }
                    break;
            }
        }
        
        private static string[] PopulateWithNumbers(int size)
        {
            string[] numbers = new string[size];

            for (int i = 1; i <= size; ++i)
            {
                numbers[i - 1] = i.ToString();
            }

            return numbers;
        }
    }
}
