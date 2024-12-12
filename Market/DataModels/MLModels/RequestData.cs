using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

namespace Market.DataModels.MLModels
{
    public class RequestData
    {
        [LoadColumn(0)] public string Method { get; set; }
        [LoadColumn(1)] public string Host { get; set; }
        [LoadColumn(2)] public string Path { get; set; }
        [LoadColumn(3)] public string QueryString { get; set; }
        [LoadColumn(4)] public string Operation { get; set; }
    }
}