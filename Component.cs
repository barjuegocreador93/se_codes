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

    public SystemOb GetSystem()
    {
        return Parent as SystemOb;
    }

    public AppBase GetAppBase()
    {
        return GetSystem().Parent as AppBase;
    }
}