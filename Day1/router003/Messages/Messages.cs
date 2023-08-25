interface IMessage
{
    int Key { get; }
}

class MyMessage : IMessage
{
    public int Key { get; }
    public string Text { get; }

    public MyMessage(int key, string text)
    {
        Key = key;
        Text = text;
    }
}