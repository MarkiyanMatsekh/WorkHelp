using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistractTracker.Trackers
{
    public class DistractTrackerManager : IDisposable
    {
        private readonly IdleTracker _idleTracker;
        private readonly LockTracker _lockTracker;

        public DistractTrackerManager()
        {
            _idleTracker = new IdleTracker(CancelScreenshot);
            _lockTracker = new LockTracker();
        }

        public  void Init()
        {
            _idleTracker.Init();
            _lockTracker.Init();
        }

        private bool CancelScreenshot()
        {
            return _lockTracker.IsAway;
        }

        public void Dispose()
        {
            _lockTracker.Dispose();
        }
    }
}
