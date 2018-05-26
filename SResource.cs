/*libs*/using System;using System.Collections.Generic;using System.Linq;using System.Text;

internal class SResource : SystemOb  
{
    protected IMyTerminalBlock Block { get; set; }
    private string Text;
    public SResource()
    {
        Type = "SResoruce";
        SetAttribute("name", "");
        Block = null;
        Text = "";       
    }   

    public override void Tick()
    {
        Block = AppBase.GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if (Block != null)
        {
            if(Text != Block.CustomData)
            {
                Children.Clear();
                OnChanges();
                XML.Read(Block.CustomData, this);
                Text = Block.CustomData;
                ForChilds(ChilsBegins);
                
            }
            base.Tick();
            OnWorking();
            Text = Block.CustomData = ChildsToString();
            
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

        protected IMyTerminalBlock Block { get; set; }

        public override void Tick()
        {
            Block = AppBase.GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
            if (Block != null)
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