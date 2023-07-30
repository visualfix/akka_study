using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;

namespace Actors
{
    public class TellTestActor : ReceiveActor
    {
        IActorRef manager;

        public TellTestActor()
        {
            manager = Context.ActorOf(TellManagerActor.Props(), "TELL-Manager");

            Receive<string>(c => c.Equals("Execute"), msg =>
            {
                System.Console.WriteLine($"{Self.Path.Name} : {msg}");
                Execute();
            });
            
            Receive<WorkDone>(msg =>
            {
                manager.Tell(msg, Sender);
                //manager.Forward(msg);
            });
        }

        private void Execute()
        {
            var worker1 = Context.ActorOf(WorkerActor.Props(), "TELL-Wroker1");
            var worker2 = Context.ActorOf(WorkerActor.Props(), "TELL-Wroker2");
            
            worker1.Tell(new DoWork());
            worker2.Tell(new DoWork());
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<TellTestActor>();
        }
    }
}