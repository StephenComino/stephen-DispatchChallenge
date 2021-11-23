using System;

namespace DispatchChallenge
{
    public class DispatchAttribute : Attribute
    {
        public DispatchAttribute(Type receiverType)
        {
            this.ReceiverType = receiverType;
        }

        public Type ReceiverType { get; private set; }
    }
}
