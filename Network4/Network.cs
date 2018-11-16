
/*libs*/using System;using System.Collections.Generic;

internal partial class Network : AppBase
{
    public string Data { get; set; }
    public string Arg { get; set; }
    Ports ports { get; set; }
    public Config config { get; set; }    

    public override void Begin()
    {        
        XML.Read(Data, this);
        base.Begin();
    }

    public override void Tick()
    {
        base.Tick();
        var messageBottle = new MessageBottle();
        XML.Read(Arg, messageBottle);
        AddChild(messageBottle.root);
        if(messageBottle.root!=null) messageBottle.root.Tick();
    }


    /// <summary>
    /// Function for create a new child when is called by XML.Read
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public override Object Types(string typeName, Object parent)
    {
        switch (typeName)
        {
            case "ports":
                if(ports == null && parent as Network != null)
                {
                    ports = new Ports();
                    return ports;
                }

                break;
            case "primary-port":
                if (ports != null && ports.Primary == null && parent as Ports != null)
                {
                    ports.Primary = new PrimaryPort();
                    return ports.Primary;
                }                  
                                    
                break;
            case "port":
                if(parent as Ports != null)
                {
                    return new Port();
                }
                break;                

            case "config":
                if(config == null && parent as Network != null)
                {
                    config = new Config();
                    return config;
                }                    
                break;
               
                
        }
        return null;
    }

    private class Ports : SystemOb
    {
        public PrimaryPort Primary { get; set; }
        public Ports():base()
        {
            Type = "ports";
        }
    }

    private class PrimaryPort : Port
    {
        public PrimaryPort():base()
        {
            Type = "primary-port";
            SetAttribute("name", "");
        }

        public override void Tick()
        {
            base.Tick();            
        }

        
    }

    private class Port : CBlock<IMyTerminalBlock>
    {
        public Port():base()
        {
            Type = "port";
            SetAttribute("block-type", "");
            SetAttribute("type", "");

        }

        internal void SendMessage(MessageBottle n)
        {
            
            switch(VarAttrs["block-type"])
            {
                case "Antenna":
                    
                    MyTransmitTarget m;
                    if (n.MetaData == null) AppBase.Debug("wow");
                    switch (n.MetaData.VarAttrs["method"])
                    {
                        case "Public": m = MyTransmitTarget.Everyone; break;
                        case "Private": m = MyTransmitTarget.Ally; break;
                        case "Protected": m = MyTransmitTarget.Ally; break;
                        default: m =MyTransmitTarget.Default; break;
                    }
                    
                    AppBase.Debug("Sending: " + n.ToString());
                    ((IMyRadioAntenna)Block).TransmitMessage(n.ToString(), m);
                    
                    break;
                case "LaserAntenna":
                    AppBase.Debug("Sending: " + n.ToString());
                    ((IMyLaserAntenna)Block).TransmitMessage(n.ToString());
                    break;

            }
            
        }
    }

    public class Config : SystemOb
    {
        public Config():base()
        {
            SetAttribute("ip", "");
            SetAttribute("namespace", "");
        }
    }

    private class CBlock<T>  : Component where T:class
    {
        
        protected T Block { get; set; }
        public override void Tick()
        {
            if (Block == null && VarAttrs.ContainsKey("name"))
            {
                Block = AppBase.GetGemeObject<T>(VarAttrs["name"]);
                if (Block == null)
                {                    
                    AppBase.Debug("Block with name '" + VarAttrs["name"] + "' not exist.");
                    End();
                }
            }

            if (!VarAttrs.ContainsKey("name"))
            {
                AppBase.Debug("Tag "+Type+" no have a attribute name.");
                End();
            }
        }
    }


    private class MessageBottle : SystemOb
    {
        public Meta MetaData { get; set; }
        public Body MBody { get; set; }
        private bool IsRoot;
        public MessageBottle root;
        public MessageBottle()
        {
            Type = "message-bottle";
            IsRoot = true;
        }

        /// <summary>
        /// Function for create a new child when is called by XML.Read
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override Object Types(string typeName, Object parent)
        {
            switch (typeName)
            {
                case "message-bottle":
                    var p = parent as MessageBottle;
                    if (p != null)
                    {
                        if (p.IsRoot)
                        {
                            root = new MessageBottle();
                            root.IsRoot = false;
                            return root;
                        }
                    }
                    break;

                case "meta":
                    if (parent as MessageBottle != null)
                    {
                        if (((MessageBottle)parent).MetaData == null)
                        {
                            ((MessageBottle)parent).MetaData = new Meta();
                            return ((MessageBottle)parent).MetaData;
                        }
                        
                    }
                    break;
                case "body":
                    {
                        if (parent as MessageBottle != null)
                        {
                            if (((MessageBottle)parent).MBody== null)
                            {
                                ((MessageBottle)parent).MBody = new Body();
                                return ((MessageBottle)parent).MBody;
                            }

                        }
                        break;
                    }
                case "send":
                    if(parent as MessageBottle != null && MetaData == null)
                    {

                    }
                    break;
                    

            }
            return null;
        }

