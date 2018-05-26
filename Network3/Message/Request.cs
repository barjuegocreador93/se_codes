

/*libs*/using System;using System.Collections.Generic;

internal class Request : NComponent
{
    public Request()
    {
        Type = "request";
        SetAttribute("last_ip", "");
        SetAttribute("next_ip", "");
        SetAttribute("owner_ip", "");
        SetAttribute("desteny_ip", "");
        SetAttribute("network-name", "");
        SetAttribute("token", "");
    }

    private void CreateToken()
    {
        Random r1 = new Random();        
        string token = string.Format("{0}{1}{2}{3}", r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9), r1.Next(0, 9));
        SetAttribute("token", token);
    }

    public void CreateRequest(string xmlchilds,SystemOb systemTask, Task task)
    {
        jq("this").Xml(xmlchilds);
        CreateToken();        
        task.SetAttribute("token", VarAttrs["token"]);
        systemTask.AddChild(task);
    }

    public void Vars(string last_ip, string next_ip, string owner_ip, string desteny_ip)
    {
        SetAttribute("last_ip", last_ip);
        SetAttribute("next_ip", next_ip);
        SetAttribute("owner_ip", owner_ip);
        SetAttribute("desteny_ip", desteny_ip);       
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
        Type = "response";
        SetAttribute("last_ip", "");
        SetAttribute("next_ip", "");
        SetAttribute("owner_ip", "");
        SetAttribute("desteny_ip", "");
        SetAttribute("token", "");
        SetAttribute("status", "");
    }

    public void CreateResponse(string xmlchild,string status = "200")
    {
        jq("this").Xml(xmlchild);
        SetAttribute("status", status);
    }

    public void Vars(string last_ip, string next_ip, string owner_ip, string desteny_ip)
    {
        SetAttribute("last_ip", last_ip);
        SetAttribute("next_ip", next_ip);
        SetAttribute("owner_ip", owner_ip);
        SetAttribute("desteny_ip", desteny_ip);
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
        Type = "task";
        SetAttribute("token", "");
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