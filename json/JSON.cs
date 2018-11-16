/*libs*/using System;using System.Collections.Generic;using System.Linq;using System.Text;


public class JSONdata<K> : IJSONDATA
{
    public T GetData<T>()
    {
        T result;
        JSON.TryCast(data, out result);
        return result;
    }

    public void SetData<T>(T value)
    {
        JSON.TryCast(value, out data);
    }

    public bool IsData<T>(out T result)
    {
        return JSON.TryCast(this, out result);
    }

    public JSONdata(K data)
    {
        this.data = data;
    }

    protected K data;

}

class JSON : Dictionary<string, IJSONDATA>, IJSONDATA
{
    public T GetData<T>()
    {
        T result;
        TryCast(this, out result);
        return result;
    }

    public void SetData<T>(T value)
    {
        Dictionary<string, IJSONDATA> n;
        if (TryCast(value, out n))
        {
            Clear();
            foreach (string k in n.Keys)
            {
                Add(k, n[k]);
            }
            return;
        }
    }

    public static IJSONDATA Data<T>(T value)
    {
        return new JSONdata<T>(value);
    }



    public static bool TryCast<T>(object obj, out T result)
    {
        if ((T)obj != null)
        {
            result = (T)obj;
            return true;
        }
        result = default(T);
        return false;
    }

    public bool IsData<T>(out T result)
    {
        return TryCast(this, out result);
    }
}

static class JSONParse
{
    public static bool Parse(string data, out JSON json)
    {
        json = new JSON();
        return Typer(ref data, 0, ref json);
    }

    private static int deep = 0;

    private static bool Typer(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if (data[i] == '{')
            {
                deep++;
                if (!onProp) return Object(ref data, i + 1, ref json);
                else
                {
                    var ob = new JSON();
                    json[key] = ob;
                    key = "";
                    deeprs.Add(json);                        
                    return Object(ref data, i + 1, ref ob);
                }
            }
            else if(onProp && data[i]== '"' || data[i] == '\'')
            {
                prop = JSON.Data("");
                return Str(ref data, i + 1, ref json);
            }
            else if (onProp && data[i] >= '0' || data[i] <= '9')
            {
                dec = data[i].ToString();
                return Dec(ref data, i + 1, ref json);
            }
            else
            {
                return Typer(ref data, i + 1, ref json);
            }
        }
        return false;
    }
    private static List<JSON> deeprs;
    private static IJSONDATA prop;
    private static string dec;
    private static bool Str(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if ((data[i] == '"' || data[i] == '\''))
            {
                json[key] = prop;
                return Next(ref data, i + 1, ref json);
            }
            else
            {
                prop.SetData(prop.GetData<string>()+data[i]);
                return Str(ref data, i + 1, ref json);
            }
        }
        return false;
    }

    private static bool Dec(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if (data[i] >= '0' || data[i] <= '9')
            {
                dec += data[i].ToString();
            }else if(data[i]==',' ){
                json[key] = JSON.Data(decimal.Parse(dec));
                onProp = false;
                key = "";
                return Key(ref data, i + 1, ref json);
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private static bool Next(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if ((data[i] == ','))
            {
                key = "";
                return Key(ref data, i + 1, ref json);
            }
            if((data[i] == '}'))
            {
                    
                deep--;
                if (deep == 0) return true;
                var last = deeprs[deep];
                deeprs.RemoveAt(deep);
                return Typer(ref data, i + 1, ref last);
            }
            else
            {
                return Next(ref data, i + 1, ref json);
            }
        }
        return false;
    }

    private static bool onKey = true;

    private static bool Object(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if (onKey && (data[i] == '"' || data[i] == '\''))
            {
                return Key(ref data, i + 1, ref json);
            }
            else
            {
                return Object(ref data, i + 1, ref json);
            }
        }
        return false;
    }
    private static string key = "";
    public static bool Key(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if ((data[i] == '"' || data[i] == '\''))
            {
                onKey = false;
                json.Add(key, null);
                return Prop(ref data, i + 1, ref json);
            }
            else
            {
                key += data[i];
                return Key(ref data, i + 1, ref json);
            }
        }
        return false;
    }
    private static bool onProp = false;
    public static bool Prop(ref string data, int i, ref JSON json)
    {
        if (i < data.Length)
        {
            if ((data[i] == ':'))
            {
                onProp = true;
                return Typer(ref data, i + 1, ref json);
            }
            else
            {
                return Prop(ref data, i + 1, ref json);
            }
        }
        return false;
    }
}

/*libs*/ class P{
void Main()
{
        var data = "{'key':'data1','key2':'data2'}";
        JSON n;
        if (JSONParse.Parse(data,out n))
        {
            foreach(string k in n.Keys)
            {
                Echo(k);
            }
        }
}

/*libs*/}

