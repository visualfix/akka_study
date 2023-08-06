using Akka.Routing;
class MyMessage : IConsistentHashable
{
    public object ConsistentHashKey {  get; }
    public string Text { get; }

    public MyMessage(int key, string text)
    {
        ConsistentHashKey = (object)key;
        Text = text;
    }
}