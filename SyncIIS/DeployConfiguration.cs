using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanJoseWaterCompany.SyncIIS
{
    class DeployConfiguration
    {
        public List<string> Target { get; set; }
        public string Source { get; set; }
        public string Site { get; set; }

        public DeployConfiguration()
        {
            Target = new List<string>();
        }
    }
}
