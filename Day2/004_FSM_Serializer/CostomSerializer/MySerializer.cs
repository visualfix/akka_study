using Akka.Actor;
using Akka.Serialization;

namespace FSM.CostomSerializer;

public class MySerializer : Serializer
{
    public MySerializer(ExtendedActorSystem system) : base(system)
    {
    }

    public override bool IncludeManifest { get; } = false;

    public override int Identifier => 1234567;

    public override byte[] ToBinary(object obj)
    {
        Console.WriteLine("Call MySerializer ToBinary");
        throw new NotImplementedException();
    }

    public override object FromBinary(byte[] bytes, Type type)
    {
        Console.WriteLine("Call MySerializer FromBinary");
        throw new NotImplementedException();
    }
}