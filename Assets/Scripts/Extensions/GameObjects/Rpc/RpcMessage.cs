﻿using NetLib.Utility;
using Unity.Netcode;

namespace Extensions.GameObjects.Rpc
{
    public class RpcMessage : INetworkSerializable
    {
        public object[] Args;
        public RpcError Error;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                var writer = serializer.GetFastBufferWriter();
                if (Error != null)
                {
                    writer.WriteValueSafe(true);
                    writer.WriteValueSafe(Error.Code);
                    writer.WriteValueSafe(Error.Reason);
                    return;
                }
                writer.WriteValueSafe(false);
                Assert.NotNull(Args);
                writer.WriteValueSafe(Args.Length);
                foreach (var arg in Args)
                {
                    Serialization.SerializeField(arg, serializer);
                }
            }
            else
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out bool hasError);
                if (hasError)
                {
                    reader.ReadValueSafe(out uint code);
                    reader.ReadValueSafe(out string reason);
                    Error = new RpcError
                    {
                        Code = code,
                        Reason = reason
                    };
                    return;
                }
                reader.ReadValueSafe(out int length);
                Args = new object[length];
                for (var i = 0; i < length; ++i)
                {
                    Args[i] = Serialization.DeserializeField(serializer);
                }
            }
        }
    }
}