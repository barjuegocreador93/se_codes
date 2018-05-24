internal class ProgrammerBlock : IMyTerminalBlock
{
   
    public string CustomData { get; set; }
    
    public string CustomName { get; set; }
    
    public string DetailedInfo { get; }
   
    public ITerminalAction GetActionWithName(string name) { return null; }
}