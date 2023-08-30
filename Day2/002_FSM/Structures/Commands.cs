using FSM.Structures.Datas;

namespace FSM.Structures.Commands;

public interface ICommand { }

public class AddItem : ICommand
{
    public AddItem(Item item)
    {
        Item = item;
    }

    public Item Item { get; set; }
}

public class Buy : ICommand
{
    public static Buy Instance { get; } = new Buy();
    private Buy() { }
}

public class Leave : ICommand
{
    public static Leave Instance { get; } = new Leave();
    private Leave() { }
}

public class GetCurrentCart : ICommand
{
    public static GetCurrentCart Instance { get; } = new GetCurrentCart();
    private GetCurrentCart() { }
}