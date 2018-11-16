

internal partial class Network : AppBase
{
    public string Data { get; set; }
    public string Arg { get; set; }
    Ports ports { get; set; }
    public Config config { get; set; }    

    public override void Begin()
    {        
        XML.Read(Data, this);
        base.Begin();
    }

    public override void Tick()
    {
        base.Tick();
        var messageBottle = new MessageBottle();
        XML.Read(Arg, messageBottle);
        AddChild(messageBottle.root);
        if(messageBottle.root!=null) messageBottle.root.Tick();
    }


    /// <summary>
    /// Function for create a new child when is called by XML.Read
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public override Object Types(string typeName, Object parent)
    {
        switch (typeName)
        {
            case "ports":
                if(ports == null && parent as Network != null)
                {
                    ports = new Ports();
                    return ports;
                }

                break;
            case "primary-port":
                if (ports != null && ports.Primary == null && parent as Ports != null)
                {
                    ports.Primary = new PrimaryPort();
                    return ports.Primary;
                }                  
                                    
                break;
            case "port":
                if(parent as Ports != null)
                {
                    return new Port();
                }
                break;                

            case "config":
                if(config == null && parent as Network != null)
                {
                    config = new Config();
                    return config;
                }                    
                break;
               
                
        }
        return null;
    }

    private class Ports : SystemOb
    {
        public PrimaryPort Primary { get; set; }
        public Ports():base()
        {
            Type = "ports";
        }
    }

    private class PrimaryPort : Port
    {
        public PrimaryPort():base()
        {
            Type = "primary-port";
            SetAttribute("name", "");
        }

        public override void Tick()
        {
            base.Tick();            
        }

        
    }

    private class Port : CBlock<IMyTerminalBlock>
    {
        public Port():base()
        {
            Type = "port";
            SetAttribute("block-type", "");
            SetAttribute("type", "");

        }

        internal void SendMessage(MessageBottle n)
        {
            
            switch(VarAttrs["block-type"])
            {
                case "Antenna":
                    
                    MyTransmitTarget m;
                    if (n.MetaData == null) AppBase.Debug("wow");
                    switch (n.MetaData.VarAttrs["method"])
                    {
                        case "Public": m = MyTransmitTarget.Everyone; break;
                        case "Private": m = MyTransmitTarget.Ally; break;
                        case "Protected": m = MyTransmitTarget.Ally; break;
                        default: m =MyTransmitTarget.Default; break;
                    }
                    
                    AppBase.Debug("Sending: " + n.ToString());
                    ((IMyRadioAntenna)Block).TransmitMessage(n.ToString(), m);
                    
                    break;
                case "LaserAntenna":
                    AppBase.Debug("Sending: " + n.ToString());
                    ((IMyLaserAntenna)Block).TransmitMessage(n.ToString());
                    break;

            }
            
        }
    }

    public class Config : SystemOb
    {
        public Config():base()
        {
            SetAttribute("ip", "");
            SetAttribute("namespace", "");
        }
    }

    private class CBlock<T>  : Component where T:class
    {
        
        protected T Block { get; set; }
        public override void Tick()
        {
            if (Block == null && VarAttrs.ContainsKey("name"))
            {
                Block = AppBase.GetGemeObject<T>(VarAttrs["name"]);
                if (Block == null)
                {                    
                    AppBase.Debug("Block with name '" + VarAttrs["name"] + "' not exist.");
                    End();
                }
            }

            if (!VarAttrs.ContainsKey("name"))
            {
                AppBase.Debug("Tag "+Type+" no have a attribute name.");
                End();
            }
        }
    }


    private class MessageBottle : SystemOb
    {
        public Meta MetaData { get; set; }
        public Body MBody { get; set; }
        private bool IsRoot;
        public MessageBottle root;
        public MessageBottle()
        {
            Type = "message-bottle";
            IsRoot = true;
        }

        /// <summary>
        /// Function for create a new child when is called by XML.Read
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override Object Types(string typeName, Object parent)
        {
            switch (typeName)
            {
                case "message-bottle":
                    var p = parent as MessageBottle;
                    if (p != null)
                    {
                        if (p.IsRoot)
                        {
                            root = new MessageBottle();
                            root.IsRoot = false;
                            return root;
                        }
                    }
                    break;

                case "meta":
                    if (parent as MessageBottle != null)
                    {
                        if (((MessageBottle)parent).MetaData == null)
                        {
                            ((MessageBottle)parent).MetaData = new Meta();
                            return ((MessageBottle)parent).MetaData;
                        }
                        
                    }
                    break;
                case "body":
                    {
                        if (parent as MessageBottle != null)
                        {
                            if (((MessageBottle)parent).MBody== null)
                            {
                                ((MessageBottle)parent).MBody = new Body();
                                return ((MessageBottle)parent).MBody;
                            }

                        }
                        break;
                    }
                case "send":
                    if(parent as MessageBottle != null && MetaData == null)
                    {

                    }
                    break;
                    

            }
            return null;
        }

        public override void Tick()
        {
            
            base.Tick();
        }

        public class Meta : Component
        {
            public Meta()
            {
                Type = "meta";
                SetAttribute("ip-desteny", "");
                SetAttribute("ip-origin", "");
                SetAttribute("path", "");
                SetAttribute("port", "");
                SetAttribute("namespace", "");
                //Public | Private
                SetAttribute("method", "");
            }

