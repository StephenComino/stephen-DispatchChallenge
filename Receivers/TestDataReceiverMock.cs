using System;
using System.Threading;

namespace DispatchChallenge
{
    public class TestDataReceiverMock :IDataReceiver
    {
        private const int MaxReceivedMessages = 1000;
        private int _cycleCount = -1;

        // test data
        private Message[] TestMessages => new Message[]  {
               new Message("Open",""),
               new Message("Close",""),
               new Message("Init","{ \"param1:\":\"value1\"}"),
               new Message("Send","{ \"text:\":\"text_to_send\"}"),
        };

        public event EventHandler<IMessage> Received;
        
        public void Init(CancellationToken token = default)
        {
            while (TestMessages.Length > 0 && _cycleCount < MaxReceivedMessages)
            {
                foreach (var m in TestMessages)
                {
                    token.ThrowIfCancellationRequested();
                    
                    _cycleCount++;

                    // Emulate reception of messages
                    Received?.Invoke(this, m);

                    // breather 
                    Thread.Sleep(500);
                }
            }
        }
    }
}
