internal class Connection : NComponent
{
    public string Nick { get; private set; }
    public string AutoConnection { get; private set; }

    public Local Local_ { get; private set; }
    public Connection()
    {       
        ObjectType = "connection";
        SetAttrs("nick","");
        SetAttrs("name", "");
        SetAttrs("auto-connection", "false");
    }

    public override void Begin()
    {
        Local_ = new Local(this);
        GetSystem().AddChild(Local_);
    }

    public override void Tick()
    {
        Childs.Clear();
        Nick = VarAttrs["nick"];
        AutoConnection = VarAttrs["auto-connection"];
        
    }

    public override void LinkerComponent(Component compTow)
    {
        
    }
    public override void End()
    {
        
        base.End();
    }

}



internal class Local : Component
{
    public Connection Connection { get; private set; }
    public string IP { get; private set; }
    public string HasConection { get; private set; }
    public string ParentIP { get; private set; }
    public string NetworkName { get; private set; }
    public Local(Connection c)
    {
        Connection = c;
        ObjectType = "local";
        SetAttrs("ip", "0");
        SetAttrs("has-connection", "false");
        SetAttrs("parent-ip", "");
        SetAttrs("network-name", "");
    }

    public override void Tick()
    {
        IP = VarAttrs["ip"];
        HasConection = VarAttrs["has-connection"];
        ParentIP = VarAttrs["parent-ip"];
        if (VarAttrs["ip"] == "0") VarAttrs["network-name"] = Connection.VarAttrs["name"];
        NetworkName = VarAttrs["network-name"];
    }    
}