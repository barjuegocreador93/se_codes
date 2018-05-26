/*libs*/using System;
internal class Component : Object
{
    public Component()
    {
        Type = "Component";
    }

    public virtual void LinkerComponent(Component compTow)
    {
        
    }

    public SystemOb System
    {
        get
        {
            return Parent as SystemOb;
        }
    }

    public AppBase AppBase
    {
        get
        { return System.Parent as AppBase; }
    }
}


internal class CComponet : Object
{
    public CComponet()
    {
        Type = "CComponent";
    }

    public Component Componet
    {
        get
         { return Parent as Component; } 
    }

    public SystemOb System
    {
        get
         { return Componet.System; } 
    }

    public AppBase AppBase
    {
        get
         { return Componet.AppBase; } 
    }
}