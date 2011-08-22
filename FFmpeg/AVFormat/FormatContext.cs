using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVCodec;
using FakeXna.FFmpeg.AVUtil;

namespace FakeXna.FFmpeg.AVFormat
{
	public class FormatContext : IDisposable
	{
		IntPtr native;
		IOContext io;

		public FormatContext(IOContext pb)
		{
			native = NativeMethods.avformat_alloc_context();
			if (native == IntPtr.Zero)
				throw new OutOfMemoryException();
			IOContext = pb;
			// NOTE: We use this special name for a reason...
			int re = NativeMethods.avformat_open_input(ref native, "@@@@@@@@", IntPtr.Zero, IntPtr.Zero);
			if (re < 0)
				throw new ArgumentException("av_format_open_input: " + re);

			int re2 = NativeMethods.av_find_stream_info(native);
			if (re2 < 0)
				throw new ArgumentException("av_find_stream_info: " + re2);
		}

		~FormatContext()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			IOContext = null;
			if (native != IntPtr.Zero)
			{
				NativeMethods.avformat_free_context(native);
				native = IntPtr.Zero;
			}
		}

		public IOContext IOContext
		{
			// There are four pointers before the ioContext (pb)
			get {
				if (native == IntPtr.Zero)
					return null;
				IntPtr pb = Marshal.ReadIntPtr(native, 4*Memory.IntPtrSize);
				if (pb != ((io == null) ? IntPtr.Zero : io.native))
					io = (pb == IntPtr.Zero) ? null : new NativeIOContext(pb);
				return io;
			}
			set {
				if (native != IntPtr.Zero)
				{
					// Keep the object reference also, to make sure we prevent bad garbage collects...
					io = value;
					Marshal.WriteIntPtr(native, 4*Memory.IntPtrSize, (io == null) ? IntPtr.Zero : io.native);
				}
			}
		}

		public Stream FindBestStream(MediaType type, out Codec decoder)
		{
			IntPtr decptr;
			int index = NativeMethods.av_find_best_stream(native, type, -1, -1, out decptr, 0);
			if (index < 0)
				throw new ArgumentException();
			decoder = (decptr == IntPtr.Zero) ? null : new Codec(decptr);
			return Streams[index];
		}

		public StreamCollection Streams
		{
			get {
				// NOTE: Technically a uint, but we shouldn't have so many that it matters...
				int count = Marshal.ReadInt32(native, 5*Memory.IntPtrSize);
				if (count < 0)
					throw new InvalidOperationException();

				// NOTE: Assume pointer are pointer-size aligned, should be the case for most compilers
				IntPtr streams = native + 6*Memory.IntPtrSize;
				// If FF_API_MAX_STREAMS is defined, it is inline, otherwise indirect
				// If Indirect access, the names follows directly
				if (Marshal.ReadInt32(streams, Memory.IntPtrSize) == 0x40404040) // '@@@@'
					streams = Marshal.ReadIntPtr(streams);

				return new StreamCollection(count, streams);
			}
		}

		public IEnumerable<Packet> Frames
		{
			get {
				// Since FFmpeg only guarantees that the packet will be valid until next call
				// of av_read_frame, we can just as well use a single internal packet aswell...
				Packet framePacket = new Packet();

				while (NativeMethods.av_read_frame(native, framePacket.native) == 0)
					yield return framePacket;
			}
		}

		public static void Init()
		{
			// TODO: Figure out what versions we actually do support
			if (NativeMethods.avformat_version() < 0x340000)
				throw new NotSupportedException();
			NativeMethods.av_register_all();
		}
	}
}
