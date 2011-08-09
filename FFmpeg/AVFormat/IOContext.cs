using System;
using System.IO;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVUtil;

namespace FakeXna.FFmpeg.AVFormat
{
	public abstract class IOContext : IDisposable
	{
		internal IntPtr native { get; private set; }

		protected IOContext(IntPtr native)
		{
			this.native = native;
		}

		~IOContext()
		{
			Dispose ();
		}

		public virtual void Dispose()
		{
			if (buffer != IntPtr.Zero)
				buffer = Memory.Free(buffer);
			
			if (native != IntPtr.Zero)
				native = Memory.Free(native);
		}

		IntPtr buffer
		{
			get {
				if (native == IntPtr.Zero)
					return IntPtr.Zero;
				return Marshal.ReadIntPtr(native);
			}
			set {
				if (native != IntPtr.Zero)
					Marshal.WriteIntPtr(native, value);
			}
		}

		public static IOContext FromStream(System.IO.Stream stream)
		{
			return StreamIOContext.Construct(stream);
		}
	}
}
