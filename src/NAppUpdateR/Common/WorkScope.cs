﻿namespace NAppUpdateR.Common
{
    public class WorkScope : IDisposable
    {
        private readonly Action<bool> isWorkingFunc;

        public WorkScope(Action<bool> b)
        {
            isWorkingFunc = b;
            isWorkingFunc(true);
        }

        internal static IDisposable New(Action<bool> action)
        {
            return new WorkScope(action);
        }

        public void Dispose()
        {
            isWorkingFunc(false);
        }
    }
}
