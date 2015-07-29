using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanJoseWaterCompany.SyncIIS
{
    class WebDeploy
    {
        public static void Deploy(DeployConfiguration config)
        {
            var verb = "-verb:sync";
            var source = "-source:appHostConfig=\"{0}\";computername={1}";
            var destination = "-dest:appHostConfig=\"{0};computername={1}\"";

            source = string.Format(source, config.Site, config.Source);

            foreach(var dest in config.Target)
            {
                RunProcess(
                    verb, 
                    source, 
                    string.Format(destination, config.Site, dest));
            }

        }

        private static void RunProcess(
            string verb, 
            string source, 
            string destination)
        {
            var cmdPath =
                System.Environment.GetEnvironmentVariable("ProgramFiles") +
                @"IIS\Microsoft Web Deploy V3\msdeploy.exe";
            var arguments = "{0} {1} {2}";

            var proc = new Process();

            proc.StartInfo.FileName = cmdPath;
            proc.StartInfo.Arguments = string.Format(arguments, verb, source, destination);
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.CreateNoWindow = true;

            proc.ErrorDataReceived += proc_DataReceived;
            proc.OutputDataReceived += proc_DataReceived;

            proc.Start();

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit();
        }

        private static void proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
