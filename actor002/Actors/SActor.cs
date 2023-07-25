using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class SActor: ReceiveActor
  {
    private readonly ILoggingAdapter log = Context.GetLogger();

    public SActor()
    {
      System.Console.WriteLine($"{this} - msg handler registered");
      Receive<string>(message => {
        //log.Info($"Received String message: {message} from {Sender}");
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