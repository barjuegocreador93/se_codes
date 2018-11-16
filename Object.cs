
/*libs*/using System;
/*libs*/using System.Collections.Generic;

internal interface IObject
{
    void Begin();
    void End();
    void Tick();
}

internal class Object : IObject
{
    public Object Parent { get; set; }
    public List<Object> Children { get; set; }
    
    public string Type { get; protected set; }
    public Object BrotherDown { get; private set; }
    public Object BrotherUp { get; private set; }

    private int deep;
    protected bool deletion;
    public Dictionary<string, string> VarAttrs;    
    
    public Object()
    {
        Children = new List<Object>();
        VarAttrs = new Dictionary<string, string>();
        Parent = null;
        deletion = false;        
        Type = "Object";
        deep = 0;        

    }

    public virtual void AddChild(Object obj)
    {
        if (obj != null)
        {
            obj.Parent = this;
            if(Children.Count>0)
            {
                Children[Children.Count - 1].BrotherDown = obj;
                obj.BrotherUp = Children[Children.Count - 1];
            }
            Children.Add(obj);
            obj.deep = deep + 1;
        }
    }


    public virtual void Begin()
    {
        ForChilds(ChildBegin);
        
    }

    private int ChildBegin(Object c, int i)
    {
        c.Begin();
        return 0;
    }


    public virtual void Tick()
    {
        ForChilds(ChildTick);
    }

    private int ChildTick(Object c,int i)
    {
        c.Tick();
        if (c.deletion)
        {            
            RemoveChildAt(i);
        }        
        return 0;
    }

    public void RemoveChildAt(int i)
    {
        var bup = Children[i].BrotherUp;
        var bdwn = Children[i].BrotherDown;
        if(bup != null && bdwn!=null)
        {
            bup.BrotherDown = bdwn;
            bdwn.BrotherUp = bup;
        }else
        {
            if(bup != null)
            {
                bup.BrotherDown = bdwn;
            }
        }
        Children.RemoveAt(i);
    }

    public virtual void End()
    {
        ForChilds(ChildEnd);
        deletion = true;

    }

    private int ChildEnd(Object c, int i)
    {
        c.End();        
        return 0;
    }

    public void AddListOfChilds<T>(List<T> n)where T: Object
    {
        foreach(T x in n)
        {
            AddChild(x);
        }
    }

    protected void ShufleByTow<T>(Func<T, T, int> n, T sys1 = null, T sys2 = null, int i = 0, int j = 1) where T : Object
    {
        if (i < Children.Count)
        {
            if (sys1 == null)
            {
                sys1 = Children[i] as T;
                if (sys1 == null)
                    ShufleByTow<T>( n, null, null, i + 1, i + 2);
            }
            if (j < Children.Count)
            {
                if (sys2 == null && sys1 != null)
                {
                    sys2 = Children[j] as T;
                    ShufleByTow( n, sys1, sys2, i, j + 1);
                }
            }
            else
            {
                ShufleByTow<T>( n, null, null, i + 1, i + 2);
            }
            if (sys2 != null && sys1 != null)
            {
                n(sys1, sys2);
            }
        }
    }

    public void ForChilds(Func<Object,int,int> n,int i=0, int size = 0)
    {
        size = Children.Count;
        if(i<Children.Count)
        {
            int result = n(Children[i],i);
            if(size>Children.Count && result == 0)
                ForChilds(n, i);
            else if(result == 0)
                ForChilds(n, i+1);
        }
    }

    public void ChangeAllChildsAttr(string attr, string value)
    {
        foreach(Object v in Children)
        {
            v.SetAttribute(attr, value);
            v.ChangeAllChildsAttr(attr, value);
        }
    }

    

    public List<Object> FindChildByName(string name)
    {
        List<Object> result = new List<Object>();
        foreach (Object n in Children)
        {
            if (n.VarAttrs.ContainsKey("name"))
                if(n.VarAttrs["name"]==name)
                    result.Add(n);
        }
        return result;
    }

