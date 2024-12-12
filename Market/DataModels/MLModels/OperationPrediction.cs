using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Market.DataModels.MLModels
{
    public class OperationPrediction
    {
        [ColumnName("PredictedLabel")] public string PredictedOperation { get; set; }
    }
}