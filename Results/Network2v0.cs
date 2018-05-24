


public  class Network : AppBase
{
    public IMyTerminalBlock ThisProgramerBlock;
    public string request;
    public NConfigSystem NetworkConfigSystem { get; private set; }
    public ConnectionSystem ConnectionSystem { get; private set; }
    public MessageSystem NodeSystem { get; private set; }
    public RequestSystem RequestSystem { get; private set; }
    public TaskSystem TaskSystem { get; private set; }
    public Network() : base()
    {
        ObjectType = "Network";        

        //Create a Configurations
        NetworkConfigSystem = new NConfigSystem();
        AddChild(NetworkConfigSystem);

        ConnectionSystem = new ConnectionSystem();
        AddChild(ConnectionSystem);

        NodeSystem = new MessageSystem();
        AddChild(NodeSystem);

        RequestSystem = new RequestSystem();
        AddChild(RequestSystem);

        TaskSystem = new TaskSystem();
        AddChild(TaskSystem);
    }

    public override void Tick()
    {
        base.Tick();        
    }
}

public Network app = new Network();

public void Main(string arg)
{
    app.request = arg;
    app.ThisProgramerBlock = Me as IMyTerminalBlock;
    app.FilterBlock = _s;
    app.CaptureCube = captureCube;
    app.Begin();
    app.Tick();
}



public class AppBase : Object
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
        ShufleByTow<SystemOb>(Linkersystem);
    }
    

    private int Linkersystem(SystemOb sysOne, SystemOb sysTwo)
    {
        sysOne.LinkerSystem(sysTwo);
        sysTwo.LinkerSystem(sysOne);
        return 0;
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



public class NetworkComponent : Component, INetworkComponent
{
    public NetworkComponent()
    {
        ObjectType = "NetworkComponent";
    }

    public Network GetNetwork()
    {
        return GetAppBase() as Network;
    }

    public NetworkSystem GetNetworkSystem()
    {
        return GetSystem() as NetworkSystem;
    }
}




public class NetworkSystem : SystemOb
{
    public Dictionary<string,Object> UniqueObject { get; private set; }

    public NetworkSystem():base()
    {
        ObjectType = "NetworkSystem";
        UniqueObject = new Dictionary<string, Object>();
    }
    public override void Begin()
    {        
        base.Begin();

    }

    

    public Network GetNetwork()
    {
        return GetAppBase() as Network;
    }

    public Object CreateUniqueObject(Object ob)
    {
        if (!UniqueObject.ContainsKey(ob.ObjectType))
        {
            UniqueObject.Add(ob.ObjectType, ob);
            return ob;
        }
        return null;
    }
}




public interface INetworkComponent
{
    Network GetNetwork();
    NetworkSystem GetNetworkSystem();
}

public class NConfigSystem : NetworkSystem
{
    public string NetworkKey { get; set; }
    public CConfigConnection ConfigConnections; 

    public NConfigSystem()
    {
        ObjectType = "NConfigSystem";       
    }

    public override void Begin()
    {
        base.Begin();
        //Default Node Config Network
        GetNetwork().ThisProgramerBlock.CustomData =
            "<CTimeStart name='nw-start'/>" +
            "<CAntennaMessage name='nw-rantenna'/>" +
            //Current connection status, Node mode, connection automatic mode(on/off)
            "<Connection nodo_type='NodoStart', is_automatic='true'/>" +
            "";
            
        //Secret
        NetworkKey = "AHMHJSKKXX-0002121";
    }

    public override void Tick()
    {        
        string data = GetNetwork().ThisProgramerBlock.CustomData;
        Childs.Clear();
        UniqueObject.Clear();
        XML<NConfigSystem>.Read(data, this);        
        base.Tick();       
        GetNetwork().ThisProgramerBlock.CustomData = this.PrintChilds();
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "CTimeStart":
                return CreateUniqueObject(new CTimeStart());
            case "CAntennaMessage":
                return CreateUniqueObject(new CAntennaMessage());
            case "Connection":
                return CreateUniqueObject(new CConfigConnection());

        }
        return null;
    }

    





}




public class MessageSystem : NetworkSystem
{
    public MessageSystem()
    {
        ObjectType = "MessageSystem";
    }

