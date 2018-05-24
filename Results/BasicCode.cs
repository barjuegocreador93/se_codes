


void Main()
{
    string strxml = "<Msg>hola mundo</Msg>" +
        "<Msg>hola mundo 2</Msg>" +
        "<Msg>hola mundo 3</Msg>";
    
    var msgs = XML<Messages>.Read(strxml, new Messages());

    msgs.textPanel = _<IMyTextPanel>("msgs");
    msgs.Tick();
    Echo(msgs.ToString());
}



public class Messages : Object
{
    public IMyTextPanel textPanel;
    public Messages()
    {
        ObjectType = "Messages";

    }

    public override Object Types(string typeName)
    {
        if (typeName == "Msg")
        {
            return new Msg();
        }
        return base.Types(typeName);
    }

    public override void Tick()
    {
        textPanel.WritePublicText("");
        base.Tick();
    }

}

public class Msg : Object
{
    string date;
    public Msg()
    {
        ObjectType = "Msg";
        date = "0/0/0";
    }

    public override string Attrs()
    {

        return "date='"+date+"'";
    }

    public override void SetAttrs(string attrs, string value)
    {
        switch(attrs)
        {
            case "date":
                date = value;
                break;
        }
        base.SetAttrs(attrs, value);
    }

    public override void Tick()
    {
        GetMessages().textPanel.WritePublicText(Text,true);
    }

    Messages GetMessages()
    {
        return Parent as Messages;
    }
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
        /*
        for (int i = 0; i < childs.Count; i++)
        {
            var sysOne = childs[i] as SystemOb;
            for (int j = i + 1; j < childs.Count; j++)
            {
                var sysTwo = childs[j] as SystemOb;
                if (sysOne != null && sysTwo != null)
                {
                    sysOne.LinkerSystem(sysTwo);
                    sysTwo.LinkerSystem(sysOne);
                }
            }
        }*/
        LinkerChilds<SystemOb>(ref childs);
    }

    private void LinkerChilds<T>(ref List<Object> childs,T sys1 = null,T sys2 = null, int i=0, int j=1, Func<T,T,int> n=null) where T: Object
    {
        if(i<childs.Count)
        {
            if(sys1==null)
            {
                sys1 = childs[i] as T;
                if(sys1 == null)
                    LinkerChilds<T>(ref childs,null, null, i + 1, i + 2,n);
            }
            if(j< childs.Count)
            {
                if (sys2 == null && sys1 != null)
                {
                    sys2 = childs[j] as T;
                    LinkerChilds(ref childs,sys1, sys2, i, j + 1);
                }
            }else
            {
                LinkerChilds<T>(ref childs, null, null, i+1, i + 2,n);
            }
            if (sys2 != null && sys1 != null)
            {
                n(sys1, sys2);
            }
        }
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
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].Begin();
        }
    }
    public virtual void Tick()
    {
        for (int i = 0; i < childs.Count;)
        {

            childs[i].Tick();
            if (childs[i].deletion)
            {
                childs.RemoveAt(i);
            }
            else i++;
        }
    }

    public virtual void End()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].End();
        }
        deletion = true;

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




public class Nodo<T>
{
    Dictionary<string, Nodo<T>> dirs;
    protected T Automata;
    public bool Last { get; private set; }
    public bool thrue;
    public Nodo(T aut)
    {
        dirs = new Dictionary<string, Nodo<T>>();
        Last = false;
        Automata = aut;
        thrue = false;

    }

    public void AddDir(string key, Nodo<T> d)
    {
        dirs.Add(key, d);
    }



    public void NextNodo(Nodo<T> next, ref string data, int i)
    {
        i++;
        next.Run(ref data, i, this);
    }

    public virtual void Run(ref string data, int i = 0, Nodo<T> lastNodo = null)
    {

        if (i < data.Length)
        {
            if (i == data.Length - 1) Last = true;

            if (dirs.ContainsKey("A-Z"))
            {
                if (data[i] >= 'A' && data[i] <= 'Z')
                {

                    dirs["A-Z"].Run(ref data, i + 1, this);
                    return;
                }


            }
            if (dirs.ContainsKey("a-z"))
            {
                if (data[i] >= 'a' && data[i] <= 'z')
                {
                    dirs["a-z"].Run(ref data, i + 1, this);
                    return;
                }

            }
            if (dirs.ContainsKey("0-9"))
            {
                if (data[i] >= '0' && data[i] <= '9')
                {
                    dirs["0-9"].Run(ref data, i + 1, this);
                    return;
                }
            }
            if (dirs.ContainsKey(data[i].ToString()))
            {

                dirs[data[i].ToString()].Run(ref data, i + 1, this);
                return;

            }
            if (dirs.ContainsKey("Subs"))
            {
                dirs["Subs"].Run(ref data, i + 1, this);
                return;
            }
        }
    }

}


