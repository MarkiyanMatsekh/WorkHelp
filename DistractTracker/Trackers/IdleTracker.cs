using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdinDazdarevic;

namespace DistractTracker.Trackers
{
    public delegate bool CancelScreenshotDelegate();

    public class IdleTracker : DistractTrackerBase
    {
        public IdleTracker(CancelScreenshotDelegate cancelScreenshotAction)
        {
            _cancelScreenshotDelegate = cancelScreenshotAction;
        }

        private const int MaxIdleTime = 30; // seconds
        private readonly CancelScreenshotDelegate _cancelScreenshotDelegate;
        private readonly SystemIdleTimer  _idleTimer = new SystemIdleTimer();

        protected override string ActionName
        {
            get { return "Idle"; }
        }

        public override void Init()
        {
            _idleTimer.MaxIdleTime = MaxIdleTime;
            _idleTimer.OnEnterIdleState += (obj, e) => StartAwayPeriod();
            _idleTimer.OnExitIdleState += (obj, e) => EndAwayPeriod();
            _idleTimer.Start();
        }

        protected override void SetLeaveTime()
        {
            LeaveTime = DateTime.Now.AddSeconds(-1 * MaxIdleTime);
        }

        protected override bool OnBeforeTakingScreenshot()
        {
            return !_cancelScreenshotDelegate();
        }
    }
}
