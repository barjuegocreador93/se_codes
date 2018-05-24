public class Network : AppBase
{
    public IMyTerminalBlock ThisProgramerBlock;
    public InputsSystem inputsSystem;
    public InternetSystem InternetSystem;
    public InformationSystem informationSystem;

    public Network() : base()
    {
        ObjectType = "Network";

        inputsSystem = new InputsSystem();
        AddChild(inputsSystem);

        InternetSystem = new InternetSystem();
        AddChild(InternetSystem);

        informationSystem = new InformationSystem();
        AddChild(informationSystem);
    }

}

public Network app = new Network();

void Main()
{
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




public class NetworkValidations
{
    public static bool Ip(string v)
    {
        string[] pip = v.Split('.');

        foreach (string n in pip)
        {
            int x;
            if (!int.TryParse(n, out x) || n == "")
                return false;
        }

        return true;
    }
}
public class NetworkSystem : SystemOb
{
    
    public NetworkSystem():base()
    {
        ObjectType = "NetworkSystem";
    }
    public override void Begin()
    {        
        base.Begin();

    }

    

    public Network GetNetwork()
    {
        return GetAppBase() as Network;
    }
}








public class InputsSystem : NetworkSystem
{
    public CInputConfig cGeneralInput;
    public InputsSystem() : base()
    {
        ObjectType = "InputsSystem";
        cGeneralInput = new CInputConfig();
        AddComponent(cGeneralInput);

    }

    public override void Begin()
    {
        if(GetNetwork().ThisProgramerBlock!=null)
            cGeneralInput.Name = GetNetwork().ThisProgramerBlock.CustomName;
        base.Begin();
    }

    public override void Tick()
    {
        
        base.Tick();
    }


}







public class InternetSystem : NetworkSystem
{
    public Router router;
    public InternetSystem()
    {
        ObjectType = "InternetSystem";
        router = new Router();
        AddComponent(router);
        
    }
}

public class InformationSystem : NetworkSystem
{
    public InformationSystem():base()
    {
        ObjectType = "InformationSystem";
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
    
    
    public AppBase GetAppBase()
    {
        return Parent as AppBase;
    }

    public override void Tick()
    {
        base.Tick();
        for(int i=0;i<childs.Count;i++)
        {
            var compOne = childs[i] as Component;
            for (int j = i+1; j < childs.Count; j++)
            {
                var compTow = childs[j] as Component;
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
        for (int i = 0; i < childs.Count; i++)
        {
            Component auxOne = childs[i] as Component;
            for (int j = 0; j < other.childs.Count; j++)
            {
                Component auxTwo = other.childs[j] as Component;
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
    public Component():base()
    {
        ObjectType = "Component";
    }
    
    public virtual void LinkerComponent(Component other)
    {

    }

    public AppBase GetAppBase()
    {
        var p = (Parent as SystemOb);
        if (p != null)
            return p.GetAppBase();
        return null;
    }

    public SystemOb GetSystem()
    {
        return Parent as SystemOb;
    }

}




public class Object
{
    public Object Parent { get; protected set; }
    protected List<Object> childs;
    public string ObjectType { get; protected set; }
    private int deep;
    protected bool deletion;
    public string Name;
    public string Text;
    public Object()
    {
        childs = new List<Object>();
        Parent = null;
        deletion = false;
        Name = "";
        ObjectType = "Object";
        deep = 0;
        Text = "";

    }

    public void AddChild(Object obj)
    {
        if (obj != null)
        {
            obj.Parent = this;
            childs.Add(obj);
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
            childs.RemoveAt(i);
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

    protected void ShufleByTow<T>(Func<T, T, int> n, T sys1 = null, T sys2 = null, int i = 0, int j = 1) where T : Object
    {
        if (i < childs.Count)
        {
            if (sys1 == null)
            {
                sys1 = childs[i] as T;
                if (sys1 == null)
                    ShufleByTow<T>( n, null, null, i + 1, i + 2);
            }
            if (j < childs.Count)
            {
                if (sys2 == null && sys1 != null)
                {
                    sys2 = childs[j] as T;
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
        size = childs.Count;
        if(i<childs.Count)
        {
            n(childs[i],i);
            if(size>childs.Count)
                ForChilds(n, i);
            else
                ForChilds(n, i+1);
        }
    }

    

    public List<Object> FindChildByName(string name)
    {
        List<Object> result = new List<Object>();
        foreach (Object n in childs)
        {
            if (n.Name == name)
                result.Add(n);
        }
        return result;
    }

    public List<T> FindAKindOfChilds<T>() where T : class
    {
        List<T> result = new List<T>();
        foreach (Object n in childs)
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
        return "name='" + Name + "' ";
    }

    public override string ToString()
    {
        string saltos = "";
        for (int i = 1; i <= deep; i++)
            saltos += "   ";
        string result = saltos + "<" + ObjectType + " " + Attrs();
        if (childs.Count == 0 && Text.Length == 0)
        {
            result += "/>";
            return result;
        }
        result += ">" + Text;
        foreach (Object n in childs)
        {
            result += "\n" + n.ToString();
        }
        result += "\n" + saltos + "</" + ObjectType + ">";


        return result;
    }

    public virtual void SetAttrs(string attrs, string value)
    {
        switch (attrs)
        {
            case "name":
                Name = value;
                break;
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
    


}

public class Table
{
    public List<List<string>> Rows;
    protected char DelColumn;
    protected char DelRow;


    public Table(string text, char delColm = '|', char delRow = '\n')
    {
        DelColumn = delColm;
        DelRow = delRow;
        Rows = new List<List<string>>();
        string[] rows = text.Split(delRow);
        foreach (string c in rows)
        {
            if (c.Length > 0)
                AddRow(c);

        }
    }

    public string RowToString(int i)
    {
        string row="";
        int j = 0;
        foreach(string n in Rows[i])
        {
            row += n;
            j++;
            if(j < Rows[i].Count)
            {
                row += DelColumn;
            }
        }
        return row;
    }
 

    public override string ToString()
    {
        string data = "";
        foreach (List<string> n in Rows)
        {
            int i = 0;
            foreach (string d in n)
            {
                data += d;
                i++;
                if (i < n.Count)
                {
                    data += DelColumn;
                }
                else
                {
                    data += DelRow;
                }

            }

        }
        return data;
    }

    public int GetColumnIndex(int i, string column)
    {
        int j = 0;
        foreach(string n in Rows[i])
        {
            if (n.Contains(column) )return j;
            j++;
        }
        return -1;
    }


    public void AddRow(string row)
    {
        string[] r = row.Split(DelColumn);
        List<string> newRow = new List<string>();
        foreach (string n in r)
        {
            newRow.Add(n);
        }
        Rows.Add(newRow);
    }

    public void FindRowsByExactColumn(int j, string column, ref List<int> rlist)
    {

        int i = 0;
        foreach (List<string> n in Rows)
        {
            if (j >= 0 && j < n.Count)
            {
                if (n[j] == column)
                    rlist.Add(i);
            }
            i++;
        }

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
        public TextObject()
        {
            ObjectType = "TextObject";

        }

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
            text.Text += data[i];
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

}public interface INetworkComponent
{
    Network GetNetwork();
    NetworkSystem GetNetworkSystem();
}
public class CConfigurationNetwork : CConfiguration, INetworkComponent
{
    public CConfigurationNetwork()
    {
        ObjectType = "CConfigurationNetwork";
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




public class CInputConfig : CConfigurationNetwork
{
    public CInputConfig()
    {
        ObjectType = "CInputConfig";
    }

    public override void Tick()
    {
        base.Tick();       
        
        SearchMultiBlocksAsComponent<IMyTextPanel>("message", new CMessage(), GetSystem());
        SearchUniqueBlocksAsComponent<IMyTerminalBlock>("networkConfig", new CNetworkConfig(), GetSystem());
        SearchUniqueBlocksAsComponent<IMyTerminalBlock>("ports", new CPortInput(), GetSystem());
        SearchUniqueBlocksAsComponent<IMyTerminalBlock>("timeStart", new CTimeStart(), GetSystem());
        SearchUniqueBlocksAsComponent<IMyTerminalBlock>("routerCaches", new CRouteCahches(), GetSystem());
        

    }
    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);
        DeleteBlocksAsComponet<CMessage>(other);
        DeleteBlocksAsComponet<CNetworkConfig>(other);
        DeleteBlocksAsComponet<CPortInput>(other);
        DeleteBlocksAsComponet<CTimeStart>(other);
        DeleteBlocksAsComponet<CRouteCahches>(other);

    }

    




}

public class CInternetPort: NetworkComponent
{
    public string namePort;
    public IMyLaserAntenna antenna;
    public IMyTimerBlock timer;
    private Table Resive, Send;
    private bool CanResiveMesage;
    private string ActualName;
    public string PortConnectedName;
    public CInternetPort(string name)
    {
        ObjectType = "CInternetPort";
        namePort = name;
        base.Name = name;
        CanResiveMesage = true;        

    }
    public override void Tick()
    {
        base.Tick();
        UpdateBlocks();
        if (antenna != null && timer != null)
        {
            GetMessagePort();
            SendPortMessage();
        }
        else End();        
        
    }

    private void UpdateBlocks()
    {
        string Timer = GetAppBase().FilterBlock(Name + ".", FilterTimerBlock);
        string Antenna = GetAppBase().FilterBlock(Name + "-", FilterLasserAntena);
        ActualName = Antenna;
        antenna = GetAppBase().GetGemeObject<IMyLaserAntenna>(Antenna);
        timer = GetAppBase().GetGemeObject<IMyTimerBlock>(Timer);
    }

    public void SendMessage(string msg)
    {
        Send = new Table((antenna as IMyTerminalBlock).CustomData);
        Send.AddRow(msg);
        (antenna as IMyTerminalBlock).CustomData = Send.ToString();
    }

    private void SendPortMessage()
    {
        if (!timer.IsCountingDown && HasConnection())
        {
            string chanel = namePort+'-', msgDef="";
            int maxChanel = 512;
            maxChanel -= chanel.Length + 1;

            Send = new Table((antenna as IMyTerminalBlock).CustomData);
            for (int i = 0; i < Send.Rows.Count;)
            {
                string msg = Send.RowToString(i);
                if (maxChanel > msg.Length + 4)
                {
                    msgDef += "{10}" + msg;
                    maxChanel -= msg.Length + 1;
                    Send.Rows.RemoveAt(i);
                }
                else i++;
            }
            if(msgDef.Length>0)
            {                
                chanel += 'm' + msgDef;                
            }
            else
            {                
                chanel += 's';
            }
            if(ActualName != chanel)
            {
                
                (antenna as IMyTerminalBlock).CustomData = Send.ToString();
                (antenna as IMyTerminalBlock).CustomName = chanel;
                IMyTerminalBlock At = antenna as IMyTerminalBlock;
                IMyTerminalBlock Ti = timer as IMyTerminalBlock;
                At.GetActionWithName("OnOff_Off").Apply(At);
                At.GetActionWithName("OnOff_On").Apply(At);
                Ti.GetActionWithName("Start").Apply(Ti);
            }
            

        }


    }

    public bool HasConnection()
    {
        Table data = new Table((antenna as IMyTerminalBlock).DetailedInfo);
        if(data.Rows.Count>=3)
        {
            if (data.Rows[2][0].Contains("Connected to"))
            {
                string d = data.Rows[2][0].Replace("Connected to ", ""), port="";
                
                foreach(char n in d)
                {
                    if (n == '-') break;
                    port += n;
                }
                PortConnectedName = port;
               return true;
            }
                
            
        }
        return false;
    }

    public string GetMessage()
    {
        return (timer as IMyTerminalBlock).CustomData;
    }

    private void GetMessagePort()
    {        
        if (HasConnection())
        {
            Table data = new Table((antenna as IMyTerminalBlock).DetailedInfo.Replace("{10}","\n"));
            data.Rows.RemoveAt(0);
            data.Rows.RemoveAt(0);
            if (data.Rows[0][0].Contains("m") && CanResiveMesage)
            {
                data.Rows[0].RemoveAt(0);
                CanResiveMesage = false;
                ResiveMessages(data.Rows);
            }
            else
            if(data.Rows[0][0].Contains("s"))
            {
                CanResiveMesage = true;
            }                   
            
        }
        
    }

    private void ResiveMessages(List<List<string>> msg)
    {
        Resive = new Table((timer as IMyTerminalBlock).CustomData);
        Resive.Rows.AddRange(msg);
        (timer as IMyTerminalBlock).CustomData = Resive.ToString();
    }

    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);
        Router router = other as Router;
        if(router != null)
        {
            if(HasConnection())
            {
                Resive = new Table((timer as IMyTerminalBlock).CustomData);

                foreach(List<string> n in Resive.Rows)
                {
                    if(n.Count==1)
                    {
                        if(n[0] == "cRouter get credentials")
                        {
                            n.Clear();
                            SendMessage("cRouterC|"+router.IP+"|"+router.NodoType);
                        }
                    }
                    if (router.ResiveCredentail(n))
                        n.Clear();
                    
                }


                (timer as IMyTerminalBlock).CustomData = Resive.ToString();
            }
        }       
        
        
    }
}
public class Router : NetworkComponent
{
    private bool hasAStandarConnection;
    public bool HasAStandarConection { get { return hasAStandarConnection; }}    
    public string NodoType;
    public string IP;
    string parentIp, parentType, parentPortConnetedWith0;
    private bool sendQuestInitRouteConfig;
    bool ResiveQuestInitRouteConfig;

    
    public Router()
    {
        ResiveQuestInitRouteConfig = sendQuestInitRouteConfig=hasAStandarConnection = false;
        NodoType = "";
        ObjectType = "Router";
    }

    public override void Tick()
    {
        base.Tick();
        

    }

    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);

        CNetworkConfig cNetworkConfig = other as CNetworkConfig;
        if(cNetworkConfig != null)
        {
            NodoType = cNetworkConfig.NodoType;
            IP = cNetworkConfig.IP;
            if (cNetworkConfig.mNetworkConfig.Where("config=IP"))
            {
                if (NodoType == "start")
                {
                    IP = "0";
                    cNetworkConfig.mNetworkConfig.SetColumnValue("data", IP);
                    hasAStandarConnection = true;

                }
                else
                if (NodoType == "normal" && ResiveQuestInitRouteConfig)
                {
                    IP = parentIp + "." + parentPortConnetedWith0;
                    cNetworkConfig.mNetworkConfig.SetColumnValue("data", IP);
                    hasAStandarConnection = true;
                }
            }
            else cNetworkConfig.mNetworkConfig.Create("config=IP|data=");

        }

        CInternetPort cInternetPort = other as CInternetPort;
        if(cInternetPort!=null)
        {
            if(cInternetPort.HasConnection())
            {
                if(! hasAStandarConnection)
                {
                    if (!sendQuestInitRouteConfig && cInternetPort.namePort == "0" && !ResiveQuestInitRouteConfig && NodoType != "start")
                    {
                        cInternetPort.SendMessage("cRouter get credentials");
                        sendQuestInitRouteConfig = true;
                        ResiveQuestInitRouteConfig = false;
                        parentPortConnetedWith0 = cInternetPort.PortConnectedName;
                    }
                }
                
            }
            
        }

    }
    
    public bool ResiveCredentail(List<string> msg)
    {
        if(msg.Count==3)
        {
            if(msg[0]=="cRouterC")
            {
                if(sendQuestInitRouteConfig)
                {
                    parentIp = msg[1];
                    parentType = msg[2];
                    ResiveQuestInitRouteConfig = true;
                    sendQuestInitRouteConfig = false;
                    return true;
                }
            }
        }
        return false;
    }
}
public class CRouteCahches : CConfigurationNetwork
{
    public CRouteCahches()
    {
        ObjectType = "CRouteCahches";
    }

    public override void Tick()
    {
        base.Tick();

        SearchMultiBlocksAsComponent<IMyTerminalBlock>("Send", new CCacheSend(), GetNetwork().informationSystem);
        SearchMultiBlocksAsComponent<IMyTerminalBlock>("Resive", new CCacheResive(), GetNetwork().informationSystem);
    }

    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);
        DeleteBlocksAsComponet<CCacheSend>(other);
        DeleteBlocksAsComponet<CCacheResive>(other);
    }
}
public class CTimeStart : Component
{
    public CTimeStart()
    {
        ObjectType = "CTimeStart";
    }

    public override void Tick()
    {
        base.Tick();
        GetAppBase().GetGemeObject<IMyTimerBlock>(Name, "Start");
    }
}public class CConfiguration : Component
{

    public MInputConfig Configurations;
    private Dictionary<string, bool> UniqueBlocksAdd;

    public CConfiguration()
    {
        UniqueBlocksAdd = new Dictionary<string, bool>();
        ObjectType = "CConfiguration";
    }

    public override void Tick()
    {
        base.Tick();
        IMyTerminalBlock block = GetAppBase().GetGemeObject<IMyTerminalBlock>(Name);
        if(block == null)
        {
            End();
            return;
        }
        Configurations = new MInputConfig(block);
        Configurations.Normalitation();
    }


    public void SearchMultiBlocksAsComponent<GameBlock>(string service, Component nuevo, SystemOb Parent) where GameBlock : class
    {
        if (!deletion)
        {


            if (Configurations.Where("service=" + service))
            {

                for (int i = 0; i < Configurations.target.Count;)
                {
                    string MessageTextPanelName = Configurations.table.Rows[Configurations.target[i]][1];

                    var messages = Parent.FindChildByName(MessageTextPanelName);
                    GameBlock tp = Parent.GetAppBase().GetGemeObject<GameBlock>(MessageTextPanelName);
                    if (tp != null)
                    {
                        if (messages.Count == 0)
                        {
                            nuevo.Name = MessageTextPanelName;
                            Parent.SpawnComponet(nuevo);

                        }
                        Configurations.target.RemoveAt(i);
                    }
                    else i++;
                }
                Configurations.Deletion();
            }
        }
    }

    public void SearchUniqueBlocksAsComponent<GameBlock>(string service, Component nuevo, SystemOb parent) where GameBlock : class
    {
        if (!deletion)
        {


            if (!UniqueBlocksAdd.ContainsKey(service))
            {
                UniqueBlocksAdd.Add(service, false);
            }
            if (Configurations.Where("service=" + service))
            {

                string blockName = Configurations.table.Rows[Configurations.target[0]][1];
                GameBlock aux = parent.GetAppBase().GetGemeObject<GameBlock>(blockName);

                if (aux != null)
                {
                    if (!UniqueBlocksAdd[service])
                    {
                        nuevo.Name = blockName;
                        parent.SpawnComponet(nuevo);
                        UniqueBlocksAdd[service] = true;
                    }
                    Configurations.target.RemoveAt(0);
                }
                Configurations.Deletion();
            }
            else UniqueBlocksAdd[service] = false;
        }
    }

    public void DeleteBlocksAsComponet<CObject>(Component other) where CObject : Component
    {
        if (!deletion)
        {
            CObject cObject = other as CObject;
            if (cObject != null)
            {
                if (!Configurations.Where("blockName=" + cObject.Name))
                {

                    cObject.End();

                }
            }
        }

    }

}

//Code Basic
public class Model 
{
    private class Indicator
    {
        public int Column;
        public string Data;
        public Indicator(int column, string data)
        {
            Column = column;
            Data = data;
        }

    }

    public Table table;    
    public IMyTerminalBlock Block;
    public IMyTextPanel Tp;
    private bool privateWhere;
    protected string Columns;
    public List<int> target;
    public Model(IMyTerminalBlock block)
    {
        Columns = "column1:int,unique|column2";
        Block = block;
    }
    public bool Read(bool isPrivate)
    {
        if (isPrivate)
        {
            table = new Table(Block.CustomData);
            return true;
        }
        else
        {
            Tp = Block as IMyTextPanel;
            if (Tp != null)
            {
                table = new Table(Tp.GetPublicText());
                return true;
            }           
        }
        return false;
    }

    public bool Write(bool isPrivate)
    {
        if (isPrivate)
        {
            Block.CustomData = table.ToString();
            return true;
        }
        
        Tp.WritePublicText(table.ToString());
            
        
        return true;
    }

    public bool Create(string cls="column1=data|column2=data", bool isPrivate = true)
    {
        var ColumnsT = new Table(Columns);
        var Data = new Table(cls);
        List<Indicator> positions = new List<Indicator>();
        if(Data.Rows.Count>0)
        {
            int columnR=-1;
            foreach (string n in Data.Rows[0])
            {
                if (Valds(n, ref ColumnsT, ref columnR))
                {
                    string[] colinfo = n.Split('=');

                    Indicator ind = new Indicator(columnR, colinfo[1]);
                    positions.Add(ind);
                }
                else return false;
            }
            List<string> newMode = new List<string>(ColumnsT.Rows[0].Count);

            Tp = Block as IMyTextPanel;
            if (isPrivate) table = new Table(Block.CustomData);
            else
            {

                if (Tp != null)
                {
                    table = new Table(Tp.GetPublicText());
                }
                else return false;
            }
            foreach (Indicator ind in positions)
            {
                newMode.Insert(ind.Column, ind.Data);
            }
             table.Rows.Add(newMode);

            if (isPrivate) Block.CustomData = table.ToString();
            else Tp.WritePublicText(table.ToString());
                return true;
            

        }
        return false;
    }

    public bool Where(string cls="column1=data|column2=data",bool isPrivate=true)
    {
        target = new List<int>();
        privateWhere = isPrivate;
        Tp = Block as IMyTextPanel;
        if (isPrivate) table = new Table(Block.CustomData);
        else
        {

            if (Tp != null)
            {
                table = new Table(Tp.GetPublicText());
            }
            else return false;
        }
        var data = new Table(cls);
        var ColumnsT = new Table(Columns);
        foreach (string n in data.Rows[0])
        {
            string[] clsinf = cls.Split('=');
            if(clsinf.Length==2)
            {
                int j = ColumnsT.GetColumnIndex(0, clsinf[0]);
                table.FindRowsByExactColumn(j, clsinf[1], ref target);
            }
        }
        if (target.Count > 0) return true;
        return false;
    }

    public void SetColumnValue(string column, string value)
    {
        var ColumnsT = new Table(Columns);
        int j = ColumnsT.GetColumnIndex(0, column);
        string[] cdata = ColumnsT.Rows[0][j].Split(':');
        foreach (int t in target)
        {
            
            bool ok=true;
            if(cdata.Length==2)
            {
                if (!validationsIn(cdata[1].Split(','),value,j))
                    ok = false;
            }
            if(ok)table.Rows[t][j] = value;
        }
        if(privateWhere)Block.CustomData = table.ToString();
        else
        {
            if(Tp!=null)
            {
                Tp.WritePublicText(table.ToString());
            }
        }
    }

    public void Deletion()
    {
        for (int i = 0; i < target.Count; i++)
        {
            table.Rows[target[i]].Clear();
        }
        if (privateWhere) Block.CustomData = table.ToString();
        else
        {
            if (Tp != null)
            {
                Tp.WritePublicText(table.ToString());
            }
        }
    }

    public void Normalitation(bool isPrivate=true)
    {
        if(isPrivate)
        {
            table = new Table(Block.CustomData);

        }else
        {
            Tp = Block as IMyTextPanel;
            if (Tp != null)
            {
                table = new Table(Tp.GetPublicText());
            }
            else return;
        }
        var ColumnT = new Table(Columns);

        
        for(int i=0; i<table.Rows.Count;i++)
        {
            if(table.Rows[i].Count!=ColumnT.Rows[0].Count)
            {
                table.Rows.RemoveAt(i);
            }
           
        }

        if (isPrivate) Block.CustomData = table.ToString();
        else
        {
            if (Tp != null)
            {
                Tp.WritePublicText(table.ToString());
            }
        }

    }


    private bool Valds(string data, ref Table ColumnsT, ref int column)
    {
        string[] colinfo = data.Split('=');
        if(colinfo.Length == 2)
        {
                        
            for(int i=0;i<ColumnsT.Rows[0].Count;i++)
            {
                if (ColumnsT.Rows[0][i].Contains(colinfo[0]))
                {
                    column = i;
                    string[] dcol = ColumnsT.Rows[0][i].Split(':');
                    if(dcol.Length==2)
                    {
                        string[] validations = dcol[1].Split(',');
                        if(! validationsIn(validations,colinfo[1],i))
                        {
                            return false;
                        }
                    }
                    return true;

                }
                
            }
            
        }
        return false;
        
    }
    private bool validationsIn(string[] validations, string data, int column)
    {
        table = new Table(Block.CustomData);       
        foreach (string v in validations)
        {
            switch(v)
            {
                case "unique":
                    List<int> m = new List<int>();
                    table.FindRowsByExactColumn(column, data, ref m);
                    if (m.Count > 0) return false;
                    return true;
                case "int":
                    int num;
                    return int.TryParse(data, out num);

                case "float":
                    float fnum;
                    return float.TryParse(data, out fnum);
                

            }
        }
        return true;
    }

}

//end BasicCode



public class CMessage : NetworkComponent
{
    public MMessages Messages;
    private List<List<string>> PrivateMsg;
    private List<List<string>>PublicMsg;
    public string IP;
    public CMessage() : base()
    {
        ObjectType = "CMessage";
        PublicMsg =PrivateMsg = new List<List<string>>();
    }   

    public override void Tick()
    {
        IMyTextPanel textPanel = GetAppBase().GetGemeObject<IMyTextPanel>(Name);
        base.Tick();

        if (textPanel == null)
        {
           End();
            return;
        }

        Messages = new MMessages(textPanel as IMyTerminalBlock)
        {
            Handle = Name
        };
        GetAppBase().GetGemeObject<IMyTextPanel>("debg").WritePublicText(GetNetwork().ToString());
        Messages.Normalitation();
        Messages.Normalitation(false);
        ControllerPrivation();
        ControllerPrivation(false);
        

    }

    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);
        GetMessageToOtherCMessage(other);
        GetMessageToOtherCMessage(other, false);
        CNetworkConfig cNetworkConfig = other as CNetworkConfig;
        if (cNetworkConfig != null)
        {
            IP = cNetworkConfig.IP;
        }    
            
        SendForainMessage(other as CCacheSend);
        SendForainMessage(other as CCacheSend, true);
        
    }

    private void SendForainMessage(CCacheSend cCacheSend, bool isPrivate = true)
    {
        if (cCacheSend != null)
        {
            Messages.Read(isPrivate);
            for (int i = 0; i < Messages.table.Rows.Count;)
            {

                if (Messages.table.Rows[i][0] != IP)
                {

                    cCacheSend.Table.Rows.Add(Messages.table.Rows[i]);
                    Messages.table.Rows.RemoveAt(i);

                }
                else i++;

            }
            Messages.Write(isPrivate);
            cCacheSend.Write();
        }

    }


    private void GetMessageToOtherCMessage(Component other, bool isPrivate = true)
    {
        CMessage cMessage = other as CMessage;
        if (cMessage !=null)
        {

            if (Messages.Where("ip=" + cMessage.IP , isPrivate))
            {

                for (int i = 0; i < Messages.target.Count;i++)
                {

                    if (Messages.table.Rows[Messages.target[i]][1] == cMessage.Name)
                    {
                        cMessage.Messages.WriteMessage(
                        Messages.table.Rows[Messages.target[i]][0],
                        Messages.table.Rows[Messages.target[i]][1],
                        Messages.table.Rows[Messages.target[i]][2],
                        Messages.table.Rows[Messages.target[i]][3]
                        );
                        Messages.table.Rows[Messages.target[i]].Clear();
                    }

                }
                Messages.Write(isPrivate);

            }
        }
    }

    private void ControllerPrivation(bool isPrivate = true)
    {
        Messages.Read(isPrivate);
        PublicMsg.Clear();
        PrivateMsg.Clear();
        for(int i=0;i<Messages.table.Rows.Count;)
        {
            switch(Messages.table.Rows[i][2])
            {
                case "GET":
                    if (isPrivate)
                    {
                        PublicMsg.Add(Messages.table.Rows[i]);
                        Messages.table.Rows.RemoveAt(i);
                    }
                    else i++;
                    break;
                case "POST":
                    if (!isPrivate)
                    {
                        PrivateMsg.Add(Messages.table.Rows[i]);
                        Messages.table.Rows.RemoveAt(i);
                    }
                    else i++;
                    break;

                default:
                    Messages.table.Rows.RemoveAt(i);
                    break;


            }
        }

        Messages.Write(isPrivate);
        Messages.Read(!isPrivate);
        for (int i = 0; i < PublicMsg.Count && (!isPrivate) == false ;i++)
        {
            Messages.table.Rows.Add(PublicMsg[i]);
        }
        for (int i = 0; i < PrivateMsg.Count && (!isPrivate) == true; i++)
        {
            Messages.table.Rows.Add(PrivateMsg[i]);
        }
        Messages.Write(!isPrivate);
    }



}









public class CNetworkConfig : NetworkComponent
{
    public MNetworkConfig mNetworkConfig;   
    public string IP;
    public string ConnectionIP;
    public string AutoIp;
    public string NodoType;

    public CNetworkConfig()
    {
        ObjectType = "CNetworkConfig";
    }

    public override void Tick()
    {
        base.Tick();
        IMyTerminalBlock Block = GetAppBase().GetGemeObject<IMyTerminalBlock>(Name);
        if(Block == null)
        {
            End();
            return;
        }
        mNetworkConfig = new MNetworkConfig(Block);
        mNetworkConfig.Normalitation();
        if(mNetworkConfig.Where("config=IP"))
        {
            IP = mNetworkConfig.table.Rows[mNetworkConfig.target[0]][1];
        }
        if (mNetworkConfig.Where("config=ConnectionIP"))
        {
            ConnectionIP = mNetworkConfig.table.Rows[mNetworkConfig.target[0]][1];
        }

        if (mNetworkConfig.Where("config=AutoIp"))
        {
            AutoIp = mNetworkConfig.table.Rows[mNetworkConfig.target[0]][1];
        }else
        {
            mNetworkConfig.Create("config=AutoIp|data=True");
        }
        if (mNetworkConfig.Where("config=nodoType"))
        {
            NodoType = mNetworkConfig.table.Rows[mNetworkConfig.target[0]][1];
        }
    }
}






//END NETWORK



public class CPortInput : NetworkComponent
{
    
    public MPortInput mPort;
    public bool AutoIp;
    public CPortInput():base()
    {
        ObjectType = "CPortInput";
        AutoIp = false;
    }

    public override void Begin()
    {
        
       
        base.Begin();        
    }

    public override void Tick()
    {
        base.Tick();
        IMyTerminalBlock Block = GetAppBase().GetGemeObject<IMyTerminalBlock>(Name);
        if(Block == null)
        {
            End();
            return;
        }
        mPort = new MPortInput(Block);
        mPort.Normalitation();
        if (AutoIp) AutoIpMananger();

        mPort.Read(true);
        foreach (List<string> n in mPort.table.Rows)
        {
            string newTimer = GetAppBase().FilterBlock(n[0] + ".", FilterTimerBlock);
            string newAntenna = GetAppBase().FilterBlock(n[0] + "-", FilterLasserAntena);
            IMyLaserAntenna antenna = GetAppBase().GetGemeObject<IMyLaserAntenna>(newAntenna);
            IMyTimerBlock timer = GetAppBase().GetGemeObject<IMyTimerBlock>(newTimer);
            var cIntsPts = GetNetwork().InternetSystem.FindChildByName(n[0]);
            if (antenna != null && timer != null && cIntsPts.Count == 0)
            {
                CInternetPort cInternet = new CInternetPort(n[0])
                {
                    antenna = antenna,
                    timer = timer

                };
                GetNetwork().InternetSystem.SpawnComponet(cInternet);
            }
            if (antenna == null || timer == null)
            {
                n.Clear();
            }
        }
        mPort.Write(true);


    }

    private void AutoIpMananger()
    {
        string newTimer = GetAppBase().FilterBlock("Timer Block",FilterTimerBlock);
        string newAntenna = GetAppBase().FilterBlock("Laser Antenna", FilterLasserAntena);
        IMyLaserAntenna antenna = GetAppBase().GetGemeObject<IMyLaserAntenna>(newAntenna);
        IMyTimerBlock timer = GetAppBase().GetGemeObject<IMyTimerBlock>(newTimer);
        
        if(!mPort.Where("port=0"))
        {
            if(antenna != null && timer != null)
            {
                (antenna as IMyTerminalBlock).CustomName = "0-";
                (timer as IMyTerminalBlock).CustomName = "0.";
                if(mPort.Create("port=0|ports=0"))
                {
                    CInternetPort cInternet = new CInternetPort("0")
                    {
                        antenna = antenna,
                        timer =timer
                        
                    };
                    GetNetwork().InternetSystem.SpawnComponet(cInternet);
                }

            }
        }
        else
        {

            int i = mPort.table.Rows.Count - 1;
            string namePort = (int.Parse(mPort.table.Rows[i][0]) + 1).ToString();
            string[] ipa = mPort.table.Rows[i][1].Split('.');
            int digts = ipa[ipa.Length - 1].Length;
            int start = (mPort.table.Rows[i][1].Length) - digts;
            string ip = (mPort.table.Rows[i][1].Remove(start) + namePort);
            if (antenna != null && timer != null)
            {              
                if(mPort.Create("port="+namePort+"|ports="+ip))
                {
                    (antenna as IMyTerminalBlock).CustomName = namePort+"-";
                    (timer as IMyTerminalBlock).CustomName = namePort+".";
                    CInternetPort cInternet = new CInternetPort(namePort)
                    {
                        antenna = antenna,
                        timer = timer
                    };
                    GetNetwork().InternetSystem.SpawnComponet(cInternet);
                }
            }

            
        }
    }

    


    public override void LinkerComponent(Component other)
    {
        base.LinkerComponent(other);
        CNetworkConfig cNetworkConfig = other as CNetworkConfig;
        if(cNetworkConfig != null)
        {
            if (cNetworkConfig.AutoIp == "True")
            {
                AutoIp = true;
            }
            else AutoIp = false;
        }
        CInternetPort cInternetPort = other as CInternetPort;
        if(cInternetPort != null)
        {
            
            
            if(!mPort.Where("port="+cInternetPort.namePort))
            {
                cInternetPort.End();
            }
                
                   
            
        }

        

    }



}



public class CDataCache : NetworkComponent
{
    IMyTerminalBlock block;    
    public Table Table;

    public CDataCache()
    {
        ObjectType = "CDataCache";
    }

    public override void Begin()
    {
        base.Begin();
    }

    public override void Tick()
    {
        base.Tick();
        block = GetAppBase().GetGemeObject<IMyTerminalBlock>(Name);
        if (block==null)
        {
            End();
            return;
        }
        Table = new Table(block.CustomData);
    }
    

    public void Write()
    {
        block.CustomData = Table.ToString();
    }
   
}

public class CCacheSend : CDataCache
{
    public CCacheSend()
    {
        ObjectType = "CCacheSend";
    }

    public override void Tick()
    {
        base.Tick();        
    }
}

public class CCacheResive : CDataCache
{
    public CCacheResive()
    {
        ObjectType = "CCacheSend";
    }
}



public class MInputConfig : Model
{
    public MInputConfig(IMyTerminalBlock block) : base(block)
    {
        Columns = "service|blockName:unique";
    }

    
}



//NETWORK
public class MMessages : Model
{
    public string Handle;
    public MMessages(IMyTerminalBlock block) : base(block)
    {
        Columns = "ip|handle|protocolo|msg";
    }

    public bool WriteMessage(string ip, string divice, string protocolo, string msg)
    {
        msg = msg.Replace('=', '*');
        switch (protocolo)
        {
            case "GET":
                return Create("ip=" + ip +"|handle="+ divice + "|protocolo=" + protocolo + "|msg=" + msg, false);
            case "POST":
                return Create("ip=" + ip + "|handle=" + divice + "|protocolo=" + protocolo + "|msg=" + msg);
        }
        return false;
    }


}
public class MNetworkConfig : Model
{
    public MNetworkConfig(IMyTerminalBlock block):base(block)
    {
        Columns = "config:unique|data";
    }
}


public class MPortInput : Model
{
    public MPortInput(IMyTerminalBlock block):base(block)
    {
        Columns = "port:unique,int|ports";
    }
        
}
