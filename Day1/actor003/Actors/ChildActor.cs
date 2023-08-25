using System;
using Akka;
using Akka.Actor;

namespace Actors
{
    public class ChildActor : ReceiveActor
    {
        public ChildActor()
        {
            Receive<string>(s => s.Equals("kill"), msg =>
            {
                Context.Stop(Self);
            });
        }

        protected override void PreStart()
        {
            System.Console.WriteLine($"{Self.Path.Name} - PreStart");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            System.Console.WriteLine($"{Self.Path.Name} - PreRestart \n reason : {reason} \n message : {message}");

            foreach (IActorRef each in Context.GetChildren())
            {
                Context.Unwatch(each);
                Context.Stop(each);
            }
            PostStop();
        }

        protected override void PostRestart(Exception reason)
        {
            System.Console.WriteLine($"{Self.Path.Name} - PostRestart {reason}");

            PreStart();
        }

        protected override void PostStop()
        {
            System.Console.WriteLine($"{Self.Path.Name} - PostStop");
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<ChildActor>();
        }
    }
}