using System;
using System.Collections.Generic;
using System.Linq;
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
            config.Target = txtDestination.Text.Split(new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            config.Site = txtSite.Text;
            config.Source = txtSource.Text;
        }
    }
}
