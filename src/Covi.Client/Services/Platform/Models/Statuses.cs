// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Covi.Client.Services.Platform.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Statuses
    {
        /// <summary>
        /// Initializes a new instance of the Statuses class.
        /// </summary>
        public Statuses()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Statuses class.
        /// </summary>
        public Statuses(IList<Status> values = default(IList<Status>), int? defaultProperty = default(int?), int? onExposure = default(int?))
        {
            Values = values;
            DefaultProperty = defaultProperty;
            OnExposure = onExposure;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "values")]
        public IList<Status> Values { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "default")]
        public int? DefaultProperty { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "onExposure")]
        public int? OnExposure { get; set; }

    }
}
