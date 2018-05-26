internal class TryConnectionRequest : Request
{
    public TryConnectionRequest()
    {
        Type = "try-connection-request";

    }

    public override void Tick()
    {
        var response = new TryToConnectionResponse();
        base.Tick();
    }
}

internal class TryToConnectionResponse : Response
{
    public TryToConnectionResponse()
    {
        Type = "try-connection-response";
    }

    public override void Tick()
    {
        base.Tick();
        if(VarAttrs["token"]==Parent.VarAttrs["token"])
        {

        }
        Parent.End();
    }
}


internal class TryToConnectionTask : Task
{
    public override void Tick()
    {
        jq("this").Xml(GetNetwork.Message);
        base.Tick();
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "try-connection-response":
                return new TryConnectionRequest();
        }
        return null;
    }
}