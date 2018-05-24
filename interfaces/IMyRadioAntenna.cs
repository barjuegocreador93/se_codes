internal interface IMyRadioAntenna
{
    bool TransmitMessage(string message, MyTransmitTarget target = MyTransmitTarget.Default);
    bool IgnoreAlliedBroadcast { get; set; }
    bool IgnoreOtherBroadcast { get; set; }
}
