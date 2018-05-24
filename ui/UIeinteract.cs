/*libs*/using System;

internal class UIeinteract : UIelement
{
    public UIeinteract()
    {
        ObjectType = "ui-einteract";
        SetAttrs("hover", "false");
        SetAttrs("selected", "false");       
    }

    public override void Tick()
    {
        
        if(GetAttr("hover")=="true")        
            OnHover();

        if (GetAttr("selected") == "true")
        {
            OnSelected();
            SetAttrs("selected", "false");
        }
            
        base.Tick();
    }

    public virtual void OnSelected()
    {
        
    }

    public virtual void OnHover()
    {
        
    }

    public override void OnCLick()
    {
        if (GetAttr("hover") == "true" && GetAttr("visible") == "true")
        {
            SetAttrs("selected", "true");
        }

    }


}
