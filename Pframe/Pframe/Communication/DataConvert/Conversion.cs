using System;

namespace Pframe.DataConvert
{
	public static class Conversion
	{
		public static int BinStringToInt(string txt)
		{
			int num = 0;
			for (int i = txt.Length - 1; i >= 0; i += -1)
			{
				if (int.Parse(txt.Substring(i, 1)) == 1)
				{
					num += (int)Math.Pow(2.0, (double)(txt.Length - 1 - i));
				}
			}
			return num;
		}

		public static byte? BinStringToByte(string txt)
		{
			int num = 0;
			byte? result;
			if (txt.Length == 8)
			{
				for (int i = 7; i >= 0; i += -1)
				{
					if (int.Parse(txt.Substring(i, 1)) == 1)
					{
						num += (int)Math.Pow(2.0, (double)(txt.Length - 1 - i));
					}
				}
				result = new byte?((byte)num);
			}
			else
			{
				result = null;
			}
			return result;
		}
        
		public static string ValToBinString(object value)
		{
			string text = "";
			string result;
			try
			{
				if (value.GetType().Name.IndexOf("[]") < 0)
				{
					string name = value.GetType().Name;
					string a = name;
					int num;
					long num2;
					if (!(a == "Byte"))
					{
						if (!(a == "Int16"))
						{
							if (!(a == "Int32"))
							{
								if (!(a == "Int64"))
								{
									throw new Exception();
								}
								num = 63;
								num2 = (long)value;
							}
							else
							{
								num = 31;
								num2 = (long)((int)value);
							}
						}
						else
						{
							num = 15;
							num2 = (long)((short)value);
						}
					}
					else
					{
						num = 7;
						num2 = (long)((ulong)((byte)value));
					}
					for (int i = num; i >= 0; i += -1)
					{
						if ((num2 & (long)Math.Pow(2.0, (double)i)) > 0L)
						{
							text += "1";
						}
						else
						{
							text += "0";
						}
					}
				}
				else
				{
					string name2 = value.GetType().Name;
					string a2 = name2;
					if (!(a2 == "Byte[]"))
					{
						if (!(a2 == "Int16[]"))
						{
							if (!(a2 == "Int32[]"))
							{
								if (!(a2 == "Int64[]"))
								{
									throw new Exception();
								}
								int num = 63;
								byte[] array = (byte[])value;
								for (int j = 0; j <= array.Length - 1; j++)
								{
									for (int i = num; i >= 0; i += -1)
									{
										if ((array[j] & (byte)Math.Pow(2.0, (double)i)) > 0)
										{
											text += "1";
										}
										else
										{
											text += "0";
										}
									}
								}
							}
							else
							{
								int num = 31;
								int[] array2 = (int[])value;
								for (int j = 0; j <= array2.Length - 1; j++)
								{
									for (int i = num; i >= 0; i += -1)
									{
										if ((array2[j] & (int)((byte)Math.Pow(2.0, (double)i))) > 0)
										{
											text += "1";
										}
										else
										{
											text += "0";
										}
									}
								}
							}
						}
						else
						{
							int num = 15;
							short[] array3 = (short[])value;
							for (int j = 0; j <= array3.Length - 1; j++)
							{
								for (int i = num; i >= 0; i += -1)
								{
									if ((array3[j] & (short)((byte)Math.Pow(2.0, (double)i))) > 0)
									{
										text += "1";
									}
									else
									{
										text += "0";
									}
								}
							}
						}
					}
					else
					{
						int num = 7;
						byte[] array4 = (byte[])value;
						for (int j = 0; j <= array4.Length - 1; j++)
						{
							for (int i = num; i >= 0; i += -1)
							{
								if ((array4[j] & (byte)Math.Pow(2.0, (double)i)) > 0)
								{
									text += "1";
								}
								else
								{
									text += "0";
								}
							}
						}
					}
				}
				result = text;
			}
			catch
			{
				result = "";
			}
			return result;
		}
	}
}
