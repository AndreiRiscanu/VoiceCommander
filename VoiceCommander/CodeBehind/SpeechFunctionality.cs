using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VoiceCommander.CodeBehind.ViewModels;
using VoiceCommander.CommandLine;
using VoiceCommander.ViewModels;

namespace VoiceCommander.CodeBehind
{
    class SpeechFunctionality
    {
        static string command = null;

        private static SpeechRecognitionEngine recognizer = null;
        private static Choices Commands = new Choices(new string[] { "count", "back", "clear",
            "enter", "fontsize", "info", "view", "file", "folder", "delete", "maximize", "minimize",
            "normal", "help", "exit" });
        private static Choices Numbers = new Choices(PopulateWithNumbers(100));
        private static Choices Alphabet = new Choices(new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" });
        private static Grammar CommandsGrammar = new Grammar(new GrammarBuilder(Commands, 0, 1));
        private static Grammar NumbersGrammar = new Grammar(new GrammarBuilder(Numbers, 0, 1));
        private static Grammar AlphabetGrammar = new Grammar(new GrammarBuilder(Alphabet, 0, 10));

        public static void DeactivateSpeechRecognition()
        {
            recognizer.RecognizeAsyncStop();
            recognizer.UnloadAllGrammars();
            recognizer.SpeechRecognized -= sre_SpeechRecognized;
            recognizer = null;
        }

        public static void InitializeSpeechFunctionality()
        {
            if (recognizer != null)
            {
                return;
            }

            recognizer = new SpeechRecognitionEngine();
            Choices Command = new Choices();
            recognizer.LoadGrammar(CommandsGrammar);

            // Register a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized += sre_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
        
        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.85)
                return;

            command = e.Result.Text;

            OutputStringItemViewModel.GetOutputStringInstance().Output = command;

            switch (command)
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
                case "maximize":
                    Command.Execute("mx", null);
                    break;
                case "minimize":
                    Command.Execute("mz", null);
                    break;
                case "normal":
                    Command.Execute("nm", null);
                    break;
                case "help":
                    Command.Execute("help", null);
                    break;
                case "exit":
                    Command.Execute("exit", null);
                    break;
                case "enter": case "fontsize": case "info": case "view": case "delete":
                    Thread thread = new Thread(ChangeCom);
                    thread.Start(NumbersGrammar);
                    break;
                default:
                    Thread thread2 = new Thread(ChangeCom);
                    thread2.Start(AlphabetGrammar);
                    break;
            }
        }
        
        public static void ChangeCom(object grammar)
        {
            App.Current.Dispatcher.Invoke(new Action(() => HandleCommand(grammar)));
        }

        public static void HandleCommand(object grammar)
        {
            recognizer.RecognizeAsyncStop();
            recognizer.UnloadAllGrammars();
            recognizer.SpeechRecognized -= sre_SpeechRecognized;
            recognizer.LoadGrammar((Grammar) grammar);
            recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(2);
            recognizer.BabbleTimeout = TimeSpan.FromSeconds(2);

            RecognitionResult result = recognizer.Recognize();

            if (result == null || result.Text == null || result.Confidence < 0.85)
            {
                recognizer.UnloadAllGrammars();
                recognizer.LoadGrammar(CommandsGrammar);
                recognizer.SpeechRecognized += sre_SpeechRecognized;
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                return;
            }
            

            string Param = result.Text;

            switch (command)
            {
                case "enter":
                    var Elemen2t = Int32.Parse(Param);

                    if (Elemen2t < DirectoryStructure.numberOfItems)
                        Command.Execute("cd", new string[] { DirectoryStructure.getElementAtPosition(Elemen2t) });
                    break;
                case "fontsize":
                    Command.Execute("fontsize", new string[] { Param });
                    break;
                case "info":
                    try
                    {
                        var Element = Int32.Parse(Param);

                        if (Element < DirectoryStructure.numberOfItems)
                            Command.Execute("info", new string[] { DirectoryStructure.getElementAtPosition(Element) });
                    }
                    catch (Exception)
                    { }
                    break;
                case "view":
                    try
                    {
                        var Element = Int32.Parse(Param);

                        if (Element < DirectoryStructure.numberOfItems)
                            Command.Execute("read", new string[] { DirectoryStructure.getElementAtPosition(Element) });
                    }
                    catch (Exception)
                    { }
                    break;
                case "delete":
                    try
                    {
                        var Index = Int32.Parse(Param);

                        if (Index < DirectoryStructure.numberOfItems)
                        {
                            var Element = DirectoryStructure.getElementAtPosition(Index);
                            if (Directory.Exists(@Element))
                            {
                                Command.Execute("rmdir", new string[] { Element, "-r" });
                            } else if (File.Exists(@Element))
                            {
                                Command.Execute("rmfile", new string[] { Element });
                            }
                        }

                        DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                    }
                    catch (Exception)
                    { }
                    break;
                case "file":
                    Command.Execute("mkfile", new string[] { Param.Replace(" ", string.Empty) });
                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                    break;
                case "folder":
                    Command.Execute("mkdir", new string[] { Param.Replace(" ", string.Empty) });
                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                    break;
            }

            recognizer.UnloadAllGrammars();
            recognizer.LoadGrammar(CommandsGrammar);
            recognizer.SpeechRecognized += sre_SpeechRecognized;
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
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
