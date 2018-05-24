



internal class Network : AppBase
{
    public string CustomName;
    public string Message;

    public SConfig sConfig { get; private set; }
    public SMessage sMessage { get; private set; }
    public Func<string, string> Debug { get; internal set; }
    public bool ReTick { get; internal set; }

    public Network()
    {
        sConfig = new SConfig();
        AddChild(sConfig);

        sMessage = new SMessage();
        AddChild(sMessage);
    }

    public override void Tick()
    {
        
        base.Tick();
        if (ReTick)
        {
            Message = "";
            Tick();
        }
    }


}







    Network app = new Network();




    public void Main(string args)
    {
        //basicCode
        app.CaptureCube = captureCube;
        app.FilterBlock = _s;
        app.CustomName = Me.CustomName;
        app.Message = args;
        app.Debug = debug;

        app.Begin();        
        app.Tick();

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

internal class SConfig : NSResource
{
    public Connection Connection { get; private set; }
    public SConfig()
    {
        ObjectType = "SConfig";
    }

    protected override void OnChanges()
    {
        Connection = null;
    }

    public override void Begin()
    {
        SetAttrs("name", GetNework.CustomName);
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if (block != null)
        {
            base.Begin();
            if(block.CustomData == "")
            {
                block.CustomData =
                "<connection name='alfa1' nick='root1' key='secret'/>" +
                "<wifi-antenna name='nw-wifi-a' type='public'/>" +
                "<xfinder-router name='nw-xfinder-r'/>" 
                ;
                
            }
            
        }
        else End();
        
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "wifi-antenna":
                return new WifiAntenna();
            case "connection":
                if(Connection == null)
                    return Connection = new Connection();
                return null;
            case "xfinder-router":
                return new XFinderRouter();            
             
        }
        return null;
    }
    

    
}


internal class SMessage : NSystem
{
    public override void Tick()
    {        
        XML.Read(GetNework.Message,this);        
        ForChilds(erraseRequestTextOut);        
        base.Tick();
        GetNework.Debug(ChildsToString());
    }

    private int erraseRequestTextOut(Object arg1, int arg2)
    {
        if ((arg1 as TextObject)!=null) arg1.End();
        return 0;
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "request":
                return new Request();

            case "response":
                return new Response();

            case "request-connection":
                return new RFinderRouter();            
        }
        return null;
    }
}

internal class NComponent : Component
{
    public Network GetNetwork
    {
        get { return GetAppBase() as Network; }
    }
}

internal class NSystem : SystemOb
{
    public Network GetNework
    {
        get { return GetAppBase() as Network; }
    }
}


internal class NSResource : SResource
{
    public Network GetNework
    {
       get { return GetAppBase() as Network; }
    }

    internal class NCResourceItem : CResourceItem
    {
        public Network GetNework
        {
            get { return GetAppBase() as Network; }
        }
    }
}

internal class NCComponet : Object
{
    public Component GetComponent { get { return Parent as Component;  } }
    public AppBase GetAppBase()
    {
        return GetComponent.GetAppBase();
    }

    public SystemOb GetSystem()
    {
        return GetComponent.GetSystem();
    }

    public Network GetNetwork
    {
        get {return  GetAppBase() as Network; }
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
            Childs.RemoveAt(i);
        }        
        return 0;
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


internal class WifiAntenna : NSResource.NCResourceItem
{
    private IMyRadioAntenna antenna;
    public WifiAntenna()
    {
        ObjectType = "wifi-antenna";
        SetAttrs("type", "public");        
    }

    protected override void OnWorking()
    {
        antenna = block as IMyRadioAntenna;
        if (antenna == null) End();
    }

    public void SendMessage(string msg)
    {
        switch(VarAttrs["type"])
        {
            case "public":
                antenna.TransmitMessage(msg, MyTransmitTarget.Everyone);
                break;
            case "private":
                antenna.TransmitMessage(msg, MyTransmitTarget.Ally);
                break;            
        }
        
    }    
    
}internal class Connection : NComponent
{
    public string Nick { get; private set; }
    public string AutoConnection { get; private set; }

    public Local Local_ { get; private set; }
    public Connection()
    {       
        ObjectType = "connection";
        SetAttrs("nick","");
        SetAttrs("name", "");
        SetAttrs("auto-connection", "false");
    }

    public override void Begin()
    {
        Local_ = new Local(this);
        GetSystem().AddChild(Local_);
    }

    public override void Tick()
    {
        Childs.Clear();
        Nick = VarAttrs["nick"];
        AutoConnection = VarAttrs["auto-connection"];
        
    }

    public override void LinkerComponent(Component compTow)
    {
        
    }
    public override void End()
    {
        
        base.End();
    }

}



internal class Local : Component
{
    public Connection Connection { get; private set; }
    public string IP { get; private set; }
    public string HasConection { get; private set; }
    public string ParentIP { get; private set; }
    public string NetworkName { get; private set; }
    public Local(Connection c)
    {
        Connection = c;
        ObjectType = "local";
        SetAttrs("ip", "0");
        SetAttrs("has-connection", "false");
        SetAttrs("parent-ip", "");
        SetAttrs("network-name", "");
    }

