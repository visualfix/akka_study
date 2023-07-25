using System;
using Akka;
using Akka.Actor;

namespace Actors
{
    public class ChildActor : ReceiveActor
    {
        public ChildActor()
        {
            System.Console.WriteLine("Call Constructor");

            Receive<string>(s => s.Equals("kill"), msg =>
            {
                System.Console.WriteLine("kek!");
                throw new Exception("kek!");
                return;
            });
        }

        protected override void PreStart()
        {
            System.Console.WriteLine("PreStart");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            System.Console.WriteLine($"PreRestart \n reason : {reason} \n message : {message}");

            foreach (IActorRef each in Context.GetChildren())
            {
                Context.Unwatch(each);
                Context.Stop(each);
            }
            PostStop();
        }

        protected override void PostRestart(Exception reason)
        {
            System.Console.WriteLine($"PostRestart {reason}");

            PreStart();
        }

        protected override void PostStop()
        {
            System.Console.WriteLine("PostStop");
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<ChildActor>();
        }
    }
}