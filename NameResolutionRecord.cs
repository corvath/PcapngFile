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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace PcapngFile
{
	public class NameResolutionRecord
	{
		private const UInt16 EndRecordType = 0;
		private const UInt16 Ip4RecordType = 1;
		private const UInt16 Ip6RecordType = 2;

		private const int Ip4Length = 4;
		private const int Ip6Length = 8;
		private const int ValueAlignmentBoundary = 4;

		public bool IsIpVersion6 { get; private set; }
		public byte[] IpAddress { get; private set; }
		public ReadOnlyCollection<string> DnsEntries { get; private set; }

		internal NameResolutionRecord(BinaryReader reader)
		{
			int entriesLength;
			int remainderLength;
			UInt16 type = reader.ReadUInt16();
			int valueLength = reader.ReadUInt16();

			if (type != EndRecordType)
			{
				entriesLength = valueLength;

				if (type == Ip6RecordType)
				{
					this.IsIpVersion6 = true;
					this.IpAddress = reader.ReadBytes(Ip6Length);
					entriesLength -= Ip6Length;
				}
				else
				{
					this.IsIpVersion6 = false;
					this.IpAddress = reader.ReadBytes(Ip4Length);
					entriesLength -= Ip4Length;
				}
				this.DnsEntries = this.ReadDnsEntries(reader, entriesLength);

				remainderLength = valueLength % ValueAlignmentBoundary;
				if (remainderLength > 0)
				{
					reader.ReadBytes(ValueAlignmentBoundary - remainderLength);		// Read fill bytes to boundary.
				}
			}
		}

		private ReadOnlyCollection<string> ReadDnsEntries(BinaryReader reader, int entriesLength)
		{
			var entries = new List<string>();
			byte[] entriesBuffer = reader.ReadBytes(entriesLength);
			int i = 0, j = 0;
			while (j < entriesLength)
			{
				if (entriesBuffer[j] == 0)
				{
					entries.Add(UTF8Encoding.UTF8.GetString(entriesBuffer, i, j - i));
					i = ++j;
				}
				else
				{
					j++;
				}
			}
			return new ReadOnlyCollection<string>(entries);
		}
	}
}
