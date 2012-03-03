using System;

namespace DistractTracker.SystemIdleTimer
{
    public class IdleEventArgs : EventArgs
    {
        public IdleEventArgs(DateTime timeOfEvent)
        {
            this.EventTime = timeOfEvent;
        }

        public DateTime EventTime { get; private set; }
    }
}

