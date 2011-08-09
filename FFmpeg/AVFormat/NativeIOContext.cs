using System;

namespace FakeXna.FFmpeg.AVFormat
{
	// TODO: Reimplement this in a more generic way...
	public class NativeIOContext : IOContext
	{
		public NativeIOContext (IntPtr native) : base(native)
		{
		}

		public override void Dispose ()
		{
			// We don't own it, so don't deallocate native resource...
		}
	}
}
