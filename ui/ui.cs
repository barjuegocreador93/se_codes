//add-XUI.cs


internal class ComponentUI : SResource.CResourceItem
{
    public XUI.XML.UIController ui { get; set;}
    public IMyTextPanel textPanel { get; private set; }
    public string strXML { get; private set; }

    public ComponentUI()
    {
        Type = "component-ui";
        SetAttribute("name", "");
        SetAttribute("key", "");
    }

    public override void Begin()
    {
        Block = GetAppBase().GetGemeObject<IMyTerminalBlock>(GetAttribute("name"));
        if (Block != null)
        {
            strXML = Block.CustomData;
            textPanel = Block as IMyTextPanel;
            OnStart();

        }
        else End();
    }

    public void KeyPress(string key)
    {
        XML.Read(key, this);
    }

    protected override void OnWorking()
    {
        textPanel = Block as IMyTextPanel;
        if (textPanel != null)
        {
            if (Block.CustomData != strXML)
            {
                strXML = Block.CustomData;
                OnStrXMLChange();
            }
        }
        else End();
    }

    protected virtual void OnStart()
    {
        UsingUI();
    }

    protected virtual void OnStrXMLChange()
    {
        UsingUI();
    }

    protected virtual void UsingUI()
    {
        ui = XUI.XML.UIController.FromXML(strXML);
        ui.ApplyScreenProperties(textPanel);
        ui.RenderTo(textPanel);
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "keypress":
                return new KeyPress();
        }
        return null;
    }
}


partial class KeyPress :Object
{
    public KeyPress()
    {
        Type = "keypress";
        SetAttribute("key", "");
        SetAttribute("ui-target", "");
        SetAttribute("secret", "");
    }

    public override void Tick()
    {
        if(Parent as ComponentUI !=null)
        {
            if(Parent.GetAttribute("key")==GetAttribute("secret")&& Parent.GetAttribute("name") == GetAttribute("ui-target"))
            {
                var cui = Parent as ComponentUI;
                cui.ui.KeyPress(GetAttribute("key"));
            }
        }
        End();
    }
}