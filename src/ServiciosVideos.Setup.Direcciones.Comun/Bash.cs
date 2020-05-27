using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class Bash : Command
    {
        public Bash(string command, string password = "", string workingDirectory = "", 
        params string[] args
        )
            : base("bash", workingDirectory: workingDirectory, args: GetArgs(command, password, args))
        {
        }


        private static string GetArgs(string command, string password = "", params string[] args)
            =>
            string.IsNullOrEmpty(password)
            ? $"-c \"{command} {EscapeArgs(args)}\"".Trim()
            : $"-c \"echo {EscapePassword(password)} | sudo -S {command} {EscapeArgs(args)}\"".Trim();//$"-c 'echo '{password}' | sudo -S {command}'";


        //private static string ScapeCommand(string command) => command;  //=> command ;// command.Replace("\"", "\\\\\\\"").Replace("'", "\\\'");

        private static string EscapeArgs(params string[] args)
        {
            if(args==null || args.Length==0) return "";
            var argsList = new List<string>();
            foreach( var arg in args){
                argsList.Add( EscapeSingleQuote( arg.Replace("\"",  arg.StartsWith("\\'") || arg.EndsWith("\\'") ? "\\\"" :"\\\\\\\"")) );
            }
            return  string.Join(" ", argsList);

        }


        private static string EscapePassword(string password) =>
            password
            .Replace("\\", "\\\\")
            .Replace("$", "\\$")
            .Replace("#","\\#")
            .Replace("%", "\\%")
            .Replace("&", "\\&")
            .Replace("@", "\\@")
            .Replace("!", "\\!")
            .Replace("*", "\\*")
            .Replace("\"", "\\\\\\\"")
            .Replace("'", "\\\'");

        

        private static string EscapeSingleQuote(string arg)
        {
            if (string.IsNullOrEmpty(arg)) return arg;

            var (startsWithSingleQuote, endsWithSingleQuote) = ExtractFromSingleQuote(ref arg);
            arg = arg.Replace("'", ( (endsWithSingleQuote || startsWithSingleQuote)? "'\\''" :  "\\\'"));
            return $"{(startsWithSingleQuote ? "'" : "")}{arg}{(endsWithSingleQuote ? "'" : "")}";

        }

        private static (bool startsWithSingleQuote, bool endsWithSingleQuote) ExtractFromSingleQuote(ref string arg)
        {

            if (string.IsNullOrEmpty(arg)) return (false, false);

            var starsWihtSingleQuote = arg.StartsWith("\\'");
            var startDelta = starsWihtSingleQuote ? 2 : 0;

            var endsWihtSingleQuote = arg.EndsWith("\\'");
            var endDelta = endsWihtSingleQuote ? 2 : 0;

            arg = (starsWihtSingleQuote || endsWihtSingleQuote)
            ? arg.Substring(startDelta, arg.Length - startDelta - endDelta)
            : arg;

            return (starsWihtSingleQuote, endsWihtSingleQuote);
        }


        public static async Task<CommandResult> ExecuteAsync(string command, string password = "", string workingDirectory = "")
        {
            var bash = new Bash(command, password, workingDirectory);
            bash.Start();
            return await bash.WaitForExitAsync();
        }


        public static CommandResult Execute(string command, string password = "", string workingDirectory = "")
        {
            var bash = new Bash(command, password, workingDirectory);
            bash.Start();
            return bash.WaitForExit();
        }


        public static async Task<string> WhoAmIAsync()
        {
            var bash = new Bash("whoami");
            bash.Start();
            var result = await bash.WaitForExitAsync();
            return result.LastOutput;
        }

        public static string WhoAmI()
        {
            var bash = new Bash("whoami");
            bash.Start();
            var result = bash.WaitForExit();
            return result.LastOutput;
        }

        public static bool IsSudoPassword(string password)
        {
            Execute("sudo -k");

            var bash = new Bash("whoami", password: password);
            bash.Start();
            var result = bash.WaitForExit();
            return result.LastOutput == "root";
        }

        public static async Task<bool> IsSudoPasswordAsync(string password)
        {
            await ExecuteAsync("sudo -k");

            var bash = new Bash("whoami", password: password);
            bash.Start();
            var result = await bash.WaitForExitAsync();
            
            return result.LastOutput == "root";
        }

    }
}
