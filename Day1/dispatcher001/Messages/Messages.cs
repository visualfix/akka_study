using Akka.Routing;
using Akka.Actor;
class Response
{
    public int Index {  get; }
    public int TID {  get; }

    public Response(int idx, int tid)
    {
        Index = idx;
        TID = tid;
    }
}