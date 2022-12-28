using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.Custom
{
	public class VDState
	{
        //ARCMinus
        public bool ARCMinus { get; set; }
        //ARCPlus
        public bool ARCPlus { get; set; }
        //DCPower
        public bool DCPower { get; set; }
        //Dooropen
        public bool Dooropen { get; set; }
        //Externalerror1
        public bool Externalerror1 { get; set; }
        //Externalerror2
        public bool Externalerror2 { get; set; }
        //HVSocket
        public bool HVSocket { get; set; }
        //Inrush
        public bool Inrush { get; set; }
        //NegativeVoltage
        public float NegativeVoltage { get; set; }
        //OutCurrent
        public float OutCurrent { get; set; }
        //OutVoltage
        public float OutVoltage { get; set; }
        //OverPower
        public bool OverPower { get; set; }
        //OverVoltage
        public bool OverVoltage { get; set; }
        //Phaseerror
        public bool Phaseerror { get; set; }
        //PositiveVoltage
        public float PositiveVoltage { get; set; }
        //PowerPolarity
        public bool PowerPolarity { get; set; }
        //PowerStatus
        public bool PowerStatus { get; set; }
        //Regulationerror
        public bool Regulationerror { get; set; }
        //Remotelocal
        public bool Remotelocal { get; set; }
        //TemperatureModule1
        public bool TemperatureModule1 { get; set; }
    }
}
