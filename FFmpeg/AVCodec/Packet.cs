using System;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVUtil;

namespace FakeXna.FFmpeg.AVCodec
{
	public class Packet : IDisposable
	{
		internal IntPtr native { get; private set; }

		public Packet()
		{
			native = Memory.Mallocz(128); // Should be bigger than sizeof(AVPacket)
			if (native == IntPtr.Zero)
				throw new OutOfMemoryException();
			NativeMethods.av_init_packet(native);
		}

		~Packet()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (native != IntPtr.Zero) {
				NativeMethods.av_free_packet(native);
				native = Memory.Free(native);
			}
		}

		public int Size
		{
			get {
				// The size field is after two uint64_t and one void* field
				return Marshal.ReadInt32(native, 2*sizeof(long) + Memory.IntPtrSize);
			}
		}
	}
}
