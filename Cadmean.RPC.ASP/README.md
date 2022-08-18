# cadRPC server library for C#

cadRPC is an easy-to-use RPC technology. 
It's goal is to simplify the communication with your web API, 
hiding all the HTTP and JSON poppycock.

This RPC server library is based on ASP.NET Core framework.
It gives you the simplicity of cadRPC and the power of ASP.NET Core.

## Installation via nuget

Install the ```Cadmean.RPC.ASP``` nuget package.

Using the package manager:

```PM> Install-Package Cadmean.RPC.ASP -Version 0.3.1```

Or just use nuget GUI in your IDE.

## How to use

### Create a new project

To create a new RPC server project use ASP.NET Core empty template:

![Create new project](https://firebasestorage.googleapis.com/v0/b/mymirea-d053b.appspot.com/o/dont_touch_dis%2Fnew_rpc_project.png?alt=media&token=73c872c6-83c9-4540-bd99-5a3bc385ca3c)

There should be the same template in Visual Studio as well.

### Setup

First, you need to add a few lines to your Startup class. It should look like this:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers(); // Library is based on regular ASP.NET controllers
    services.AddRpc(); // Add RPC service
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting(); // Use ASP.NET built-in routing 
    app.UseRpc(); // Use RPC middleware
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

### Add functions

In your project create a new folder for your functions. 
Call it ```Controllers``` or ```Functions```.
Than create a new class, for example ```SumController```:

```c#
[FunctionRoute("sum")]
public class SumController : FunctionController
{
    public int OnCall(int a, int b)
    {
        return a + b;
    }
}
```

Let me highlight the key moments here:
1. Your class must inherit from ```FunctionController``` class.
2. It must have ```FunctionRoute``` attribute. 
The argument of this attribute is the name of your RPC function. 
You will use it on the client side to call this function.
3. It must either contain a method called ```OnCall``` or 
a method marked with ```OnCall``` attribute.
This method is executed when the function is called.

#### The OnCall method

As mentioned earlier this method is executed on function call.
It can have any access modifier.
It may contain any number of parameters of any type. 
cadRPC will try to fill these parameters with the arguments sent from the client. 
The method may have any return type you want.
It can also be async.
In other words, it can be just any C# method.

#### Function naming

Function name passed into the ```FunctionRoute``` attribute 
may only contain english characters, numbers and one dot.
For example, here are some valid function names:

* sum
* getDate
* weatherForecast.get213
* user.getInfo

The name of the function should represent some action. If the name contains dot, 
the part before dot should represent some object or resource and the one after dor - some action.
If the function name is invalid it won't work.

#### Errors

In cadRPC a function outputs some result object and an error code.
Than you can then handle this error code on the client side. 
The ```sum``` function above always returns empty error code, indicating no error.
You can return a custom error from your function by throwing ```FunctionException```:

```c#
[FunctionRoute("error")]
public class GetErrorController : FunctionController
{
    public string OnCall(bool throwError)
    {
        if (throwError)
            throw new FunctionException("some_error");

        return "no error";
    }
}
```

The function above can return an error with code "some_error". 
You will need to handle this error on client side.
You can use any string for your error codes.

### More examples of function controllers

You can find some more examples in the TestServer project in this repository 
or in the [Example server repo](https://github.com/cadmean-ru/ExampleRpcServer).

### Authorization

cadRPC simplifies authorization process in your web API. 
Let's see how you can setup authorization in your project.

#### Setup

You will need to modify the ConfigureServices method of the Startup class so it will look like this:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddRpc(rpcConfiguration =>
    {
        rpcConfiguration.UseAuthorization(token => true);
    });
}
```

Note, this code will authorize any call! We will change that in a minute.

The ```UseAuthorization``` method here accepts an validation function, that 
takes an access token ```string``` and returns a ```bool```: whether to authorize call or not.

You could validate JWT just using Microsoft's library. 
But we will use ```Cadmean.CoreKit``` library for simplicity.
You need to install ```Cadmean.CoreKit``` nuget package.

Now create a class called ```JwtAuthorizationOptions```:

```c#
public static class JwtAuthorizationOptions
{
    public static AuthOptions Default = new AuthOptions
    (
        "localhost:5001",      // Token issuer
        "localhost:5001",      // Token audience
        "bruebukljhuih91283u", // Your super secret security key (don't store it here in production)
        60                     // Lifetime of the token (in minutes)
    );
}
```

Than replace ```ConfigureServices``` method's code:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddRpc(rpcConfiguration =>
    {
        rpcConfiguration.UseAuthorization(token => 
            new JwtToken(token).Validate(JwtAuthorizationOptions.Default));
    });
}
```

That's it!

#### Authenticate the user

Now you can create a new function to authenticate the user:

```c#
[FunctionRoute("auth")]
public class AuthController : FunctionController
{
    public Task<JwtAuthorizationTicket> OnCall(string email, string password)
    {
        if (email != "email@example.com" || password != "password") 
            throw new FunctionException(101);
        
        var accessToken = new JwtToken(
            JwtAuthorizationOptions.Default, 
            new List<Claim> {new Claim("email", email)}, 
            "cadmean"
        );
        
        var refreshToken = new JwtToken(
            JwtAuthorizationOptions.Default, 
            new List<Claim> {new Claim("email", email)}, 
            "cadmean"
        );
        
        return Task.FromResult(
            new JwtAuthorizationTicket(accessToken.ToString(), refreshToken.ToString())
        );
    }
}
```  

Key moments here:
1. This function will grant the user authorization to call functions, that require it. 
1. We use the ```Cadmean.CoreKit``` library here again for creating JWT tokens.
1. Such functions must return ```JwtAuthorizationTicket```, that contains access and refresh token.

cadRPC only specifies the way of exchanging these JWT tokens, and it's up to you how to generate 
and validate them. You may not want to use refresh token, or use another library for 
managing JWT tokens.

#### Require authorization for a function

Now that you have set up authentication/authorization process, you can 
create a function, thar requires to authorize user before calling it:

```c#
[FunctionRoute("user.get")]
public class GetUserController : FunctionController
{
    public new class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    [OnCall]
    [RpcAuthorize]
    public Task<User> GetUser()
    {
        var jwt = new JwtToken(Call.Authorization);
        
        var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "";

        return Task.FromResult(new User
        {
            Email = email,
            Name = "George",
        });
    }
}
```

Key moments here:
1. You can see an example of async function here
1. This function returns a POCO object
1. ```RpcAuthorize``` indicates, that this function can be called only when authorized. 
Otherwise function call will result in an error ("authorization_error").
1. The ```Call``` property is part of ```FunctionController``` class and 
represents the call object received from client. We use it's ```Authorization``` 
to get the access token.

That's how cadRPC simplifies authorization.

## See also

* [C# client readme](https://github.com/cadmean-ru/Cadmean.RPC/blob/master/Cadmean.RPC/README.md)
* [Android client library](https://github.com/cadmean-ru/androidRPCKit)
* [Python client library](https://github.com/cadmean-ru/pythonRPCKit)
* [Example server](https://github.com/cadmean-ru/ExampleRpcServer)