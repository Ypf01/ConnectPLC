using System;
using System.Threading;

namespace Pframe.Common
{
	public sealed class SimpleHybirdLock : IDisposable
	{
		private void method_0(bool bool_1)
		{
			if (!this.disposedValue)
			{
				if (!bool_1)
				{
				}
				this.m_waiterLock.Close();
				this.disposedValue = true;
			}
		}

		public void Dispose()
		{
			this.method_0(true);
		}

		public void Enter()
		{
			if (Interlocked.Increment(ref this.m_waiters) != 1)
			{
				this.m_waiterLock.WaitOne();
			}
		}

		public void Leave()
		{
			if (Interlocked.Decrement(ref this.m_waiters) != 0)
			{
				this.m_waiterLock.Set();
			}
		}

		public bool IsWaitting
		{
			get
			{
				return this.m_waiters == 0;
			}
		}

		public SimpleHybirdLock()
		{
			this.disposedValue = false;
			this.m_waiters = 0;
			this.m_waiterLock = new AutoResetEvent(false);
		}

		private bool disposedValue;

		private int m_waiters;

		private AutoResetEvent m_waiterLock;
	}
}
