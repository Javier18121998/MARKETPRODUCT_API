using Market.DataModels.MLModels;
using Market.MLServices;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

namespace Market.Services
{
    public static class CommonProgram
    {
        private static readonly ILogger _logger;

        static CommonProgram()
        {
            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger("CommonProgram");
        }

        public static void CommonMainPrediction()
        {
            #region Loading the training data
            var mlContext = new MLContext();
            var predictor = new OperationPredictor();
            string dataPath = "request_logs.csv";
            IDataView data = mlContext.Data.LoadFromTextFile<RequestData>(dataPath, hasHeader: true, separatorChar: ',');
            #endregion

            #region Processing data and training the model
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Operation")
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Path"))
            .Append(mlContext.Transforms.Concatenate("Features", "Method", "Host", "Path", "QueryString"))
            .Append(mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));
            #endregion

            TrainingModelsAndSavedPredictions(pipeline, data, predictor, mlContext);
        }

        private static void TrainingModelsAndSavedPredictions(
            Microsoft.ML.Data.EstimatorChain<Microsoft.ML.Transforms.KeyToValueMappingTransformer> pipeline,
            IDataView data,
            OperationPredictor predictor,
            MLContext mlContext)
        {
            #region Training and Saving the model
            var model = pipeline.Fit(data);
            mlContext.Model.Save(model, data.Schema, "OperationPredictionModel.zip");
            _logger.LogInformation("Model trained and saved successfully.");
            #endregion

            #region Predictions on the marketProduct wall
            string predictedOperation = predictor.Predict("GET", "api.example.com", "/products", "");
            _logger.LogInformation($"Operación predicha: {predictedOperation}");
            #endregion
        }
    }
}
