    internal UI ui { get; set; }


    public Program()
    {
        //keyboard:
        //<ui-keyboard ui-target='myUi' value='up' key='1234'/>
        //<ui-keyboard ui-target='myUi' value='down' key='1234'/>
        //<ui-keyboard ui-target='myUi' value='select' key='1234'/>

        ui = new UI();
        string uixml = "" +
            "<ui-einteract/>" +
            "<ui-einteract/>" +
            "<ui-einteract/>" +
            "";
        
        ui.SetAttrs("name", "myUi");
        ui.SetAttrs("key", "1234");

        ui.jq("this").Xml(uixml);
        var tp = _<IMyTextPanel>("ui-example-tp");
        (tp as IMyTerminalBlock).CustomData = ui.ChildsToString();

        Me.CustomData = ui.Childs[0].BrotherDown.BrotherDown.ToString();
    }


    public void Main(string args)
    {
        var text = _<IMyTerminalBlock>("ui-example-tp").CustomData;
        //1
        ui.jq("this").Xml(text);
        //2
        ui.KeyBoardEnter(args);
        //3
        ui.Tick();
        _<IMyTextPanel>("ui-example-tp").WritePublicText(ui.ChildsToString());
        _<IMyTerminalBlock>("ui-example-tp").CustomData = ui.ChildsToString();
    }

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


internal class AppBase : Object
{
    private bool beginRun;
    private bool endRun;

    public Func<string, IMyTerminalBlock> CaptureCube;
    public Func<string, Func<IMyTerminalBlock, bool>, string> FilterBlock;

    public AppBase():base()
    {
        endRun =beginRun = true;
        ObjectType = "AppBase";
    }

    public override void Begin()
    {
        if(beginRun)
        {
            base.Begin();
            beginRun = false;
        }
        
    }

    public override void Tick()
    {
        base.Tick();       
    }
    

    protected void Linkersystem(SystemOb sysOne, SystemOb sysTwo)
    {
        sysOne.LinkerSystem(sysTwo);
        sysTwo.LinkerSystem(sysOne);        
    }

    public override void End()
    {
        if (endRun)
        {
            base.End();
            endRun = false;
        }
    }

    public T GetGemeObject<T>(string name, string action = "") where T : class
    {
        if (action.Length != 0)
        {
            string[] actions = action.Split(',');
            foreach (string act in actions)
                ChangeActionToObject(name, act);
        }
        return CaptureCube(name) as T;
    }

    private void ChangeActionToObject(string _objeto, string _accion)
    {
        IMyTerminalBlock objeto = CaptureCube(_objeto);
        ITerminalAction accion = objeto.GetActionWithName(_accion);
        accion.Apply(objeto);
    }

}

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

internal class SystemOb : Object
{
    public SystemOb():base()
    {
        ObjectType = "SystemOb";
    }
    public void AddComponent(Component comp)
    {
        AddChild(comp);
    }

    public void SpawnComponet(Component comp)
    {
        AddChild(comp);
        comp.Begin();
    }

    public string PrintChilds(char c = '\n')
    {
        string data = "";
        foreach (Object m in Childs)
        {
            if (m as Component != null)
                data += m.ToString() + c;
        }
        return data;
    }

    public AppBase GetAppBase()
    {
        return Parent as AppBase;
    }

    public override void Tick()
    {
        base.Tick();
        for(int i=0;i<Childs.Count;i++)
        {
            var compOne = Childs[i] as Component;
            for (int j = i+1; j < Childs.Count; j++)
            {
                var compTow = Childs[j] as Component;
                if (compOne != null && compTow != null)
                {
                    compOne.LinkerComponent(compTow);
                    compTow.LinkerComponent(compOne);
                }
            }
        }
    }
    
