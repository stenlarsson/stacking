using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using FakeXna.FFmpeg.AVUtil;

namespace FakeXna.FFmpeg.AVFormat
{
	public class StreamCollection : IEnumerable<Stream>
	{
		IntPtr start;
		public int Length { get; private set; }

		internal StreamCollection(int count, IntPtr start)
		{
			this.Length = count;
			this.start = start;
		}

		public Stream this[int index] {
			get {
				if (index < 0 || index >= Length)
					throw new ArgumentOutOfRangeException();
				IntPtr ptr = Marshal.ReadIntPtr(start, index * Memory.IntPtrSize);
				if (Marshal.ReadInt32(ptr) != index)
					throw new InvalidOperationException();
				return new Stream(ptr);
			}
		}

		public IEnumerator<Stream> GetEnumerator()
		{
			for (int i = 0; i < Length; i++) {
				IntPtr ptr = Marshal.ReadIntPtr(start, i * Memory.IntPtrSize);
				if (Marshal.ReadInt32(ptr) == i)
					yield return new Stream(ptr);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
