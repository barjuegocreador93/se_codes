/*libs*/using System;

internal class UIelement : Object
{
    public bool IsCursorAllowed { get; protected set; }

    public UIelement()
    {
        ObjectType = "ui-element";
        SetAttrs("visible", "true");
        IsCursorAllowed = false;
    }

    public override void Tick()
    {
        if (GetAttr("visible") == "true")
             ChangeAllChildsAttr("visible", "true");
        else ChangeAllChildsAttr("visible", "false");

        base.Tick();
    }

    public void Click()
    {
        OnCLick();
    }

    public virtual void OnCLick()
    {
        
    }

    public virtual string Render()
    {
        return "";
    }

    public string GlobalRender()
    {

        string result = Render();
        foreach(Object v in Childs)
        {
            var u = v as UIelement;
            if(u!=null)
            {
                result += u.GlobalRender();
            }
        }
        if (VarAttrs["visible"] == "true")
            return result;

        return "";
    }

    public UI UI(Object u = null)
    {
        if (u as UI != null) return u as UI;
        if(Parent != null)
        {
            return UI(Parent);
        }
        return null;
    }
    

}
