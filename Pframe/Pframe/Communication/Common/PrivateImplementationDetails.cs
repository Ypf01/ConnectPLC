﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pframe
{
    [CompilerGenerated]
    internal sealed class PrivateImplementationDetails
    {
        internal static uint ComputeStringHash(string s)
        {
            uint num = 0;
            if (s != null)
            {
                num = 2166136261U;
                for (int i = 0; i < s.Length; i++)
                    num = ((uint)s[i] ^ num) * 16777619U;
            }
            return num;
        }
    }
}
