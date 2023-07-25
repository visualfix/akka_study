using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class RActor: ReceiveActor
  {
    private readonly ILoggingAdapter log = Context.GetLogger();

    public RActor()
    {
      Receive<string>(message => {
        //log.Info($"Received String message: {message} from {Sender}");
        System.Console.WriteLine($"{Sender} : {message}");
        Sender.Tell("I'm fine thank you, and you?");
      });
    }
  }
}