public class XML<T> where T : Object
{
    T Tree;
    Object CurrentChild, NewChild;
    List<Nodo<XML<T>>> nodos;
    bool CurrentReturns;
    string text, nameType, attrName, attrValue;
    public XML(string xmlstr, T container)
    {
        CurrentChild = Tree = container;
        CurrentReturns = false;
        text = "";
        nodos = new List<Nodo<XML<T>>>() {
            new Nodo1(this) ,
            new Nodo2(this) ,
            new Nodo3(this) ,
            new Nodo4(this) ,
            new Nodo5(this) ,
            new Nodo6(this) ,
            new Nodo7(this) ,
            new Nodo8(this) ,
            new Nodo9(this)
        };

        AddDirOnNodo(1, 2, "<");
        AddDirOnNodo(1, 1, "Subs");
        AddDirOnNodo(2, 3, "A-Z");
        AddDirOnNodo(2, 3, "a-z");
        AddDirOnNodo(2, 3, "/");
        AddDirOnNodo(2, 1, "Subs");
        AddDirOnNodo(3, 3, "A-Z");
        AddDirOnNodo(3, 3, "a-z");
        AddDirOnNodo(3, 9, "/");
        AddDirOnNodo(3, 4, " ");
        AddDirOnNodo(3, 1, ">");
        AddDirOnNodo(3, 1, "Subs");
        AddDirOnNodo(4, 4, " ");
        AddDirOnNodo(4, 1, ">");
        AddDirOnNodo(4, 9, "/");
        AddDirOnNodo(4, 5, "A-Z");
        AddDirOnNodo(4, 5, "a-z");
        AddDirOnNodo(4, 1, "Subs");
        AddDirOnNodo(5, 5, "A-Z");
        AddDirOnNodo(5, 5, "a-z");
        AddDirOnNodo(5, 6, "=");
        AddDirOnNodo(5, 1, "Subs");
        AddDirOnNodo(6, 7, "'");
        AddDirOnNodo(6, 1, "Subs");
        AddDirOnNodo(7, 8, "'");
        AddDirOnNodo(7, 7, "Subs");
        AddDirOnNodo(8, 8, " ");
        AddDirOnNodo(8, 5, "A-Z");
        AddDirOnNodo(8, 5, "a-z");
        AddDirOnNodo(8, 1, ">");
        AddDirOnNodo(8, 9, "/");
        AddDirOnNodo(8, 1, "Subs");
        AddDirOnNodo(9, 1, ">");
        AddDirOnNodo(9, 1, "Subs");

        nodos[0].Run(ref xmlstr);
    }

    public static T Read(string strxml, T continer)
    {
        var xml = new XML<T>(strxml, continer);
        return xml.GetObject();

    }

    public T GetObject()
    {
        return Tree;
    }

    private void AddDirOnNodo(int i, int d, string m)
    {
        nodos[i - 1].AddDir(m, nodos[d - 1]);
    }
    private class AXMLNodo : Nodo<XML<T>>
    {

        public AXMLNodo(XML<T> aut) : base(aut)
        {

        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {

            if (data.Length > i) Automata.text += data[i];
            base.Run(ref data, i, lastNodo);
        }

    }

