/*libs*/using System;
/*libs*/using System.Collections.Generic;

internal class XFinderRouter : NSResource.NCResourceItem
{

    public  bool IsSendingFinderingRouter { get; private set; }
    public RouterConfig RouterConfig { get; private set; }

    public XFinderRouter()
    {
        ObjectType = "xfinder-router";       
        SetAttrs("is-finding", "true");        
    }

    

    public override void Begin()
    {
        InitVars();
        base.Begin();
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if (block!=null)
        {
            Text = block.CustomData;
            if (Text == "")
                Text = "<router-config start-find='true'/>";
            RouterConfig = null;
            jq("this").Xml(Text);
            
        }
    }
    protected override void OnWorking()
    {
        InitVars();
        if(IsSendingFinderingRouter)
        {
            var network = GetNework;            
            var rfr = new RFinderRouter();           
            var tfr = new TFinderRouter();
            tfr.xFinderRouter = this;            
            List<Object> msgs = network.sConfig.jq("this").FindByType<WifiAntenna>().Targets;            
            if (msgs.Count>1)
            {
                rfr.CreateRequest("<finding-router />", network.sMessage, tfr);
                VarAttrs["is-finding"] = "false";                
                GetNework.Debug(rfr.VarAttrs["token"]);
                GetNework.Tick();
            }
            foreach (Object v in msgs)
            {
                var a = v as WifiAntenna;
                if(a!=null)
                     a.SendMessage(rfr.ToString());                     
                          
                
            }
        }

       
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if(block!=null)
        {
            if (Text != block.CustomData)
            {
                
                Text = block.CustomData;
                if(Text=="")                
                    Text = "<router-config start-find='true'/>";                
                RouterConfig = null;
                jq("this").Xml(Text);
                
            }
            Text = block.CustomData = ChildsToString();
            
        }     
        


    }

    public override string ToString()
    {
        return string.Format("<{0} {1}/>\n",ObjectType,Attrs());
    }

    void InitVars()
    {
        IsSendingFinderingRouter = VarAttrs["is-finding"] == "true" ? true : false;
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "router":
                return new Router();
            case "router-config":
                if(RouterConfig==null)
                    return RouterConfig = new RouterConfig();
                return null;
        }
        return null;
    }
}

internal class RFinderRouter : Request
{
    public RFinderRouter()
    {
        ObjectType = "request-connection";
        SetAttrs("token", "");
    }

    public override void Tick()
    {
        var text = ChildsToString();
        jq("this").Xml(text);       
        base.Tick();
        
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "finding-router":
                return new FindigRouter();
        }
        return null;
    }
    

    internal class FindigRouter : Object
    {
        public FindigRouter()
        {
            ObjectType = "finding-router";
        }

        public override void Tick()
        {
            var network = (Parent as NComponent).GetNetwork;
            var rsfr = new RsFinderRouter();
            rsfr.CreateResponse(
                string.Format("<router nick='{0}' network-name='{1}'/>",
                network.sConfig.Connection.Nick,
                network.sConfig.Connection.Local_.NetworkName)
                );
            
            rsfr.VarAttrs["token"] = Parent.VarAttrs["token"];
            List<Object> wifis = network.sConfig.jq("this").FindByType<WifiAntenna>().Targets;
            foreach(Object v in wifis)
            {
                var a = v as WifiAntenna;
                if(a!=null)
                {
                    a.SendMessage(rsfr.ToString());
                   
                }
                    
                
            }
            
        }
    }
}

internal class RsFinderRouter : Response
{
    public int TimeDead { get; private set; }

    public RsFinderRouter()
    {
        ObjectType = "response-connection";
        SetAttrs("token", "");
        TimeDead = 0;
    }

    public override Object Types(string typeName)
    {
        switch (typeName)
        {
            case "router":
                return new Router();
        }
        return null;
    }

    public override void Tick()
    {       
        if(Parent.VarAttrs["token"] == VarAttrs["token"])
        {
            var t = Parent as TFinderRouter;
            if(t!=null)
            {
                List<Object> rs = jq("this").FindObjectByTag("router").Targets;
                rs.RemoveAt(0);
                foreach (Object d in rs)
                {
                    if (t.xFinderRouter.jq("this").FindObjectsByAttr("nick", d.VarAttrs["nick"]).Targets.Count == 1)
                        if(d as TextObject == null)
                            t.xFinderRouter.AddChild(d);
                }
            }               
            
        } 
        
    }

}


internal class TFinderRouter : TaskWithTime
{
    internal XFinderRouter xFinderRouter;
    public string Request { get; internal set; }

    public override void Tick()
    {        
        jq("this").Xml(GetNetwork.Message);
        base.Tick();
        GetNetwork.ReTick = true;
        if (Childs.Count == 0) GetNetwork.ReTick = false;
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "response-connection":
                return new RsFinderRouter();
            case "router":
                return new Router();
        }
        return null;
    }

}

internal class RouterConfig : NCComponet
{
    public RouterConfig()
    {
        ObjectType = "router-config";
        SetAttrs("start-find", "false");
    }

    public override void Tick()
    {
        var network = GetNetwork;
        if (network != null)
        {
            if (VarAttrs["start-find"] == "true")
            {
                Parent.VarAttrs["is-finding"] = "true";
                network.ReTick = true;
                VarAttrs["start-find"] = "false";
                GetComponent.ForChilds(eraseRoutes);
            }
            else network.ReTick = false;

        }
        base.Tick();
    }

    private int eraseRoutes(Object r, int i)
    {
        if (r as Router != null)
            GetComponent.RemoveChildAt(i);
        return 0;
    }
}


internal class Router : NCComponet
{
    public Router()
    {
        ObjectType = "router";
        SetAttrs("try-to-connect", "false");
        SetAttrs("nick", "");
        SetAttrs("network-name", "");
    }

    public override void Tick()
    {
        if(GetComponent as XFinderRouter != null)
        {
            if(VarAttrs["try-to-connect"]=="true")
            {
                GetComponent.ForChilds(eraseRoutes);
                var request = new TryConnectionRequest();
            }
        }
    }

    private int eraseRoutes(Object r, int i)
    {
        if (r.VarAttrs["nick"] != VarAttrs["nick"])
            GetComponent.RemoveChildAt(i);
        return 0;
    }
}

//add-TryConnectionRequest.cs

