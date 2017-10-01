namespace VoiceCommander.CommandLine
{
    class Register
    {
        public Register()
        {
            Command.Register("count", LineCommands.CountElements);
            Command.Register("clear", LineCommands.Clear);
            Command.Register("info", LineCommands.ShowItemDetails);
            Command.Register("fontsize", LineCommands.ChangeFontSize);
            Command.Register("cd", LineCommands.ChangeLocation);
            Command.Register("read", LineCommands.ReadContent);
            Command.Register("mkfile", LineCommands.CreateFile);
            Command.Register("rmfile", LineCommands.DeleteFile);
            Command.Register("mkdir", LineCommands.CreateFolder);
            Command.Register("rmdir", LineCommands.DeleteFolder);
            Command.Register("mv", LineCommands.MoveFile);
        }
    }
}