            public override void Tick()
            {                
                var network = AppBase as Network;
                var mbottle = System as MessageBottle;
                var paths = new List<string>(VarAttrs["path"].Split(','));
                if(network.config != null)
                {                  
                    if (!paths.Contains(network.config.VarAttrs["namespace"]+"/"+network.config.VarAttrs["ip"]))
                    {
                        if(network.config.VarAttrs["namespace"] == VarAttrs["namespace"])
                        {
                            VarAttrs["path"] +=","+network.config.VarAttrs["namespace"] + "/" + network.config.VarAttrs["ip"];
                            if (network.config.VarAttrs["ip"] == VarAttrs["ip-desteny"])
                            {
                                if (VarAttrs["port"] != "")
                                    network.SendPortMessage(mbottle);
                                else AppBase.Debug("Recibing: " + mbottle.MBody.ToString());
                            }
                            else
                                network.SendNetworkMessage(mbottle);
                        }
                        else
                        {
                            var a_msg_nsp = VarAttrs["namespace"].Split('.');
                            var a_nsp = network.config.VarAttrs["namespace"].Split('.');
                            bool namespace_found = false;
                            VarAttrs["path"] += "," + network.config.VarAttrs["namespace"] + "/" + network.config.VarAttrs["ip"];

                            if (a_msg_nsp.Length > a_nsp.Length)
                            {
                                int k = -1;
                                foreach (string j in a_nsp)
                                {
                                    if(a_msg_nsp[++k]!= j)
                                    {
                                        mbottle.End();
                                        return;
                                    }
                                }
                                network.ports.ForChilds((Object n, int i) =>
                                {
                                    var x = n as Port;
                                    if (x != null)
                                    {
                                        if(x.VarAttrs["type"]=="namespace" && x.VarAttrs["name"] == a_msg_nsp[k + 1])
                                        {
                                            x.SendMessage(mbottle);
                                            namespace_found = true;
                                            return -1;
                                        }
                                    }
                                    return 0;
                                });
                                if (!namespace_found)
                                {
                                    network.SendNetworkMessage(mbottle);
                                }
                            }else if(a_msg_nsp.Length < a_nsp.Length)
                            {
                                int k = -1;
                                foreach (string j in a_msg_nsp)
                                {
                                    if (a_nsp[++k] != j)
                                    {
                                        mbottle.End();
                                        return;
                                    }
                                }
                                network.ports.ForChilds((Object n, int i) =>
                                {
                                    var x = n as Port;
                                    if (x != null)
                                    {
                                        if (x.VarAttrs["type"] == "namespace" && x.VarAttrs["name"] == a_msg_nsp[k])
                                        {
                                            x.SendMessage(mbottle);
                                            namespace_found = true;
                                            return -1;
                                        }
                                    }
                                    return 0;
                                });
                            }

                        }
                        
                    }
                    mbottle.End();
                }
                
            }
        }

        public class Body : Component
        {
           public Body()
            {
                Type = "body";
            }
        }
    }

    private void SendNetworkMessage(MessageBottle n)
    {
        ports.Primary.SendMessage(n);
    }

    private void SendPortMessage(MessageBottle n)
    {
        ports.ForChilds((Object x, int i) =>
        {
            var port = x as Port;
            if(port != null)
            {
                if (port.VarAttrs["name"] == n.MetaData.VarAttrs["port"])
                {
                    port.SendMessage(n);
                    return -1;
                }
            }            
            return 0;
        }); 
    }
}

    Network app = new Network();
    public Program()
    {
        app.CaptureCube = captureCube;
        app.Debug = debug;
        app.Data = Me.CustomData;
        app.FilterBlock = _s;
        app.Begin();
    }

    public void Main(string arg)
    {
        app.Arg = arg;
        app.Tick();
    }


internal class AppBase : Object
{
    private bool beginRun;
    private bool endRun;

    public Func<string, string> Debug { get; internal set; }

    public Func<string, IMyTerminalBlock> CaptureCube;
    public Func<string, Func<IMyTerminalBlock, bool>, string> FilterBlock;

    public AppBase():base()
    {
        endRun =beginRun = true;
        Type = "AppBase";
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

internal class SystemOb : Object
{
    public SystemOb():base()
    {
        Type = "SystemOb";
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
        foreach (Object m in Children)
        {
            if (m as Component != null)
                data += m.ToString() + c;
        }
        return data;
    }

    public AppBase AppBase
    {
        get
        {
            return Parent as AppBase;
        }
    }

    public override void Tick()
    {
        base.Tick();        
    }    
    
}

internal class Component : Object
{
    public Component()
    {
        Type = "Component";
    }

    public virtual void LinkerComponent(Component compTow)
    {
        
    }

    public SystemOb System
    {
        get
        {
            return Parent as SystemOb;
        }
    }

    public AppBase AppBase
    {
        get
        { return System.Parent as AppBase; }
    }
}


internal class CComponet : Object
{
    public CComponet()
    {
        Type = "CComponent";
    }

    public Component Componet
    {
        get
         { return Parent as Component; } 
    }

    public SystemOb System
    {
        get
         { return Componet.System; } 
    }

    public AppBase AppBase
    {
        get
         { return Componet.AppBase; } 
    }
}

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






    IMyTerminalBlock captureCube(string _obj)
    {
        return GridTerminalSystem.GetBlockWithName(_obj);
    }

   internal string debug(string n)
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
    public string Text { get; set; }

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
        if (strxml != "")
        {
            strxml = strxml.Replace('\n', ' ');
            var xml = new XML(strxml, container);
            return xml.GetResult();
        }
        return container;            
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
                    if (data[i + 1] == '\'' || data[i + 1] == '"')
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

                NewChild.SetAttribute(AttrcName, attrValue);
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
                if (fobject == CurrentChild.Type && CurrentChild != Tree)
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
        NewChild = Tree.Types(TypeName,CurrentChild);
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

    
}