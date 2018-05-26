/*libs*/using System;using System.Collections.Generic;using System.Linq;using System.Text;



internal partial class XJS : XUI
{
    
    public class Base : XML.XMLTree, IObject
    {
        public Base()
        {
            Type = "base";
            
        }

        public virtual void Begin(){}

        public virtual void End(){}

        public virtual void Tick()
        {
            foreach(XML.XMLTree v in Children)
            {
                var b = v as IObject;
                if(b!=null)
                {
                    b.Tick();
                }
            }
        }
    }

    private class Global : Base
    {
        public Global()
        {
            Type = "global";
            AddChild(new Types.Int());
        }

       
    }


}

