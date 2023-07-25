using Akka;
using Akka.Actor;

namespace Actors
{
    public class AskManagerActor : ReceiveActor
    {
        public AskManagerActor()
        {
            Receive<WorkDone[]>(msgs =>
            {
                foreach(var msg in msgs)
                {
                    System.Console.WriteLine($"{Sender.Path.Name} - {msg.Name} : Done");
                }
            }); 

            Receive<string>( msg =>
            {
                System.Console.WriteLine($"{Self.Path.Name} : {msg}");
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<AskManagerActor>();
        }
    }
}