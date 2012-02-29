using System;
using System.IO;
using System.Threading;
using EdinDazdarevic;
using Microsoft.Win32;

namespace DistractTracker
{
    public enum DistractReason { Lock, Idle }

    // todo: consider creating class hierarchy with overrided start/end away period methods because there are too many 'if's already
    public class DistractTracker : IDisposable
    {
        private string LogFileName { get { return string.Format(@"{0}\log.txt", Today, DateTime.Now.ToString("yyyy-MM-dd(HH.mm.ss)")); } }
        private string Today
        {
            get
            {
                // if application runs more then 1 day this approach is best probably
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(date))
                    Directory.CreateDirectory(date);
                return date;
            }
        }
        private DateTime _leaveTime = DateTime.Now;
        private bool _isLocked = false;
        private bool _isIdle = false;

        private const int MaxIdleTime = 30; // seconds

        readonly SystemIdleTimer _idleTimer = new SystemIdleTimer();

        public void Init()
        {
            SystemEvents.SessionSwitch += OnSessionSwitch;
            _idleTimer.MaxIdleTime = MaxIdleTime;
            _idleTimer.OnEnterIdleState += (obj, e) => StartAwayPeriod(DistractReason.Idle);
            _idleTimer.OnExitIdleState += (obj, e) => EndAwayPeriod(DistractReason.Idle);
            _idleTimer.Start();
        }

        public void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                StartAwayPeriod(DistractReason.Lock);
            }
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                EndAwayPeriod(DistractReason.Lock);
            }
        }

        private void StartAwayPeriod(DistractReason reason)
        {
            switch (reason)
            {
                case DistractReason.Lock:
                    // dont reset timer if we've been idle for a while
                    if (!_isIdle)
                        _leaveTime = DateTime.Now;
                    _isLocked = true;
                    break;
                case DistractReason.Idle:
                    // don't reset timer if we've been locked for a while
                    if (!_isLocked)
                        _leaveTime = DateTime.Now.AddSeconds(-1 * MaxIdleTime);
                    _isIdle = true;
                    break;
            }

            File.AppendAllText(LogFileName, String.Format("{0} at {1}; ", reason, _leaveTime.ToString("HH:mm:ss")));
        }

        private void EndAwayPeriod(DistractReason reason)
        {
            File.AppendAllText(LogFileName, String.Format("Back from {0} at {1}. ", reason, 
                                                           DateTime.Now.ToString("HH:mm:ss")));

            int suspendScreenshotTime = 0;

            if (reason == DistractReason.Idle)
            {
                _isIdle = false;
            }

            if (_isLocked)
            {
                if (reason != DistractReason.Lock)
                    return; // wait for unlock event
                else
                {
                    _isLocked = false;
                    suspendScreenshotTime = 1000;
                }
                
            }

            var awayTime = DateTime.Now - _leaveTime;

            // skip short pauses
            if (awayTime.Minutes > 0)
            {
                // wait until screen refreshes
                Thread.Sleep(suspendScreenshotTime);

                TakeScreenshot(awayTime);
            }

            // make log more readable and notify that away period is ended
            File.AppendAllText(LogFileName, String.Format("Away time = {0}{1}", awayTime.ToString("c"), Environment.NewLine));
        }


        private void TakeScreenshot(TimeSpan awayTime)
        {
            var sc = new ScreenCapture();
            var img = sc.CaptureScreen();
            try
            {
                img.Save(String.Format(@"{0}\{1} - {2}.jpg",
                                       Today,
                                       DateTime.Now.ToString("HH mm"),
                                       String.Format("{0}m", awayTime.Minutes)));
            }
            finally
            {
                img.Dispose();
            }
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
