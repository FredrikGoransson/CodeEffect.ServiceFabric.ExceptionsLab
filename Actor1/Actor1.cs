using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Actor1.Interfaces;
using Microsoft.ServiceFabric.Data;

namespace Actor1
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Actor1 : Actor, IActor1
    {
        private int _getCountFailedCalls = 0;
        private int _getStateFailedCalls = 0;

        public Actor1(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<int> GetCountFailsFirst2TimesAsync()
        {
            _getCountFailedCalls++;
            if( _getCountFailedCalls == 3)
            {
                _getCountFailedCalls = 0;
                return Task.FromResult<int>(100);
            }
            throw new InvalidOperationException($"First 2 calls are not supported. Call {(3 - _getCountFailedCalls)} times.");
        }

        public Task<int> GetCountFailsAlwaysAsync()
        {
            throw new NotSupportedException($"This one always fails. Boom.");
        }

        public Task<int> GetCountNeverFailsAsync()
        {
            return Task.FromResult<int>(200);
        }

        public async Task<int> GetStateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(GetStateAsync)} called");
            var state = await this.StateManager.GetOrAddStateAsync<int>("state", 0, CancellationToken.None);
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(GetStateAsync)} completed");
            return state;
        }

        public async Task IncreaseStateNeverFailsAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateNeverFailsAsync)} called");
            var state = await this.StateManager.GetOrAddStateAsync<int>("state", 0, CancellationToken.None);
            state++;
            await this.StateManager.AddOrUpdateStateAsync("state", state, (stateName, value) => state);
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateNeverFailsAsync)} completed");
        }

        public async Task IncreaseStateFailsFirst2TimesAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateFailsFirst2TimesAsync)} called");
            _getStateFailedCalls++;
            if (_getStateFailedCalls == 3)
            {
                _getStateFailedCalls = 0;
                var state = await this.StateManager.GetOrAddStateAsync<int>("state", 0, CancellationToken.None);
                state++;
                await this.StateManager.AddOrUpdateStateAsync("state", state, (stateName, value) => state);
                ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateFailsFirst2TimesAsync)} completed");
                return;
            }
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateFailsFirst2TimesAsync)} failed");
            throw new InvalidOperationException($"First 2 calls are not supported. Call {(3 - _getStateFailedCalls)} times.");
        }

        public Task IncreaseStateAlwaysFailsAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateAlwaysFailsAsync)} called");
            ActorEventSource.Current.ActorMessage(this, $"{this.GetType().Name}/{this.Id.GetLongId()}/{nameof(IncreaseStateAlwaysFailsAsync)} failed");
            throw new NotSupportedException($"This one always fails. Boom.");
        }
    }

}
