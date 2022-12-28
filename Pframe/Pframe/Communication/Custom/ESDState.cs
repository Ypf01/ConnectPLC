using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.Custom
{
	public class ESDState
	{
		public bool ESDError { get; set; }

		public string ESDFaultDescrib { get; set; }

		public float ESDCurrent { get; set; }

		public float ESDVoltage { get; set; }

		public int ESDMode { get; set; }

		public bool ESDFault { get; set; }

		public bool ESDEnable { get; set; }

	}
}
