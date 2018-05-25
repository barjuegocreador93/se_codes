/*libs*/using System;using System.Collections.Generic;

internal class UIcursor : UIelement
{
    public UIcursor()
    {
        ObjectType = "ui-cursor";
    }

    public UIeinteract CursorHoverOption { get; private set; }

    private bool found;
    private bool startfind;

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
            found = false;
            startfind = false;
            switch(key)
            {
                case "up":
                    SetAttrs("keyboard", "up");
                    MoveCursorUP(CursorHoverOption);
                    break;
                case "down":
                    SetAttrs("keyboard", "down");
                    MoveCursorDown(CursorHoverOption);
                    break;

                case "select":
                    SetAttrs("keyboard", "select");
                    SelectCursor();
                    break;
            }
        }
        base.Tick();
    }

    private void MoveCursorDown(Object crs)
    {
        
        if (crs != null && !found)
        {
            if (startfind)
            {
                if (!found)
                {
                    ChangeCursor(crs as UIeinteract);
                }

                if (!found)
                {
                    if (crs.Childs.Count > 0)
                    {
                        startfind = true;
                        MoveCursorDown(crs.Childs[0]);
                        if (!found)
                        {
                            MoveCursorDown(crs.BrotherDown);
                        }
                    }
                    else MoveCursorDown(crs.BrotherDown);

                }
                if (!found && crs.Parent != null && crs.Parent as UIelement != null)
                {
                    MoveCursorDown(crs.Parent.BrotherDown);
                }
            }

            if (!startfind)
            {
                startfind = true;
                if (crs.Childs.Count > 0)
                    MoveCursorDown(crs.Childs[0]);
                if(!found)
                    MoveCursorDown(crs.BrotherDown);
            }
        }
        
    }

    private void MoveCursorUP(Object crs)
    {
        
        if(crs != null && !found)
        {
            if (startfind)
            {
                if (!found)
                {
                    ChangeCursor(crs as UIeinteract);
                }

                if (!found)
                {
                    if (crs.Childs.Count > 0)
                    {
                        startfind = true;
                        MoveCursorUP(crs.Childs[crs.Childs.Count - 1]);
                        if (!found)
                        {
                            MoveCursorUP(crs.BrotherUp);
                        }
                    }else  MoveCursorUP(crs.BrotherUp);

                }
                if (!found && crs.Parent!=null && crs.Parent as UIelement != null)
                {
                    MoveCursorUP(crs.Parent.BrotherUp);
                }
            }            

            if (!startfind)
            {
                startfind = true;
                MoveCursorUP(crs.BrotherUp);                
            }
            
        }

    }

    private void ChangeCursor(UIeinteract bei)
    {
        if(bei!=null)
        if (bei.GetAttr("visible") == "true")
        {
            bei.SetAttrs("hover", "true");
            CursorHoverOption.SetAttrs("hover", "false");
            CursorHoverOption = bei;
            found = true;
        }
    }

    private void FindFristOption(List<UIeinteract> uiol)
    {
        if (uiol.Count > 0)
        {
            var op = uiol[0];
            if (op.GetAttr("visible") == "true")
            {
                if (CursorHoverOption != null) CursorHoverOption.SetAttrs("hover", "false");
                op.SetAttrs("hover", "true");
                CursorHoverOption = op;
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
                if (CursorHoverOption != null) CursorHoverOption.SetAttrs("hover", "false");
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

    
}