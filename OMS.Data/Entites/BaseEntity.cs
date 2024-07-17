using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites
{
    public class BaseEntity<T>
    {
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
