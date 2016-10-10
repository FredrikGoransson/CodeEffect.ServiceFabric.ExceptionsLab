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
| [http://localhost:8897/fails/addok](http://localhost:8897/fails/addok) | should return an incremented value for each successive call |
| [http://localhost:8897/fails/addtrice](http://localhost:8897/fails/addtrice) | should return an incremented value for each successive call but after 2 retries to the actor. Note that this mehod is very simply (stupidly) implemented in the `Actor1`, it just keeps a local counter that resets after 3 calls. |
|  [http://localhost:8897/fails/addnever](http://localhost:8897/fails/addnever) | throws an exception, just to show you that exceptions are recreated on the client if not handled by an  [`IExceptionHandler`]( https://msdn.microsoft.com/en-us/library/microsoft.servicefabric.services.communication.client.iexceptionhandler.aspx) |
| [http://localhost:8897/fails/addok/fireandforget](http://localhost:8897/fails/addok/fireandforget) | increments the value directly, but returns nothing. Call [http://localhost:8897/fails/value](http://localhost:8897/fails/value) to see the result |
| [http://localhost:8897/fails/addtrice/fireandforget](http://localhost:8897/fails/addtrice/fireandforget) | increments the value but returns nothing. The call fails the first 2 time, retries 3 times each after 2 seconds so the result should be visible after 6 seconds or little more. Note that this mehod is very simply (stupidly) implemented in the `Actor1`, it just keeps a local counter that resets after 3 calls. |
|  [http://localhost:8897/fails/addnever/fireandforget](http://localhost:8897/fails/addnever/fireandforget) | does nothing and the exception thrown by the underlying `Actor1` is not seen in the API call as it returns directly. |
| [http://localhost:8897/fails/mass/waitall](http://localhost:8897/fails/mass/waitall) | calls the all three actor methods 100 times each in a round-robin fashion by creating tasks for each call and then calling [Task.WaitAll(...)](https://msdn.microsoft.com/en-us/library/dd270695%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) on all the created tasks. |
| [http://localhost:8897/fails/mass/fireandhandle](http://localhost:8897/fails/mass/fireandhandle) | calls the all three actor methods 100 times each in a round-robin fashion. It just runs the tasks and then handles the return on a callback lambda. |
| [http://localhost:8897/fails/mass/fireandforget](http://localhost:8897/fails/mass/fireandforget) | is just included for reference so we can measure what time it would take to just fire off the Tasks. There is no way to handle a failure or even know about failures in this scenario. |


## Findings
* The retry functionality is executed by the client, not the service. This means that even though the client call is only made once and the client execution `await`s the result, the call is actually made three times to the failing actor method. This also means that another (non-failing) call to the same actor can sneak in there while the retrying call is waiting out it's delay time. **Don't expect reentrancy to block this second call for you.**
* The attribute is set on the assembly that contains the acotr interface, here `IActor.cs`. It could also be added to the Web Api assembly, but then it would anly work for client calls made from that client, not for clients in other assemblies.
* I had to implement the method `GetDefaultFabricTransportSettings` in `FabricTransportActorRemotingProviderAttribute` by looking at the default implementation in [`FabricTransportSettings`](https://msdn.microsoft.com/en-us/library/azure/microsoft.servicefabric.services.communication.fabrictransport.common.fabrictransportsettings_methods.aspx?f=255&MSPPError=-2147217396) since the code in the default attribute [`FabricTransportActorRemotingProviderAttribute`](https://msdn.microsoft.com/en-us/library/azure/microsoft.servicefabric.actors.remoting.fabrictransport.fabrictransportactorremotingproviderattribute.aspx) uses an internal method to do that. _Thanks dotPeek for revealing that..._
* In the fire-and-forget and fire-and-handle-later scenarios, it is difficult to see how reentrancy and call-chains are handled, need to investigate this more.
* Awaiting Task.WaitAll(...) will obviously not return until all the included calls have finished (or timed-out). In the case of retrying client calls to an actor method this could take a long time, depending on the retry delay.
* The measured time for firing off tasks in the fire-and-forget and the fire-and-handle scnearios is similar, as it should be. I'm just asserting that it actually behaves the way we expect by checking this.
* The time for Task.WaitAll(...) is actually longer than the time for fire-and-handle-later where we just do a Task.Delay(...) in a loop until we got all the results back. _Not sure why this is_.
