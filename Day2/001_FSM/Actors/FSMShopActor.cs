using Akka.Actor;
using Akka.Event;
using Akka.Persistence.Fsm;

using FSM.Structures.Events.Domains;
using FSM.Structures.Events.Reports;
using FSM.Structures.Commands;
using FSM.Structures.Datas;
using FSM.Structures.States;


namespace FSM.Actors;

public class FSMShopActor : PersistentFSM<IUserState, IShoppingCart, IDomainEvent>
{
private readonly ILoggingAdapter _log = Context.GetLogger();
private IActorRef reportActor;

public override string PersistenceId { get; } = "my-stable-persistence-id";

public static Props Props(IActorRef repoActor)
{
    return Akka.Actor.Props.Create<FSMShopActor>(repoActor);
}

protected override IShoppingCart ApplyEvent(IDomainEvent evt, IShoppingCart cartBeforeEvent)
{
    switch (evt)
    {
        case ItemAdded itemAdded:
            return cartBeforeEvent.AddItem(itemAdded.Item);
        case OrderExecuted _: 
            return cartBeforeEvent;
        case OrderDiscarded _: 
            return cartBeforeEvent.Empty();
        default: return cartBeforeEvent;
    }
}

public FSMShopActor(IActorRef repoActor)
{
    reportActor = repoActor;

    StartWith(LookingAround.Instance, new EmptyShoppingCart());

    When(LookingAround.Instance, (evt, _) =>
    {
        _log.Debug($"LookingAround : {evt.FsmEvent.ToString()}");

        if (evt.FsmEvent is AddItem addItem)
        {
            return GoTo(Shopping.Instance)
                .Applying(new ItemAdded(addItem.Item))
                .ForMax(TimeSpan.FromSeconds(1))
                .AndThen(cart => reportActor.Tell(new PurchaseWasMade(((NonEmptyShoppingCart)cart).Items)));
        }
        else if (evt.FsmEvent is GetCurrentCart)
        {
            return Stay().Replying(evt.StateData);
        }
        
        return Stay();
    });

    When(Shopping.Instance, (evt, _) =>
    {
        _log.Debug($"Shopping : {evt.FsmEvent.ToString()}");

        if (evt.FsmEvent is AddItem addItem)
        {
            return Stay()
                .Applying(new ItemAdded(addItem.Item))
                .ForMax(TimeSpan.FromSeconds(1))
                .AndThen(cart => reportActor.Tell(new PurchaseWasMade(((NonEmptyShoppingCart)cart).Items)));
        }
        else if (evt.FsmEvent is Buy)
        {
            return GoTo(Paid.Instance).Applying(OrderExecuted.Instance)
                .AndThen(cart =>
                {
                    if (cart is NonEmptyShoppingCart nonShoppingCart)
                    {
                        reportActor.Tell(new PurchaseWasMade(nonShoppingCart.Items));
                        SaveStateSnapshot();
                    }
                    else if (cart is EmptyShoppingCart)
                    {
                        SaveStateSnapshot();
                    }
                });
        }
        else if (evt.FsmEvent is Leave)
        {
            return Stop().Applying(OrderDiscarded.Instance)
                .AndThen(_ =>
                {
                    reportActor.Tell(ShoppingCardDiscarded.Instance);
                    SaveStateSnapshot();
                });
        }
        else if (evt.FsmEvent is GetCurrentCart)
        {
            return Stay().Replying(evt.StateData);
        }
        else if (evt.FsmEvent is FSMBase.StateTimeout)
        {
            return GoTo(Inactive.Instance).ForMax(TimeSpan.FromSeconds(2));
        }

        return Stay();
    });

    When(Inactive.Instance, (evt, _) =>
    {
        _log.Debug($"Inactive : {evt.FsmEvent.ToString()}");

        if (evt.FsmEvent is AddItem addItem)
        {
            return GoTo(Shopping.Instance)
                .Applying(new ItemAdded(addItem.Item))
                .ForMax(TimeSpan.FromSeconds(1));
        }
        else if (evt.FsmEvent is FSMBase.StateTimeout)
        {
            return Stop()
                .Applying(OrderDiscarded.Instance)
                .AndThen(_ => reportActor.Tell(ShoppingCardDiscarded.Instance));
        }

        return Stay();
    });

    When(Paid.Instance, (evt, _) =>
    {
        _log.Debug($"Paid : {evt.FsmEvent.ToString()}");

        if (evt.FsmEvent is Leave)
        {
            return Stop();
        }
        else if (evt.FsmEvent is GetCurrentCart)
        {
            return Stay().Replying(evt.StateData);
        }

        return Stay();
    });
}
}