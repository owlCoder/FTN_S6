using Common;
using Modbus.FunctionParameters;
using Modbus.PacketFunctions.Implementations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }
        public override byte[] PackRequest()
        {
            PreparePacketRequest request = new PreparePacketRequest();
            return request.PreparePacket((ModbusWriteCommandParameters)CommandParameters);
        }
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort adresa = BitConverter.ToUInt16(response, 8);
            adresa = (ushort)IPAddress.NetworkToHostOrder((short)adresa);
            ushort vrednost = BitConverter.ToUInt16(response, 10);
            vrednost = (ushort)IPAddress.NetworkToHostOrder((short)vrednost);

            dic.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, adresa), vrednost);

            return dic;
        }
    }
}