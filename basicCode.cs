/*libs*/using System.Collections.Generic;
/*libs*/using System;

/*libs*/partial class main{

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

/*libs*/}
//end BasicCode

