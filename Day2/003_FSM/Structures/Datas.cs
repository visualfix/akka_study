using System.Collections.Immutable;

namespace FSM.Structures.Datas;

public class Item
    {
        public Item(string id, string name, double price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public string Id { get; }

        public string Name { get; }

        public double Price { get; }

        public override string ToString()
        {
            return $"{Id}, {Name}, {Price}";
        }
    }

    public interface IShoppingCart
    {
        IShoppingCart AddItem(Item item);
        IShoppingCart Empty();
    }

    public class EmptyShoppingCart : IShoppingCart
    {
        public IShoppingCart AddItem(Item item)
        {
            return new NonEmptyShoppingCart(ImmutableList.Create(item));
        }

        public IShoppingCart Empty()
        {
            return this;
        }
    }

    public class NonEmptyShoppingCart : IShoppingCart
    {
        public NonEmptyShoppingCart(ImmutableList<Item> items)
        {
            Items = items;
        }

        public IShoppingCart AddItem(Item item)
        {
            return new NonEmptyShoppingCart(Items.Add(item));
        }

        public IShoppingCart Empty()
        {
            return new EmptyShoppingCart();
        }

        public ImmutableList<Item> Items { get; }
    }