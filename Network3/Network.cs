


/*libs*/using System;

internal class Network : AppBase
{
    public string CustomName;
    public string Message;

    public SConfig sConfig { get; private set; }
    public SMessage sMessage { get; private set; }
    public Func<string, string> Debug { get; internal set; }
    public bool ReTick { get; internal set; }

    public Network()
    {
        sConfig = new SConfig();
        AddChild(sConfig);

        sMessage = new SMessage();
        AddChild(sMessage);
    }

    public override void Tick()
    {
        
        base.Tick();
        if (ReTick)
        {
            Message = "";
            Tick();
        }
    }


}


//add-Config/SConfig.cs
//add-Message/SMessage.cs
//add-NetworkComponent.cs




/*libs*/partial class main {

    Network app;
    /*libs*/ProgrammerBlock Me;

    public void Program()
    {
        app = new Network();
        app.CaptureCube = captureCube;
        app.FilterBlock = _s;
        app.CustomName = Me.CustomName;        
        app.Debug = debug;
        app.Begin();
    }

    /*libs*/ public main() {


        /*libs*/ Me = new ProgrammerBlock();
        /*libs*/ }

    public void Main(string args)
    {
        //basicCode        
        app.Message = args;
        app.Tick();

    }

    
    /*libs*/}

