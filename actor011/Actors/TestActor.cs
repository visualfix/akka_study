using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class TestActor: UntypedActor
  {
    public TestActor()
    {
    }

    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case "test":
                System.Console.WriteLine("received test");
                break;
            default:
                System.Console.WriteLine("received unknown message");
                break;
        }
    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<TestActor>();
    }
  }
}