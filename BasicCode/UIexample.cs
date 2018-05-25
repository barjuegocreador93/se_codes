/*libs*/partial class UIexample:main{
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
/*libs*/}