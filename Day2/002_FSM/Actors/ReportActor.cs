using Akka.Actor;
using FSM.Structures.Events.Reports;

namespace FSM.Actors;

public class ReportActor: ReceiveActor
{
  public ReportActor()
  {
    Receive<PurchaseWasMade>(message => {
      foreach(var item in message.Items)
      {
          Console.WriteLine(item);
      }
    });

    Receive<ShoppingCardDiscarded>(message => {
      System.Console.WriteLine($"ShoppingCardDiscarded");
    });
  }

  public static Props Props()
  {
    return Akka.Actor.Props.Create<ReportActor>();
  }
}