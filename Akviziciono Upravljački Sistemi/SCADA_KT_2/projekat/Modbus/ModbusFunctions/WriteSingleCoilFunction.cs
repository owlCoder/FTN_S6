using Common;
using Modbus.FunctionParameters;
using Modbus.PacketFunctions.Implementations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    public class WriteSingleCoilFunction : ModbusFunction
    {
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
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
            ushort vrednost = (ushort)(response[11] & 1);

            dic.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, adresa), vrednost);

            return dic;
        }
    }
}