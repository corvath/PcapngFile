﻿/*
Copyright (c) 2013, Andrew Walsh
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the <organization>.
4. Neither the name of the <organization> nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY <COPYRIGHT HOLDER> ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace PcapngFile
{
	public class SectionHeaderBlock : BlockBase
	{		
		private const UInt16 HardwareOptionCode = 2;
		private const UInt16 OperatingSystemOptionCode = 3;
		private const UInt16 UserApplicationOptionCode = 4;

		public UInt32 ByteOrderMagic { get; private set; }		
		public string Hardware { get; private set; }
		public UInt16 MajorVersion { get; private set; }
		public UInt16 MinorVersion { get; private set; }
		public string OperatingSystem { get; private set; }
		public Int64 SectionLength { get; private set; }
		public string UserApplication { get; private set; }

		internal SectionHeaderBlock(BinaryReader reader)
			: base(reader)
		{
			this.ByteOrderMagic = reader.ReadUInt32();
			this.MajorVersion = reader.ReadUInt16();
			this.MinorVersion = reader.ReadUInt16();
			this.SectionLength = reader.ReadInt64();
			this.ReadOptions(reader);			
			this.ReadClosingField(reader);
		}

		override protected void OnReadOptionsCode(UInt16 code, byte[] value)
		{
			switch (code)
			{				
				case HardwareOptionCode:
					this.Hardware = UTF8Encoding.UTF8.GetString(value);
					break;
				case OperatingSystemOptionCode:
					this.OperatingSystem = UTF8Encoding.UTF8.GetString(value);
					break;
				case UserApplicationOptionCode:
					this.UserApplication = UTF8Encoding.UTF8.GetString(value);
					break;
			}
		}
	}
}
