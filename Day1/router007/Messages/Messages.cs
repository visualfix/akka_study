using Akka.Routing;
using Akka.Actor;
class Response
{
    public IActorRef Sender {  get; }

    public Response(IActorRef sender)
    {
        Sender = sender;
    }
}