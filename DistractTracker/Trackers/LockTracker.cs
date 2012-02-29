using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace DistractTracker.Trackers
{
    public class LockTracker : DistractTrackerBase, IDisposable
    {
        protected override string ActionName
        {
            get { return "Lock"; }
        }

        public override void Init()
        {
            SystemEvents.SessionSwitch += OnSessionSwitch;
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
                StartAwayPeriod();
            if (e.Reason == SessionSwitchReason.SessionUnlock)
                EndAwayPeriod();
        }

        protected override bool OnBeforeTakingScreenshot()
        {
            Thread.Sleep(1000);
            return base.OnBeforeTakingScreenshot();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                SystemEvents.SessionSwitch -= OnSessionSwitch;
            }
        }
    }
}
