
using Akka.Actor;

namespace Actors
{
    public class SupervisorActor : ReceiveActor
    {
        List<IActorRef> children = new List<IActorRef>();

        public SupervisorActor()
        {
            children.Add(Context.ActorOf(VictimActor.Props(), "VictimActor1"));
            children.Add(Context.ActorOf(VictimActor.Props(), "VictimActor2"));

            ReceiveAny(write =>
            {
                children[0].Tell(Kill.Instance, ActorRefs.NoSender);
            });
        }

        public static Props PropsWithOne4One()
        {
            return Akka.Actor.Props.Create<SupervisorActor>()
            .WithSupervisorStrategy(
                new OneForOneStrategy(exception =>
                {
                    return Directive.Restart;
                    //return Directive.Stop;
                })
            );
        }

        public static Props PropsWithAll4One()
        {
            return Akka.Actor.Props.Create<SupervisorActor>()
            .WithSupervisorStrategy(
                new AllForOneStrategy(exception =>
                {
                    return Directive.Restart;
                    //return Directive.Stop;
                })
            );
        }
    }
}