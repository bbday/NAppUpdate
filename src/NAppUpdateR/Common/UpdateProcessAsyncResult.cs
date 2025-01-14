﻿namespace NAppUpdateR.Common
{
    public class UpdateProcessAsyncResult : IAsyncResult
    {
        private readonly AsyncCallback _asyncCallback;
        private readonly object _asyncState;

        private const int StatePending = 0;
        private const int StateCompletedSynchronously = 1;
        private const int StateCompletedAsynchronously = 2;
        private int _completedState = StatePending;

        private ManualResetEvent _asyncWaitHandle;
        private Exception _exception;

        public UpdateProcessAsyncResult(AsyncCallback asyncCallback, object state)
        {
            _asyncCallback = asyncCallback;
            _asyncState = state;
        }

        public void SetAsCompleted(Exception exception, bool completedSynchronously)
        {
            // Passing null for exception means no error occurred. 
            // This is the common case
            _exception = exception;

            // The m_CompletedState field MUST be set prior calling the callback
            int prevState = Interlocked.Exchange(ref _completedState,
               completedSynchronously ? StateCompletedSynchronously : StateCompletedAsynchronously);
            if (prevState != StatePending)
                throw new InvalidOperationException("You can set a result only once");

            // If the event exists, set it
            if (_asyncWaitHandle != null) _asyncWaitHandle.Set();

            // If a callback method was set, call it
            if (_asyncCallback != null) _asyncCallback(this);
        }

        public void EndInvoke()
        {
            // This method assumes that only 1 thread calls EndInvoke 
            // for this object
            if (!IsCompleted)
            {
                // If the operation isn't done, wait for it
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                _asyncWaitHandle = null;  // Allow early GC
            }

            // Operation is done: if an exception occured, throw it
            if (_exception != null) throw _exception;
        }

        public bool IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref _completedState) != StatePending;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_asyncWaitHandle == null)
                {
                    bool done = IsCompleted;
                    var mre = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref _asyncWaitHandle, mre, null) != null)
                    {
                        // Another thread created this object's event; dispose 
                        // the event we just created
                        mre.Close();
                    }
                    else
                    {
                        if (!done && IsCompleted)
                        {
                            // If the operation wasn't done when we created 
                            // the event but now it is done, set the event
                            _asyncWaitHandle.Set();
                        }
                    }
                }
                return _asyncWaitHandle;
            }
        }

        public object AsyncState { get { return _asyncState; } }

        public bool CompletedSynchronously
        {
            get
            {
                return Thread.VolatileRead(ref _completedState) == StateCompletedSynchronously;
            }
        }
    }
}
