//add-XUI.cs


/*libs*/using System;
/*libs*/using System.Collections.Generic;

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
        Block = AppBase.GetGemeObject<IMyTerminalBlock>(GetAttribute("name"));
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
        UIrender();
    }

    public virtual void UIrender()
    {
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


partial class KeyPress : CComponet
{
    public KeyPress()
    {
        //keyboard:
        //<keypress ui-target='myUi' key='up' secret='1234'/>
        //<keypress ui-target='myUi' key='down' secret='1234'/>
        //<keypress ui-target='myUi' key='right/submit' secret='1234'/>
        //<keypress ui-target='myUi' key='left/abort' secret='1234'/>
        Type = "keypress";
        SetAttribute("key", "");
        SetAttribute("ui-target", "");
        SetAttribute("secret", "");
    }

    public override void Tick()
    {
        var cui = Parent as ComponentUI;
        if (cui != null)
        {            
            if(Parent.GetAttribute("key")==GetAttribute("secret")&& Parent.GetAttribute("name") == GetAttribute("ui-target"))
            {
                cui.AppBase.Debug(GetAttribute("key"));
                cui.ui.Call(new List<string>() { "key",GetAttribute("key") });
                cui.UIrender();
            }
        }
        End();
    }

    

}

//add-XJS/XJS.cs