    private class Nodo1 : AXMLNodo
    {
        public Nodo1(XML<T> aut) : base(aut)
        {

            thrue = true;
        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {

            if ((lastNodo as Nodo3 != null || lastNodo as Nodo4 != null))
            {
                if (!Automata.CurrentReturns && data[i - 1] == '>')
                {

                    Automata.NewChild = Automata.Tree.Types(Automata.nameType);
                    if (Automata.NewChild != null)
                    {
                        Automata.CurrentChild.AddChild(Automata.NewChild);
                        Automata.CurrentChild = Automata.NewChild;
                        Automata.text = "";
                        Automata.NewChild = null;
                    }
                    else
                    {
                        Automata.CurrentChild.Text += Automata.text;
                        Automata.text = "";
                    }
                }
                else
                    if (data[i - 1] == '>')
                {
                    Automata.CurrentChild = Automata.CurrentChild.Parent;
                    Automata.CurrentReturns = false;
                    Automata.text = "";
                }

            }
            else
            if (lastNodo as Nodo9 != null)
            {
                if (data[i - 1] == '>')
                {
                    if (Automata.NewChild != null)
                    {
                        Automata.CurrentChild.AddChild(Automata.NewChild);
                        Automata.text = "";
                        Automata.NewChild = null;
                    }
                    else
                    {
                        Automata.NewChild = Automata.Tree.Types(Automata.nameType);
                        if (Automata.NewChild != null)
                            Automata.CurrentChild.AddChild(Automata.NewChild);
                        Automata.text = "";
                    }

                }

            }
            else
            if (lastNodo as Nodo8 != null)
            {
                if (data[i - 1] == '>')
                {
                    if (Automata.NewChild != null)
                    {
                        Automata.CurrentChild.AddChild(Automata.NewChild);
                        Automata.CurrentChild = Automata.NewChild;
                        Automata.text = "";
                        Automata.NewChild = null;
                    }

                }
            }
            if (lastNodo as Nodo1 != null)
            {
                Automata.CurrentChild.Text += Automata.text;
                Automata.text = "";
            }


            base.Run(ref data, i, lastNodo);
        }

    }

    private class Nodo2 : AXMLNodo
    {
        public Nodo2(XML<T> aut) : base(aut)
        {

        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {
            if (Automata.CurrentChild == Automata.Tree && data[i] == '/')
            {
                if (data.Length > i) Automata.text += data[i];
                NextNodo(Automata.nodos[0], ref data, i);
                return;
            }
            base.Run(ref data, i, lastNodo);
        }

    }

    private class Nodo3 : AXMLNodo
    {

        public Nodo3(XML<T> aut) : base(aut)
        {

        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {
            if (lastNodo as Nodo2 != null)
            {
                if (data[i - 1] == '/')
                    Automata.CurrentReturns = true;
                Automata.nameType = data[i - 1].ToString();
            }

            if (data.Length > i)
                if (data[i] != '>' && data[i] != ' ' && data[i] != '/')
                    Automata.nameType += data[i].ToString();

            base.Run(ref data, i, lastNodo);
        }

    }

    private class Nodo4 : AXMLNodo
    {


        public Nodo4(XML<T> aut) : base(aut)
        {

        }

    }

    private class Nodo5 : AXMLNodo
    {
        public Nodo5(XML<T> aut) : base(aut)
        {

        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {
            if (lastNodo as Nodo4 != null)
            {
                Automata.NewChild = Automata.Tree.Types(Automata.nameType);

                if (Automata.NewChild == null)
                {
                    NextNodo(Automata.nodos[0], ref data, i);
                    return;
                }
                Automata.attrName = data[i - 1].ToString();
            }

            if (data.Length > i)
                if (data[i] != '=')
                    Automata.attrName += data[i].ToString();

            base.Run(ref data, i, lastNodo);
        }

    }


    private class Nodo6 : AXMLNodo
    {
        public Nodo6(XML<T> aut) : base(aut)
        {

        }
    }

    private class Nodo7 : AXMLNodo
    {
        public Nodo7(XML<T> aut) : base(aut)
        {

        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {

            if (data.Length > i)
                if (data[i].ToString() != "'")
                    Automata.attrValue += data[i].ToString();
            base.Run(ref data, i, lastNodo);
        }

    }

    private class Nodo8 : AXMLNodo
    {
        public Nodo8(XML<T> aut) : base(aut)
        {

        }
        public override void Run(ref string data, int i = 0, Nodo<XML<T>> lastNodo = null)
        {

            Automata.NewChild.SetAttrs(Automata.attrName, Automata.attrValue);
            base.Run(ref data, i, lastNodo);
        }

    }

    private class Nodo9 : AXMLNodo
    {
        public Nodo9(XML<T> aut) : base(aut)
        {

        }

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
}
public class CConfiguration : Component
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





public class MInputConfig : Model
{
    public MInputConfig(IMyTerminalBlock block) : base(block)
    {
        Columns = "service|blockName:unique";
    }

    
}



