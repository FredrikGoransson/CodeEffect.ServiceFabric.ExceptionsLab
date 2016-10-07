using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Validation;
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

        [Route("fails/value")]
        public async Task<State> GetValue()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            var state = new State();
            state.OldValue = await actor.GetStateAsync();
            state.NewValue = state.OldValue;
            return state;
        }

        [Route("fails/addok/fireandforget")]
        public Task GetAddOkFireAndForget()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            actor.IncreaseStateNeverFailsAsync().FireAndForget();
            return Task.FromResult(true);
        }

        [Route("fails/addtrice/fireandforget")]
        public Task GetAddTriceFireAndForget()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            actor.IncreaseStateFailsFirst2TimesAsync().FireAndForget();
            return Task.FromResult(true);
        }

        [Route("fails/addnever/fireandforget")]
        public Task GetAddNeverFireAndForget()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            actor.IncreaseStateAlwaysFailsAsync().FireAndForget();
            return Task.FromResult(true);
        }

        [Route("fails/addok/fireandhandle")]
        public Task GetAddOkFireAndHandle()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            actor.IncreaseStateNeverFailsAsync().FireAndHandleLater();
            return Task.FromResult(true);
        }

        [Route("fails/addtrice/fireandhandle")]
        public Task GetAddTriceFireAndHandle()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            actor.IncreaseStateFailsFirst2TimesAsync().FireAndHandleLater();
            return Task.FromResult(true);
        }

        [Route("fails/addnever/fireandhandle")]
        public Task GetAddNeverFireAndHandle()
        {
            var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
            actor.IncreaseStateAlwaysFailsAsync().FireAndHandleLater();
            return Task.FromResult(true);
        }

        [Route("fails/mass/waitall")]
        public Task<MassCalls> GetMassWaitAll()
        {
            var stopwatch = new Stopwatch();

            var massCalls = new MassCalls();
            stopwatch.Start();
            var tasks = new List<Task>();
            for (var i = 0; i < 300; i++)
            {
                Task task = null;
                var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
                if (i % 3 == 0)
                    task = actor.IncreaseStateNeverFailsAsync();
                if (i % 3 == 1)
                    task = actor.IncreaseStateFailsFirst2TimesAsync();
                if (i % 3 == 2)
                    task = actor.IncreaseStateAlwaysFailsAsync();
                tasks.Add(task);
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException aex)
            {
                aex = aex.Flatten();
                aex.Handle(ex =>
                {
                    if (ex is NotSupportedException)
                    {
                        Debug.WriteLine($"Exception: {ex.Message}");
                        return true;
                    }
                    else if (ex is InvalidOperationException)
                    {
                        Debug.WriteLine($"Exception: {ex.Message}");
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
            stopwatch.Stop();
            massCalls.ElapsedCalls = stopwatch.ElapsedMilliseconds;
            massCalls.ElapsedResults = stopwatch.ElapsedMilliseconds;
            massCalls.Successful = tasks.Count(t => t.IsCompleted && !t.IsFaulted && !t.IsCanceled);
            massCalls.Failed= tasks.Count(t => t.IsCompleted && t.IsFaulted && !t.IsCanceled);
            massCalls.Others = tasks.Count() - massCalls.Successful - massCalls.Failed;

            return Task.FromResult(massCalls);
        }

        [Route("fails/mass/fireandhandle")]
        public Task<MassCalls> GetMassHandle()
        {
            var locker = new object();
            var stopwatch = new Stopwatch();

            var massCalls = new MassCalls();
            stopwatch.Start();
            var tasks = new Dictionary<int, string>();

            var onDone = (Action<Task>)((task) => {
                lock (locker)
                {
                    massCalls.Successful++;
                    tasks[task.Id] = "success";
                }
            });
            var onFault = (Action<Task, AggregateException>) ((task, aex) =>
            {
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    Debug.WriteLine($"Exception: {ex.Message}");
                }
                lock (locker)
                {
                    massCalls.Failed++;
                    tasks[task.Id] = "fault";
                }
            });
            var onCancel = (Action<Task>)((task) =>
            {
                lock (locker)
                {
                    massCalls.Others++;
                    tasks[task.Id] = "cancel";
                }
            });
            var onOther = (Action<Task>)((task) =>
            {
                lock (locker)
                {
                    massCalls.Others++;
                    tasks[task.Id] = "other";
                }
            });

            for (var i = 0; i < 300; i++)
            {
                var actor =
                    Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(
                        new Microsoft.ServiceFabric.Actors.ActorId(1));
                if (i%3 == 0)
                {
                    var taskId = actor.IncreaseStateNeverFailsAsync().FireAndHandleLater(onFault: onFault, onDone: onDone, onCancel: onCancel, onOther: onOther);
                    tasks.Add(taskId, null);
                }
                if (i%3 == 1)
                {
                    var taskId = actor.IncreaseStateFailsFirst2TimesAsync().FireAndHandleLater(onFault: onFault, onDone: onDone, onCancel: onCancel, onOther: onOther);
                    tasks.Add(taskId, null);
                }
                if (i%3 == 2)
                {
                    var taskId = actor.IncreaseStateAlwaysFailsAsync().FireAndHandleLater(onFault: onFault, onDone: onDone, onCancel: onCancel, onOther: onOther);
                    tasks.Add(taskId, null);
                }
            }

            massCalls.ElapsedCalls = stopwatch.ElapsedMilliseconds;

            while ((massCalls.Failed + massCalls.Successful + massCalls.Others) < 300)
            {
                Task.Delay(10);
                var unhandledTasks = tasks.ToArray().Where(t => t.Value == null).ToArray();
                
            }
            stopwatch.Stop();
            massCalls.ElapsedResults = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();

            return Task.FromResult(massCalls);
        }

        [Route("fails/mass/fireandforget")]
        public Task<long> GetMassForget()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (var i = 0; i < 300; i++)
            {
                var actor = Microsoft.ServiceFabric.Actors.Client.ActorProxy.Create<Actor1.Interfaces.IActor1>(new Microsoft.ServiceFabric.Actors.ActorId(1));
                if (i % 3 == 0)
                    actor.IncreaseStateNeverFailsAsync().FireAndForget();
                if (i % 3 == 1)
                    actor.IncreaseStateFailsFirst2TimesAsync().FireAndForget();
                if (i % 3 == 2)
                    actor.IncreaseStateAlwaysFailsAsync().FireAndForget();
            }

            stopwatch.Stop();

            return Task.FromResult(stopwatch.ElapsedMilliseconds);
        }

        public class MassCalls
        {
            public long ElapsedCalls { get; set; }
            public long ElapsedResults { get; set; }
            public int Successful { get; set; }
            public int Failed { get; set; }
            public int Others { get; set; }
        }

        public class State
        {
            public int OldValue { get; set; }
            public int NewValue { get; set; }
        }
    }
}