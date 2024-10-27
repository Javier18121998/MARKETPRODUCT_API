using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Utilities.MQServices.MQModels
{
    public class MQBodyTransaction
    {
        public LogMessage Message { get; set; }
        public string TypeElement { get; set; }
    }
}
