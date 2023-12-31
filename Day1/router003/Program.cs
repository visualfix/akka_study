﻿using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Router003
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyRouterSystem003", config);

            //var route_wokers = system.ActorOf(
            //    Worker.Props().WithRouter(FromConfig.Instance)
            //    , "worker_pool");

            var route_wokers = system.ActorOf(
                Worker.Props().WithRouter(
                    new ConsistentHashingPool(5).WithHashMapping(o =>
                    {
                        if (o is IMessage)
                            return ((IMessage)o).Key;

                        return null;
                    })
                )
                , "worker_hash_pool");
                
            int call_cnt = 10;
            while( call_cnt --> 0)
            {
                route_wokers.Tell(new MyMessage(call_cnt, $"message {call_cnt}"));
                Thread.Sleep(100);
            }
            
            Thread.Sleep(1000);
        }
    }
}