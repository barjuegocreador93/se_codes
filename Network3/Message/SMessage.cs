/*libs*/using System;

internal class SMessage : NSystem
{
    public override void Tick()
    {        
        XML.Read(GetNework.Message,this);        
        ForChilds(erraseRequestTextOut);        
        base.Tick();
        GetNework.Debug(ChildsToString());
    }

    private int erraseRequestTextOut(Object arg1, int arg2)
    {
        if ((arg1 as TextObject)!=null) arg1.End();
        return 0;
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "request":
                return new Request();

            case "response":
                return new Response();

            case "request-connection":
                return new RFinderRouter();

            case "try-connection-request":
                return new TryConnectionRequest();
        }
        return null;
    }
}

//add-Request.cs