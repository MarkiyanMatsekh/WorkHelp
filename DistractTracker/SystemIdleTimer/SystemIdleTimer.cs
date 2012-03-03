using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace DistractTracker.SystemIdleTimer
{
    public class SystemIdleTimer
    {
        private static readonly List<WeakReference> EncList = new List<WeakReference>();
        private const double InternalTimerInterval = 550.0;
        private bool _isIdle;
        private int _maxIdleTime;
        private readonly object _lockObject;
        private readonly Timer _ticker;

        [Description("Event that if fired when idle state is entered.")]
        public event OnEnterIdleStateEventHandler OnEnterIdleState;

        [Description("Event that is fired when leaving idle state.")]
        public event OnExitIdleStateEventHandler OnExitIdleState;

        public SystemIdleTimer()
        {
            List<WeakReference> list = EncList;
            lock (list)
            {
                EncList.Add(new WeakReference(this));
            }
            this._isIdle = false;
            this._lockObject = RuntimeHelpers.GetObjectValue(new object());
            this._ticker = new Timer(InternalTimerInterval);
            this._ticker.Elapsed += new ElapsedEventHandler(this.InternalTickerElapsed);
        }

        private void InternalTickerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Win32Wrapper.GetIdle() > (this.MaxIdleTime * 0x3e8L))
            {
                if (!this._isIdle)
                {
                    object lockObject = this._lockObject;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockObject);
                    lock (lockObject)
                    {
                        this._isIdle = true;
                    }
                    IdleEventArgs args = new IdleEventArgs(e.SignalTime);
                    OnEnterIdleStateEventHandler onEnterIdleStateEvent = this.OnEnterIdleState;
                    if (onEnterIdleStateEvent != null)
                    {
                        onEnterIdleStateEvent(this, args);
                    }
                }
            }
            else if (this._isIdle)
            {
                object expression = this._lockObject;
                ObjectFlowControl.CheckForSyncLockOnValueType(expression);
                lock (expression)
                {
                    this._isIdle = false;
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
            this._ticker.Start();
        }

        public void Stop()
        {
            this._ticker.Stop();
            object lockObject = this._lockObject;
            ObjectFlowControl.CheckForSyncLockOnValueType(lockObject);
            lock (lockObject)
            {
                this._isIdle = false;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this._ticker.Enabled;
            }
        }

        [Description("Maximum idle time in seconds.")]
        public uint MaxIdleTime
        {
            get
            {
                return (uint)this._maxIdleTime;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("MaxIdleTime must be larger then 0.");
                }
                this._maxIdleTime = (int)value;
            }
        }

        public delegate void OnEnterIdleStateEventHandler(object sender, IdleEventArgs e);

        public delegate void OnExitIdleStateEventHandler(object sender, IdleEventArgs e);
    }
}

