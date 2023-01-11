namespace TiltDemoApi.MsgContracts;

public class CreateUser
{
    public int Age { get; set; }
}

public class CreatedUser
{
    public int Age { get; set; }
    
    public string Name { get; set; }
}