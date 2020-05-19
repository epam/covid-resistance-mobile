using System;
using System.Threading.Tasks;

namespace Covi.Features.BluetoothTracing.TracingInformation
{
    /// <summary>
    /// Stores information required for tracing logic to work.
    /// </summary>
    public interface ITracingInformationContainer
    {
        IObservable<BluetoothTracing.TracingInformation.TracingInformation> Changes { get; }
        Task SetAsync(BluetoothTracing.TracingInformation.TracingInformation tracingInformation);
        Task<BluetoothTracing.TracingInformation.TracingInformation> GetAsync();
    }
}
