//add-SResource.cs

internal class SystemOb : Object
{
    public SystemOb():base()
    {
        ObjectType = "SystemOb";
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
        foreach (Object m in Childs)
        {
            if (m as Component != null)
                data += m.ToString() + c;
        }
        return data;
    }

    public AppBase GetAppBase()
    {
        return Parent as AppBase;
    }

    public override void Tick()
    {
        base.Tick();
        for(int i=0;i<Childs.Count;i++)
        {
            var compOne = Childs[i] as Component;
            for (int j = i+1; j < Childs.Count; j++)
            {
                var compTow = Childs[j] as Component;
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
        for (int i = 0; i < Childs.Count; i++)
        {
            Component auxOne = Childs[i] as Component;
            for (int j = 0; j < other.Childs.Count; j++)
            {
                Component auxTwo = other.Childs[j] as Component;
                if (auxOne != null && auxTwo != null)
                {
                    auxOne.LinkerComponent(auxTwo);
                    
                }
            }
        }
    }
}

