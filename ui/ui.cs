/*libs*/using System;

internal class UI : UIelement
{    
    public string StrRender { get; private set; }
    public UIKeyBoard KeyBoard { get; set; }
    public UIcursor Cursor { get; protected set; }

    public UI()
    {
        ObjectType = "ui";
        SetAttrs("name", "");
        SetAttrs("key", "secret");
        KeyBoard = null;       
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "ui-keyboard":
                return new UIKeyBoard();

            case "ui-cursor":
                if (Cursor == null)
                    return Cursor = new UIcursor();
                return null;

            case "ui-element":
                return new UIelement();

            case "ui-einteract":
                return new UIeinteract();
        }
        return null;
    }

    public override void Tick()
    {       
        
        Cursor = new UIcursor();
        AddChild(Cursor);        
        base.Tick();
        ForChilds(uielements);
    }

    public void KeyBoardEnter(string kb)
    {
        KeyBoard = null;
        XML.Read(kb,this);
    }

    private int uielements(Object v, int i)
    {
        var uie = v as UIelement;
        if (v as TextObject != null)
        {
            Childs.RemoveAt(i);
            return 0;
        }
        if (uie!= null)
        {           
           StrRender += uie.GlobalRender();
        }
        return 0;
    }
}

internal class UIKeyBoard : Object
{
    public UIKeyBoard()
    {
        ObjectType = "ui-keyboard";
        SetAttrs("value", "");
        SetAttrs("ui-target","" );
        SetAttrs("key", "");
    }

    public override void Tick()
    {
        var ui = Parent as UI;
        if(ui!=null)
        {
            if(ui.VarAttrs["key"]==VarAttrs["key"] && Parent.VarAttrs["name"]==GetAttr("ui-target"))
            {
                ui.KeyBoard = this;
            }
            End();
        }
        
    }
    
}
//add-UIcursor.cs
//add-UIelement.cs
//add-UIeinteract.cs
