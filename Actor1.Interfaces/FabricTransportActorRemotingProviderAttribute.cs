using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.FabricTransport.Common;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Actor1.Interfaces
{
    public class FabricTransportActorRemotingProviderAttribute : Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.FabricTransportActorRemotingProviderAttribute
    {
        /// <summary>
        ///     Creates a service remoting client factory to connect to the remoted actor interfaces.
        /// </summary>
        /// <param name="callbackClient">
        ///     Client implementation where the callbacks should be dispatched.
        /// </param>
        /// <returns>
        ///     A <see cref="T:Microsoft.ServiceFabric.Actors.Remoting.FabricTransport.FabricTransportActorRemotingClientFactory" />
        ///     as <see cref="T:Microsoft.ServiceFabric.Services.Remoting.Client.IServiceRemotingClientFactory" />
        ///     that can be used with <see cref="T:Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory" /> to
        ///     generate actor proxy to talk to the actor over remoted actor interface.
        /// </returns>
        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient callbackClient)
        {
            FabricTransportSettings fabricTransportSettings = GetDefaultFabricTransportSettings("TransportSettings");
            fabricTransportSettings.MaxMessageSize = this.GetAndValidateMaxMessageSize(fabricTransportSettings.MaxMessageSize);
            fabricTransportSettings.OperationTimeout = this.GetandValidateOperationTimeout(fabricTransportSettings.OperationTimeout);
            fabricTransportSettings.KeepAliveTimeout = this.GetandValidateKeepAliveTimeout(fabricTransportSettings.KeepAliveTimeout);
            var exceptionHandlers = new IExceptionHandler[] { new ActorExceptionHandler() };
            return (IServiceRemotingClientFactory)new FabricTransportActorRemotingClientFactory(fabricTransportSettings, callbackClient, (IServicePartitionResolver)null, exceptionHandlers, (string)null);
        }

        private long GetAndValidateMaxMessageSize(long maxMessageSize)
        {
            if (this.MaxMessageSize <= 0L)
                return maxMessageSize;
            return this.MaxMessageSize;
        }

        private TimeSpan GetandValidateOperationTimeout(TimeSpan operationTimeout)
        {
            if (this.OperationTimeoutInSeconds <= 0L)
                return operationTimeout;
            return TimeSpan.FromSeconds((double)this.OperationTimeoutInSeconds);
        }

        private TimeSpan GetandValidateKeepAliveTimeout(TimeSpan keepAliveTimeout)
        {
            if (this.KeepAliveTimeoutInSeconds <= 0L)
                return keepAliveTimeout;
            return TimeSpan.FromSeconds((double)this.KeepAliveTimeoutInSeconds);
        }


        /// <summary>
        ///  FabricTransportSettings returns the default Settings .Loads the configuration file from default Config Package"Config" , if not found then try to load from  default config file "ClientExeName.Settings.xml"  from Client Exe directory.
        /// </summary>
        /// <param name="sectionName">Name of the section within the configuration file. If not found section in configuration file, it will return the default Settings</param>
        /// <returns></returns>
        private static FabricTransportSettings GetDefaultFabricTransportSettings(string sectionName = "TransportSettings")
        {
            FabricTransportSettings settings = (FabricTransportSettings)null;
            if (!FabricTransportSettings.TryLoadFrom(sectionName, out settings, (string)null, (string)null))
            {
                settings = new FabricTransportSettings();
            }
            return settings;
        }
    }
}