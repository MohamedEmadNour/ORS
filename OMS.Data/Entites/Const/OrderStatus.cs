using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OMS.Data.Entites.Const
{
    public enum OrderStatus
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        Pending,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Processing,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Shipped,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Delivered,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Canceled,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Paid,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        PaymentFailed
    }

}
