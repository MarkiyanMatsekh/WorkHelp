using System;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace DistractTracker.Trackers
{
    public abstract class DistractTrackerBase
    {
        private string LogFileName { get { return string.Format(@"{0}\log.txt", Today, DateTime.Now.ToString("yyyy-MM-dd(HH.mm.ss)")); } }
        private string Today
        {
            get
            {
                // if application runs more then 1 day this approach is probably the best
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(date))
                    Directory.CreateDirectory(date);
                return date;
            }
        }

        protected DateTime LeaveTime = DateTime.Now;
        protected abstract string ActionName { get; }


        public bool IsAway { get; private set; }
        public abstract void Init();

        protected void StartAwayPeriod()
        {
            IsAway = true;
            SetLeaveTime();
            File.AppendAllText(LogFileName, String.Format("{0} at {1}; ", ActionName, LeaveTime.ToString("HH:mm:ss")));
        }

        protected void EndAwayPeriod()
        {
            File.AppendAllText(LogFileName, String.Format("Back from {0} at {1}. ",
                                                           ActionName,
                                                           DateTime.Now.ToString("HH:mm:ss")));
            IsAway = false;
            var awayTime = DateTime.Now - LeaveTime;
            
            if (!OnBeforeTakingScreenshot())
            {
                File.AppendAllText(LogFileName, @"Screenshot canceled. ");
                return;
            }
            if (awayTime.Minutes > 0)
            {
                TakeScreenshot(awayTime);
                File.AppendAllText(LogFileName, @"Screenshot taken. ");
            }

            // notify the end of away period
            File.AppendAllText(LogFileName, String.Format("Away time = {0}{1}", awayTime.ToString("c"), Environment.NewLine));
        }

        protected virtual void SetLeaveTime()
        {
            LeaveTime = DateTime.Now;
        }

        protected virtual bool OnBeforeTakingScreenshot()
        {
            return true;
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
    }
}
