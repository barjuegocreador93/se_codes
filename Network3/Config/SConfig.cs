
internal class SConfig : NSResource
{
    public Connection Connection { get; private set; }
    public SConfig()
    {
        Type = "SConfig";
    }

    protected override void OnChanges()
    {
        Connection = null;
    }

    public override void Begin()
    {
        SetAttribute("name", GetNework.CustomName);
        Block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if (Block != null)
        {
            base.Begin();
            if(Block.CustomData == "")
            {
                Block.CustomData =
                "<connection name='alfa1' nick='root1' key='secret'/>" +
                "<wifi-antenna name='nw-wifi-a' type='public'/>" +
                "<xfinder-router name='nw-xfinder-r'/>" 
                ;
                
            }
            
        }
        else End();
        
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "wifi-antenna":
                return new WifiAntenna();
            case "connection":
                if(Connection == null)
                    return Connection = new Connection();
                return null;
            case "xfinder-router":
                return new XFinderRouter();            
             
        }
        return null;
    }
    

    
}

//add-WifiAntenna.cs
//add-Connection.cs
//add-XFinderRouter.cs