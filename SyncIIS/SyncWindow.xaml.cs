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

namespace SanJoseWaterCompany.SyncIIS
{
    /// <summary>
    /// Interaction logic for SyncWindow.xaml
    /// </summary>
    public partial class SyncWindow : Window
    {
        public SyncWindow()
        {
            InitializeComponent();
        }

        private void btnSynchronize_Click(object sender, RoutedEventArgs e)
        {
            var config = new DeployConfiguration();
            
            var userDomain = txtUsername.Text.Split(new string[] {@"\"}, StringSplitOptions.RemoveEmptyEntries);

            config.Target = txtDestination.Text.Split(new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            config.Site = txtSite.Text;
            config.Source = txtSource.Text;
            config.Username = (userDomain.Length > 1 ? userDomain [1] : userDomain [0]);
            config.Password = new SecureString();
            config.Domain = (userDomain.Length > 1 ? userDomain[0] : "");

            foreach (char c in txtPassword.Password.ToCharArray())
            {
                config.Password.AppendChar(c);
            }
           

            WebDeploy deployment = new WebDeploy();
            deployment.OutputGenerated += deployment_OutputGenerated;
            deployment.Deploy(config);
        }

        void deployment_OutputGenerated(object sender, string dataReceived)
        {
            this.Dispatcher.Invoke((Action)(() => {
                txtOutput.Text += dataReceived;
            }));
        }
    }
}
