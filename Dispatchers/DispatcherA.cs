using System;

namespace DispatchChallenge.Dispatchers
{
    [Dispatch(typeof(TestDataReceiverMock))]
    public class DispatcherA : DispatcherBase
    {

        // handler for each type of message - naming conventions "Handle_XXX" where XXX is the message code
        public void Handle_Init(IMessage message)
        {

        }

        public void Handle_Open(IMessage message)
        {

        }

        public void Handle_Close(IMessage message)
        {

        }

        public override void DefaultAction(IMessage message)
        {
            // override for action that will be called if message code is not matched with any existing handler method
            Console.WriteLine("This is A");
        }

    }
}
