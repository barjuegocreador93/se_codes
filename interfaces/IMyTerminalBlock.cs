internal interface IMyTerminalBlock
{
    ITerminalAction GetActionWithName(string name);
    string CustomData { get; set; }
    string CustomName { get; set; }
    string DetailedInfo { get; }
}

