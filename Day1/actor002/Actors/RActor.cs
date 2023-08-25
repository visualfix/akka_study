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

      Receive<PublicMessage>(message => {
        System.Console.WriteLine($"{Sender} : {message.text} - [FromPublic]");
        Self.Tell(new PrivateMessage(message.text));
      });

      Receive<PrivateMessage>(message => {
        System.Console.WriteLine($"{Sender} : {message.text} - [FromPrivate]");
      });
    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<RActor>();
    }

    private class PrivateMessage
    {
      public string text{ get; set; }

      public PrivateMessage(string message)
      {
        this.text = message;
      }
    }
  }
}