﻿using OMS.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.OrderService
{
    public interface IOrderService
    {
        Task<bool> ValidateStockAsync(Order order);
        decimal ApplyDiscount(Order order);
    }
}
