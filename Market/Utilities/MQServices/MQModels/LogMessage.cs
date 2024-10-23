using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Utilities.MQServices.MQModels
{
    public class LogMessage
    {
        public short TransactionId;
        public string TransactionName;
        public string TransactionDescription;
        public DateTime TransactionTime;
        public string TransactionPathEndpoint;
    }
}
