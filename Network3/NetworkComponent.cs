internal class NComponent : Component
{
    public Network GetNetwork
    {
        get { return GetAppBase() as Network; }
    }
}

internal class NSystem : SystemOb
{
    public Network GetNework
    {
        get { return GetAppBase() as Network; }
    }
}


internal class NSResource : SResource
{
    public Network GetNework
    {
       get { return GetAppBase() as Network; }
    }

    internal class NCResourceItem : CResourceItem
    {
        public Network GetNework
        {
            get { return GetAppBase() as Network; }
        }
    }
}

internal class NCComponet : Object
{
    public Component GetComponent { get { return Parent as Component;  } }
    public AppBase GetAppBase()
    {
        return GetComponent.GetAppBase();
    }

    public SystemOb GetSystem()
    {
        return GetComponent.GetSystem();
    }

    public Network GetNetwork
    {
        get {return  GetAppBase() as Network; }
    }
}
