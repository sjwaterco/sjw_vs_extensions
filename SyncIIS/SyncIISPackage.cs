﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System.Linq;
using System.Collections.Generic;

namespace SanJoseWaterCompany.SyncIIS
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidSyncIISPkgString)]
    public sealed class SyncIISPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public SyncIISPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidSyncIISCmdSet, (int)PkgCmdIDList.cmdSyncIIS);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            DeployConfiguration config = new DeployConfiguration();
            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            WritableSettingsStore userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!userSettingsStore.CollectionExists("SyncIIS"))
            {
                userSettingsStore.CreateCollection("SyncIIS");
            }

            config.Domain = userSettingsStore.GetString("SyncIIS", "Domain", "");
            config.SetSecurePassword(userSettingsStore.GetString("SyncIIS", "Password", ""));
            config.Site = userSettingsStore.GetString("SyncIIS", "Site", "");
            config.Source = userSettingsStore.GetString("SyncIIS", "Source", "");
            config.Target = userSettingsStore.GetString("SyncIIS", "Target", "").Split(new string[] { "," },
                StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList<string>();
            config.Username = userSettingsStore.GetString("SyncIIS", "Username", "");

            SyncWindow window = new SyncWindow(config);
            window.ShowDialog();

            var settings = window.Configuration;
            
            userSettingsStore.SetString("SyncIIS", "Domain", settings.Domain);
            userSettingsStore.SetString("SyncIIS", "Password", settings.GetUnsecurePassword());
            userSettingsStore.SetString("SyncIIS", "Site", settings.Site);
            userSettingsStore.SetString("SyncIIS", "Source", settings.Source);
            userSettingsStore.SetString("SyncIIS", "Target", string.Join(",", settings.Target.ToArray()));
            userSettingsStore.SetString("SyncIIS", "Username", settings.Username);
        }

    }
}