        public override void Tick()
        {
            
            base.Tick();
        }

        public class Meta : Component
        {
            public Meta()
            {
                Type = "meta";
                SetAttribute("ip-desteny", "");
                SetAttribute("ip-origin", "");
                SetAttribute("path", "");
                SetAttribute("port", "");
                SetAttribute("namespace", "");
                //Public | Private
                SetAttribute("method", "");
            }

            public override void Tick()
            {                
                var network = AppBase as Network;
                var mbottle = System as MessageBottle;
                var paths = new List<string>(VarAttrs["path"].Split(','));
                if(network.config != null)
                {                  
                    if (!paths.Contains(network.config.VarAttrs["namespace"]+"/"+network.config.VarAttrs["ip"]))
                    {
                        if(network.config.VarAttrs["namespace"] == VarAttrs["namespace"])
                        {
                            VarAttrs["path"] +=","+network.config.VarAttrs["namespace"] + "/" + network.config.VarAttrs["ip"];
                            if (network.config.VarAttrs["ip"] == VarAttrs["ip-desteny"])
                            {
                                if (VarAttrs["port"] != "")
                                    network.SendPortMessage(mbottle);
                                else AppBase.Debug("Recibing: " + mbottle.MBody.ToString());
                            }
                            else
                                network.SendNetworkMessage(mbottle);
                        }
                        else
                        {
                            var a_msg_nsp = VarAttrs["namespace"].Split('.');
                            var a_nsp = network.config.VarAttrs["namespace"].Split('.');
                            bool namespace_found = false;
                            VarAttrs["path"] += "," + network.config.VarAttrs["namespace"] + "/" + network.config.VarAttrs["ip"];

                            if (a_msg_nsp.Length > a_nsp.Length)
                            {
                                int k = -1;
                                foreach (string j in a_nsp)
                                {
                                    if(a_msg_nsp[++k]!= j)
                                    {
                                        mbottle.End();
                                        return;
                                    }
                                }
                                network.ports.ForChilds((Object n, int i) =>
                                {
                                    var x = n as Port;
                                    if (x != null)
                                    {
                                        if(x.VarAttrs["type"]=="namespace" && x.VarAttrs["name"] == a_msg_nsp[k + 1])
                                        {
                                            x.SendMessage(mbottle);
                                            namespace_found = true;
                                            return -1;
                                        }
                                    }
                                    return 0;
                                });
                                if (!namespace_found)
                                {
                                    network.SendNetworkMessage(mbottle);
                                }
                            }else if(a_msg_nsp.Length < a_nsp.Length)
                            {
                                int k = -1;
                                foreach (string j in a_msg_nsp)
                                {
                                    if (a_nsp[++k] != j)
                                    {
                                        mbottle.End();
                                        return;
                                    }
                                }
                                network.ports.ForChilds((Object n, int i) =>
                                {
                                    var x = n as Port;
                                    if (x != null)
                                    {
                                        if (x.VarAttrs["type"] == "namespace" && x.VarAttrs["name"] == a_msg_nsp[k])
                                        {
                                            x.SendMessage(mbottle);
                                            namespace_found = true;
                                            return -1;
                                        }
                                    }
                                    return 0;
                                });
                            }

                        }
                        
                    }
                    mbottle.End();
                }
                
            }
        }

        public class Body : Component
        {
           public Body()
            {
                Type = "body";
            }
        }
    }

    private void SendNetworkMessage(MessageBottle n)
    {
        ports.Primary.SendMessage(n);
    }

    private void SendPortMessage(MessageBottle n)
    {
        ports.ForChilds((Object x, int i) =>
        {
            var port = x as Port;
            if(port != null)
            {
                if (port.VarAttrs["name"] == n.MetaData.VarAttrs["port"])
                {
                    port.SendMessage(n);
                    return -1;
                }
            }            
            return 0;
        }); 
    }
}

/*libs*/internal partial class Program{
    Network app = new Network();
    public Program()
    {
        app.CaptureCube = captureCube;
        app.Debug = debug;
        app.Data = Me.CustomData;
        app.FilterBlock = _s;
        app.Begin();
    }

    public void Main(string arg)
    {
        app.Arg = arg;
        app.Tick();
    }
/*libs*/}