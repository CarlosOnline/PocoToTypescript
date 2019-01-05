using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pocoyo
{
    public class SemaphoreLock : IDisposable
    {
        private readonly Semaphore _semaphore;
        private readonly bool _locked;

        public SemaphoreLock(Semaphore semaphore)
        {
            _semaphore = semaphore;
            _semaphore.WaitOne();
            _locked = true;
        }

        public void Dispose()
        {
            if (_locked)
                _semaphore.Release();
        }
    }
}
