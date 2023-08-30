using FSM.Structures.Datas;

namespace FSM.Structures.Events.Reports
{
    public interface IReportEvent { }

    public class PurchaseWasMade : IReportEvent
    {
        public PurchaseWasMade(IEnumerable<Item> items)
        {
            Items = items;
        }

        public IEnumerable<Item> Items { get; }
    }

    public class ShoppingCardDiscarded : IReportEvent
    {
        public static ShoppingCardDiscarded Instance { get; } = new ShoppingCardDiscarded();
        private ShoppingCardDiscarded() { }
    }
}