    public virtual void LinkerSystem(SystemOb other)
    {
        for (int i = 0; i < Childs.Count; i++)
        {
            Component auxOne = Childs[i] as Component;
            for (int j = 0; j < other.Childs.Count; j++)
            {
                Component auxTwo = other.Childs[j] as Component;
                if (auxOne != null && auxTwo != null)
                {
                    auxOne.LinkerComponent(auxTwo);
                    
                }
            }
        }
    }
}

internal class Component : Object
{
    public Component()
    {
        ObjectType = "Component";
    }

    public virtual void LinkerComponent(Component compTow)
    {
        
    }

    public SystemOb GetSystem()
    {
        return Parent as SystemOb;
    }

    public AppBase GetAppBase()
    {
        return GetSystem().Parent as AppBase;
    }
}


internal class Object
{
    public Object Parent { get; protected set; }
    public List<Object> Childs { get; set; }
    
    public string ObjectType { get; protected set; }
    public Object BrotherDown { get; private set; }
    public Object BrotherUp { get; private set; }

    private int deep;
    protected bool deletion;
    public Dictionary<string, string> VarAttrs;    
    public string Text;
    public Object()
    {
        Childs = new List<Object>();
        VarAttrs = new Dictionary<string, string>();
        Parent = null;
        deletion = false;        
        ObjectType = "Object";
        deep = 0;
        Text = "";

    }

