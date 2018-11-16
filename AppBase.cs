/*libs*/using System.Collections.Generic;using System;

//add-SystemOb.cs
//add-Component.cs
//add-Object.cs
//add-basicCode.cs
//add-Automata.cs

internal class AppBase : Object
{
    private bool beginRun;
    private bool endRun;

    public Func<string, string> Debug { get; internal set; }

    public Func<string, IMyTerminalBlock> CaptureCube;
    public Func<string, Func<IMyTerminalBlock, bool>, string> FilterBlock;

    public AppBase():base()
    {
        endRun =beginRun = true;
        Type = "AppBase";
    }

    public override void Begin()
    {
        if(beginRun)
        {
            base.Begin();
            beginRun = false;
        }
        
    }

    public override void Tick()
    {
        base.Tick();       
    }
    

    

    public override void End()
    {
        if (endRun)
        {
            base.End();
            endRun = false;
        }
    }

    public T GetGemeObject<T>(string name, string action = "") where T : class
    {
        if (action.Length != 0)
        {
            string[] actions = action.Split(',');
            foreach (string act in actions)
                ChangeActionToObject(name, act);
        }
        return CaptureCube(name) as T;
    }

    private void ChangeActionToObject(string _objeto, string _accion)
    {
        IMyTerminalBlock objeto = CaptureCube(_objeto);
        ITerminalAction accion = objeto.GetActionWithName(_accion);
        accion.Apply(objeto);
    }

    
}
