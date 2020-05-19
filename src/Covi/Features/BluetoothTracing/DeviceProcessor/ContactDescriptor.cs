using System;

namespace Covi.Features.BluetoothTracing.DeviceProcessor
{
    /// <summary>
    /// Represents a device contact.
    /// </summary>
    public class ContactDescriptor
    {
        public ContactDescriptor(string deviceToken, DateTime contactTimestamp)
        {
            DeviceToken = deviceToken;
            ContactTimestamp = contactTimestamp;
        }

        public string DeviceToken { get; set; }
        public DateTime ContactTimestamp { get; set; }
    }
}
