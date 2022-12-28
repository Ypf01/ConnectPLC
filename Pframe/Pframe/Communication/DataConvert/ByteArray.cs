using System;
using System.Collections.Generic;

namespace Pframe.DataConvert
{
	public class ByteArray
	{
		public byte this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}
        
		public int Length
		{
			get
			{
				return this.list.Count;
			}
		}
        
		public byte[] array
		{
			get
			{
				return this.list.ToArray();
			}
		}
        
		public void Clear()
		{
			this.list = new List<byte>();
		}
        
		public void Add(byte item)
		{
			this.list.Add(item);
		}
        
		public void Add(byte[] items)
		{
			this.list.AddRange(items);
		}
        
		public void Add(ByteArray byteArray)
		{
			this.list.AddRange(byteArray.array);
		}
        
		public void Add(ushort value)
		{
			this.list.Add((byte)(value >> 8));
			this.list.Add((byte)value);
		}
        
		public void Add(short value)
		{
			this.list.Add((byte)(value >> 8));
			this.list.Add((byte)value);
		}
        
		public ByteArray()
		{
			this.list = new List<byte>();
		}
        
		private List<byte> list;
	}
}