    public override void Tick()
    {
        IP = VarAttrs["ip"];
        HasConection = VarAttrs["has-connection"];
        ParentIP = VarAttrs["parent-ip"];
        if (VarAttrs["ip"] == "0") VarAttrs["network-name"] = Connection.VarAttrs["name"];
        NetworkName = VarAttrs["network-name"];
    }    
}
internal class XFinderRouter : NSResource.NCResourceItem
{

    public  bool IsSendingFinderingRouter { get; private set; }
    public RouterConfig RouterConfig { get; private set; }

    public XFinderRouter()
    {
        ObjectType = "xfinder-router";       
        SetAttrs("is-finding", "true");        
    }

    

    public override void Begin()
    {
        InitVars();
        base.Begin();
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if (block!=null)
        {
            Text = block.CustomData;
            if (Text == "")
                Text = "<router-config start-find='true'/>";
            RouterConfig = null;
            jq("this").Xml(Text);
            
        }
    }
    protected override void OnWorking()
    {
        InitVars();
        if(IsSendingFinderingRouter)
        {
            var network = GetNework;            
            var rfr = new RFinderRouter();           
            var tfr = new TFinderRouter();
            tfr.xFinderRouter = this;            
            List<Object> msgs = network.sConfig.jq("this").FindByType<WifiAntenna>().Targets;            
            if (msgs.Count>1)
            {
                rfr.CreateRequest("<finding-router />", network.sMessage, tfr);
                VarAttrs["is-finding"] = "false";                
                GetNework.Debug(rfr.VarAttrs["token"]);
                GetNework.Tick();
            }
            foreach (Object v in msgs)
            {
                var a = v as WifiAntenna;
                if(a!=null)
                     a.SendMessage(rfr.ToString());                     
                          
                
            }
        }

       
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(VarAttrs["name"]);
        if(block!=null)
        {
            if (Text != block.CustomData)
            {
                
                Text = block.CustomData;
                if(Text=="")                
                    Text = "<router-config start-find='true'/>";                
                RouterConfig = null;
                jq("this").Xml(Text);
                
            }
            Text = block.CustomData = ChildsToString();
            
        }     
        


    }

    public override string ToString()
    {
        return string.Format("<{0} {1}/>\n",ObjectType,Attrs());
    }

    void InitVars()
    {
        IsSendingFinderingRouter = VarAttrs["is-finding"] == "true" ? true : false;
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "router":
                return new Router();
            case "router-config":
                if(RouterConfig==null)
                    return RouterConfig = new RouterConfig();
                return null;
        }
        return null;
    }
}

internal class RFinderRouter : Request
{
    public RFinderRouter()
    {
        ObjectType = "request-connection";
        SetAttrs("token", "");
    }

    public override void Tick()
    {
        var text = ChildsToString();
        jq("this").Xml(text);       
        base.Tick();
        
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "finding-router":
                return new FindigRouter();
        }
        return null;
    }
    

    internal class FindigRouter : Object
    {
        public FindigRouter()
        {
            ObjectType = "finding-router";
        }

        public override void Tick()
        {
            var network = (Parent as NComponent).GetNetwork;
            var rsfr = new RsFinderRouter();
            rsfr.CreateResponse(
                string.Format("<router nick='{0}' network-name='{1}'/>",
                network.sConfig.Connection.Nick,
                network.sConfig.Connection.Local_.NetworkName)
                );
            
            rsfr.VarAttrs["token"] = Parent.VarAttrs["token"];
            List<Object> wifis = network.sConfig.jq("this").FindByType<WifiAntenna>().Targets;
            foreach(Object v in wifis)
            {
                var a = v as WifiAntenna;
                if(a!=null)
                {
                    a.SendMessage(rsfr.ToString());
                   
                }
                    
                
            }
            
        }
    }
}

internal class RsFinderRouter : Response
{
    public int TimeDead { get; private set; }

    public RsFinderRouter()
    {
        ObjectType = "response-connection";
        SetAttrs("token", "");
        TimeDead = 0;
    }

    public override Object Types(string typeName)
    {
        switch (typeName)
        {
            case "router":
                return new Router();
        }
        return null;
    }

