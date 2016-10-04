# CodeEffect.ServiceFabric.ExceptionsLab

This Service Fabric solution shows how to add a custom [`ActorRemotingProviderAttribute`](https://msdn.microsoft.com/en-us/library/microsoft.servicefabric.actors.remoting.actorremotingproviderattribute.aspx) in order to add custom [`IExceptionHandler`s]( https://msdn.microsoft.com/en-us/library/microsoft.servicefabric.services.communication.client.iexceptionhandler.aspx) to manage retries of specific exceptions in Actor methods.

## Project
The client service is a regular, out-of-the-box, web api with one relevant controller: FailsController. Call the methods to call the different actor methods for Actor1.

The Actor1 project contains the Actor with a number of methods where some of them are designed to fail.

## Running the project
Just clone the repo, open in VS and hit F5. The Api is hosted by default at port 8897: [http://localhost:8897](http://localhost:8897).

After that you can test out the different behaviors by calling:

| Uri | Expected result |
|-----|-----------------|
| [http://localhost:8897/fails/addok] | should return an incremented value for each successive call |
| [http://localhost:8897/fails/addtrice] | should return an incremented value for each successive call but after 2 retries to the actor. Note that this mehod is very simply implemented in the `Actor1`, it just keeps a local counter that resets after 3 calls. |
|  [http://localhost:8897/fails/addnever] | throws an exception, just to show you that exceptions are recreated on the client if not handled by an  [`IExceptionHandler`]( https://msdn.microsoft.com/en-us/library/microsoft.servicefabric.services.communication.client.iexceptionhandler.aspx) |

## Findings
* The retry functionality is executed by the client, not the service. This means that even though the client call is only made once and the client execution `await`s the result, the call is actually made three times to the failing actor method. This also means that another (non-failing) call to the same actor can sneak in there while the retrying call is waiting out it's delay time. **Don't expect reentrancy to block this second call for you.**
* The attribute is set on the assembly that contains the acotr interface, here `IActor.cs`. It could also be added to the Web Api assembly, but then it would anly work for client calls made from that client, not for clients in other assemblies.
