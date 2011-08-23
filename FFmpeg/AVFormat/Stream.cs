using System;
using System.IO;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVCodec;

namespace FakeXna.FFmpeg.AVFormat
{
	public class Stream
	{
		FormatContext owner;
		IntPtr native;

		internal Stream(FormatContext owner, IntPtr native)
		{
			this.owner = owner;
			this.native = native;
		}

		public CodecContext Codec
		{
			get {
				if (this.owner.Disposed)
					throw new InvalidOperationException();

				// The codec field is just past two int fields
				return new CodecContext(owner, Marshal.ReadIntPtr(native, 2 * sizeof(int)));
			}
		}
		
	}
}

