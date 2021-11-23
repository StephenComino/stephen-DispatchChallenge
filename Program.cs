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
            
            DispatcherA dipA = new DispatcherA();
            
            var testDataReceivers = Assembly.GetExecutingAssembly().GetTypes().Where(q => q.Name == "TestDataReceiverMock");
            var dispatchers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute(typeof(DispatchAttribute)) != null);


            foreach (var t in dispatchers)
            {
                var args = t.GetRuntimeMethod("OnReceivedMessage", new[] {typeof(DispatcherBase)}) ?? dipA.GetType().GetMethod("OnReceivedMessage");

                foreach (var rec in testDataReceivers)
                {
                    var received = rec.GetRuntimeEvent("Received");
                    Delegate handler =
                        Delegate.CreateDelegate(received.EventHandlerType,
                            Activator.CreateInstance(t),
                            args);

                    foreach (var message in Instance._receivers)
                    {
                        received.AddEventHandler(message, handler);
                    }
                }
            }
        }
    }
}
