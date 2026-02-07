// See https://aka.ms/new-console-template for more information

using KR2;

var reflector = new Reflector();
Reflector.PrintStructure(typeof(You<int>));

public class You<T>
{
    public string YouName { get; set; }

    private T Data { get; set; }

    public string ReturnName()
    {
        return YouName;
    }

    private T ReturnData()
    {
        return Data;
    }
}

