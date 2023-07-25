using Akka;
using Akka.Actor;

namespace Actors
{
    public class TellManagerActor : ReceiveActor
    {
        public TellManagerActor()
        {
            Receive<WorkDone>(msg =>
            {
                Sender.Tell(new Salary());
            });

            Receive<string>(msg =>
            {
                System.Console.WriteLine($"{Self.Path.Name} : {msg}");
            });  
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<TellManagerActor>();
        }
    }
}