    public void AddChild(Object obj)
    {
        if (obj != null)
        {
            obj.Parent = this;
            if(Childs.Count>0)
            {
                Childs[Childs.Count - 1].BrotherDown = obj;
                obj.BrotherUp = Childs[Childs.Count - 1];
            }
            Childs.Add(obj);
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
        var bup = Childs[i].BrotherUp;
        var bdwn = Childs[i].BrotherDown;
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
        Childs.RemoveAt(i);
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
        if (i < Childs.Count)
        {
            if (sys1 == null)
            {
                sys1 = Childs[i] as T;
                if (sys1 == null)
                    ShufleByTow<T>( n, null, null, i + 1, i + 2);
            }
            if (j < Childs.Count)
            {
                if (sys2 == null && sys1 != null)
                {
                    sys2 = Childs[j] as T;
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
        size = Childs.Count;
        if(i<Childs.Count)
        {
            n(Childs[i],i);
            if(size>Childs.Count)
                ForChilds(n, i);
            else
                ForChilds(n, i+1);
        }
    }

    public void ChangeAllChildsAttr(string attr, string value)
    {
        foreach(Object v in Childs)
        {
            v.SetAttrs(attr, value);
            v.ChangeAllChildsAttr(attr, value);
        }
    }

    

    public List<Object> FindChildByName(string name)
    {
        List<Object> result = new List<Object>();
        foreach (Object n in Childs)
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
        foreach (Object n in Childs)
        {
            if ((n as T) != null)
                result.Add(n as T);
        }
        return result;
    }

    public virtual Object Types(string typeName)
    {
        if (ObjectType == typeName)
        {
            return new Object();
        }
        return null;
    }

    public virtual string Attrs()
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
        foreach (Object n in Childs)
        {
            result += n.ToString();
        }
        return result;
    }

    public override string ToString()
    {        
        if (Childs.Count == 0)                    
            return string.Format("<{0} {1}/>\n", ObjectType, Attrs());        
        return string.Format("<{0} {1}>\n{2}</{0}>\n", ObjectType, Attrs(),ChildsToString());
    }

    public virtual void SetAttrs(string attrs, string value)
    {
        if (VarAttrs.ContainsKey(attrs))
        {
            VarAttrs[attrs] = (value);
        }
        else VarAttrs.Add(attrs, value);
    }

    public string GetAttr(string attr)
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
            foreach (Object v in cursor.Childs)
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
            foreach (Object v in cursor.Childs)
            {
                cursor = v;
                FindObjectsByAttr(key, val);
            }
            return this;
        }

        public _jq FindObjectByTag(string objectType)
        {
            if (cursor.ObjectType == objectType)
                Targets.Add(cursor);

            foreach (Object v in cursor.Childs)
            {
                cursor = v;
                FindObjectByTag(objectType);
            }
            return this;
        }

        public _jq Xml(string strxml)
        {
            
            foreach (Object v in Targets)
            {
                v.Childs.Clear();
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





    IMyTerminalBlock captureCube(string _obj)
    {
        return GridTerminalSystem.GetBlockWithName(_obj);
    }

    string debug(string n)
    {
        Echo(n);
        return n;
    }

    void cambiarAccionObjeto(string _objeto, string _accion)
    {
        IMyTerminalBlock objeto = captureCube(_objeto);
        ITerminalAction accion = objeto.GetActionWithName(_accion);
        accion.Apply(objeto);
    }

    bool Filter_Method_Laser_Antenna(IMyTerminalBlock block)
    {
        IMyLaserAntenna la = block as IMyLaserAntenna;
        return la != null;
    }
    bool Filter_Method_Timer_Block(IMyTerminalBlock block)
    {
        IMyTimerBlock la = block as IMyTimerBlock;
        return la != null;
    }
    string _s(string name, Func<IMyTerminalBlock, bool> collect = null)
    {
        List<IMyTerminalBlock> list = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName(name, list, collect);
        if (list.Count != 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CustomName.StartsWith(name))
                    return list[i].CustomName;
            }
        }
        return "";
    }

    T _<T>(string name, string action = "") where T : class
    {
        if (action.Length != 0)
        {
            string[] actions = action.Split(',');
            foreach (string act in actions)
                cambiarAccionObjeto(name, act);
        }
        return captureCube(name) as T;
    }



    void __<T>(string name, string action) where T : class
    {
        string[] names = name.Split(',');
        foreach (string n in names)
        {
            for (int i = 1; exist(n + i.ToString()); i++)
            {
                _<T>(n + i.ToString(), action);
            }
        }
    }

    void __<T>(string name, ref List<string> blocks) where T : class
    {
        string[] names = name.Split(',');
        foreach (string n in names)
        {
            for (int i = 1; exist(n + i.ToString()); i++)
            {
                blocks.Add(n + i.ToString());
            }
        }
    }

    bool exist(string nombre_objeto)
    {
        if (captureCube(nombre_objeto) == null)
            return false;
        return true;
    }

//end BasicCode


internal class TextObject : Object
{
    public override string ToString()
    {
        return Text;
    }
}



internal class XML
{
    Object CurrentChild, NewChild;
    Object Tree;

    

    TextObject text;

    private XML(string strxml, Object container)
    {
        CurrentChild = Tree = container;
        text = null;
        INoObject = -1;
        AttrcName = TypeName = "";
        Run(ref strxml);
    }

    public static Object Read(string strxml, Object container)
    {
        strxml = strxml.Replace('\n', ' ');
        var xml = new XML(strxml, container);
        return xml.GetResult();
    }
    private void Run(ref string data, int i = 0)
    {
        if (i < data.Length)
        {

            if (i + 1 < data.Length && i != INoObject)
            {
                if (data[i] == '<' && data[i + 1] != '/' && data[i + 1] != ' ' && data[i + 1] != '<')
                {
                    AddTextToCurrentChild();
                    typer(ref data, i + 1);
                    return;
                }
                else if (data[i + 1] == '/' && data[i] == '<' && i != INoObject)
                {
                    AddTextToCurrentChild();
                    INoObject = i;
                    fobject = "";
                    FinishObject(ref data, i + 2);
                    return;
                }
            }
            if (text == null)
            {
                text = new TextObject();
            }
            if(data[i]!='\n')text.Text += data[i];
            Run(ref data, i + 1);

            if (i == data.Length - 1) AddTextToCurrentChild();
        }
    }

    string TypeName;
    int TypeNameLength;
    int INoObject;
    private void typer(ref string data, int i = 0)
    {
        if (i < data.Length)
        {

            if (data[i] == ' ')
            {

                if (HasNewChild(i))
                {

                    EndOfBeginObject(ref data, i+1);

                }
                else
                {
                    Run(ref data, INoObject);
                }
            }
            else
            if (data[i] == '>')
            {
                if (NewChild != null)
                {
                    CurrentChild.AddChild(NewChild);
                    CurrentChild = NewChild;
                    NewChild = null;
                    Run(ref data, i + 1);
                }
                else
                if (HasNewChild(i))
                {
                    CurrentChild.AddChild(NewChild);
                    CurrentChild = NewChild;
                    NewChild = null;
                    Run(ref data, i + 1);
                }
                else
                    Run(ref data, INoObject);
            }
            else
            if (data[i] == '/')
            {

                if (i + 1 < data.Length && NewChild != null)
                {
                    if (data[i + 1] == '>')
                    {
                        CurrentChild.AddChild(NewChild);
                        NewChild = null;
                        Run(ref data, i + 2);
                    }
                }
                else
                    if (i + 1 < data.Length && HasNewChild(i))
                    {
                        if (data[i + 1] == '>')
                        {
                            CurrentChild.AddChild(NewChild);
                            Run(ref data, i + 2);
                        }
                    }
                    else
                        Run(ref data, INoObject);
            }
            else
            {
                TypeName += data[i];
                typer(ref data, i + 1);
            }


        }
    }
    string AttrcName;
    private void AttrName(ref string data, int i = 0)
    {
        if (i < data.Length)
        {
            if (data[i] == ' ')
            {
                AttrcName = "";
                EndOfBeginObject(ref data, i + 1);

            }
            else
            if (data[i] == '=')
            {
                if (i + 2 < data.Length)
                {
                    if (data[i + 1].ToString() == "'")
                    {
                        attrValue = "";
                        AttrValue(ref data, i + 2);
                        return;
                    }
                }
                AttrcName = "";
                Run(ref data, INoObject);
            }
            else
            {

                AttrcName += data[i];
                AttrName(ref data, i + 1);
            }


        }
    }

    string attrValue;
    private void AttrValue(ref string data, int i = 0)
    {
        if (i < data.Length)
        {

            if (data[i].ToString() == "'")
            {

                NewChild.SetAttrs(AttrcName, attrValue);
                attrValue = AttrcName = "";
                EndOfBeginObject(ref data, i + 1);
            }
            else
            {

                attrValue += data[i];
                AttrValue(ref data, i + 1);
            }
        }
    }

    private Object GetResult()
    {
        return Tree;
    }

    private void EndOfBeginObject(ref string data, int i = 0)
    {
        if (i < data.Length)
        {
            if (data[i] == ' ')
            {
                EndOfBeginObject(ref data, i + 1);
            }
            else
            if (data[i] == '/' || data[i] == '>')
            {

                typer(ref data, i);
            }
            else
            if (i == data.Length - 1)
            {
                Run(ref data, INoObject);
            }
            else
            {
                AttrName(ref data, i);
            }
        }
    }
    string fobject;
    private void FinishObject(ref string data, int i)
    {
        if (i < data.Length)
        {
            if (data[i] == '>')
            {
                if (fobject == CurrentChild.ObjectType && CurrentChild != Tree)
                {
                    CurrentChild = CurrentChild.Parent;
                    Run(ref data, i + 1);
                }
                else Run(ref data, INoObject);
            }
            else
            {
                fobject += data[i];
                FinishObject(ref data, i + 1);
            }

        }
    }

    private void AddTextToCurrentChild()
    {
        if (text != null)
        {
            CurrentChild.AddChild(text);
            text = null;
        }

    }

    private bool HasNewChild(int i)
    {
        NewChild = Tree.Types(TypeName);
        TypeNameLength = TypeName.Length;
        INoObject = i - TypeNameLength - 1;
        TypeName = "";
        if (NewChild != null)
        {
            return true;
        }
        return false;
    }

}
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