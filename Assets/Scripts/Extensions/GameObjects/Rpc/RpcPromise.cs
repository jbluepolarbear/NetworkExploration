using System;
using UnityEngine;

namespace Extensions.GameObjects.Rpc
{
    public class RpcError
    {
        public uint Code;
        public string Reason;
    }
    
    public class RpcPromise : CustomYieldInstruction, IDisposable
    {
        public RpcPromise(uint messageId)
        {
            MessageId = messageId;
            Fulfilled = false;
            Value = default;
        }
        
        public uint MessageId { get; }
        public bool Fulfilled { get; protected set; }
        public object Value { get; protected set; }
        public RpcError Error { get; protected set; }

        public void Fulfill(object value = null)
        {
            Value = value;
            Fulfilled = true;
        }

        public void FillError(RpcError error)
        {
            Error = error;
            Fulfilled = true;
        }

        public override bool keepWaiting => !Fulfilled && Error == null;

        public void Dispose()
        {
        }
    }

    public class RpcPromise<T> : RpcPromise
    {
        public RpcPromise(uint messageId) : base(messageId)
        {
        }

        public void Fulfill(T value)
        {
            Value = value;
            Fulfilled = true;
        }
        
        public T GetValue() => (T) Value;

        public Type GetValueType => typeof(T);
    }
}