
    internal MyExampleApp.MyApp myApp { get; set; }


    public Program()
    {
        //keyboard:
        //<keypress ui-target='myUi' key='up' secret='1234'/>
        //<keypress ui-target='myUi' key='down' secret='1234'/>
        //<keypress ui-target='myUi' key='right/submit' secret='1234'/>
        //<keypress ui-target='myUi' key='left/abort' secret='1234'/>

        myApp = new MyExampleApp.MyApp();
        //func to filter blocks
        myApp.FilterBlock = _s;
        //func to take blocks by name
        myApp.CaptureCube = captureCube;
        myApp.SetAttribute("name", Me.CustomName);
        myApp.Debug = debug;
        myApp.Begin();
    }


    public void Main(string args)
    {
        myApp.Args = args;
        myApp.Tick();
    }


internal partial class MyExampleApp
{
    internal class MyApp : AppBase
    {
        public MyApp()
        {
            AddChild(new SUIs());
        }

        public string Args { get; internal set; }
        

        public override void Tick()
        {
            base.Tick();
        }
    }

    internal class SUIs : SResource
    {
        public override void Begin()
        {
            SetAttribute("name", AppBase.GetAttribute("name"));
            base.Begin();
        }
        public override Object Types(string typeName)
        {
            switch(typeName)
            {
                case "component-ui":
                    return new ComponentUI();
            }
            return null;
        }

        public override void Tick()
        {
            ForChilds(uis);
            base.Tick();
        }

        private int uis(Object v, int i)
        {
            var ui = v as ComponentUI;
            if(ui!=null)
                ui.KeyPress((Parent as MyApp).Args);
            return 0;
        }
    }

}
    







internal class ComponentUI : SResource.CResourceItem
{
    public XUI.XML.UIController ui { get; set;}
    public IMyTextPanel textPanel { get; private set; }
    public string strXML { get; private set; }

    public ComponentUI()
    {
        Type = "component-ui";
        SetAttribute("name", "");
        SetAttribute("key", "");
    }

    public override void Begin()
    {
        Block = AppBase.GetGemeObject<IMyTerminalBlock>(GetAttribute("name"));
        if (Block != null)
        {
            strXML = Block.CustomData;
            textPanel = Block as IMyTextPanel;
            OnStart();

        }
        else End();
    }

    public void KeyPress(string key)
    {
        XML.Read(key, this);
    }

    protected override void OnWorking()
    {
        textPanel = Block as IMyTextPanel;
        if (textPanel != null)
        {
            if (Block.CustomData != strXML)
            {
                strXML = Block.CustomData;
                OnStrXMLChange();
            }
        }
        else End();
    }

    protected virtual void OnStart()
    {
        UsingUI();
    }

    protected virtual void OnStrXMLChange()
    {
        UsingUI();
    }

    protected virtual void UsingUI()
    {
        ui = XUI.XML.UIController.FromXML(strXML);
        ui.ApplyScreenProperties(textPanel);
        ui.RenderTo(textPanel);
    }

    public override Object Types(string typeName)
    {
        switch(typeName)
        {
            case "keypress":
                return new KeyPress();
        }
        return null;
    }
}


partial class KeyPress : CComponet
{
    public KeyPress()
    {
        //keyboard:
        //<keypress ui-target='myUi' key='up' secret='1234'/>
        //<keypress ui-target='myUi' key='down' secret='1234'/>
        //<keypress ui-target='myUi' key='right/submit' secret='1234'/>
        //<keypress ui-target='myUi' key='left/abort' secret='1234'/>

        Type = "keypress";
        SetAttribute("key", "");
        SetAttribute("ui-target", "");
        SetAttribute("secret", "");
    }

