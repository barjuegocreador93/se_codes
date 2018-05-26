internal class NComponent : Component
{
    public Network GetNetwork
    {
        get { return AppBase as Network; }
    }
}

internal class NSystem : SystemOb
{
    public Network GetNework
    {
        get { return AppBase as Network; }
    }
}


internal class NSResource : SResource
{
    public Network GetNework
    {
       get { return AppBase as Network; }
    }

    internal class NCResourceItem : CResourceItem
    {
        public Network GetNework
        {
            get { return AppBase as Network; }
        }
    }
}

internal class NCComponet : Object
{
    public Component GetComponent { get { return Parent as Component;  } }
    public AppBase AppBase
    {
        get
        {
            return GetComponent.AppBase;
        }
    }

    public SystemOb GetSystem()
    {
        return GetComponent.System;
    }

    public Network GetNetwork
    {
        get {return  AppBase as Network; }
    }
}