    public List<T> FindAKindOfChilds<T>() where T : class
    {
        List<T> result = new List<T>();
        foreach (Object n in Children)
        {
            if ((n as T) != null)
                result.Add(n as T);
        }
        return result;
    }

    public virtual Object Types(string typeName, Object parent=null)
    {
        if (Type == typeName)
        {
            return new Object();
        }
        return null;
    }

    public virtual string StrAttributes()
    {
        string result = "";
        foreach (string key in VarAttrs.Keys)
        {
            result += string.Format("{0}='{1}' ", key, VarAttrs[key].ToString());
        }
        return result;
    }

    public string ChildsToString()
    {
        string result = "";
        foreach (Object n in Children)
        {
            result += n.ToString();
        }
        return result;
    }

    public override string ToString()
    {        
        if (Children.Count == 0)                    
            return string.Format("<{0} {1}/>\n", Type, StrAttributes());        
        return string.Format("<{0} {1}>\n{2}</{0}>\n", Type, StrAttributes(),ChildsToString());
    }

    public virtual void SetAttribute(string attrs, string value)
    {
        if (VarAttrs.ContainsKey(attrs))
        {
            VarAttrs[attrs] = (value);
        }
        else VarAttrs.Add(attrs, value);
    }

    public virtual string GetAttribute(string attr)
    {
        if (VarAttrs.ContainsKey(attr))
            return VarAttrs[attr];
        return "";
    }


    public bool FilterTimerBlock(IMyTerminalBlock block)
    {
        IMyTimerBlock la = block as IMyTimerBlock;
        return la != null;
    }



    public bool FilterLasserAntena(IMyTerminalBlock block)
    {
        IMyLaserAntenna la = block as IMyLaserAntenna;
        return la != null;
    }

    public bool FilterRadioAntenna(IMyTerminalBlock block)
    {
        IMyRadioAntenna la = block as IMyRadioAntenna;
        return la != null;
    }

    public class _jq
    {
        Object content;        
        Object cursor;
        public List<Object> Targets { get; private set; }

        public _jq(Object content)
        {
            cursor=this.content = content;
            Targets = new List<Object>();
        }

        public _jq Root()
        {
            if(cursor.Parent != null)
            {
                cursor = cursor.Parent;
                Root();
            }
            return this;
        }

        public _jq FindByType<T>()where T:class
        {
            if (cursor as T != null)
                Targets.Add(cursor);
            foreach (Object v in cursor.Children)
            {
                cursor = v;                
                FindByType<T>();
            }
            return this;
        }

        public _jq jq(string selector)
        {
            Targets.Clear();
            if (selector == "this") This();
            if (selector == "root") Root();            
            
            return this;
        }

        public _jq FindObjectsByAttr(string key, string val = "")
        {
            if(cursor.VarAttrs.ContainsKey(key))
            {
                if (val != "")
                    if (val == cursor.VarAttrs[key])
                        Targets.Add(cursor);
                    else {;}
                else Targets.Add(cursor);
            }
            foreach (Object v in cursor.Children)
            {
                cursor = v;
                FindObjectsByAttr(key, val);
            }
            return this;
        }

        public _jq FindObjectByTag(string objectType)
        {
            if (cursor.Type == objectType)
                Targets.Add(cursor);

            foreach (Object v in cursor.Children)
            {
                cursor = v;
                FindObjectByTag(objectType);
            }
            return this;
        }

        public _jq Xml(string strxml)
        {
            if (strxml != "")            
            foreach (Object v in Targets)
            {
                v.Children.Clear();
                XML.Read(strxml, v);
            }
            
            return this;
        }

        public _jq This()
        {
            Targets.Clear();
            Targets.Add(content);
            cursor = content;
            return this;
        }

        

    }

    public _jq jq(string selector)
    {
        var jQ = new _jq(this);        
        return jQ.jq(selector);
    }

}




