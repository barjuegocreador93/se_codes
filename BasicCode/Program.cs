partial class Programa
{
    private static string strxml;

    public static void Main()
    {
        var ui = new UI();
        strxml = "<ui-selectorview>" +
            "</ui-selectorview>";
        ui.jq("this").Xml(strxml);
        ui.Tick();        
        System.Console.Write(ui.StrRender);
    }

}

