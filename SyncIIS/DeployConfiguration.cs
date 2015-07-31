using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SanJoseWaterCompany.SyncIIS
{
    public class DeployConfiguration
    {
        public List<string> Target { get; set; }
        public string Source { get; set; }
        public string Site { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public SecureString Password { get; set; }

        public DeployConfiguration()
        {
            Target = new List<string>();
        }

        public string GetUnsecurePassword()
        {
            if (this.Password == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(this.Password);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public void SetSecurePassword(string value)
        {
            this.Password = new SecureString();

            foreach (char c in value.ToCharArray())
            {
                this.Password.AppendChar(c);
            }
        }
    }
}
