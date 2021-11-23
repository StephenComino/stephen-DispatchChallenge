using System;
using System.Threading;

namespace DispatchChallenge
{
    public interface IDataReceiver
    {
        void Init(CancellationToken token);

        event EventHandler<IMessage> Received;
    }
}
