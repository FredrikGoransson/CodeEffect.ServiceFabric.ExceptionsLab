using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Actor1.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IActor1 : IActor
    {
        Task<int> GetCountFailsFirst2TimesAsync();

        Task<int> GetCountFailsAlwaysAsync();

        Task<int> GetCountNeverFailsAsync();

        Task<int> GetStateAsync();

        Task IncreaseStateNeverFailsAsync();

        Task IncreaseStateFailsFirst2TimesAsync();

        Task IncreaseStateAlwaysFailsAsync();
    }
}
