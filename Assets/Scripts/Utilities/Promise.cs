using System;
using UnityEngine;

namespace Utilities
{
    public enum PromiseErrorCodes
    {
        BadArgument,
        NotAllowed,
    }
    
    public class PromiseError
    {
        public PromiseErrorCodes Code;
        public string Reason;
        public Exception Exception;
    }
    
    public class Promise: CustomYieldInstruction
    {
        public bool Fulfilled { get; protected set; }
        public object Value { get; protected set; }
        public PromiseError Error { get; protected set; }
        public virtual Type GetValueType => Value?.GetType();

        public void Fulfill(object value = null)
        {
            Value = value;
            Fulfilled = true;
        }

        public void FillError(PromiseError error)
        {
            Error = error;
            Fulfilled = true;
        }

        public override bool keepWaiting => !Fulfilled && Error == null;
    }

    public class Promise<T> : Promise
    {
        public Promise() : base()
        {
        }

        public void Fulfill(T value)
        {
            Value = value;
            Fulfilled = true;
        }
        
        public T GetValue() => (T) Value;

        public override Type GetValueType => typeof(T);
    }
}