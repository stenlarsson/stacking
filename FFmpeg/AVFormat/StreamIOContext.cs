using System;
using System.IO;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVUtil;

namespace FakeXna.FFmpeg.AVFormat
{
	internal class StreamIOContext : IOContext
	{
		System.IO.Stream stream;

		// Keep as delegates to avoid actual delegates from being garbage collected
		// while we are being called back from native code.
#pragma warning disable 0414
		Delegates.ReadPacketCallback reader;
		Delegates.WritePacketCallback writer;
		Delegates.SeekCallback seeker;
#pragma warning restore 0414

		StreamIOContext(
			System.IO.Stream stream,
			Delegates.ReadPacketCallback reader,
			Delegates.WritePacketCallback writer,
			Delegates.SeekCallback seeker,
			IntPtr native)
			: base(native)
		{
			this.stream = stream;
			this.reader = reader;
			this.writer = writer;
			this.seeker = seeker;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (stream != null) {
				stream.Dispose();
				stream = null;
			}
		}

		public static IOContext Construct(System.IO.Stream stream)
		{
			Delegates.ReadPacketCallback reader = null;
			Delegates.WritePacketCallback writer = null;
			Delegates.SeekCallback seeker = null;;
			
			// TODO: Implement write support whenever we need it...
			if (!stream.CanRead)
				throw new ArgumentException();
			
			if (stream.CanRead) {
				reader = delegate(IntPtr opaque, IntPtr buf, int buf_size) {
					try {
						byte[] buffer = new byte[buf_size];
						int count = stream.Read(buffer, 0, buf_size);
						Marshal.Copy(buffer, 0, buf, count);
						return count;
					} catch (Exception e) {
						// We must not throw exception back to into unmanaged world...
						Console.WriteLine(e.StackTrace);
						return -1;
					}
				};
			}

			if (stream.CanSeek) {
				seeker = delegate(IntPtr opaque, long offset, int whence) {
					try {
						// TODO: Figure out if SEEK_SET, SEEK_CUR, SEEK_END are different on other platforms...
						whence &= ~0x20000; // Ignore AVSEEK_FORCE
						switch (whence) {
						case 0x00000: // SEEK_SET
							return stream.Seek(offset, SeekOrigin.Begin);
						case 0x00001: // SEEK_CUR
							return stream.Seek(offset, SeekOrigin.Current);
						case 0x00002: // SEEK_END
							return stream.Seek(offset, SeekOrigin.End);
						case 0x10000: // AVSEEK_SIZE
							return stream.Length;
						default:
							Console.WriteLine("Strange seek whence {0}", whence);
							return -1;
						}
					} catch (Exception e) {
						// We must not throw exception back to into unmanaged world...
						Console.WriteLine(e.StackTrace);
						return -1;
					}
				};
			}
			
			IntPtr native = NativeMethods.avio_alloc_context(Memory.Mallocz(40960), 40960, 0, IntPtr.Zero, reader, writer, seeker);
			if (native == IntPtr.Zero)
				throw new InvalidOperationException();
			
			return new StreamIOContext(stream, reader, writer, seeker, native);
		}
	}
}
