/*libs*/using System;

/*libs*/partial class main{
    internal MyExampleApp.MyApp myApp { get; set; }


    public Program()
    {
        //keyboard:
        //<keypress ui-target='myUi' key='up' secret='1234'/>
        //<keypress ui-target='ui-tp' key='down' secret='1234'/>
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
/*libs*/}


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
    




