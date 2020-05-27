using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class Command: IDisposable
    {
        public const int WaitForExitCode = -9999;
        public const int UnknownExitCode = -10000;
        private readonly Process _process;
        private readonly StringBuilder stringBuilderError = new StringBuilder();
        private readonly List<string> outputList = new List<string>();

        private string lastOutput;

        TaskCompletionSource<bool> errorCloseEvent;
        TaskCompletionSource<bool> completeCloseEvent;


        public Command(string command, IList<string> args, string workingDirectory = "")
            : this(command, workingDirectory: workingDirectory,  args: string.Join(" ", args ?? new string[] { }))
        {

        }

        public Command(string command, string workingDirectory = "",  params string[] args)
        {
            _process = new Process();

            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            _process.StartInfo.UseShellExecute = false;

            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardError = true;

            _process.StartInfo.FileName = command;

            if (args != null && args.Length > 0)
            {
                _process.StartInfo.Arguments = string.Join(" ", args);
            }

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                _process.StartInfo.WorkingDirectory = workingDirectory;
            }

            _process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!(e.Data == null))
                {
                    stringBuilderError.AppendLine(e.Data);
                    if (string.IsNullOrEmpty(lastOutput) || lastOutput.Trim() == "") lastOutput = e.Data;
                }
                else
                {
                    errorCloseEvent?.SetResult(true);
                }


            });

            _process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!(e.Data == null))
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        lastOutput = e.Data;
                        outputList.Add(e.Data);
                    }
                    outputList.Add(Environment.NewLine);

                }
                else
                {
                    completeCloseEvent?.SetResult(true);
                }
            });

        }

        public bool Start()
        {
            outputList.Clear();
            stringBuilderError.Clear();
            var startresult = _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            return startresult;
        }

        public CommandResult WaitForExit(int timeout = -1)
        {

            var ontime = _process.WaitForExit(timeout);
            var exit_code = GetExitCode(_process);

            return new CommandResult
            {
                OutputList = new List<string>(outputList.ToArray()),
                LastOutput = lastOutput,
                Error = stringBuilderError.ToString(),
                ExitCode = exit_code,
                TimeoutCode = exit_code == WaitForExitCode ?
                 TimeoutCode.WaitForExit :
                 (exit_code == 0 || ontime) ? TimeoutCode.NoTimeout : TimeoutCode.Delay
            };
        }

        public async Task<CommandResult> WaitForExitAsync(int millisecondsTimeout = -1,
         CancellationToken cancellationToken = default(CancellationToken), string closeSignal="q")
        {
            errorCloseEvent = new TaskCompletionSource<bool>();
            completeCloseEvent = new TaskCompletionSource<bool>();

            
            var waitForExitTime = millisecondsTimeout >= 0 ? millisecondsTimeout + 2000 : -1;

            var waitForExit = Task.Run(() => WaitForExit(waitForExitTime));

            var mres = new ManualResetEventSlim(false);
            var waitCancelationToken = Task.Run(() =>
            {
                try
                {
                    mres.Wait(cancellationToken);
                }
                catch (Exception)
                {
                }

            });

            var processTask = Task.WhenAll(waitForExit, completeCloseEvent.Task, errorCloseEvent.Task);


            var anytask = waitForExitTime >= 0
                ? await Task.WhenAny(Task.Delay(waitForExitTime), waitCancelationToken, processTask)
                : await Task.WhenAny(waitCancelationToken, processTask);

            if (anytask == waitCancelationToken)
            {
                Close(closeSignal);
            }
            else if (!(anytask == processTask && waitForExit.Result.ExitCode != WaitForExitCode))
            {
                try
                {
                    _process.Kill();
                }
                catch (Exception) { }
            }

            mres.Set();

            return waitForExit.Result;
        }

        //proc.StandardInput.WriteLine("\x3"); // CTRL-C

        public void Close(string signal)
        {
            var inputWriter = _process.StandardInput;
            inputWriter.WriteLine(signal);
            //_process.Close();
            inputWriter.Close();
        }

        private static int GetExitCode(Process process)
        {
            try
            {
                if (process.HasExited) return process.ExitCode;
                return WaitForExitCode;
            }
            catch (Exception)
            {
                return UnknownExitCode;
            }
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }


    public class CommandResult
    {
        private List<string> outputList = new List<string>();

        public List<string> OutputList
        {
            get => outputList.Where(q => !(string.IsNullOrEmpty(q) || q == Environment.NewLine || q.Trim() == "")).ToList();
            internal set => outputList = value;
        }

        public int ExitCode { get; internal set; }
        public string LastOutput { get; internal set; }
        public string Error { get; internal set; }
        public string Output { get => string.Join("", outputList); }

        public TimeoutCode TimeoutCode { get; set; }

    }

    public enum TimeoutCode
    {
        NoTimeout = 0,
        WaitForExit,
        Delay
    }

}