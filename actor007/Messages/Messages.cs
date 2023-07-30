using Akka.Actor;

public class PingPongMessage{
        
}

public class SetPlayer{

    public IActorRef Player{ get; }
    public SetPlayer(IActorRef player)
    {
        Player = player;
    }
}

public class Shutdown{
        
}