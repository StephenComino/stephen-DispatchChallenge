using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DispatchChallenge.Dispatchers;

namespace DispatchChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            MainLogic.Instance.RegisterReceivers(new[] { new TestDataReceiverMock() });

             MainLogic.Instance.StartListening();
            
        }
    }

    public class MainLogic
    {
        private static MainLogic _instance;

        public static MainLogic Instance => _instance ?? (_instance = new MainLogic());

        private Dictionary<Type, DispatcherBase> _dispatchers = new Dictionary<Type, DispatcherBase>();
        private IDataReceiver[] _receivers;
        
        public void RegisterReceivers(IDataReceiver[] receivers)
        {
            _receivers = receivers;
        }

        public void StartListening()
        {

            var ts = new CancellationTokenSource();

            SetUpLogic();

            Console.WriteLine("Listening, press '0' to cancel...");
            if (Console.ReadKey().KeyChar == '0')
            {
                ts.Cancel();
            }

            try
            {
                Task.WaitAll(
                    ((_receivers ?? Array.Empty<IDataReceiver>())
                        .Select(r => Task.Run(
                            () => r.Init(ts.Token), ts.Token))).ToArray(),
                    ts.Token);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Listening was cancelled");
            }
            finally
            {
                ts.Dispose();
            }

            Console.WriteLine("Main logic execution was completed");
        }

        private void SetUpLogic()
        {
            // Add required logic that will set up dispatchers based on attributes
            // dispatcher should only register to events of relevant listener as specificed in the attribute
            DispatcherA dipA = new DispatcherA();
            var testDataReceiver = Assembly.GetExecutingAssembly().GetTypes().Single(q => q.Name == "TestDataReceiverMock");
            // Dispatcher register to received function
            var dispatchers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute(typeof(DispatchAttribute)) != null);
            // and one of ways to get individual attribute (not the only one!)
            foreach (var t in dispatchers)
            {
                Console.WriteLine(t.Name);
                var methods = t.GetRuntimeMethods();

                var args = t.GetRuntimeMethod("OnReceivedMessage", new[] {typeof(DispatcherBase)}) ??
                           dipA.GetType().GetMethod("OnReceivedMessage");

                var received = testDataReceiver.GetRuntimeEvent("Received");
                Delegate handler =
                    Delegate.CreateDelegate(received.EventHandlerType,
                        null,
                        args);
                foreach (var message in Instance._receivers)
                {
                    received.AddEventHandler(message, handler);
                }
            }
        }
    }
}
