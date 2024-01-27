using System;

namespace Extensions.GameObjects.Rpc
{
    public abstract class RpcTargetAttribute : Attribute
    {
        public abstract uint Target { get; }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcTargetServerAttribute : RpcTargetAttribute
    {
        public override uint Target { get; }
        public RpcTargetServerAttribute(uint target)
        {
            Target = target;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcTargetClientAttribute : RpcTargetAttribute
    {
        public override uint Target { get; }
        public RpcTargetClientAttribute(uint target)
        {
            Target = target;
        }
    }
}