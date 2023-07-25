using Akka;
using Akka.Actor;

namespace Actors
{
    public class Parent : ReceiveActor
    {
        public Parent()
        {
            Receive<string>(msg =>
            {
                System.Console.WriteLine($"{this.GetType().Name} : {msg}");
            });
        }
    }

    public class JackParent : Parent
    {
        public JackParent()
            :base()
        {
            int n = 3;
            while(n --> 0)
            {
                Context.ActorOf(JackChild.Props(n), $"JackChild{n}");
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<JackParent>();
        }
    }
    public class QueenParent : Parent
    {
        public QueenParent()
            :base()
        {
            int n = 3;
            while(n --> 0)
            {
                Context.ActorOf(QueenChild.Props(n), $"QueenChild{n}");
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<QueenParent>();
        }
    }
    public class KingParent : Parent
    {
        public KingParent()
            :base()
        {
            int n = 3;
            while(n --> 0)
            {
                Context.ActorOf(KingChild.Props(n), $"KingChild{n}");
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<KingParent>();
        }
    }
}