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
        Pending = 1,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Processing = 2, 

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Shipped = 3,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Delivered = 4,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Canceled = 5,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        Paid = 6,

        [JsonConverter(typeof(JsonStringEnumConverter))]
        PaymentFailed = 7
    }

}
