/*libs*/


internal partial class XJS
{
    public class ScrptController : Base
    {
        public ScrptController()
        {
            XML.NodeRegister["script"] = () => { return new Script(); };
        }

        public XML.UIController MyUi { get; private set; }
        public XML.XMLTree Dom { get; private set; }
        private Global G { get; set; }

        public void  UI (XML.UIController ui)
        {
            MyUi = ui;
            Dom = ui.GetNode((node) => { return node.Type == "root"; });
            G = new Global();
            AddChild(G);
            var listscript = ui.GetAllNodes((node) => { return node as Script != null; });
            foreach(XML.XMLTree  v in listscript)
            {
                
            }
        }

        
    }

    public class Script : Base
    {
        public Script()
        {
            Type = "script";
        }

    }
    

}