    public override void Tick()
    {
        var mc =XML<MessageCapsule>.Read(GetNetwork().request, new MessageCapsule());
        var mcl = mc.FindAKindOfChilds<CPackageMessage>();

        foreach (CPackageMessage n in mcl)
        {
            n.MessageType = MessageTypeEnum.ToRead;
        }
        AddListOfChilds(mcl);        
        base.Tick();
    }
    private  class MessageCapsule : Object
    {
        public override Object Types(string typeName)
        {
            switch (typeName)
            {
                case "msg":
                    return new CPackageMessage();
            }
            return null;
        }
    }
    
}


public class RequestSystem : NetworkSystem
{
    public RequestSystem()
    {
        ObjectType = "RequestSystem";
    }
}



public class TaskSystem : NetworkSystem
{
    public TaskSystem()
    {
        ObjectType = "TaskSystem";
    }

    public Dictionary<string, CTaskNetwork> TaskDictionary;


    public bool CreateTask(CTaskNetwork task)
    {
        if(task!=null)
        {
                     
            task.Begin();
            Random r1 = new Random();
            Random r2 = new Random();
            Random r3 = new Random();
            Random r4 = new Random();


            string key=r1.Next(0, 9).ToString()+
            r2.Next(0, 9).ToString()+
            r3.Next(0, 9).ToString()+
            r4.Next(0, 9).ToString();

            if(!TaskDictionary.ContainsKey(key))
            {
                TaskDictionary.Add(key, task);
                AddChild(task);
                return true;
            }            
        }
        return false;
    }

    
    public void OnEndOfTask()
    {
       
    }
}




public class ConnectionSystem : NetworkSystem
{
    public bool HasConnection { get; private set; }
    public bool IsAutomaticConection { get; private set; }
    public string CurrentIp { get; private set; }
    public NodeType NodoType { get; private set; }
    private CTaskConnectionTypeCable taskConnectionTypeCable;
    public ConnectionSystem()
    {
        ObjectType = "ConnectionSystem";
        HasConnection = false;
    }

    public override void Tick()
    {
        base.Tick();
        CConfigConnection data = GetNetwork().NetworkConfigSystem.ConfigConnections;
        if (data != null)
        {
            IsAutomaticConection = data.AutomaticConnection;
            NodoType = data.NodeType;

            if (NodoType == NodeType.NodoStart)
            {
                CurrentIp = "0";
                HasConnection = true;
            }

            if (IsAutomaticConection)
            {
                if(!HasConnection)
                {                   
                    if(NodoType == NodeType.Cable && taskConnectionTypeCable!=null)
                    {
                        taskConnectionTypeCable = new CTaskConnectionTypeCable();
                       if(GetNetwork().TaskSystem.CreateTask(taskConnectionTypeCable))
                       {
                            taskConnectionTypeCable.connectionSystem = this;                            
                       }
                        else
                            taskConnectionTypeCable = null;
                    }
                    else
                    if(NodoType == NodeType.User)
                    {

                    }
                }
                
            }else
            {
                CurrentIp = data.CurrentIp;
                if(CurrentIp != "" && !HasConnection)
                {

                }

            }
            
        }

    }


}


public class SystemOb : Object
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



public class Component : Object
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


public class Object
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
            result += string.Format("{0}='{1}'", key, VarAttrs[key].ToString());
        }
        return result;
    }

    public override string ToString()
    {
        string saltos = "";       
        string result = saltos + "<" + ObjectType + " " + Attrs();
        if (Childs.Count == 0 && Text.Length == 0)
        {
            result += "/>";
            return result;
        }
        result += ">\n" + Text;
        foreach (Object n in Childs)
        {
            result += n.ToString();
        }
        result += saltos + "</" + ObjectType + ">\n";


        return result;
    }

    public virtual void SetAttrs(string attrs, string value)
    {
        if(VarAttrs.ContainsKey(attrs))
        {
            VarAttrs[attrs]=(value);
        }
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

}





