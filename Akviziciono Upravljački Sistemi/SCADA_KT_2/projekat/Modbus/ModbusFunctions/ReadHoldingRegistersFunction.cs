using Common;
using Modbus.FunctionParameters;
using Modbus.PacketFunctions.Implementations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }
        public override byte[] PackRequest()
        {
            PreparePacketRequest request = new PreparePacketRequest();
            return request.PreparePacket((ModbusReadCommandParameters)CommandParameters);
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

                ushort b1 = 7;
                ushort b2 = 8;
                for (int i = 0; i < response[8] / 2; i++)
                {
                    vrednost = (ushort)(response[b2 += 2] + (response[b1 += 2] << 8));
                    dic.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)(mrcp.StartAddress + i)), vrednost);
                }

            }
            return dic;
        }
    }
}