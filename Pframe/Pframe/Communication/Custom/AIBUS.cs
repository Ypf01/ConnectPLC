using System;
using System.IO;
using System.Threading;
using Pframe.Base;
using Pframe.DataConvert;

namespace Pframe.Custom
{
	public class AIBUS : SerialDeviceBase
	{
		public AIBUSParam ReadParam(byte iParamNo, byte iDevAdd)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.Add((byte)(128 + iDevAdd));
			byteArray.Add((byte)(128 + iDevAdd));
			byteArray.Add(82);
			byteArray.Add(iParamNo);
			byteArray.Add(new byte[2]);
			byteArray.Add(this.GetReadParity(iParamNo, iDevAdd));
			byte[] byte_ = null;
			AIBUSParam result;
			if (this.SendAndReceive(byteArray.array, ref byte_))
			{
				result = this.TranslateToParam(byte_, iDevAdd);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private AIBUSParam TranslateToParam(byte[] result2, byte devId)
		{
			AIBUSParam result;
			if (this.CheckResult(result2, devId))
			{
				result = new AIBUSParam
				{
					ActualValue = (int)result2[1] * 256 + (int)result2[0],
					SetValue = (int)result2[3] * 256 + (int)result2[2],
					HIAL = BitLib.GetBitFromByte(result2[5], 0),
					LoAL = BitLib.GetBitFromByte(result2[5], 1),
					ParamValue = (int)result2[7] * 256 + (int)result2[6]
				};
			}
			else
			{
				result = null;
			}
			return result;
		}

		private bool CheckResult(byte[] byte_0, byte byte_1)
		{
			bool result;
			if (byte_0 != null && byte_0.Length == 10)
			{
				long num = 0L;
				for (int i = 0; i < 4; i++)
				{
					num += (long)((int)byte_0[2 * i] + (int)byte_0[2 * i + 1] * 256);
				}
				num += (long)((ulong)byte_1);
				byte[] bytes = BitConverter.GetBytes(num);
				result = (bytes[0] == byte_0[8] && bytes[1] == byte_0[9]);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public AIBUSParam SetParam(byte iParamNo, short iValue, byte iDevAdd)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.Add((byte)(128 + iDevAdd));
			byteArray.Add((byte)(128 + iDevAdd));
			byteArray.Add(67);
			byteArray.Add(iParamNo);
			byteArray.Add((byte)(iValue % 256));
			byteArray.Add((byte)(iValue / 256));
			byteArray.Add(this.GetWriteParity(iParamNo, (int)iValue, iDevAdd));
			byte[] byte_ = null;
			AIBUSParam result;
			if (this.SendAndReceive(byteArray.array, ref byte_))
			{
				result = this.TranslateToParam(byte_, iDevAdd);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private byte[] GetReadParity(byte iParamNo, byte iDevAdd)
		{
			int num = (int)iParamNo * 256 + 82 + (int)iDevAdd;
			return new byte[]
			{
				(byte)(num % 256),
				(byte)(num / 256)
			};
		}

		private byte[] GetWriteParity(byte iParamNo, int iValue, byte iDevAdd)
		{
			int num = (int)iParamNo * 256 + 67 + iValue + (int)iDevAdd;
			return new byte[]
			{
				(byte)(num % 256),
				(byte)(num / 256)
			};
		}

		private bool SendAndReceive(byte[] sendByte, ref byte[] response)
		{
			this.InteractiveLock.Enter();
			bool result;
			try
			{
				this.MyCom.Write(sendByte, 0, sendByte.Length);
				byte[] array = new byte[1024];
				MemoryStream memoryStream = new MemoryStream();
				DateTime now = DateTime.Now;
				while (true)
				{
					Thread.Sleep(base.SleepTime);
					if (this.MyCom.BytesToRead >= 1)
					{
						int count = this.MyCom.Read(array, 0, array.Length);
						memoryStream.Write(array, 0, count);
					}
					else
					{
						if ((DateTime.Now - now).TotalMilliseconds > (double)base.ReceiveTimeOut)
						{
							goto IL_A2;
						}
						if (memoryStream.Length > 0L)
						{
							break;
						}
					}
				}
                response = memoryStream.ToArray();
				memoryStream.Dispose();
				return true;
				IL_A2:
				memoryStream.Dispose();
				result = false;
			}
			catch (Exception)
			{
				result = false;
			}
			finally
			{
				this.InteractiveLock.Leave();
			}
			return result;
		}
	}
}