    public override void Tick()
    {       
        if(Parent.VarAttrs["token"] == VarAttrs["token"])
        {
            var t = Parent as TFinderRouter;
            if(t!=null)
            {
                List<Object> rs = jq("this").FindObjectByTag("router").Targets;
                rs.RemoveAt(0);
                foreach (Object d in rs)
                {
                    if (t.xFinderRouter.jq("this").FindObjectsByAttr("nick", d.VarAttrs["nick"]).Targets.Count == 1)
                        if(d as TextObject == null)
                            t.xFinderRouter.AddChild(d);
                }
            }               
            
        } 
        
    }

}


internal class TFinderRouter : Task
{
    internal XFinderRouter xFinderRouter;
    public string Request { get; internal set; }

    public override void Tick()
    {        
        jq("this").Xml(GetNetwork.Message);
        base.Tick();
        GetNetwork.ReTick = true;
        if (Childs.Count == 0) GetNetwork.ReTick = false;
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "response-connection":
                return new RsFinderRouter();
            case "router":
                return new Router();
        }
        return null;
    }

}




internal class Router : Object
{
    public Router()
    {
        ObjectType = "router";
        SetAttrs("try-to-connect", "false");
        SetAttrs("nick", "");
        SetAttrs("network-name", "");
    }

    public override void Tick()
    {
        base.Tick();
    }
}

internal class RouterConfig : NCComponet
{
    public RouterConfig()
    {
        ObjectType = "router-config";
        SetAttrs("start-find", "false");
    }

    public override void Tick()
    {
        var network = GetNetwork;        
        if (network != null)
        {
            if (VarAttrs["start-find"] == "true")
            {
                Parent.VarAttrs["is-finding"] = "true";
                network.ReTick = true;
                VarAttrs["start-find"] = "false";
                GetComponent.ForChilds(eraseRoutes);
            }
            else network.ReTick = false;

        }
        base.Tick();
    }

    private int eraseRoutes(Object r, int i)
    {
        if (r as Router != null)
            GetComponent.Childs.RemoveAt(i);
        return 0;
    }
}


internal class Request : NComponent
{
    public Request()
    {
        ObjectType = "request";
        SetAttrs("last_ip", "");
        SetAttrs("next_ip", "");
        SetAttrs("owner_ip", "");
        SetAttrs("desteny_ip", "");
        SetAttrs("token", "");
    }

    private void CreateToken()
    {
        Random r1 = new Random();        
        string token = string.Format("{0}{1}{2}{3}", r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));
        SetAttrs("token", token);
    }

    public void CreateRequest(string xmlchilds,SystemOb systemTask, Task task)
    {
        jq("this").Xml(xmlchilds);
        CreateToken();        
        task.SetAttrs("token", VarAttrs["token"]);
        systemTask.AddChild(task);
    }

    public void Vars(string last_ip, string next_ip, string owner_ip, string desteny_ip)
    {
        SetAttrs("last_ip", last_ip);
        SetAttrs("next_ip", next_ip);
        SetAttrs("owner_ip", owner_ip);
        SetAttrs("desteny_ip", desteny_ip);       
    }
    

    public override Object Types(string typeName)
    {
        return null;
    }

    public override void Tick()
    {
        base.Tick();
        End();
    }

}



internal class Response : NComponent
{
    public Response()
    {
        ObjectType = "response";
        SetAttrs("last_ip", "");
        SetAttrs("next_ip", "");
        SetAttrs("owner_ip", "");
        SetAttrs("desteny_ip", "");
        SetAttrs("token", "");
        SetAttrs("status", "");
    }

    public void CreateResponse(string xmlchild,string status = "200")
    {
        jq("this").Xml(xmlchild);
        SetAttrs("status", status);
    }

    public void Vars(string last_ip, string next_ip, string owner_ip, string desteny_ip)
    {
        SetAttrs("last_ip", last_ip);
        SetAttrs("next_ip", next_ip);
        SetAttrs("owner_ip", owner_ip);
        SetAttrs("desteny_ip", desteny_ip);
    }

    public override void Tick()
    {
        base.Tick();        
    }


}


internal class Task : NComponent
{  
    
    public int MaxTime;

    public bool HasTaskPettition { get; private set; }
    public int TimeAlive { get; private set; }

    public Task()
    {
        ObjectType = "task";
        SetAttrs("token", "");       
        MaxTime = 10;
        HasTaskPettition = false;
        TimeAlive = 0;
    }

    public override void Tick()
    {
        base.Tick();
        if (TimeAlive >= MaxTime) End();
        TimeAlive++;
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
internal class CTimeStart : Component
{
    public CTimeStart()
    {
        ObjectType = "time-bucle";
        VarAttrs.Add("name", "");
    }   

    public override void Tick()
    {        
        var timer = GetAppBase().GetGemeObject<IMyTimerBlock>(VarAttrs["name"]);
        if(timer!=null)
        {
            GetAppBase().GetGemeObject<IMyTimerBlock>(VarAttrs["name"], "Start");
        }else
        {
            End();
        }
    }
}