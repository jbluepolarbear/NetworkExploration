using Unity.Netcode;
using UnityEngine;

namespace Input
{
    public class InputState : INetworkSerializable
    {
        public Vector2 LeftAxis { get; set; }
        public bool Jump { get; set; }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(LeftAxis.x);
                writer.WriteValueSafe(LeftAxis.y);
                writer.WriteValueSafe(Jump);
            }
            else
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out float x);
                reader.ReadValueSafe(out float y);
                LeftAxis = new Vector2(x, y);
                reader.ReadValueSafe(out bool jump);
                Jump = jump;
            }
        }
    }
}