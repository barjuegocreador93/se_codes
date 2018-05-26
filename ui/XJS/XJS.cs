/*libs*/using System;using System.Collections.Generic;using System.Linq;using System.Text;

//require XUI framework
//add-XJS.Script.cs
//add-XJS.Parser.cs
//add-Types/Types.cs
internal partial class XJS : XUI
{   
    
    public class Base : XML.XMLTree
    {
        public Base()
        {
            Type = "base";
            
        }       

        public virtual void Tick()
        {
            foreach(XML.XMLTree v in Children)
            {
                var b = v as Base;
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

