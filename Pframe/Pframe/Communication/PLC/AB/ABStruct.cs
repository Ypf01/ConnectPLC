using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pframe.PLC.AB
{
	public class ABStruct
	{
		public ushort Count { get; set; }
        
		public uint TemplateObjectDefinitionSize { get; set; }
        
		public uint TemplateStructureSize { get; set; }
        
		public ushort MemberCount { get; set; }
        
		public ushort StructureHandle { get; set; }
	}
}
