using Akka;
using Akka.Actor;

namespace Actors
{
    public class TimeoutActor : ReceiveActor
    {
        //private ILoggingAdapter log = Context.GetLogger();

        public TimeoutActor()
        {
            Receive<string>(s => s.Equals("Hello"), msg =>
            {
                Context.SetReceiveTimeout(TimeSpan.FromMilliseconds(1000));
            });

            Receive<ReceiveTimeout>(msg =>
            {
                System.Console.WriteLine("I get the ReceiveTimeout");
                Context.SetReceiveTimeout(null);
                //throw new Exception("Receive timed out");
                //return;
            });
        }
                
        public static Props Props()
        {
            return Akka.Actor.Props.Create<TimeoutActor>();
        }
    }
}