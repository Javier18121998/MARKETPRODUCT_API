using Market.DataModels.MLModels;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.MLServices
{
    public class OperationPredictor
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<RequestData, OperationPrediction> _predictionEngine;
        private ITransformer _modelTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationPredictor"/> class.
        /// </summary>
        public OperationPredictor()
        {
            _mlContext = new MLContext();

            try
            {
                _modelTransformer = _mlContext.Model.Load("OperationPredictionModel.zip", out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<RequestData, OperationPrediction>(_modelTransformer);
                Console.WriteLine("Model loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading the model: {ex.Message}");
                throw new InvalidOperationException("Failed to initialize the OperationPredictor. Ensure the model file is available and valid.", ex);
            }
        }

        /// <summary>
        /// Predicts the operation type based on the provided request parameters.
        /// </summary>
        /// <param name="method">HTTP method of the request (e.g., GET, POST).</param>
        /// <param name="host">Host of the request (e.g., api.example.com).</param>
        /// <param name="path">Path of the request (e.g., /products/create).</param>
        /// <param name="queryString">Query string of the request (e.g., ?id=123).</param>
        /// <returns>The predicted operation type as a string.</returns>
        public string Predict(string method, string host, string path, string queryString)
        {
            if (string.IsNullOrWhiteSpace(method) || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Method, Host, and Path cannot be null or empty.");
            }

            try
            {
                var input = new RequestData
                {
                    Method = method.ToUpperInvariant(),
                    Host = host.ToLowerInvariant(),
                    Path = path.ToLowerInvariant(),
                    QueryString = queryString ?? string.Empty
                };

                var prediction = _predictionEngine.Predict(input);
                Console.WriteLine($"Prediction made: {prediction.PredictedOperation}");
                return prediction.PredictedOperation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during prediction: {ex.Message}");
                throw new InvalidOperationException("Prediction failed. Ensure input data is valid and model is correctly configured.", ex);
            }
        }

        /// <summary>
        /// Loads a new model to replace the existing one, allowing updates to the predictor.
        /// </summary>
        /// <param name="modelPath">The path to the new model file.</param>
        public void LoadNewModel(string modelPath)
        {
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                throw new ArgumentException("Model path cannot be null or empty.", nameof(modelPath));
            }

            try
            {
                _modelTransformer = _mlContext.Model.Load(modelPath, out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<RequestData, OperationPrediction>(_modelTransformer);
                Console.WriteLine("New model loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading new model: {ex.Message}");
                throw new InvalidOperationException("Failed to load new model. Ensure the file path is valid and model is compatible.", ex);
            }
        }
    }
}

