/*libs*/using System;using System.Collections.Generic;using System.Linq;using System.Text;

internal class SResource : SystemOb  
{
    protected IMyTerminalBlock block;
    private string text;
    public SResource()
    {
        ObjectType = "SResoruce";
        SetAttrs("name", "");
        block = null;
        text = "";
    }   

    public override void Tick()
    {
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if (block != null)
        {
            if(text != block.CustomData)
            {
                Childs.Clear();
                OnChanges();
                XML.Read(block.CustomData, this);
                text = block.CustomData;
                ForChilds(ChilsBegins);
                
            }
            base.Tick();
            OnWorking();
            text = block.CustomData = ChildsToString();
            
        }
        else
        {
            End();
        }

    }

    protected virtual void OnChanges()
    {
        
    }

    private int ChilsBegins(Object arg1, int arg2)
    {
        if (arg1 as TextObject != null) arg1.End();
        else arg1.Begin();

        return 0;
    }

    public class CResourceItem : Component
    {        

        protected IMyTerminalBlock block;

        public override void Tick()
        {
            block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
            if (block != null)
            {
                OnWorking();
                base.Tick();
            }
            else End();
            
        }

        protected virtual void OnWorking()
        {

        }
    }
    

    protected virtual void OnWorking()
    {

    }

    public override Object Types(string typeName)
    {
        return base.Types(typeName);
    }
}