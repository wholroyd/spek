namespace ElectricSky.Spek
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// 
    /// The minimum requirement for a class to be considered a valid package for Visual Studio is to
    /// implement the IVsPackage interface and register itself with the shell. This package uses the
    /// helper classes defined inside the Managed Package Framework (MPF) to do it: it derives from
    /// the Package class that provides the implementation of the IVsPackage interface and uses the
    /// registration attributes defined in the framework to register itself and its components with
    /// the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideServiceAttribute(typeof(SpekLanguageService), ServiceName = "Spek Language Service")]
    [ProvideLanguageServiceAttribute(typeof(SpekLanguageService),
                            "Spek Language",
                            106,             // resource ID of localized language name
                            CodeSense = true,             // Supports IntelliSense
                            RequestStockColors = false,   // Supplies custom colors
                            EnableCommenting = true,      // Supports commenting out code
                            EnableAsyncCompletion = true  // Supports background parsing
                            )]
    [ProvideLanguageExtensionAttribute(typeof(SpekLanguageService),
                                       ".spek")]
    [Microsoft.VisualStudio.Shell.ProvideService(typeof(TestLanguageService))]
    [Microsoft.VisualStudio.Shell.ProvideLanguageExtension(typeof(TestLanguageService), ".testext")]
    [Microsoft.VisualStudio.Shell.ProvideLanguageService(typeof(TestLanguageService), "Test Language", 0,
        AutoOutlining = true,
        EnableCommenting = true,
        MatchBraces = true,
        ShowMatchingBrace = true)]
    [Guid(GuidList.guidSpekPkgString)]
    public sealed class SpekPackage : Package, IOleComponent
    {
        private uint componentId;

        /// <summary>
        /// Default constructor of the package. Inside this method you can place any initialization
        /// code that does not require any Visual Studio service because at this point the package
        /// object is created but not sited yet inside Visual Studio environment. The place to do
        /// all the other initialization is the Initialize method.
        /// </summary>
        public SpekPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited,
        /// so this is the place where you can put all the initialization code that rely on services
        /// provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandId = new CommandID(GuidList.guidSpekCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(this.MenuItemCallback, menuCommandId);
                mcs.AddCommand( menuItem );
            }

            // Proffer the service.
            IServiceContainer serviceContainer = this as IServiceContainer;
            SpekLanguageService langService = new SpekLanguageService();
            langService.SetSite(this);
            serviceContainer.AddService(typeof(SpekLanguageService), langService, true);

            // Register a timer to call our language service during
            // idle periods.
            IOleComponentManager mgr = this.GetService(typeof(SOleComponentManager))
                                       as IOleComponentManager;
            if (this.componentId == 0 && mgr != null)
            {
                OLECRINFO[] crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                              (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                              (uint)_OLECADVF.olecadvfRedrawOff |
                                              (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                int hr = mgr.FRegisterComponent(this, crinfo, out this.componentId);
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
            IVsUIShell uiShell = (IVsUIShell)this.GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "Spek",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }

        #region Proffering via IOleComponent

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void OnActivationChange(
            IOleComponent pic,
            int fSameComponent,
            OLECRINFO[] pcrinfo,
            int fHostIsActivating,
            OLECHOSTINFO[] pchostinfo,
            uint dwReserved)
        {
        }

        public int FDoIdle(uint grfidlef)
        {
            bool bPeriodic = (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0;
            // Use typeof(TestLanguageService) because we need to
            // reference the GUID for our language service.
            LanguageService service = GetService(typeof(SpekLanguageService))
                                      as LanguageService;
            if (service != null)
            {
                service.OnIdle(bPeriodic);
            }
            return 0;
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 0;
        }

        public void Terminate()
        {
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        #endregion Proffering
    }
}
