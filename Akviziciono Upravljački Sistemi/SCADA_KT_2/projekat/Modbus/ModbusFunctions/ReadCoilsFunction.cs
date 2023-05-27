using Common;
using Modbus.FunctionParameters;
using Modbus.PacketFunctions.Implementations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    public class ReadCoilsFunction : ModbusFunction
    {
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }
        public override byte[] PackRequest()
        {
            PreparePacketRequest request = new PreparePacketRequest();
            return request.PreparePacket((ModbusReadCommandParameters) CommandParameters);
        }
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusReadCommandParameters mrcp = CommandParameters as ModbusReadCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();
            if (response[7] == CommandParameters.FunctionCode + 0x80)
            {
                HandleException(response[8]);
            }
            else
            {
                ushort vrednost;

                int brojac = 0;
                for (int i = 0; i < response[8]; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (mrcp.Quantity == brojac)
                        {
                            break;
                        }

                        vrednost = (ushort)(response[9 + i] & 1);
                        response[9 + i] >>= 1;
                        dic.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, (ushort)(mrcp.StartAddress + brojac)), vrednost);
                        brojac++;
                    }
                }
            }
            return dic;
        }
    }
}