internal class ProgrammerBlock : IMyTerminalBlock
{
   
    public string CustomData { get; set; }
    
    public string CustomName { get; set; }
    
    public string DetailedInfo { get; }
   
    public ITerminalAction GetActionWithName(string name) { return null; }
}

partial class main
{
    public static void Echo(string s)
    {

    }
}