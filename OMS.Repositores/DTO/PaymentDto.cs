using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class PaymentRequest
    {
        public string Method { get; set; }
        public int Amount { get; set; }
        public string Token { get; set; }

    }

    public class PaymentDetails
    {
        public int Amount { get; set; }
        public string Token { get; set; }

    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
