

/*libs*/using System;using System.Collections.Generic;

internal class Request : NComponent
{
    public Request()
    {
        ObjectType = "request";
        SetAttrs("last_ip", "");
        SetAttrs("next_ip", "");
        SetAttrs("owner_ip", "");
        SetAttrs("desteny_ip", "");
        SetAttrs("network-name", "");
        SetAttrs("token", "");
    }

    private void CreateToken()
    {
        Random r1 = new Random();        
        string token = string.Format("{0}{1}{2}{3}", r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));
        SetAttrs("token", token);
    }

    public void CreateRequest(string xmlchilds,SystemOb systemTask, Task task)
    {
        jq("this").Xml(xmlchilds);
        CreateToken();        
        task.SetAttrs("token", VarAttrs["token"]);
        systemTask.AddChild(task);
    }

    public void Vars(string last_ip, string next_ip, string owner_ip, string desteny_ip)
    {
        SetAttrs("last_ip", last_ip);
        SetAttrs("next_ip", next_ip);
        SetAttrs("owner_ip", owner_ip);
        SetAttrs("desteny_ip", desteny_ip);       
    }
    

    public override Object Types(string typeName)
    {
        return null;
    }

    public override void Tick()
    {
        base.Tick();
        End();
    }

}



internal class Response : NComponent
{
    public Response()
    {
        ObjectType = "response";
        SetAttrs("last_ip", "");
        SetAttrs("next_ip", "");
        SetAttrs("owner_ip", "");
        SetAttrs("desteny_ip", "");
        SetAttrs("token", "");
        SetAttrs("status", "");
    }

    public void CreateResponse(string xmlchild,string status = "200")
    {
        jq("this").Xml(xmlchild);
        SetAttrs("status", status);
    }

    public void Vars(string last_ip, string next_ip, string owner_ip, string desteny_ip)
    {
        SetAttrs("last_ip", last_ip);
        SetAttrs("next_ip", next_ip);
        SetAttrs("owner_ip", owner_ip);
        SetAttrs("desteny_ip", desteny_ip);
    }

    public override void Tick()
    {
        base.Tick();        
    }


}
internal class Task : NComponent
{
    public Task()
    {
        ObjectType = "task";
        SetAttrs("token", "");
    }
}

internal class TaskWithTime : Task
{  
    
    public int MaxTime;

    public bool HasTaskPettition { get; private set; }
    public int TimeAlive { get; private set; }

    public TaskWithTime()
    {              
        MaxTime = 10;
        HasTaskPettition = false;
        TimeAlive = 0;
    }

    public override void Tick()
    {
        base.Tick();
        if (TimeAlive >= MaxTime) End();
        TimeAlive++;
    }    

    
}