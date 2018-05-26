


internal class WifiAntenna : NSResource.NCResourceItem
{
    private IMyRadioAntenna antenna;
    public WifiAntenna()
    {
        Type = "wifi-antenna";
        SetAttribute("type", "public");        
    }

    protected override void OnWorking()
    {
        antenna = Block as IMyRadioAntenna;
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