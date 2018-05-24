/*libs*/using System;using System.Collections.Generic;

internal class UIcursor : UIelement
{
    public UIcursor()
    {
        ObjectType = "ui-cursor";
    }

    public UIeinteract CursorHoverOption { get; private set; }

    public override void Tick()
    {
        if(CursorHoverOption==null)
        {
            List<UIeinteract> uiol = UI().FindAKindOfChilds<UIeinteract>();
            foreach (UIeinteract v in uiol)
            {
                if (v.GetAttr("hover") == "true")
                {
                    if (CursorHoverOption == null && v.GetAttr("Visible")=="true")
                        CursorHoverOption = v;
                    else v.SetAttrs("hover", "false");
                }
            }
            if(CursorHoverOption==null)
            {
                FindFristOption(uiol);
                if(uiol.Count>0)
                {
                    var op = uiol[0];
                    if(op.GetAttr("visible")=="true")
                    {
                        op.SetAttrs("hover","true");
                        CursorHoverOption = op;
                    }
                }
            }
        }
        if(UI().KeyBoard!=null)
        {
            var kb = UI().KeyBoard;
            var key = kb.GetAttr("value");
            switch(key)
            {
                case "up":
                    MoveCursorUP();
                    break;
                case "down":
                    MoveCursorDown();
                    break;

                case "select":
                    SelectCursor();
                    break;
            }
        }
        base.Tick();
    }

    private void FindFristOption(List<UIeinteract> uiol)
    {
        if (uiol.Count > 0)
        {
            var op = uiol[0];
            if (op.GetAttr("visible") == "true")
            {
                op.SetAttrs("hover", "true");
                CursorHoverOption = op;
            }
        }
    }

    private void SelectCursor()
    {
        if(CursorHoverOption!=null)
            CursorHoverOption.Click();
    }

    private void MoveCursorDown()
    {
        if(CursorHoverOption!=null)
        {           
            var crs = CursorHoverOption;
            crs.SetAttrs("hover", "false");
            CursorHoverOption = null;
            FindNextCursorDown(crs);
            FindNextCursorParentDown(crs.Parent);
            if(CursorHoverOption == null)
            {
                List<UIeinteract> uiol = UI().FindAKindOfChilds<UIeinteract>();
                FindFristOption(uiol);
            }
        }
    }

    private void FindNextCursorParentDown(Object crs)
    {
        if (CursorHoverOption == null && crs as UI == null && crs != null)
        {
            FindNextCursorDown(crs.BrotherDown);
            if (CursorHoverOption == null)
            {
                FindNextCursorParentDown(crs.Parent);
            }
        }
    }

    private void FindNextCursorDown(Object crs)
    {
        if(CursorHoverOption == null && crs != null)
        {
            if(crs.Childs.Count>0)
            {
                var op = crs.Childs[0];
                if(op as UIeinteract != null)
                {
                   if( op.GetAttr("visible")=="true")
                    {
                        
                        CursorHoverOption = op as UIeinteract;
                    }
                }
                FindNextCursorDown(op);
                if (CursorHoverOption == null && crs.BrotherDown!=null)
                {
                    FindNextCursorDown(crs.BrotherDown);
                }
                
            }            
        }
    }

    private void MoveCursorUP()
    {
        if (CursorHoverOption != null)
        {
            var crs = CursorHoverOption;
            crs.SetAttrs("hover", "false");
            CursorHoverOption = null;
            FindNextCursorUp(crs.BrotherUp);
            FindNextCursorParentUp(crs.Parent);
            if (CursorHoverOption == null)
            {
                List<UIeinteract> uiol = UI().FindAKindOfChilds<UIeinteract>();
                FindLastOption(uiol);
            }
        }
    }

    private void FindLastOption(List<UIeinteract> uiol)
    {
        if (uiol.Count > 0)
        {
            var op = uiol[uiol.Count-1];
            if (op.GetAttr("visible") == "true")
            {
                op.SetAttrs("hover", "true");
                CursorHoverOption = op;
            }
        }
    }

    private void FindNextCursorParentUp(Object crs)
    {
        if (CursorHoverOption == null && crs as UI == null && crs != null)
        {
            FindNextCursorDown(crs.BrotherUp);
            if (CursorHoverOption == null)
            {
                FindNextCursorParentDown(crs.Parent);
            }
        }
    }

    private void FindNextCursorUp(Object crs)
    {
        if (CursorHoverOption == null)
        {
            if (crs != null)
            {
                var op = crs;
                if (op as UIeinteract != null)
                {
                    if (op.GetAttr("visible") == "true")
                    {

                        CursorHoverOption = op as UIeinteract;
                    }
                }                
                if (CursorHoverOption == null )
                {
                    for(var i= crs.Childs.Count -1;i>-1 && CursorHoverOption == null; i--)
                    {                       
                       FindNextCursorUp(crs.Childs[i]);               
                        
                    }                   
                }
                
            }
        }
    }
}