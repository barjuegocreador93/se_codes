


internal partial class XJS
{
    private partial class Types
    {
        public interface IType
        {
            void Call(Var v);            
        }

        public class Var : Base, IType
        {
            IType MyType { get; set; }
            public Var()
            {
                Type = "";
                SetAttribute("name", "");
                SetAttribute("type", "");
                SetAttribute("value", "");
            }

            private bool FindType(Global g)
            {
                MyType = g.GetNode((node)=> { return node.Type == GetAttribute("type"); }) as IType;
                return MyType != null;
            }

           public void Call(Var v)
           {
                
           }
        }
    }
}

//add-Int.cs