    public override void Tick()
    {
        var cui = Parent as ComponentUI;
        if (cui != null)
        {            
            if(Parent.GetAttribute("key")==GetAttribute("secret")&& Parent.GetAttribute("name") == GetAttribute("ui-target"))
            {
                cui.AppBase.Debug(GetAttribute("key"));
                cui.ui.Call(new List<string>() { "key",GetAttribute("key") });
            }
        }
        End();
    }

    

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
public class XUI
{
    public static class XML{public static Dictionary<string,Func<XML.XMLTree>>NodeRegister=new Dictionary<string,Func<XML.XMLTree>>{{"root",()=>{return new XML.RootNode();}},{"menu",()=>{return new XML.Menu();}},{"menuitem",()=>{return new XML.MenuItem();}},{"progressbar",()=>{return new XML.ProgressBar();}},{"container",()=>{return new XML.Container();}},{"hl",()=>{return new XML.HorizontalLine();}},{"uicontrols",()=>{return new UIControls();}},{"textinput",()=>{return new TextInput();}},{"submitbutton",()=>{return new SubmitButton();}},{"br",()=>{return new Break();}},{"space",()=>{return new Space();}},{"hidden",()=>{return new Hidden();}},{"hiddendata",()=>{return new Hidden();}},{"meta",()=>{return new o();}}};public static XMLTree CreateNode(string a){a=a.ToLower();if(NodeRegister.ContainsKey(a)){return NodeRegister[a]();}else{return new Generic(a);}}public static XMLTree ParseXML(string a){char[]b={' ','\n'};RootNode d=new RootNode();XMLTree e=d;string f;while(a.Length>0){if(a[0]=='<'){if(a[1]=='/'){int g=a.IndexOfAny(b);int h=a.IndexOf('>');int i=(g==-1?h:Math.Min(g,h))-2;f=a.Substring(2,i).ToLower();if(f!=e.Type){throw new Exception("Invalid end tag ('"+f+"(!= "+e.Type+"') found (node has been ended but not started)!");}e=e.GetParent()as XMLTree;a=a.Substring(h+1);}else{int g=a.IndexOfAny(b);int h=Parser.GetNextOutsideQuotes('>',a);int i=(g==-1?h:Math.Min(g,h))-1;f=a.Substring(1,i).ToLower().TrimEnd(new char[]{'/'});XMLTree j=XML.CreateNode(f);if(j==null){int k=a.IndexOf("<");int l=k==-1?a.Length:k;j=new XML.TextNode(a.Substring(0,l).Trim());}e.AddChild(j);if(g!=-1&&g<h){string m=a.Substring(i+2,h-i-2);m=m.TrimEnd(new char[]{'/'});Dictionary<string,string>n=Parser.GetXMLAttributes(m);foreach(string key in n.Keys){j.SetAttribute(key,n[key]);}}if(j.Type=="textnode"||h==-1||a[h-1]!='/'){e=j;}a=a.Substring(h+1);}}else{int h=a.IndexOf("<");int l=h==-1?a.Length:h;XMLTree j=new XML.TextNode(a.Substring(0,l).Trim());if(j.Render(0)!=null){e.AddChild(j);}a=h==-1?"":a.Substring(h);}}return d;}public class RootNode:XMLTree{public RootNode():base(){Type="root";PreventDefault("UP");PreventDefault("DOWN");}public override string GetAttribute(string a){XMLTree b=GetNode(d=>{return d.Type=="meta";});string e;if(b!=null){e=b.GetAttribute(a);}else{e=base.GetAttribute(a);}switch(a){case "width":if(e==null){e="100%";}break;}return e;}public override void SetAttribute(string a,string b){XMLTree d=GetNode(e=>{return e.Type=="meta";});if(d!=null){d.SetAttribute(a,b);}else{base.SetAttribute(a,b);}}public override void UpdateSelectability(XMLTree a){base.UpdateSelectability(a);if(IsSelectable()&&!IsSelected()){SelectFirst();}}public override bool SelectNext(){if(IsSelectable()&&!base.SelectNext()){return SelectNext();}return true;}public override bool SelectPrevious(){if(!base.SelectPrevious()){return SelectPrevious();}return true;}public override void OnKeyPressed(string a){switch(a){case "UP":SelectPrevious();break;case "DOWN":SelectNext();break;}}}public abstract class XMLTree:XMLParentNode{public string Type;private XMLParentNode Parent;private List<string>PreventDefaults;protected List<XMLTree>Children;protected bool Selectable;protected bool ChildrenAreSelectable;private bool Selected;protected int SelectedChild;protected bool Activated;protected Dictionary<string,string>Attributes;private bool _hasUserInputBindings;public bool HasUserInputBindings{get{return _hasUserInputBindings;}set{_hasUserInputBindings=value;if(Parent!=null&&HasUserInputBindings){Parent.HasUserInputBindings=true;}}}public XMLTree(){HasUserInputBindings=false;PreventDefaults=new List<string>();Parent=null;Children=new List<XMLTree>();Selectable=false;ChildrenAreSelectable=false;Selected=false;SelectedChild=-1;Activated=false;Attributes=new Dictionary<string,string>();Type="NULL";SetAttribute("alignself","left");SetAttribute("aligntext","left");SetAttribute("selected","false");SetAttribute("selectable","false");SetAttribute("flow","vertical");}public bool IsSelectable(){return Selectable||ChildrenAreSelectable;}public bool IsSelected(){return Selected;}public XMLTree GetSelectedSibling(){if(!Selected){return null;}if(SelectedChild==-1){return this;}else{return Children[SelectedChild].GetSelectedSibling();}}public virtual void AddChild(XMLTree a){AddChildAt(Children.Count,a);}public virtual void AddChildAt(int a,XMLTree b){if(a>Children.Count){throw new Exception("XMLTree.AddChildAt - Exception: position must be less than number of children!");}Children.Insert(a,b);b.SetParent(this as XMLParentNode);UpdateSelectability(b);}public int NumberOfChildren(){return Children.Count;}public void SetParent(XMLParentNode a){Parent=a;if(HasUserInputBindings&&Parent!=null){Parent.HasUserInputBindings=true;}}public XMLParentNode GetParent(){return Parent;}public XMLTree GetChild(int a){return a<Children.Count?Children[a]:null;}public XMLTree GetNode(Func<XMLTree,bool>a){if(a(this)){return this;}else{XMLTree b=GetChild(0);XMLTree d;for(int e=1;b!=null;e++){d=b.GetNode(a);if(d!=null){return d;}b=GetChild(e);}}return null;}public List<XMLTree>GetAllNodes(Func<XMLTree,bool>a){List<XMLTree>b=new List<XMLTree>();GetAllNodes(a,ref b);return b;}private void GetAllNodes(Func<XMLTree,bool>a,ref List<XMLTree>b){if(a(this)){b.Add(this);}XMLTree d=GetChild(0);for(int e=1;d!=null;e++){d.GetAllNodes(a,ref b);d=GetChild(e);}}public virtual void UpdateSelectability(XMLTree a){bool b=ChildrenAreSelectable;ChildrenAreSelectable=ChildrenAreSelectable||a.IsSelectable();if(Parent!=null&&(Selectable||ChildrenAreSelectable)!=(Selectable||b)){Parent.UpdateSelectability(this);}}public bool SelectFirst(){if(SelectedChild!=-1){Children[SelectedChild].Unselect();}SelectedChild=-1;bool a=(Selectable||ChildrenAreSelectable)?SelectNext():false;return a;}public bool SelectLast(){if(SelectedChild!=-1){Children[SelectedChild].Unselect();}SelectedChild=-1;return(Selectable||ChildrenAreSelectable)?SelectPrevious():false;}public void Unselect(){if(SelectedChild!=-1){Children[SelectedChild].Unselect();}Selected=false;Activated=false;}public virtual bool SelectNext(){bool a=IsSelected();if(SelectedChild==-1||!Children[SelectedChild].SelectNext()){SelectedChild++;while((SelectedChild<Children.Count&&(!Children[SelectedChild].SelectFirst()))){SelectedChild++;}if(SelectedChild==Children.Count){SelectedChild=-1;Selected=Selectable&&!Selected;}else{Selected=true;}}if(!Selected){Unselect();}if(!a&&IsSelected()){OnSelect();}return Selected;}public virtual bool SelectPrevious(){bool a=IsSelected();if(SelectedChild==-1){SelectedChild=Children.Count;}if(SelectedChild==Children.Count||!Children[SelectedChild].SelectPrevious()){SelectedChild--;while(SelectedChild>-1&&!Children[SelectedChild].SelectLast()){SelectedChild--;}if(SelectedChild==-1){Selected=Selectable&&!Selected;}else{Selected=true;}}if(!Selected){Unselect();}if(!a&&IsSelected()){OnSelect();}return Selected;}public virtual void OnSelect(){}public virtual string GetAttribute(string a){if(Attributes.ContainsKey(a)){return Attributes[a];}else if(a=="flowdirection"&&Attributes.ContainsKey("flow")){return Attributes["flow"];}return null;}public virtual void SetAttribute(string a,string b){if(a=="selectable"){bool d=b=="true";if(Selectable!=d){Selectable=d;if(Parent!=null){Parent.UpdateSelectability(this);}}}if(a=="activated"){bool e=b=="true";Activated=e;}if(a=="flowdirection"){Attributes["flow"]=b;}if(a=="inputbinding"){HasUserInputBindings=true;if(Parent!=null){Parent.HasUserInputBindings=true;}}Attributes[a]=b;}public XMLParentNode RetrieveRoot(){XMLParentNode a=this;while(a.GetParent()!=null){a=a.GetParent();}return a;}public void KeyPress(string a){OnKeyPressed(a);if(Parent!=null&&!PreventDefaults.Contains(a)){Parent.KeyPress(a);}}public virtual void OnKeyPressed(string a){switch(a){case "ACTIVATE":ToggleActivation();break;default:break;}}public virtual void ToggleActivation(){Activated=!Activated;}public void PreventDefault(string a){if(!PreventDefaults.Contains(a)){PreventDefaults.Add(a);}}public void AllowDefault(string a){if(PreventDefaults.Contains(a)){PreventDefaults.Remove(a);}}public void FollowRoute(Route a){if(Parent!=null){Parent.FollowRoute(a);}}public virtual Dictionary<string,string>GetValues(Func<XMLTree,bool>a){Logger.log(Type+": GetValues()");Dictionary<string,string>b=new Dictionary<string,string>();string d=GetAttribute("name");string e=GetAttribute("value");if(d!=null&&e!=null){Logger.log("Added entry {{{name}: {value}}}");b[d]=e;}Dictionary<string,string>f;foreach(XMLTree child in Children){f=child.GetValues(a);foreach(string key in f.Keys){if(!b.ContainsKey(key)){b[key]=f[key];}}}return b;}public int GetWidth(int a){string b=GetAttribute("width");if(b==null){return 0;}else{if(b[b.Length-1]=='%'){return(int)(Single.Parse(b.Substring(0,b.Length-1))/100f*a);}else if(a==0){return Int32.Parse(b);}else{return Math.Min(a,Int32.Parse(b));}}}public string Render(int a){List<string>b=new List<string>();int d=GetWidth(a);PreRender(ref b,d,a);RenderText(ref b,d,a);string e=PostRender(b,d,a);return e;}protected virtual void PreRender(ref List<string>a,int b,int d){}protected virtual void RenderText(ref List<string>a,int b,int d){for(int e=0;e<Children.Count;e++){if(GetAttribute("flow")=="vertical"){string f=RenderChild(Children[e],b);if(f!=null){if(e>0&&Children[e-1].Type=="textnode"&&(Children[e].Type=="textnode"||Children[e].Type=="br")){a[a.Count-1]+=f;}else{a.Add(f);}}else{}}else{string f=RenderChild(Children[e],b);if(f!=null){d-=TextUtils.GetTextWidth(f);a.Add(f);}}}}protected virtual string PostRender(List<string>a,int b,int d){string e="";string f=GetAttribute("flow");string g=GetAttribute("alignchildren");string h=GetAttribute("alignself");int i=0;foreach(string segment in a){int j=TextUtils.GetTextWidth(segment);if(j>i){i=j;}}i=Math.Min(d,Math.Max(b,i));if(f=="vertical"){for(int k=0;k<a.Count;k++){switch(g){case "right":a[k]=TextUtils.PadText(a[k],i,TextUtils.PadMode.LEFT);break;case "center":a[k]=TextUtils.CenterText(a[k],i);break;default: 
    a[k]=TextUtils.PadText(a[k],i,TextUtils.PadMode.RIGHT);break;}}e=String.Join("\n",a.ToArray());}else{e=String.Join("",a.ToArray());}if(d-i>0){if(h=="center"){Logger.log("Center element...");e=TextUtils.CenterText(e,d);}else if(h=="right"){Logger.log("Aligning element right...");e=TextUtils.PadText(e,d,TextUtils.PadMode.RIGHT);}}return e;}protected virtual string RenderChild(XMLTree a,int b){Logger.log(Type+".RenderChild()");return a.Render(b);}public void DetachChild(XMLTree a){Children.Remove(a);}public void Detach(){if(GetParent()!=null){GetParent().DetachChild(this);}}}public interface XMLParentNode{bool HasUserInputBindings{get;set;}XMLParentNode GetParent();void UpdateSelectability(XMLTree a);void KeyPress(string a);void FollowRoute(Route a);bool SelectNext();void DetachChild(XMLTree a);}public class TextNode:XMLTree{public string Content;public TextNode(string a):base(){Type="textnode";Content=a.Replace("\n","");Content=Content.Trim(new char[]{'\n',' ','\r'});if(Content==""){Content=null;}}protected override void RenderText(ref List<string>a,int b,int d){}protected override string PostRender(List<string>a,int b,int d){return Content;}}public class Route{static public Dictionary<string,Action<string,UIController>>RouteHandlers=new Dictionary<string,Action<string,UIController>>{{"revert",(e,f)=>{f.RevertUI();}},{"xml",(e,f)=>{XMLTree g=ParseXML(Parser.UnescapeQuotes(e));f.LoadUI(g);}},{"fn",(e,f)=>{if(UIFactories.ContainsKey(e)){UIFactories[e](f);}}}};static Dictionary<string,Action<UIController>>UIFactories=new Dictionary<string,Action<UIController>>();string Definition;public Route(string a){Definition=a;}public void Follow(UIController a){string[]b=Definition.Split(new char[]{':'},2);if(Route.RouteHandlers.ContainsKey(b[0].ToLower())){Route.RouteHandlers[b[0].ToLower()](b.Length>=2?b[1]:null,a);}}static public void RegisterRouteFunction(string a,Action<UIController>b){UIFactories[a]=b;}}public class UIController:XMLParentNode{XMLTree ui;public Stack<XMLTree>UIStack;public string Type;bool UserInputActive;IMyTerminalBlock UserInputSource;TextInputMode UserInputMode;List<XMLTree>UserInputBindings;string InputDataCache;public bool HasUserInputBindings{get{return UserInputActive&&UserInputSource!=null&&UserInputBindings.Count>0;}set{}}public enum TextInputMode{PUBLIC_TEXT,CUSTOM_DATA}public UIController(XMLTree a){Type="CTRL";UIStack=new Stack<XMLTree>();UserInputBindings=new List<XMLTree>();UserInputActive=false;InputDataCache="";ui=a;ui.SetParent(this);if(GetSelectedNode()==null&&ui.IsSelectable()){ui.SelectFirst();}CollectUserInputBindings();}public static UIController FromXML(string a){XMLTree b=XML.ParseXML(a);return new UIController(b);}public void ApplyScreenProperties(IMyTextPanel a){if(ui.GetAttribute("fontcolor")!=null){string b=ui.GetAttribute("fontcolor");b="FF"+b.Substring(b.Length-2,2)+b.Substring(b.Length-4,2)+b.Substring(b.Length-6,2);Color d=new Color(uint.Parse(b,System.Globalization.NumberStyles.AllowHexSpecifier));a.SetValue<Color>("FontColor",d);}if(ui.GetAttribute("fontsize")!=null){a.SetValue<Single>("FontSize",Single.Parse(ui.GetAttribute("fontsize")));}if(ui.GetAttribute("backgroundcolor")!=null){string b=ui.GetAttribute("backgroundcolor");b="FF"+b.Substring(b.Length-2,2)+b.Substring(b.Length-4,2)+b.Substring(b.Length-6,2);Color d=new Color(uint.Parse(b,System.Globalization.NumberStyles.AllowHexSpecifier));a.SetValue<Color>("BackgroundColor",d);}}public void Call(List<string>a){switch(a[0]){case "key":XMLTree b=GetSelectedNode();if(b!=null){b.KeyPress(a[1].ToUpper());}break;case "refresh":string d=ui.GetAttribute("refresh");if(d!=null){FollowRoute(new Route(d));}break;case "revert":RevertUI();break;default:break;}return;}public void LoadXML(string a){LoadUI(XML.ParseXML(a));}public void LoadUI(XMLTree a){if(ui.GetAttribute("historydisabled")==null||ui.GetAttribute("historydisabled")!="true"){UIStack.Push(ui);}if(a.GetAttribute("revert")!=null&&a.GetAttribute("revert")=="true"){RevertUI();}else{ui=a;ui.SetParent(this);}UserInputBindings=new List<XMLTree>();CollectUserInputBindings();}public void ClearUIStack(){UIStack=new Stack<XMLTree>();}public void RevertUI(){Logger.log("UIController: RevertUI():");if(UIStack.Count==0){Logger.log("Error: Can't revert: UI stack is empty.");return;}ui=UIStack.Pop();ui.SetParent(this);}public string Render(){return ui.Render(0);}public void RenderTo(IMyTextPanel a){int b=0;string d=a.BlockDefinition.SubtypeId;if(d=="LargeTextPanel"||d=="SmallTextPanel"){b=658;}else if(d=="LargeLCDPanel"||d=="SmallLCDPanel"){b=658;}else if(d=="SmallLCDPanelWide"||d=="LargeLCDPanelWide"){b=1316;}else if(d=="LargeBlockCorner_LCD_1"||d=="LargeBlockCorner_LCD_2"||d=="SmallBlockCorner_LCD_1"||d=="SmallBlockCorner_LCD_2"){}else if(d=="LargeBlockCorner_LCD_Flat_1"||d=="LargeBlockCorner_LCD_Flat_2"||d=="SmallBlockCorner_LCD_Flat_1"||d=="SmallBlockCorner_LCD_Flat_2"){}int e=(int)(((float)b)/a.GetValue<Single>("FontSize"));string f=ui.Render(e);a.WritePublicText(f);}public void KeyPress(string a){switch(a){case "LEFT/ABORT":RevertUI();break;}}public XMLTree GetSelectedNode(){XMLTree a=ui.GetSelectedSibling();return a;}public XMLTree GetNode(Func<XMLTree,bool>a){return ui.GetNode(a);}public List<XMLTree>GetAllNodes(Func<XMLTree,bool>a){return ui.GetAllNodes(a);}public void UpdateSelectability(XMLTree a){}public void FollowRoute(Route a){a.Follow(this);}public XMLParentNode GetParent(){return null;}public Dictionary<string,string>GetValues(){return GetValues(a=>true);}public Dictionary<string,string>GetValues(Func<XMLTree,bool>a){if(ui==null){return null;}return ui.GetValues(a);}public string GetPackedValues(Func<XMLTree,bool>a){return Parser.PackData(GetValues(a)).ToString();}public void DetachChild(XMLTree a){if(a==ui){ui=null;}}public string GetPackedValues(){return GetPackedValues(a=>true);}public bool SelectNext(){return ui.SelectNext();}public void SetUserInputSource(IMyTerminalBlock a,TextInputMode b){if(b==TextInputMode.PUBLIC_TEXT&&(a as IMyTextPanel)==null){throw new Exception("Only Text Panels can be used as user input if PUBLIC_TEXT mode is selected!");}UserInputSource=a;UserInputMode=b;}public void EnableUserInput(){UserInputActive=true;}public void DisableUserInput(){UserInputActive=false;}public void RegisterInputBinding(XMLTree a){UserInputBindings.Add(a);}public bool UpdateUserInput(){if(!UserInputActive||UserInputSource==null){return false;}string a=null;switch(UserInputMode){case TextInputMode.CUSTOM_DATA:a=UserInputSource.CustomData;break;case TextInputMode.PUBLIC_TEXT:a=(UserInputSource as IMyTextPanel).GetPublicText();break;}bool b=true;if(a==null||a==InputDataCache){b=false;}string d;string e;foreach(XMLTree node in UserInputBindings){d=node.GetAttribute("inputbinding");if(d!=null){e=node.GetAttribute(d.ToLower());if(!b&&e!=null&&e!=InputDataCache){a=e;b=true;}else if(b){node.SetAttribute(d.ToLower(),a);}}}if(b){InputDataCache=a;}switch(UserInputMode){case TextInputMode.CUSTOM_DATA:if(UserInputSource!=null){UserInputSource.CustomData=InputDataCache;}break;case TextInputMode.PUBLIC_TEXT:(UserInputSource as IMyTextPanel).WritePublicText(InputDataCache);break;}return b;}private void CollectUserInputBindings(){XMLTree a;Queue<XMLParentNode>b=new Queue<XMLParentNode>();b.Enqueue(ui);while(b.Count!=0){a=b.Dequeue()as XMLTree;if(!a.HasUserInputBindings){}if(a!=null&&a.HasUserInputBindings){for(int d=0;d<a.NumberOfChildren();d++){b.Enqueue(a.GetChild(d));}if(a.GetAttribute("inputbinding")!=null){RegisterInputBinding(a);}}}}}public abstract class UIFactory{private int Count;private int Max;private List<UIController>UIs;public UIFactory():this(null){}public UIFactory(List<UIController>a){if(a==null){UIs=new List<UIController>();}UIs=a;}public abstract XMLTree Render(UIController a);protected void UpdateUIs(XMLTree a){foreach(UIController ui in UIs){ui.LoadUI(a);}}}public class Generic:XMLTree{public Generic(string a):base(){Type=a.ToLower();}}public class Menu:XMLTree{public Menu():base(){Type="menu";}public override void AddChild(XMLTree a){if(a.Type!="menuitem"&&a.IsSelectable()){throw new Exception("ERROR: Only children of type <menupoint> or children that are not selectable are allowed!"+" (type was: <"+a.Type+">)");}base.AddChild(a);}protected override string RenderChild(XMLTree a,int b){string d="";string e="     ";if(a.Type=="menuitem"){d+=(a.IsSelected()?">> ":e);}d+=base.RenderChild(a,b);return d;}}public class MenuItem:XMLTree{Route TargetRoute;public MenuItem():this(null){}public MenuItem(Route a):base(){Type="menuitem";Selectable=true;SetRoute(a);PreventDefault("RIGHT/SUBMIT");}public override void SetAttribute(string a,string b){switch(a){case "route":SetRoute(new Route(b));if(TargetRoute==null){}else{}break;default:base.SetAttribute(a,b);break;}}public override void OnKeyPressed(string a){switch(a){case "RIGHT/SUBMIT":if(TargetRoute!=null){FollowRoute(TargetRoute);}else{}break;}base.OnKeyPressed(a);}public void SetRoute(Route a){TargetRoute=a;}}public class ProgressBar:XMLTree{float StepSize{get{float a;if(!Single.TryParse(GetAttribute("stepsize"),out a)){return 0.1f;}return a;}set{string b=Math.Max(0.001f,Math.Min(0.009f,value)).ToString();if(b.Length>5){b+=b.Substring(0,5);}SetAttribute("stepsize",b);}}public float FillLevel{get{float a;if(!Single.TryParse(GetAttribute("value"),out a)){return 0.0f;}return a;}set{string b=Math.Max(0f,Math.Min(1f,value)).ToString();if(b.Length>5){b=b.Substring(0,5);}SetAttribute("value",b);}}public ProgressBar():this(0f){}public ProgressBar(float a):this(a,false){}public ProgressBar(float a,bool b):base(){Type="progressbar";PreventDefault("LEFT/ABORT");PreventDefault("RIGHT/SUBMIT");SetAttribute("width","500");SetAttribute("filledstring","|");SetAttribute("emptystring","'");SetAttribute("value",a.ToString());SetAttribute("stepsize","0.05");SetAttribute("selectable",b?"true":"false");}public void IncreaseFillLevel(){FillLevel+=StepSize;}public void DecreaseFillLevel(){FillLevel-=StepSize;}public override void 
    OnKeyPressed(string a){switch(a){case "LEFT/ABORT":DecreaseFillLevel();break;case "RIGHT/SUBMIT":IncreaseFillLevel();break;}base.OnKeyPressed(a);}protected override void RenderText(ref List<string>a,int b,int d){string e=IsSelected()?">":"  ";string f=IsSelected()?"<":"  ";string g=f+"[";float h=FillLevel;string i=GetAttribute("filledstring");string j=GetAttribute("emptystring");int k=(b-2*TextUtils.GetTextWidth("[]"));g+=TextUtils.CreateStringOfLength(i,(int)(k*h));g+=TextUtils.CreateStringOfLength(j,(int)(k*(1-h)));g+="]"+e;a.Add(g);}}public class Container:XMLTree{public Container():base(){Type="container";}}public class HorizontalLine:XMLTree{public HorizontalLine():base(){Type="hl";SetAttribute("width","100%");}protected override void RenderText(ref List<string>a,int b,int d){a.Add(TextUtils.CreateStringOfLength("_",b,TextUtils.RoundMode.CEIL));}}public class UIControls:XMLTree{UIController Controller;public UIControls():base(){Type="uicontrols";Controller=null;SetAttribute("selectable","false");}private void UpdateController(){Controller=RetrieveRoot()as UIController;SetAttribute("selectable",(Controller!=null&&Controller.UIStack.Count>0)?"true":"false");if(IsSelectable()){PreventDefault("LEFT/ABORT");PreventDefault("RIGHT/SUBMIT");}else{AllowDefault("LEFT/ABORT");AllowDefault("RIGHT/SUBMIT");}GetParent().UpdateSelectability(this);if(IsSelected()&&!IsSelectable()){GetParent().SelectNext();}}public override void OnKeyPressed(string a){if(Controller==null){UpdateController();}switch(a){case "LEFT/ABORT":case "RIGHT/SUBMIT":if(Controller!=null&&Controller.UIStack.Count>0){Controller.RevertUI();}break;}}protected override string PostRender(List<string>a,int b,int d){if(Controller==null){UpdateController();}string e;if(!IsSelectable()){e="";}else{e=IsSelected()?"<<":TextUtils.CreateStringOfLength(" ",TextUtils.GetTextWidth("<<"));}string f=base.PostRender(a,b,d);int g=TextUtils.CreateStringOfLength(" ",TextUtils.GetTextWidth(e)).Length;string h="";for(int i=0;i<g;i++){if((f.Length-1)<i||f[i]!=' '){h+=" ";}}f=e+(h+f).Substring(g);return f;}}public class TextInput:XMLTree{int CursorPosition;public TextInput(){Logger.log("TextInput constructor()");Type="textinput";Selectable=true;CursorPosition=-1;PreventDefault("LEFT/ABORT");PreventDefault("RIGHT/SUBMIT");SetAttribute("maxlength","10");SetAttribute("value","");SetAttribute("allowedchars"," a-z0-9");}public override void OnKeyPressed(string a){switch(a){case "LEFT/ABORT":DecreaseCursorPosition();break;case "RIGHT/SUBMIT":IncreaseCursorPosition();break;case "UP":DecreaseLetter();break;case "DOWN":IncreaseLetter();break;default:base.OnKeyPressed(a);break;}}public override void SetAttribute(string a,string b){if(a=="allowedchars"){if(!System.Text.RegularExpressions.Regex.IsMatch(b,@"([^-\\]-[^-\\]|[^-\\]|\\-|\\\\)*")){throw new Exception("Invalid format of allowed characters!");}}base.SetAttribute(a,b);}private void IncreaseLetter(){Logger.log("TextInput.IncreaseLetter()");if(CursorPosition==-1){return;}char[]a=GetAttribute("value").ToCharArray();char b=a[CursorPosition];string[]d=GetAllowedCharSets();for(int e=0;e<d.Length;e++){if((d[e].Length==1&&d[e][0]==a[CursorPosition])||(d[e].Length==3&&d[e][2]==a[CursorPosition])){Logger.log("letter outside class, setting to: "+d[e==0?d.Length-1:e-1][0]+". (chars["+((e+1)%d.Length)+"])");a[CursorPosition]=d[(e+1)%d.Length][0];SetAttribute("value",new string(a));return;}}Logger.log("letter inside class, setting to: "+(char)(((int)a[CursorPosition])+1));a[CursorPosition]=(char)(((int)a[CursorPosition])+1);SetAttribute("value",new string(a));}private void DecreaseLetter(){Logger.log("TextInput.DecreaseLetter()");if(CursorPosition==-1){return;}char[]a=GetAttribute("value").ToCharArray();char[]b=GetAttribute("allowedchars").ToCharArray();string[]d=GetAllowedCharSets();for(int e=0;e<d.Length;e++){if(d[e][0]==a[CursorPosition]){int f=(e==0?d.Length-1:e-1);Logger.log("letter outside class, setting to: "+d[f][d[f].Length-1]+". (chars["+(f)+"])");a[CursorPosition]=d[f][d[f].Length-1];SetAttribute("value",new string(a));return;}}Logger.log("letter inside class, setting to: "+(char)(((int)a[CursorPosition])-1));a[CursorPosition]=(char)(((int)a[CursorPosition])-1);SetAttribute("value",new string(a));}private string[]GetAllowedCharSets(){string a=GetAttribute("allowedchars");System.Text.RegularExpressions.MatchCollection b=System.Text.RegularExpressions.Regex.Matches(a,@"[^-\\]-[^-\\]|[^-\\]|\\-|\\\\");string[]d=new string[b.Count];int e=0;foreach(System.Text.RegularExpressions.Match match in b){string f=match.ToString();if(f=="\\-"){d[e]="-";}else if(f=="\\\\"){d[e]="\\";}else{d[e]=f;}e++;}return d;}private void IncreaseCursorPosition(){if(CursorPosition<Single.Parse(GetAttribute("maxlength"))-1){CursorPosition++;}else{CursorPosition=0;DecreaseCursorPosition();KeyPress("DOWN");}if(CursorPosition!=-1){PreventDefault("UP");PreventDefault("DOWN");}if(CursorPosition>=GetAttribute("value").Length){string[]a=GetAllowedCharSets();SetAttribute("value",GetAttribute("value")+a[0][0]);}}private void DecreaseCursorPosition(){if(CursorPosition>-1){CursorPosition--;}if(CursorPosition==-1){AllowDefault("UP");AllowDefault("DOWN");}}protected override void RenderText(ref List<string>a,int b,int d){string e=GetAttribute("value");if(CursorPosition!=-1){e=e.Substring(0,CursorPosition)+"|"+e.Substring(CursorPosition,1)+"|"+e.Substring(CursorPosition+1);}else if(e.Length==0){e="_"+e;}a.Add((IsSelected()?new string(new char[]{(char)187}):"  ")+" "+e);}}public abstract class DataStore:XMLTree{public DataStore():base(){}public override Dictionary<string,string>GetValues(Func<XMLTree,bool>a){Dictionary<string,string>b=base.GetValues(a);if(!a(this)){return b;}foreach(KeyValuePair<string,string>data in Attributes){if(!b.ContainsKey(data.Key)){b[data.Key]=data.Value;}}return b;}}public class SubmitButton:MenuItem{public SubmitButton(){Type="submitbutton";SetAttribute("flowdirection","horizontal");}protected override void PreRender(ref List<string>a,int b,int d){a.Add(IsSelected()?"[[  ":"[   ");base.PreRender(ref a,b,d);}protected override string PostRender(List<string>a,int b,int d){a.Add(IsSelected()?"  ]]":"   ]");return base.PostRender(a,b,d);}}public class Break:TextNode{public Break():base(""){Type="br";}protected override void RenderText(ref List<string>a,int b,int d){}protected override string PostRender(List<string>a,int b,int d){return "";}}public class Space:XMLTree{public Space():base(){Type="space";SetAttribute("width","0");}protected override void RenderText(ref List<string>a,int b,int d){a.Add(TextUtils.CreateStringOfLength(" ",b));}}public class Hidden:XMLTree{public Hidden():base(){Type="hidden";}protected override string PostRender(List<string>a,int b,int d){return null;}}public class HiddenData:DataStore{public HiddenData():base(){Type="hiddendata";}protected override string PostRender(List<string>a,int b,int d){return null;}}class o:Hidden{public o():base(){Type="meta";}public override Dictionary<string,string>GetValues(Func<XMLTree,bool>a){if(a(this)){return Attributes;}else{return new Dictionary<string,string>();}}}}public static class TextUtils{public enum FONT{DEFAULT,MONOSPACE,}public static bool DEBUG=true;private static FONT selectedFont=FONT.DEFAULT;private static Dictionary<FONT,Dictionary<char,int>>LetterWidths=new Dictionary<FONT,Dictionary<char,int>>{{FONT.DEFAULT,new Dictionary<char,int>{{' ',8},{'!',8},{'"',10},{'#',19},{'$',20},{'%',24},{'&',20},{'\'',6},{'(',9},{')',9},{'*',11},{'+',18},{',',9},{'-',10},{'.',9},{'/',14},{'0',19},{'1',9},{'2',19},{'3',17},{'4',19},{'5',19},{'6',19},{'7',16},{'8',19},{'9',19},{':',9},{';',9},{'<',18},{'=',18},{'>',18},{'?',16},{'@',25},{'A',21},{'B',21},{'C',19},{'D',21},{'E',18},{'F',17},{'G',20},{'H',20},{'I',8},{'J',16},{'K',17},{'L',15},{'M',26},{'N',21},{'O',21},{'P',20},{'Q',21},{'R',21},{'S',21},{'T',17},{'U',20},{'V',20},{'W',31},{'X',19},{'Y',20},{'Z',19},{'[',9},{'\\',12},{']',9},{'^',18},{'_',15},{'`',8},{'a',17},{'b',17},{'c',16},{'d',17},{'e',17},{'f',9},{'g',17},{'h',17},{'i',8},{'j',8},{'k',17},{'l',8},{'m',27},{'n',17},{'o',17},{'p',17},{'q',17},{'r',10},{'s',17},{'t',9},{'u',17},{'v',15},{'w',27},{'x',15},{'y',17},{'z',16},{'{',9},{'|',6},{'}',9},{'~',18},{' ',8},{'¡',8},{'¢',16},{'£',17},{'¤',19},{'¥',19},{'¦',6},{'§',20},{'¨',8},{'©',25},{'ª',10},{'«',15},{'¬',18},{'­',10},{'®',25},{'¯',8},{'°',12},{'±',18},{'²',11},{'³',11},{'´',8},{'µ',17},{'¶',18},{'·',9},{'¸',8},{'¹',11},{'º',10},{'»',15},{'¼',27},{'½',29},{'¾',28},{'¿',16},{'À',21},{'Á',21},{'Â',21},{'Ã',21},{'Ä',21},{'Å',21},{'Æ',31},{'Ç',19},{'È',18},{'É',18},{'Ê',18},{'Ë',18},{'Ì',8},{'Í',8},{'Î',8},{'Ï',8},{'Ð',21},{'Ñ',21},{'Ò',21},{'Ó',21},{'Ô',21},{'Õ',21},{'Ö',21},{'×',18},{'Ø',21},{'Ù',20},{'Ú',20},{'Û',20},{'Ü',20},{'Ý',17},{'Þ',20},{'ß',19},{'à',17},{'á',17},{'â',17},{'ã',17},{'ä',17},{'å',17},{'æ',28},{'ç',16},{'è',17},{'é',17},{'ê',17},{'ë',17},{'ì',8},{'í',8},{'î',8},{'ï',8},{'ð',17},{'ñ',17},{'ò',17},{'ó',17},{'ô',17},{'õ',17},{'ö',17},{'÷',18},{'ø',17},{'ù',17},{'ú',17},{'û',17},{'ü',17},{'ý',17},{'þ',17},{'ÿ',17},{'Ā',20},{'ā',17},{'Ă',21},{'ă',17},{'Ą',21},{'ą',17},{'Ć',19},{'ć',16},{'Ĉ',19},{'ĉ',16},{'Ċ',19},{'ċ',16},{'Č',19},{'č',16},{'Ď',21},{'ď',17},{'Đ',21},{'đ',17},{'Ē',18},{'ē',17},{'Ĕ',18},{'ĕ',17},{'Ė',18},{'ė',17},{'Ę',18},{'ę',17},{'Ě',18},{'ě',17},{'Ĝ',20},{'ĝ',17},{'Ğ',20},{'ğ',17},{'Ġ',20},{'ġ',17},{'Ģ',20},{'ģ',17},{'Ĥ',20},{'ĥ',17},{'Ħ',20},{'ħ',17},{'Ĩ',8},{'ĩ',8},{'Ī',8},{'ī',8},{'Į',8},{'į',8},{'İ',8},{'ı',8},{'Ĳ',24},{'ĳ',14},{'Ĵ',16},{'ĵ',8},{'Ķ',17},{'ķ',17},{'Ĺ',15},{'ĺ',8},{'Ļ',15},{'ļ',8},{'Ľ',15},{'ľ',8},{'Ŀ',15},{'ŀ',10},{'Ł',15},{'ł',8},{'Ń',21},{'ń',17},{'Ņ',21},{'ņ',17},{'Ň',21},{'ň',17},{'ŉ',17},{'Ō',21},{'ō',17},{'Ŏ',21},{'ŏ',17},{'Ő',21},{'ő',17},{'Œ',31},{'œ',28},{'Ŕ',21},{'ŕ',10},{'Ŗ',21},{'ŗ',10},{'Ř',21},{'ř',10},{'Ś',21},{'ś',17},{'Ŝ',21},{'ŝ',17},{'Ş',21},{'ş',17},{'Š',21},{'š',17},{'Ţ',17},{'ţ',9},{'Ť',17},{'ť',9},{'Ŧ',17},{'ŧ',9},{'Ũ',20},{'ũ',17},{'Ū',20},{'ū',17}, 
    {'Ŭ',20},{'ŭ',17},{'Ů',20},{'ů',17},{'Ű',20},{'ű',17},{'Ų',20},{'ų',17},{'Ŵ',31},{'ŵ',27},{'Ŷ',17},{'ŷ',17},{'Ÿ',17},{'Ź',19},{'ź',16},{'Ż',19},{'ż',16},{'Ž',19},{'ž',16},{'ƒ',19},{'Ș',21},{'ș',17},{'Ț',17},{'ț',9},{'ˆ',8},{'ˇ',8},{'ˉ',6},{'˘',8},{'˙',8},{'˚',8},{'˛',8},{'˜',8},{'˝',8},{'Ё',19},{'Ѓ',16},{'Є',18},{'Ѕ',21},{'І',8},{'Ї',8},{'Ј',16},{'Љ',28},{'Њ',21},{'Ќ',19},{'Ў',17},{'Џ',18},{'А',19},{'Б',19},{'В',19},{'Г',15},{'Д',19},{'Е',18},{'Ж',21},{'З',17},{'И',19},{'Й',19},{'К',17},{'Л',17},{'М',26},{'Н',18},{'О',20},{'П',19},{'Р',19},{'С',19},{'Т',19},{'У',19},{'Ф',20},{'Х',19},{'Ц',20},{'Ч',16},{'Ш',26},{'Щ',29},{'Ъ',20},{'Ы',24},{'Ь',19},{'Э',18},{'Ю',27},{'Я',20},{'а',16},{'б',17},{'в',16},{'г',15},{'д',17},{'е',17},{'ж',20},{'з',15},{'и',16},{'й',16},{'к',17},{'л',15},{'м',25},{'н',16},{'о',16},{'п',16},{'р',17},{'с',16},{'т',14},{'у',17},{'ф',21},{'х',15},{'ц',17},{'ч',15},{'ш',25},{'щ',27},{'ъ',16},{'ы',20},{'ь',16},{'э',14},{'ю',23},{'я',17},{'ё',17},{'ђ',17},{'ѓ',16},{'є',14},{'ѕ',16},{'і',8},{'ї',8},{'ј',7},{'љ',22},{'њ',25},{'ћ',17},{'ќ',16},{'ў',17},{'џ',17},{'Ґ',15},{'ґ',13},{'–',15},{'—',31},{'‘',6},{'’',6},{'‚',6},{'“',12},{'”',12},{'„',12},{'†',20},{'‡',20},{'•',15},{'…',31},{'‰',31},{'‹',8},{'›',8},{'€',19},{'™',30},{'−',18},{'∙',8},{'□',21},{'',40},{'',40},{'',40},{'',40},{'',41},{'',41},{'',32},{'',32},{'',40},{'',40},{'',34},{'',34},{'',40},{'',40},{'',40},{'',41},{'',32},{'',41},{'',32},{'',40},{'',40},{'',40},{'',40},{'',40},{'',40},{'',40},{'',40}}},{FONT.MONOSPACE,new Dictionary<char,int>{{' ',24},{'!',24},{'"',24},{'#',24},{'$',24},{'%',24},{'&',24},{'\'',24},{'(',24},{')',24},{'*',24},{'+',24},{',',24},{'-',24},{'.',24},{'/',24},{'0',24},{'1',24},{'2',24},{'3',24},{'4',24},{'5',24},{'6',24},{'7',24},{'8',24},{'9',24},{':',24},{';',24},{'<',24},{'=',24},{'>',24},{'?',24},{'@',24},{'A',24},{'B',24},{'C',24},{'D',24},{'E',24},{'F',24},{'G',24},{'H',24},{'I',24},{'J',24},{'K',24},{'L',24},{'M',24},{'N',24},{'O',24},{'P',24},{'Q',24},{'R',24},{'S',24},{'T',24},{'U',24},{'V',24},{'W',24},{'X',24},{'Y',24},{'Z',24},{'[',24},{'\\',24},{']',24},{'^',24},{'_',24},{'`',24},{'a',24},{'b',24},{'c',24},{'d',24},{'e',24},{'f',24},{'g',24},{'h',24},{'i',24},{'j',24},{'k',24},{'l',24},{'m',24},{'n',24},{'o',24},{'p',24},{'q',24},{'r',24},{'s',24},{'t',24},{'u',24},{'v',24},{'w',24},{'x',24},{'y',24},{'z',24},{'{',24},{'|',24},{'}',24},{'~',24},{' ',24},{'¡',24},{'¢',24},{'£',24},{'¤',24},{'¥',24},{'¦',24},{'§',24},{'¨',24},{'©',24},{'ª',24},{'«',24},{'¬',24},{'­',24},{'®',24},{'¯',24},{'°',24},{'±',24},{'²',24},{'³',24},{'´',24},{'µ',24},{'¶',24},{'·',24},{'¸',24},{'¹',24},{'º',24},{'»',24},{'¼',24},{'½',24},{'¾',24},{'¿',24},{'À',24},{'Á',24},{'Â',24},{'Ã',24},{'Ä',24},{'Å',24},{'Æ',24},{'Ç',24},{'È',24},{'É',24},{'Ê',24},{'Ë',24},{'Ì',24},{'Í',24},{'Î',24},{'Ï',24},{'Ð',24},{'Ñ',24},{'Ò',24},{'Ó',24},{'Ô',24},{'Õ',24},{'Ö',24},{'×',24},{'Ø',24},{'Ù',24},{'Ú',24},{'Û',24},{'Ü',24},{'Ý',24},{'Þ',24},{'ß',24},{'à',24},{'á',24},{'â',24},{'ã',24},{'ä',24},{'å',24},{'æ',24},{'ç',24},{'è',24},{'é',24},{'ê',24},{'ë',24},{'ì',24},{'í',24},{'î',24},{'ï',24},{'ð',24},{'ñ',24},{'ò',24},{'ó',24},{'ô',24},{'õ',24},{'ö',24},{'÷',24},{'ø',24},{'ù',24},{'ú',24},{'û',24},{'ü',24},{'ý',24},{'þ',24},{'ÿ',24},{'Ā',24},{'ā',24},{'Ă',24},{'ă',24},{'Ą',24},{'ą',24},{'Ć',24},{'ć',24},{'Ĉ',24},{'ĉ',24},{'Ċ',24},{'ċ',24},{'Č',24},{'č',24},{'Ď',24},{'ď',24},{'Đ',24},{'đ',24},{'Ē',24},{'ē',24},{'Ĕ',24},{'ĕ',24},{'Ė',24},{'ė',24},{'Ę',24},{'ę',24},{'Ě',24},{'ě',24},{'Ĝ',24},{'ĝ',24},{'Ğ',24},{'ğ',24},{'Ġ',24},{'ġ',24},{'Ģ',24},{'ģ',24},{'Ĥ',24},{'ĥ',24},{'Ħ',24},{'ħ',24},{'Ĩ',24},{'ĩ',24},{'Ī',24},{'ī',24},{'Į',24},{'į',24},{'İ',24},{'ı',24},{'Ĳ',24},{'ĳ',24},{'Ĵ',24},{'ĵ',24},{'Ķ',24},{'ķ',24},{'Ĺ',24},{'ĺ',24},{'Ļ',24},{'ļ',24},{'Ľ',24},{'ľ',24},{'Ŀ',24},{'ŀ',24},{'Ł',24},{'ł',24},{'Ń',24},{'ń',24},{'Ņ',24},{'ņ',24},{'Ň',24},{'ň',24},{'ŉ',24},{'Ō',24},{'ō',24},{'Ŏ',24},{'ŏ',24},{'Ő',24},{'ő',24},{'Œ',24},{'œ',24},{'Ŕ',24},{'ŕ',24},{'Ŗ',24},{'ŗ',24},{'Ř',24},{'ř',24},{'Ś',24},{'ś',24},{'Ŝ',24},{'ŝ',24},{'Ş',24},{'ş',24},{'Š',24},{'š',24},{'Ţ',24},{'ţ',24},{'Ť',24},{'ť',24},{'Ŧ',24},{'ŧ',24},{'Ũ',24},{'ũ',24},{'Ū',24},{'ū',24},{'Ŭ',24},{'ŭ',24},{'Ů',24},{'ů',24},{'Ű',24},{'ű',24},{'Ų',24},{'ų',24},{'Ŵ',24},{'ŵ',24},{'Ŷ',24},{'ŷ',24},{'Ÿ',24},{'Ź',24},{'ź',24},{'Ż',24},{'ż',24},{'Ž',24},{'ž',24},{'ƒ',24},{'Ș',24},{'ș',24},{'Ț',24},{'ț',24},{'ˆ',24},{'ˇ',24},{'ˉ',24},{'˘',24},{'˙',24},{'˚',24},{'˛',24},{'˜',24},{'˝',24},{'Ё',24},{'Ѓ',24},{'Є',24},{'Ѕ',24},{'І',24},{'Ї',24},{'Ј',24},{'Љ',24},{'Њ',24},{'Ќ',24},{'Ў',24},{'Џ',24},{'А',24},{'Б',24},{'В',24},{'Г',24},{'Д',24},{'Е',24},{'Ж',24},{'З',24},{'И',24},{'Й',24},{'К',24},{'Л',24},{'М',24},{'Н',24},{'О',24},{'П',24},{'Р',24},{'С',24},{'Т',24},{'У',24},{'Ф',24},{'Х',24},{'Ц',24},{'Ч',24},{'Ш',24},{'Щ',24},{'Ъ',24},{'Ы',24},{'Ь',24},{'Э',24},{'Ю',24},{'Я',24},{'а',24},{'б',24},{'в',24},{'г',24},{'д',24},{'е',24},{'ж',24},{'з',24},{'и',24},{'й',24},{'к',24},{'л',24},{'м',24},{'н',24},{'о',24},{'п',24},{'р',24},{'с',24},{'т',24},{'у',24},{'ф',24},{'х',24},{'ц',24},{'ч',24},{'ш',24},{'щ',24},{'ъ',24},{'ы',24},{'ь',24},{'э',24},{'ю',24},{'я',24},{'ё',24},{'ђ',24},{'ѓ',24},{'є',24},{'ѕ',24},{'і',24},{'ї',24},{'ј',24},{'љ',24},{'њ',24},{'ћ',24},{'ќ',24},{'ў',24},{'џ',24},{'Ґ',24},{'ґ',24},{'–',24},{'—',24},{'‘',24},{'’',24},{'‚',24},{'“',24},{'”',24},{'„',24},{'†',24},{'‡',24},{'•',24},{'…',24},{'‰',24},{'‹',24},{'›',24},{'€',24},{'™',24},{'−',24},{'∙',24},{'□',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24},{'',24}}}};public enum PadMode{LEFT,RIGHT,}public enum RoundMode{FLOOR,CEIL,}public static void SelectFont(FONT a){selectedFont=a;}public static int GetTextWidth(string a){int b=0;a=a.Replace("\r","");string[]d=a.Split('\n');foreach(string line in d){b=Math.Max(b,GetLineWidth(line.ToCharArray()));}return b;}private static int GetLineWidth(char[]a){int b=0;if(a.Length==0){return b;}foreach(char c in a){if(LetterWidths[selectedFont].ContainsKey(c)){b+=LetterWidths[selectedFont][c]+1;}else{b+=6;}}return b-1;}public static string RemoveLastTrailingNewline(string a){return(a.Length>1&&a[a.Length-1]=='\n')?a.Remove(a.Length-1):a;}public static string RemoveFirstTrailingNewline(string a){return(a.Length>1&&a[0]=='\n')?a.Remove(0):a;}public static string CenterText(string a,int b){string d="";string[]e=a.Split('\n');int f;foreach(string line in e){f=GetLineWidth(line.ToCharArray());d+=CreateStringOfLength(" ",(b-f)/2)+line+CreateStringOfLength(" ",(b-f)/2)+"\n";}d=RemoveLastTrailingNewline(d);return d;}public static string CreateStringOfLength(string a,int b){return CreateStringOfLength(a,b,RoundMode.FLOOR);}public static string CreateStringOfLength(string a,int b,RoundMode d){int e=GetLineWidth(a.ToCharArray());if(d==RoundMode.CEIL){b+=e;}string f="";if(b<e){return "";}for(int g=-1;g<b;g=g+e+1){f+=a;}return f;}public static string PadString(string a,int b,PadMode d,string e){if(d==PadMode.LEFT){return CreateStringOfLength(e,b-GetLineWidth(a.ToCharArray()))+a;}else if(d==PadMode.RIGHT){return a+CreateStringOfLength(e,b-GetLineWidth(a.ToCharArray()));}return a;}public static string PadText(string a,int b,PadMode d){return PadText(a,b,d," ");}public static string PadText(string a,int b,PadMode d,string e){string[]f=a.Split('\n');string g="";foreach(string line in f){g+=PadString(line,b,d,e)+"\n";}return g.Trim(new char[]{'\n'});}}public static class Parser{public static string PackData(Dictionary<string,string>a){StringBuilder b=new StringBuilder();foreach(string key in a.Keys){b.Append(key+"=\""+a[key]+"\" ");}return b.ToString();}public static string Sanitize(string a){return a.Replace("\"","\\\"").Replace("'","\\'");}public static string UnescapeQuotes(string a){return a.Replace("\\\"","\"").Replace("\\'","'");}public static int GetNextUnescaped(char[]a,string b){return GetNextUnescaped(a,b,0);}public static int GetNextUnescaped(char[]a,string b,int d){return GetNextUnescaped(a,b,d,b.Length-d);}public static int GetNextUnescaped(char[]a,string b,int d,int e){int f=d+e-1;int g=b.IndexOfAny(a,d,f-d+1);while(g>0&&b[g-1]=='\\'){g=b.IndexOfAny(a,g+1,f-g);}return g;}public static int GetNextOutsideQuotes(char a,string b){return GetNextOutsideQuotes(new char[]{a},b);}public static int GetNextOutsideQuotes(char a,string b,bool d){return GetNextOutsideQuotes(new char[]{a},b,d);}public static int GetNextOutsideQuotes(char[]a,string b){return GetNextOutsideQuotes(a,b,true);}public static int GetNextOutsideQuotes(char[]a,string b,bool d){char[]e=new char[]{'\'','"'};int f=-1;int g=-1;int h;while(f==-1){if(d){h=GetNextUnescaped(e,b,g+1);}else{h=b.IndexOfAny(e,g+1);}if(h==-1){f=GetNextUnescaped(a,b,g+1);}else{f=GetNextUnescaped(a,b,g+1,h-g-1);if(f!=-1){}if(d){g=GetNextUnescaped(new char[]{b[h]},b,h+1);}else{g=b.IndexOf(b[h],h+1);}}}return f;}public static List<String>ParamString2List(string a){a=a.Trim()+" ";List<string>b=new List<string>();char[]d=new char[]{'\'','"'};int e=-1;while(e!=a.Length-1){a=a.Substring(e+1);e=Parser.GetNextOutsideQuotes(new char[]{' ','\n'},a);b.Add(a.Substring(0,e).Trim(d));}return b;}public static Dictionary<string,string>GetXMLAttributes(string a){Dictionary<string,string>b=new Dictionary<string,string>();char[]d=new char[]{'\'','"'};List<string>e=ParamString2List(a);int f;foreach(string attribute in e){f=attribute.IndexOf('=');if(f==-1){b[attribute.Substring(0).ToLower()]="true";}else{b[attribute.Substring(0,f).ToLower()]=attribute.Substring(f+1).Trim(d);}}return b;}}public static class Logger{public static string History="";static IMyTextPanel DebugPanel;static public bool DEBUG=false;public static int offset=0;public static void log(string a){if(DebugPanel==null){}string b="";for(int d=0;d<offset;d++){b+="  ";}History+=b+a+"\n";}public static void 
    debug(string a){if(!DEBUG){string b="";for(int d=0;d<offset;d++){b+="  ";}History+=b+a+"\n";return;}log(a);}public static void IncLvl(){offset+=2;}public static void DecLvl(){offset=offset-2;}}
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
        for(int i=0;i<Children.Count;i++)
        {
            var compOne = Children[i] as Component;
            for (int j = i+1; j < Children.Count; j++)
            {
                var compTow = Children[j] as Component;
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
        for (int i = 0; i < Children.Count; i++)
        {
            Component auxOne = Children[i] as Component;
            for (int j = 0; j < other.Children.Count; j++)
            {
                Component auxTwo = other.Children[j] as Component;
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


internal class Object
{
    public Object Parent { get; protected set; }
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
            n(Children[i],i);
            if(size>Children.Count)
                ForChilds(n, i);
            else
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

    public virtual Object Types(string typeName)
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