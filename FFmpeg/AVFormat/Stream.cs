using System;
using System.IO;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVCodec;

namespace FakeXna.FFmpeg.AVFormat
{
	public class Stream
	{
		IntPtr native;

		public Stream(IntPtr native)
		{
			this.native = native;
		}

		public CodecContext Codec
		{
			get {
				// The codec field is just past two int fields
				return new CodecContext(Marshal.ReadIntPtr(native, 2 * sizeof(int)));
			}
		}
		
	}
}

