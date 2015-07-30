namespace SanJoseWaterCompany.SyncIIS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate void OutputTextHandler(object sender, string dataReceived);
    
    class WebDeploy
    {
        public event OutputTextHandler OutputGenerated;

        public void Deploy(DeployConfiguration config)
        {
            var verb = "-verb:sync";
            var source = "-source:appHostConfig=\"{0}\";computername={1}";
            var destination = "-dest:appHostConfig=\"{0};computername={1}\"";

            source = string.Format(source, config.Site, config.Source, config);

            foreach(var dest in config.Target)
            {
                RunProcess(
                    verb, 
                    source, 
                    string.Format(destination, config.Site, dest), 
                    config);
            }

        }

        private void RunProcess(
            string verb, 
            string source, 
            string destination,
            DeployConfiguration config)
        {
            var cmdPath = @"c:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe";
            var arguments = "{0} {1} {2}";
            var timeout = 10000;

            var proc = new Process();

            proc.StartInfo.FileName = cmdPath;
            proc.StartInfo.Arguments = string.Format(arguments, verb, source, destination);
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.Domain = config.Domain;
            proc.StartInfo.UserName = config.Username;
            proc.StartInfo.Password = config.Password;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.CreateNoWindow = true;

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                proc.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        OutputGenerated(sender, e.Data);
                    }
                };
                proc.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        OutputGenerated(sender, e.Data);
                    }
                }; 
                
                proc.Start();

                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();

                if (proc.WaitForExit(timeout) &&
                    outputWaitHandle.WaitOne(timeout) &&
                    errorWaitHandle.WaitOne(timeout))
                {
                    OutputGenerated(this, "Process Completed.");
                }
                else
                {
                    OutputGenerated(this, "Process Timed Out.");
                }
            }

            
        }

        private void proc_DataReceived(object sender, string data)
        {
            if (OutputGenerated != null) { OutputGenerated(sender, data); }
            
        }
    }
}
