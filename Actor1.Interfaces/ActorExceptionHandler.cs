using System;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace Actor1.Interfaces
{
    public class ActorExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {           
            if (exceptionInformation.Exception is InvalidOperationException)
            {                
                result = new ExceptionHandlingRetryResult(exceptionInformation.Exception,
                    isTransient: true,
                    retryDelay: TimeSpan.FromSeconds(2),
                    maxRetryCount: 3);
                return true;
            }
            result = new ExceptionHandlingThrowResult();
            return false;
        }
    }
}