using System;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Actors.Remoting;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace WebApi.Controllers
{
    public class FailsController : ApiController
    {

        public FailsController()
        {
            var fabricClient = new FabricClient();            
        }

        [Route("fails/ok")]
        public async Task<int> GetOk()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var result = await actor.GetCountNeverFailsAsync();
            return result;
        }
        [Route("fails/trice")]
        public async Task<int> GetTrice()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var result = await actor.GetCountFailsFirst2TimesAsync();
            return result;
        }

        [Route("fails/never")]
        public async Task<int> GetNever()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var result = await actor.GetCountFailsAlwaysAsync();
            return result;
        }


        [Route("fails/addok")]
        public async Task<State> GetAddOk()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var state = new State();
            state.OldValue = await actor.GetStateAsync();
            await actor.IncreaseStateNeverFailsAsync();
            state.NewValue = await actor.GetStateAsync();
            return state;
        }

        [Route("fails/addtrice")]
        public async Task<State> GetAddTrice()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var state = new State();
            state.OldValue = await actor.GetStateAsync();
            await actor.IncreaseStateFailsFirst2TimesAsync();
            state.NewValue = await actor.GetStateAsync();
            return state;
        }

        [Route("fails/addnever")]
        public async Task<State> GetAddNever()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var state = new State();
            state.OldValue = await actor.GetStateAsync();
            await actor.IncreaseStateAlwaysFailsAsync();
            state.NewValue = await actor.GetStateAsync();
            return state;
        }


        public class State
        {
            public int OldValue { get; set; }
            public int NewValue { get; set; }
        }
    }
}