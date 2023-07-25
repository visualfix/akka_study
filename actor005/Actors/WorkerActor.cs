using Akka;
using Akka.Actor;

namespace Actors
{
    public class WorkerActor : ReceiveActor
    {
        public WorkerActor()
        {
            Receive<WorkOrder>(msg =>
            {
                //Thread.Sleep(3000);
                System.Console.WriteLine($"{Self.Path.Name} get WorkOrder.");
                Sender.Tell(new WorkDone(Self.Path.Name), Self);
            });

            Receive<Salary>(msg =>
            {
                System.Console.WriteLine($"{Self.Path.Name} : I have got the salary!");
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<WorkerActor>();
        }
    }
}