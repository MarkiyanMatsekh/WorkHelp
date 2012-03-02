using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace DistractTracker.SystemIdleTimer
{
    public class SystemIdleTimer : Component
    {
        private static List<WeakReference> __ENCList = new List<WeakReference>();
        private const double INTERNAL_TIMER_INTERVAL = 550.0;
        private bool m_IsIdle;
        private object m_LockObject;
        private int m_MaxIdleTime;
        private Timer ticker;

        [Description("Event that if fired when idle state is entered.")]
        public event OnEnterIdleStateEventHandler OnEnterIdleState;

        [Description("Event that is fired when leaving idle state.")]
        public event OnExitIdleStateEventHandler OnExitIdleState;

        public SystemIdleTimer()
        {
            List<WeakReference> list = __ENCList;
            lock (list)
            {
                __ENCList.Add(new WeakReference(this));
            }
            this.m_IsIdle = false;
            this.m_LockObject = RuntimeHelpers.GetObjectValue(new object());
            this.ticker = new Timer(550.0);
            this.ticker.Elapsed += new ElapsedEventHandler(this.InternalTickerElapsed);
        }

        private void InternalTickerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Win32Wrapper.GetIdle() > (this.MaxIdleTime * 0x3e8L))
            {
                if (!this.m_IsIdle)
                {
                    object lockObject = this.m_LockObject;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockObject);
                    lock (lockObject)
                    {
                        this.m_IsIdle = true;
                    }
                    IdleEventArgs args = new IdleEventArgs(e.SignalTime);
                    OnEnterIdleStateEventHandler onEnterIdleStateEvent = this.OnEnterIdleState;
                    if (onEnterIdleStateEvent != null)
                    {
                        onEnterIdleStateEvent(this, args);
                    }
                }
            }
            else if (this.m_IsIdle)
            {
                object expression = this.m_LockObject;
                ObjectFlowControl.CheckForSyncLockOnValueType(expression);
                lock (expression)
                {
                    this.m_IsIdle = false;
                }
                IdleEventArgs args2 = new IdleEventArgs(e.SignalTime);
                OnExitIdleStateEventHandler onExitIdleStateEvent = this.OnExitIdleState;
                if (onExitIdleStateEvent != null)
                {
                    onExitIdleStateEvent(this, args2);
                }
            }
        }

        public void Start()
        {
            this.ticker.Start();
        }

        public void Stop()
        {
            this.ticker.Stop();
            object lockObject = this.m_LockObject;
            ObjectFlowControl.CheckForSyncLockOnValueType(lockObject);
            lock (lockObject)
            {
                this.m_IsIdle = false;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this.ticker.Enabled;
            }
        }

        [Description("Maximum idle time in seconds.")]
        public uint MaxIdleTime
        {
            get
            {
                return (uint) this.m_MaxIdleTime;
            }
            set
            {
                if (value == 0L)
                {
                    throw new ArgumentException("MaxIdleTime must be larger then 0.");
                }
                this.m_MaxIdleTime = (int) value;
            }
        }

        public delegate void OnEnterIdleStateEventHandler(object sender, IdleEventArgs e);

        public delegate void OnExitIdleStateEventHandler(object sender, IdleEventArgs e);
    }
}

