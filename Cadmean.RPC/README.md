# cadRPC client library for C#

cadRPC is an easy-to-use RPC technology. It's goal is to simplify the communication with your web API, hiding all the HTTP and JSON poppycock.

## Installation via nuget

Install the ```Cadmean.RPC``` nuget package.

Using the package manager:

```PM> Install-Package Cadmean.RPC -Version 0.3.1```

Or just use nuget GUI in your IDE.

## How to use

An example is worth a thousand words.

```c#
var rpc = new RpcClient("http://testrpc.cadmean.dev");
try 
{
    var sum = await rpc.Function("sum").CallThrowing<int>(a, b);
} 
catch (FunctionException ex) 
{
    Console.WriteLine(ex);
}
```

First you need to create an instance of `RpcClient(string serverUrl)` with the url of your RPC 
server. Then you use the `Function(string name)` function to obtain a reference of the 
specific function at your server. Finally, you use 
`async Task<FunctionOutput<TResult>> Call<TResult>(params object[] functionArguments)`  
to call the specified function. The generic argument is the expected result type of the RPC function. 
`functionArguments` are the arguments you pass to the RPC function. 
The ```CallThrowing``` method returns function result. 
If the functions returns an error ```FunctionException``` is thrown. 
In this case we are calling the "sum" function at testrpc.cadmean.dev, that takes two integers and 
returns an integer.
You can also use your custom POCO classes as function's arguments and return type.

## Authorization

cadRPC simplifies authorization process in your web API.
You just call a function on your RPC server, that will authenticate the user 
and return a JWT authorization ticket(access and refresh token).
The client will store this ticket for you and will authorize further calls with the access token.
Note, that you need to setup authorization on your server (see 
[this readme](https://github.com/cadmean-ru/Cadmean.RPC/blob/master/Cadmean.RPC.ASP/README.md)).

Here is an example:

```c#
public struct User
{
    public string Email { get; set; }
    public string Name { get; set; }
}
```
```c#
await rpc.Function("auth").CallThrowing("email@example.com", "password");
var userInfo = await rpc.Function("user.get").CallThrowing<User>();
```

## See also

* [C# server readme](https://github.com/cadmean-ru/Cadmean.RPC/blob/master/Cadmean.RPC.ASP/README.md)
* [Android client library](https://github.com/cadmean-ru/androidRPCKit)
* [Python client library](https://github.com/cadmean-ru/pythonRPCKit)
* [Example server](https://github.com/cadmean-ru/ExampleRpcServer)
