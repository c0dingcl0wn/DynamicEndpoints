# Dynamic Endpoint Generation 
This repository provides a proof of concept on how to dynamically "generate" code, 
compile it and add a new endpoint to an MVC web application on the fly. 

## Getting started
This project runs on .net6.0. Make sure you have a sdk and a runtime installed. 
Then, either build and start the application in your favourite IDE, or from the command line with:

```shell
cd /path/to/project/DynamicEndpoints/DynamicEndpoints
dotnet build . # this step is optional
dotnet run .
```

### Try it out 
In order to try it out you can either use the Postman collection and environment in `SampleRequests` or 
use `curl` to make the requests:

#### Create a dynamic endpoint
```shell
curl --location --request POST 'localhost:5034/EndpointGenerator' \
--header 'Content-Type: application/json' \
--data-raw '{
  "ControllerName": "PrintYay",
  "Code": "var result = \"\";\nfor(var x = 0; x<11; x++){result += $\"{x}: Yay\\n\";}\nreturn Ok(result);"
}'
```

#### Call the custom endpoint
```shell
curl --location --request GET 'localhost:5034/custom/PrintYay'
```

> __Note__: If an endpoint for a given name in `ControlleName` already exists, multiple requests will not overwrite.
> If you want to change the behavior of that endpoint, go to `GeneratedAssemblies` and delete the dll with that name. 
> Otherwise, just add a new endpoint with another name and your custom code. 


## How it works
This description is going to be very basic. For further information, please refer to the .NET documentation.

The `EndpointGeneratorController` accepts a request which passes the `GenerationOptions` to it as `json`. 
The controller then calls the `AssemblyProvider` with these options. 

The `AssemblyProvider` does some string replacements in `Templates/default.txt`, loads all assemblies that are currently referenced in the current context
and then attempts to compile the resulting code. 

If the code can't be compiled, the list of compiler messages is returned. 
For example: 
```
(12,25): error CS1002: ; expected
(13,50): error CS1002: ; expected
(12,17): error CS0818: Implicitly-typed variables must be initialized
(12,25): error CS0201: Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement
```

If the code is compiled successfully, you will receive something like this:
```
Created PrintYay
Navigate to custom/PrintYay to see it in action.
```
Before returning this information to you, the controller will call the `ActionDescriptorChangeProvider` will inform the MVC Framework of the changes in order to get the endpoint added. 
All endpoints created by you are going to be accessible with the `custom/` prefix. 

