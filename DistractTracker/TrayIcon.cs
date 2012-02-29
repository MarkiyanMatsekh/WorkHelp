using System;
using System.Drawing;
using System.Windows.Forms;
using DistractTracker.Trackers;
using Microsoft.Win32;

namespace DistractTracker
{
    class TrayIcon : Form
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.Run(new TrayIcon());
        }

        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu;
        private readonly DistractTrackerManager _tracker = new DistractTrackerManager();


        public TrayIcon()
        {
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Exit", OnExit);

            _trayIcon = new NotifyIcon
                           {
                               Text = @"DistractTracker",
                               ContextMenu = _trayMenu,
                               Visible = true,
                               Icon = new Icon(Properties.Resources.LockTrackerIcon, 40, 40)
                           };
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; 
            ShowInTaskbar = false;

            _tracker.Init();

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _trayIcon.Dispose();
                _tracker.Dispose();
            }
            base.Dispose(isDisposing);
        }
    }
}
