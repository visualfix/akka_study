﻿using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Router002
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyRouterSystem001", config);

            var parent = system.ActorOf(ParentWorker.Props(3), "workers");

            var route_wokers = system.ActorOf(
                Props.Empty.WithRouter(FromConfig.Instance)
                , "worker_group");

            int call_cnt = 10;
            while( call_cnt --> 0)
            {
                route_wokers.Tell("test");
                Thread.Sleep(100);
            }
            
            Thread.Sleep(1000);
        }
    }
}