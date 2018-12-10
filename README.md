
[![Nuget](https://img.shields.io/nuget/v/Zabbix_Sender.svg?style=plastic)](https://www.nuget.org/packages/Zabbix_Sender) [![Nuget](https://img.shields.io/nuget/dt/Zabbix_Sender.svg?style=plastic)](https://www.nuget.org/packages/Zabbix_Sender)

# Zabbix_Sender
Very simple C# library to send messages to a zabbix trapper item

## How to use :

* Add the NuGet Package Zabbix_Sender
* Add a reference to the package in your code :
```
using Zabbix_Sender;
```

* To send a message to zabbix :
```
ZS_Request r = new ZS_Request("NameOfZabbixHost", "NameOfZabbixKey", "StringValueToSend");
Console.WriteLine(r.Send("ZabbixServerURL").Response);
```
        


