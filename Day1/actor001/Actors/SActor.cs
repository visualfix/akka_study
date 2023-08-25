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
  }
}