IMyTerminalBlock captureCube(string _obj)
{
    return GridTerminalSystem.GetBlockWithName(_obj);
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



public class XML<T> where T : Object
{
    Object CurrentChild, NewChild;
    T Tree;

    private class TextObject : Object
    {        
        public override string ToString()
        {
            return Text;
        }
    }

    TextObject text;

    private XML(string strxml, T container)
    {
        CurrentChild = Tree = container;
        text = null;
        INoObject = -1;
        AttrcName = TypeName = "";
        Run(ref strxml);
    }

    public static Object Read(string strxml, T container)
    {
        var xml = new XML<T>(strxml, container);
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

                    AttrName(ref data, i + 1);

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

    private T GetResult()
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
public partial class CConfigConnection : NetworkComponent
{
    public string CurrentIp { get; set; }    
    public bool AutomaticConnection { get; set; }
    public NodeType NodeType;
    public CConfigConnection()
    {
        ObjectType = "Connection";
        AutomaticConnection = true;
        NodeType = NodeType.NodoStart;        
        CurrentIp = "";
    }


    public override void Tick()
    {
        NConfigSystem Nconfig = GetNetworkSystem() as NConfigSystem;
        if (Nconfig != null)
        {
            Nconfig.ConfigConnections = this;
        }
    }

    public override void LinkerComponent(Component other)
    {
       
    }

    

    public override void SetAttrs(string attrs, string value)
    {
        switch(attrs)
        {
            case "current_ip":
                if(!AutomaticConnection)CurrentIp = value;
                break;
            case "node_type":               

                switch(value)
                {
                    case "Cable":
                        NodeType = NodeType.Cable;
                        break;
                    case "NodoStart":
                        NodeType = NodeType.NodoStart;
                        break;
                    case "User":
                        NodeType = NodeType.User;
                        break;
                    default:
                        NodeType = NodeType.Cable;
                        break;
                }
                break;
            case "is_automatic":
                switch(value)
                {
                    case "true":
                        AutomaticConnection = true;
                        break;
                    case "false":
                        AutomaticConnection = false;
                        break;
                    default:
                        AutomaticConnection = false;
                        break;
                }
                break;

        }
        base.SetAttrs(attrs, value);
    }

    public override string Attrs()
    {
        if(!AutomaticConnection)
        return string.Format("node_type='{1}'  is_automatic='{2}' current_ip='{0}'",
            CurrentIp,
            NodeType.ToString(),
            AutomaticConnection.ToString().ToLower()
            );
        return string.Format("node_type='{0}'  is_automatic='{1}'",
            NodeType.ToString(),
            AutomaticConnection.ToString().ToLower()
            );
    }
}


public enum NodeType
{
    Cable = 0,
    NodoStart = Cable | 1,
    User = Cable | 2,
}



public class CAntennaMessage : NetworkComponent
{
    private IMyRadioAntenna antenna;
    public CAntennaMessage()
    {
        ObjectType = "CAntennaMessage";
        VarAttrs.Add("name", "");
    }

    public override void Tick()
    {
        base.Tick();
        antenna = GetNetwork().GetGemeObject<IMyRadioAntenna>(VarAttrs["name"]);
        if(antenna == null)
        {
            End();
            return;
        }
        
    }

    public override void LinkerComponent(Component other)
    {
       CPackageMessage msg = other as CPackageMessage;
       if (msg != null)
       {            
            if (msg.MessageType == MessageTypeEnum.ToSend)
                SendMessagePackage(msg, msg.IsPublic);
       }
    }

    public void SendMessagePackage(CPackageMessage msg, bool ispublic)
    {
        if (!ispublic)
            antenna.TransmitMessage(msg.ToString());
        else antenna.TransmitMessage(msg.ToString(), MyTransmitTarget.Everyone);
    }
}


public enum MessageTypeEnum
{
    ToRead = 0,
    ToSend = 1,
    ToSave = 2,
    ToDelete = 3,
    
}

public class CPackageMessage : NetworkComponent
{
    
    public MessageTypeEnum MessageType { get; set; }    
    public bool IsPublic { get; set; }
    private ConnectionSystem ConnectionSys;
    public CPackageMessage()
    {
        ObjectType = "msg";
        MessageType = MessageTypeEnum.ToSend;        
        VarAttrs.Add("desteny_ip", "");
        VarAttrs.Add("lastnodo_ip", "");
        VarAttrs.Add("nextnodo_ip", "");
    }

    public override void Tick()
    {
        ConnectionSys = GetNetwork().ConnectionSystem;
        if (ConnectionSys != null)
        {
            if (ConnectionSys.HasConnection)
            {
                if (VarAttrs["desteny_ip"]  == ""
                    && MessageType == MessageTypeEnum.ToRead)
                {
                    ReadMessageAsRequest(new BasicalsRequest());
                    End();
                }else
                if (VarAttrs["desteny_ip"] != ConnectionSys.CurrentIp
                    && MessageType == MessageTypeEnum.ToRead
                    && BetterPathCalculus()
                    )
                {
                   if(MessageTypeEnum.ToDelete != MessageType)
                        MessageType = MessageTypeEnum.ToSend;
                }                
                else
                {
                    MessageType = MessageTypeEnum.ToSave;
                }
            }
            else
            {
                if(VarAttrs["desteny_ip"] == "" && MessageTypeEnum.ToRead==MessageType)
                {
                    ReadMessageAsRequest(new BasicalsRequest());
                }
                End();
            }
        }
        else End();

    }

    private bool BetterPathCalculus()
    {
        if (ConnectionSys != null)
        {
            var lastip = VarAttrs["lastnodo_ip"];
            var currentip = ConnectionSys.CurrentIp;
            var nextip = VarAttrs["nextnodo_ip"];

            if (currentip != nextip)
            {
                MessageType = MessageTypeEnum.ToDelete;
                End();
                return true;
            }
        }     

        return false;
    }

    private void ReadMessageAsRequest<T>(T ob)where T: Object
    {
        var rpm = XML<T>.Read(ToString(), ob);
        var requestList = rpm.FindAKindOfChilds<NetworkRequest>();
        GetNetwork().RequestSystem.AddListOfChilds(requestList);
    }

    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);
    }

    private class BasicalsRequest : Object
    {
        public override Object Types(string typeName)
        {
            switch(typeName)
            {
                
            }
            return null;
        }
    }

    public override void SetAttrs(string attrs, string value)
    {
        switch(attrs)
        {           
            case "message_type":
                switch(value)
                {
                    case "ToRead":
                        MessageType = MessageTypeEnum.ToRead;
                        break;
                    case "ToSend":
                        MessageType = MessageTypeEnum.ToSend;
                        break;
                }
                break;
            case "is_public":
                switch(value)
                {
                    case "true":
                        IsPublic = true;
                        break;
                    case "false":
                        IsPublic = false;
                        break;
                    default:
                        IsPublic = true;
                        break;
                }
                break;
        }
        base.SetAttrs(attrs, value);
        
    }

    public override string Attrs()
    {
        string attrs = string.Format("message_type='{0}'", MessageType.ToString())
            + base.Attrs()
            ;
        if (IsPublic) attrs += "is_public='true'";
        else attrs += "is_public='false'";
        return attrs;
    }

    
}
public enum RequestTypes
{
    Guest = 0,
    RequiereConnection = 1,
}

public enum RequestStatus
{
    Error = 404,
    Normal = 200,
    Server_error = 500,
}
public class NetworkRequest : NetworkComponent
{
    public RequestTypes request_type;
    public RequestStatus request_status;
    public NetworkRequest()
    {
        ObjectType = "request";
        VarAttrs.Add("request_id", "");
        request_type = RequestTypes.Guest;
        request_status = RequestStatus.Normal;
    }

    public RequestSystem GetRequestSystem()
    {
        return GetNetworkSystem() as RequestSystem;
    }

    public override void SetAttrs(string attrs, string value)
    {  
        switch(attrs)
        {
            case "request_type":
                        switch(value)
                        {
                            case "Guest":
                                request_type = RequestTypes.Guest;
                                break;
                            case "RequireConection":
                                request_type = RequestTypes.RequiereConnection;
                                break;                          
                        }
                         break;
            case "request_status":
                switch (value)
                {
                    case "Error":
                        request_status = RequestStatus.Error;
                        break;
                    case "Normal":
                        request_status = RequestStatus.Normal;
                        break;
                    case "Server_error":
                        request_status = RequestStatus.Server_error;
                        break;
                }
                break;
            default:
                    base.SetAttrs(attrs, value);
                break;
        }
        
    }

    public override string Attrs()
    {
        string result = "" + base.Attrs() +
            string.Format("{0}='{1}'","request_type",request_type.ToString())+
            string.Format("{0}='{1}'", "request_status", request_status.ToString())
            ;
        return result;
    }
}




public class CTaskNetwork : NetworkComponent
{
    
    public CTaskNetwork()
    {
        ObjectType = "CTaskNetwork";
        VarAttrs.Add("task_key", "");
    }    

    public override void End()
    {
        base.End();
        TaskSystem tasksystem = Parent as TaskSystem;
        tasksystem.TaskDictionary.Remove(VarAttrs["task_key"]);
    }

}


public class CTaskConnectionTypeCable : CTaskNetwork
{
    public ConnectionSystem connectionSystem;
    public CTaskConnectionTypeCable()
    {
        ObjectType = "CTaskConnectionTypeCable";

    }

    public override void Tick()
    {
        base.Tick();
    }
}


public class CTimeStart : Component
{
    public CTimeStart()
    {
        ObjectType = "CTimeStart";
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