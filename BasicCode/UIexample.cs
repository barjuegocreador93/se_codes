/*libs*/partial class UIexample{
    void Main(string args)
    {
        string uixml = "<ui name='myUi' key='secret'>" +
            "<ui-einteract/>" +
            "<ui-einteract/>" +
            "</ui>";
        var ui = new UI();
        //1
        ui.jq("this").Xml(uixml);
        //2
        ui.KeyBoardEnter(args);
        //3
        ui.Tick();

    }
/*libs*/}