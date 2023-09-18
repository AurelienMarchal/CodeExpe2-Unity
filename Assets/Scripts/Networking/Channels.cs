using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Connect_
{
    public int id { get; set; }
}

public class ConnectChannel
{
    public Connect_ connect { get; set; }
    public string identify { get; set; }
}


public class ClientInfo_
{
    public string role { get; set; }
    public string name { get; set; }
}

public class ClientInfoChannel
{
    public ClientInfo_ client_info { get; set; }
    public string identify { get; set; }

    public ClientInfoChannel(string name, string role){
        identify = "client_info";
        client_info = new ClientInfo_();
        client_info.name = name;
        client_info.role = role;

    }
}

public class Block_
{
    public int id { get; set; }
    public int trainingTrialsCount { get; set; }
    public int monitoredTrialsCount { get; set; }
    public int currentTrial { get; set; }
    public Params_ @params { get; set; }
}

public class Params_
{
    public string TI { get; set; }
    public string Visualization { get; set; }
    public string Task { get; set; }
}

public class BlockChannel
{
    public Block_ block { get; set; }
    public string identify { get; set; }
}

public class User_
{
    public int id { get; set; }
    public int group { get; set; }
}

public class UserChannel
{
    public User_ user { get; set; }
    public string identify { get; set; }
}


