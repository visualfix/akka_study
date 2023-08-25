using Akka;
using Akka.Actor;

namespace Actors
{
    public class Child : ReceiveActor
    {
        public Child(int id)
        {
            Receive<string>(msg =>
            {
                System.Console.WriteLine($"{this.GetType().Name}{id} : {msg}");
            });
        }
    }

    public class JackChild : Child
    {
        public JackChild(int id)
            :base(id)
        {
        }

        public static Props Props(int id)
        {
            return Akka.Actor.Props.Create<JackChild>(id);
        }
    }
    public class QueenChild : Child
    {
        public QueenChild(int id)
            :base(id)
        {
        }

        public static Props Props(int id)
        {
            return Akka.Actor.Props.Create<QueenChild>(id);
        }
    }
    public class KingChild : Child
    {
        public KingChild(int id)
            :base(id)
        {
        }

        public static Props Props(int id)
        {
            return Akka.Actor.Props.Create<KingChild>(id);
        }
    }
}