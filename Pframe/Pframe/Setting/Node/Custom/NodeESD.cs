using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pframe;
using Pframe.Common;
using Pframe.Custom;
using Pframe.DataConvert;
using NodeSettings.Node.Group;
using NodeSettings.Node.NodeBase;
using NodeSettings.Node.Variable;

namespace NodeSettings.Node.Custom
{
    /// <summary>
    /// ESD节点
    /// </summary>
	public class NodeESD : CustomNode, IXmlConvert
	{
		public NodeESD()
		{
			this.sw = new Stopwatch();
			this.CustomGroupList = new List<ESDGroup>();
			base.Name = "ESD电源";
			base.Description = "SC200引出区电源";
			base.CustomType = 100000;
			this.IpAddress = "192.168.0.3";
			this.Port = 4002;
			this.ESDType = "Glassman";
            FirstConnect = false;
		}
        /// <summary>
        /// 第一次连接
        /// </summary>
		public bool FirstConnect { get; set; }
        /// <summary>
        /// 通信周期
        /// </summary>
		public long CommRate { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
		public string IpAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
		public int Port { get; set; }
        /// <summary>
        /// ESD类型
        /// </summary>
		public string ESDType { get; set; }
        /// <summary>
        /// 最大电压
        /// </summary>
		public float MaxVoltage { get; set; }
        /// <summary>
        /// 最大电流
        /// </summary>
		public float MaxCurrent { get; set; }
        /// <summary>
        /// 连接状态
        /// </summary>
		public bool IsConnected { get; set; }
        /// <summary>
        /// 加载Xml元素
        /// </summary>
        /// <param name="element"></param>
		public override void LoadByXmlElement(XElement element)
		{
			base.LoadByXmlElement(element);
			this.IpAddress = element.Attribute("IpAddress").Value;
			this.Port = int.Parse(element.Attribute("Port").Value);
			this.ESDType = element.Attribute("ESDType").Value;
			this.MaxVoltage = float.Parse(element.Attribute("MaxVoltage").Value);
			this.MaxCurrent = float.Parse(element.Attribute("MaxCurrent").Value);
		}
        /// <summary>
        /// 获取Xml元素
        /// </summary>
        /// <returns></returns>
		public override XElement ToXmlElement()
		{
			XElement xelement = base.ToXmlElement();
			xelement.SetAttributeValue("IpAddress", this.IpAddress);
			xelement.SetAttributeValue("Port", this.Port);
			xelement.SetAttributeValue("ESDType", this.ESDType);
			xelement.SetAttributeValue("MaxVoltage", this.MaxVoltage);
			xelement.SetAttributeValue("MaxCurrent", this.MaxCurrent);
			return xelement;
		}
        /// <summary>
        /// 获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns></returns>
		public override List<NodeClassRenderItem> GetNodeClassRenders()
		{
			List<NodeClassRenderItem> nodeClassRenders = base.GetNodeClassRenders();
			nodeClassRenders.Add(new NodeClassRenderItem("IP地址", this.IpAddress));
			nodeClassRenders.Add(new NodeClassRenderItem("端口号", this.Port.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("ESD电源型号", this.ESDType));
			nodeClassRenders.Add(new NodeClassRenderItem("额定电压", this.MaxVoltage.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("额定电流", this.MaxCurrent.ToString()));
			nodeClassRenders.Add(new NodeClassRenderItem("激活情况", base.IsActive ? "已激活" : "未激活"));
			nodeClassRenders.Add(new NodeClassRenderItem("连接情况", this.IsConnected ? "已连接" : "未连接"));
			return nodeClassRenders;
		}
        /// <summary>
        /// 开启线程
        /// </summary>
		public void Start()
		{
			foreach (ESDGroup esdgroup in this.CustomGroupList)
			{
				foreach (ESDVariable esdvariable in esdgroup.varList)
				{
					if (esdvariable.Config.ArchiveEnable)
					{
						this.StoreVarList.Add(esdvariable);
					}
					if (this.CurrentVarList.ContainsKey(esdvariable.KeyName))
					{
						this.CurrentVarList[esdvariable.KeyName] = esdvariable;
					}
					else
					{
						this.CurrentVarList.Add(esdvariable.KeyName, esdvariable);
					}
				}
			}
			this.cts = new CancellationTokenSource();
			Task.Run(new Action(this.GetValue), this.cts.Token);
		}
        /// <summary>
        /// 停止线程
        /// </summary>
		public void Stop()
		{
			this.cts.Cancel();
		}

		private void GetValue()
		{
			this.esd = new ESD(this.MaxVoltage, this.MaxCurrent);
			while (!this.cts.IsCancellationRequested)
			{
				if (this.IsConnected)
				{
					ESDState currentStatus = this.esd.GetCurrentStatus();
					if (currentStatus != null)
					{
						using (List<ESDGroup>.Enumerator enumerator = this.CustomGroupList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ESDGroup esdgroup = enumerator.Current;
								foreach (ESDVariable esdvariable in esdgroup.varList)
								{
									string varAddress = esdvariable.VarAddress;
									string text = varAddress;
									uint num = PrivateImplementationDetails.ComputeStringHash(text);
									if (num <= 2142027009U)
									{
										if (num <= 1149362035U)
										{
											if (num != 973015057U)
											{
												if (num == 1149362035U)
												{
													if (text == "ESDCurrentSetBack")
													{
														esdvariable.Value = this.esd.CurrentSetBack;
													}
												}
											}
											else if (text == "ESDFault")
											{
												esdvariable.Value = currentStatus.ESDFault;
											}
										}
										else if (num != 1376256928U)
										{
											if (num == 2142027009U)
											{
												if (text == "ESDFaultDescrib")
												{
													esdvariable.Value = currentStatus.ESDFaultDescrib;
												}
											}
										}
										else if (text == "ESDEnable")
										{
											esdvariable.Value = currentStatus.ESDEnable;
										}
									}
									else if (num <= 2565201360U)
									{
										if (num != 2518482280U)
										{
											if (num == 2565201360U)
											{
												if (text == "ESDMode")
												{
													esdvariable.Value = currentStatus.ESDMode;
												}
											}
										}
										else if (text == "ESDCurrent")
										{
											esdvariable.Value = ((!(esdvariable.Scale == "1") || !(esdvariable.Offset == "0")) ? (Convert.ToSingle(currentStatus.ESDCurrent) * Convert.ToSingle(esdvariable.Scale) + Convert.ToSingle(esdvariable.Offset)) : esdvariable.Value);
										}
									}
									else if (num != 2834389002U)
									{
										if (num != 3296761831U)
										{
											if (num == 3945184775U)
											{
												if (text == "ESDError")
												{
													esdvariable.Value = currentStatus.ESDError;
												}
											}
										}
										else if (text == "ESDVoltage")
										{
											esdvariable.Value = ((!(esdvariable.Scale == "1") || !(esdvariable.Offset == "0")) ? (Convert.ToSingle(currentStatus.ESDVoltage) * Convert.ToSingle(esdvariable.Scale) + Convert.ToSingle(esdvariable.Offset)) : esdvariable.Value);
										}
									}
									else if (text == "ESDVoltageSetBack")
									{
										esdvariable.Value = this.esd.VoltageSetBack;
									}
									esdvariable.Value = MigrationLib.GetMigrationValue(esdvariable.Value, esdvariable.Scale, esdvariable.Offset);
									base.UpdateCurrentValue(esdvariable);
								}
							}
							continue;
						}
					}
					this.IsConnected = false;
				}
				else
				{
					if (!this.FirstConnect)
					{
						Thread.Sleep(base.ReConnectTime);
						ESD esd = this.esd;
						if (esd != null)
							esd.DisConnect();
					}
					this.IsConnected = this.esd.Connect(this.IpAddress, this.Port);
				}
			}
		}
        /// <summary>
        /// 带有错误信息的返回值
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
		public CalResult Write(string keyName, string setValue)
		{
			CalResult result;
			if (!this.CurrentVarList.ContainsKey(keyName))
			{
				result = new CalResult
				{
					IsSuccess = false,
					Message = "无法通过变量名称获取到变量"
				};
			}
			else
			{
				ESDVariable esdvariable = this.CurrentVarList[keyName] as ESDVariable;
				CalResult<string> xktResult = Common.VerifyInputValue(esdvariable, esdvariable.VarType, setValue);
				if (xktResult.IsSuccess)
				{
					string varAddress = esdvariable.VarAddress;
					string a = varAddress;
					if (!(a == "ESDVoltageSet"))
					{
						if (!(a == "ESDCurrentSet"))
						{
							if (!(a == "ESDHVON"))
							{
								if (!(a == "ESDHVOFF"))
								{
									if (!(a == "ESDHVRESET"))
									{
										result = CalResult.CreateFailedResult();
									}
									else
									{
										this.esd.HVRESET = (xktResult.Content == "1" || xktResult.Content == "true");
										result = CalResult.CreateSuccessResult();
									}
								}
								else
								{
									this.esd.HVOFF = (xktResult.Content == "1" || xktResult.Content == "true");
									result = CalResult.CreateSuccessResult();
								}
							}
							else
							{
								this.esd.HVON = (xktResult.Content == "1" || xktResult.Content == "true");
								result = CalResult.CreateSuccessResult();
							}
						}
						else
						{
							this.esd.CurrentSet = float.Parse(xktResult.Content);
							result = CalResult.CreateSuccessResult();
						}
					}
					else
					{
						this.esd.VoltageSet = float.Parse(xktResult.Content);
						result = CalResult.CreateSuccessResult();
					}
				}
				else
				{
					result = xktResult;
				}
			}
			return result;
		}
        /// <summary>
        /// 定义一个信号源
        /// </summary>
		public CancellationTokenSource cts;
        /// <summary>
        /// 计时器
        /// </summary>
		public Stopwatch sw;
        /// <summary>
        /// GM类型
        /// </summary>
		public const string GMType = "Glassman";
        /// <summary>
        /// 通信对象
        /// </summary>
		public ESD esd;
        /// <summary>
        ///  自定义组集合
        /// </summary>
		public List<ESDGroup> CustomGroupList;
	}
}
