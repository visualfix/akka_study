using Akka;
using Akka.Actor;

namespace Actors
{
    public class WorkerActor : ReceiveActor
    {
        public WorkerActor()
        {
            Receive<DoWork>(msg =>
            {
                //Thread.Sleep(3000);
                System.Console.WriteLine($"{Self.Path.Name} get work.");
                Sender.Tell(new WorkDone(Self.Path.Name), Self);
            });

            Receive<GiveMoney>(msg =>
            {
                System.Console.WriteLine($"{Self.Path.Name} : get money!");
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<WorkerActor>();
        }
    }
}