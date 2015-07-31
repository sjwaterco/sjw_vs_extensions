namespace SanJoseWaterCompany.SyncIIS
{
    using Microsoft.VisualStudio.Settings;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Settings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    /// <summary>
    /// Interaction logic for SyncWindow.xaml
    /// </summary>
    public partial class SyncWindow : Window
    {
        public SyncWindow(DeployConfiguration config)
        {
            InitializeComponent();
            this.Configuration = config;
            txtDestination.Text = string.Join(",", config.Target.ToArray());
            txtPassword.Password = config.GetUnsecurePassword();
            txtSite.Text = config.Site;
            txtSource.Text = config.Source;
            txtUsername.Text = (
                config.Domain == "" ? 
                config.Username : 
                config.Domain + @"\" + config.Username);
        }

        public DeployConfiguration Configuration {get; set;}

        private void btnSynchronize_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = "";

            var userDomain = txtUsername.Text.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            this.Configuration.Target = txtDestination.Text.Split(new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList<string>();
            this.Configuration.Site = txtSite.Text;
            this.Configuration.Source = txtSource.Text;
            this.Configuration.Username = (userDomain.Length > 1 ? userDomain[1] : userDomain[0]);
            this.Configuration.Domain = (userDomain.Length > 1 ? userDomain[0] : "");

            this.Configuration.SetSecurePassword(txtPassword.Password);

            WebDeploy deployment = new WebDeploy();
            deployment.OutputGenerated += deployment_OutputGenerated;
            deployment.Deploy(this.Configuration);
        }

        void deployment_OutputGenerated(object sender, string dataReceived)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                txtOutput.Text += dataReceived;
            }));
        }

        private void txtOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtOutput.ScrollToEnd();
        }
    }
}
