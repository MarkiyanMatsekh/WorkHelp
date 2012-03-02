using System;

namespace DistractTracker.SystemIdleTimer
{
    public class IdleEventArgs : EventArgs
    {
        private DateTime m_EventTime;

        public IdleEventArgs(DateTime timeOfEvent)
        {
            this.m_EventTime = timeOfEvent;
        }

        public DateTime EventTime
        {
            get
            {
                return this.m_EventTime;
            }
        }
    }
}

