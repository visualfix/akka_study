using FSM001.Structures.Datas;

namespace FSM001.Structures.Events.Domains
{
    public interface IDomainEvent { }

    public class ItemAdded : IDomainEvent
    {
        public ItemAdded(Item item)
        {
            Item = item;
        }

        public Item Item { get; set; }
    }

    public class OrderExecuted : IDomainEvent
    {
        public static OrderExecuted Instance { get; } = new OrderExecuted();
        private OrderExecuted() { }
    }

    public class OrderDiscarded : IDomainEvent
    {
        public static OrderDiscarded Instance { get; } = new OrderDiscarded();
        private OrderDiscarded() { }
    }
}