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
        for(int i=0;i<Children.Count;i++)
        {
            var compOne = Children[i] as Component;
            for (int j = i+1; j < Children.Count; j++)
            {
                var compTow = Children[j] as Component;
                if (compOne != null && compTow != null)
                {
                    compOne.LinkerComponent(compTow);
                    compTow.LinkerComponent(compOne);
                }
            }
        }
    }
    
    public virtual void LinkerSystem(SystemOb other)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            Component auxOne = Children[i] as Component;
            for (int j = 0; j < other.Children.Count; j++)
            {
                Component auxTwo = other.Children[j] as Component;
                if (auxOne != null && auxTwo != null)
                {
                    auxOne.LinkerComponent(auxTwo);
                    
                }
            }
        }
    }
}

