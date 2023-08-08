using Akka.Routing;
using Akka.Actor;
class Response
{
    public int TID {  get; }

    public Response(int tid)
    {
        TID = tid;
    }
}