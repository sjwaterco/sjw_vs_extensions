namespace SanJoseWaterCompany.SyncIIS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate void OutputTextHandler(object sender, string dataReceived);
    
    class WebDeploy
    {
        public event OutputTextHandler OutputGenerated;
        private string webDeployLocation;

        /// <summary>
        /// Calls Web Deploy (repeatedly, if necessary) given a certain 
        /// configuration.
        /// </summary>
        /// <param name="config"></param>
        public void Deploy(DeployConfiguration config)
        {
            var verb = "-verb:sync";
            var source = "-source:appHostConfig=\"{0}\",computername={1}";
            var destination = "-dest:appHostConfig=\"{0}\",computername={1}";

            source = string.Format(source, config.Site, config.Source);

            if (!SetWebDeployLocation())
            {
                return;
            }

            foreach (var dest in config.Target)
            {
                RunProcess(
                    verb,
                    source,
                    string.Format(destination, config.Site, dest),
                    config);
            }
            
        }

        /// <summary>
        /// Sets the path for the web deploy executable.
        /// </summary>
        /// <returns>true if found; false if not found</returns>
        private bool SetWebDeployLocation()
        {
            var amd64 = @"c:\Program Files\";
            var x86 = @"c:\Program Files (x86)\";
            var wdpath = @"IIS\Microsoft Web Deploy V3\";
            if (File.Exists(amd64 + wdpath + "msdeploy.exe"))
            {
                webDeployLocation = amd64 + wdpath;
                return true;
            }
            if (File.Exists(x86 + wdpath + "msdeploy.exe"))
            {
                webDeployLocation = x86 + wdpath;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Runs the web deploy executable and redirects output to the data
        /// received event.
        /// </summary>
        /// <param name="verb">Action to be performed</param>
        /// <param name="source">Source Machine</param>
        /// <param name="destination">Destination Machine</param>
        /// <param name="config">Configuration parameter for determining
        /// things like username, password, domain</param>
        private void RunProcess(
            string verb,
            string source,
            string destination,
            DeployConfiguration config)
        {
            var arguments = "{0} {1} {2}";

            try
            {
                var proc = new Process();

                proc.StartInfo.FileName = webDeployLocation + "msdeploy.exe";
                proc.StartInfo.Arguments = string.Format(arguments, verb, source, destination);
                OutputGenerated(this, "Source:" + source + "\r\n");
                OutputGenerated(this, "Destination:" + destination + "\r\n");
                OutputGenerated(this, webDeployLocation + proc.StartInfo.FileName + " " + proc.StartInfo.Arguments + "\r\n");
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.Domain = config.Domain;
                proc.StartInfo.UserName = config.Username;
                proc.StartInfo.Password = config.Password;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.CreateNoWindow = true;

                proc.Exited += proc_Exited;
                proc.OutputDataReceived += (sender, e) =>
                {
                    OutputGenerated(sender, e.Data + "\r\n");
                };
                proc.ErrorDataReceived += (sender, e) =>
                {
                    OutputGenerated(sender, e.Data + "\r\n");
                };

                proc.Start();

                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
            }
            catch(Exception e)
            {
                OutputGenerated(this, e.StackTrace + e.Message.ToString() + "\r\n");
            }

        }

        void proc_Exited(object sender, EventArgs e)
        {
            if (OutputGenerated != null) 
            { 
                OutputGenerated(sender, "Program has exited normally."); 
            }
        }

        private void proc_DataReceived(object sender, string data)
        {
            if (OutputGenerated != null) { OutputGenerated(sender, data); }
            
        }
    }
}
