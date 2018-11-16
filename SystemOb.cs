//add-SResource.cs

internal class SystemOb : Object
{
    public SystemOb():base()
    {
        Type = "SystemOb";
    }
    public void AddComponent(Component comp)
    {
        AddChild(comp);
    }

    public void SpawnComponet(Component comp)
    {
        AddChild(comp);
        comp.Begin();
    }

    public string PrintChilds(char c = '\n')
    {
        string data = "";
        foreach (Object m in Children)
        {
            if (m as Component != null)
                data += m.ToString() + c;
        }
        return data;
    }

    public AppBase AppBase
    {
        get
        {
            return Parent as AppBase;
        }
    }

    public override void Tick()
    {
        base.Tick();        
    }    
    
}

