using Akka.Persistence.Fsm;

namespace FSM002.Structures.States
{
    public interface IUserState : Akka.Persistence.Fsm.PersistentFSM.IFsmState { }

    public class LookingAround : IUserState
    {
        public static LookingAround Instance { get; } = new LookingAround();
        private LookingAround() { }
        public string Identifier => "Looking Around";
    }

    public class Shopping : IUserState
    {
        public static Shopping Instance { get; } = new Shopping();
        private Shopping() { }
        public string Identifier => "Shopping";
    }

    public class Inactive : IUserState
    {
        public static Inactive Instance { get; } = new Inactive();
        private Inactive() { }
        public string Identifier => "Inactive";
    }

    public class Paid : IUserState
    {
        public static Paid Instance { get; } = new Paid();
        private Paid() { }
        public string Identifier => "Paid";
    }
}