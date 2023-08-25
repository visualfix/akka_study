using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class RActor: ReceiveActor
  {
    public RActor()
    {
      Receive<string>(message => {
        System.Console.WriteLine($"{Sender} : {message}");
        Sender.Tell("I'm fine thank you, and you?");
      });
    }
  }
}