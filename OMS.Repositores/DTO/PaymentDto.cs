using OMS.Data.Entites.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public PaymentMethods Method { get; set; }
        public string Token { get; set; }

    }

    public class PaymentDetails
    {
        public decimal Amount { get; set; }
        public int OrderId { get; set; }
        public string Token { get; set; }

    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
