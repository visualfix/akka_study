
public class Write{
    public string Text{get;}

    public Write(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return $"[ Write - {Text} ]";
    }
}