﻿namespace Modbus.FunctionParameters
{
	public abstract class ModbusCommandParameters
	{
		private ushort transactionId;
		private ushort protocolId;
		private ushort length;
		private byte unitId;
		private byte functionCode;
		public ModbusCommandParameters(ushort length, byte functionCode, ushort transactionId, byte unitId)
		{
            TransactionId = transactionId;
            UnitId = unitId;

            ProtocolId = 0;
			Length = length;
			FunctionCode = functionCode;
		}
		public ushort TransactionId
		{
			get
			{
				return transactionId;
			}

			private set
			{
				transactionId = value;
			}
		}
		public ushort ProtocolId
		{
			get
			{
				return protocolId;
			}

			private set
			{
				protocolId = value;
			}
		}
		public ushort Length
		{
			get
			{
				return length;
			}

			private set
			{
				length = value;
			}
		}
		public byte UnitId
		{
			get
			{
				return unitId;
			}

			private set
			{
				unitId = value;
			}
		}
		public byte FunctionCode
		{
			get
			{
				return functionCode;
			}

			private set
			{
				functionCode = value;
			}
		}
	}
}