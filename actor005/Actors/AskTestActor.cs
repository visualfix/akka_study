using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;

namespace Actors
{
    public class AskTestActor : ReceiveActor
    {
        public AskTestActor()
        {
            Receive<string>(c => c.Equals("Execute"), msg =>
            {
                System.Console.WriteLine($"{Self.Path.Name} : {msg}");
                Execute();
            });
        }

        private void Execute()
        {
            var worker1 = Context.ActorOf(WorkerActor.Props(), "ASK-Wroker1");
            var worker2 = Context.ActorOf(WorkerActor.Props(), "ASK-Wroker2");
            var manager = Context.ActorOf(AskManagerActor.Props(), "ASK-Manager");

            var tasks = new List<Task<WorkDone>>();
            tasks.Add(worker1.Ask<WorkDone>(new WorkOrder(), TimeSpan.FromSeconds(5)));
            tasks.Add(worker2.Ask<WorkDone>(new WorkOrder(), TimeSpan.FromSeconds(1)));

            Task.WhenAll(tasks).PipeTo(manager, Self, ()=>{return "success";}, (e)=>{return "fail";});
            //Task.WhenAll(tasks).PipeTo(manager, Self);
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<AskTestActor>();
        }
    }
}