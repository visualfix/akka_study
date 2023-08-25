using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class SActor: ReceiveActor
  {
    public SActor()
    {
      Receive<string>(message => {
        System.Console.WriteLine($"{Sender} : {message}");
      });      
    }

    public SActor(IActorRef receiver, string firstMsg)
    : this()
    {
      receiver.Tell(firstMsg);
    }

    public static  Props Props()
    {
      return Akka.Actor.Props.Create<SActor>();
    }

    public static Props Props(IActorRef receiver, string firstMsg)
    {
      return Akka.Actor.Props.Create<SActor>(receiver, firstMsg);
    }
  }
}