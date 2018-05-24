


internal class WifiAntenna : NSResource.NCResourceItem
{
    private IMyRadioAntenna antenna;
    public WifiAntenna()
    {
        ObjectType = "wifi-antenna";
        SetAttrs("type", "public");        
    }

    protected override void OnWorking()
    {
        antenna = block as IMyRadioAntenna;
        if (antenna == null) End();
    }

    public void SendMessage(string msg)
    {
        switch(VarAttrs["type"])
        {
            case "public":
                antenna.TransmitMessage(msg, MyTransmitTarget.Everyone);
                break;
            case "private":
                antenna.TransmitMessage(msg, MyTransmitTarget.Ally);
                break;            
        }
        
    }    
    
}