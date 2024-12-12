using Market.BL.IBL;
using Market.DAL.IDAL;
using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Market.DataModels.MLModels;
using Market.Exceptions;
using Market.MLServices;
using Market.Utilities.MQServices.IManageServices;
using Market.Utilities.MQServices.IProduceServices;
using Market.Utilities.MQServices.ManageServices;
using Market.Utilities.MQServices.MQModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.BL
{
    public class ProductServiceBL : IProductServiceBL
    {
        private readonly IProductService _productService;
        private readonly IMQProducer _mQProducer;
        private readonly OperationPredictor _operationPredictor;
        private readonly ILogger<ProductServiceBL> _logger;


        public ProductServiceBL(
            IProductService productService, 
            IMQProducer mQProducer, 
            OperationPredictor operationPredictor, 
            ILogger<ProductServiceBL> logger)
        {
            _productService = productService;
            _mQProducer = mQProducer;
            _operationPredictor = operationPredictor;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to create.</param>
        /// <returns>The created product DTO.</returns>
        public async Task<ProductDto> CreateProductAsync(Product product, LogMessage logMessage)
        {
            try
            {
                var productCreated = await _productService.CreateProductAsync(product);
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return productCreated;
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public async Task DeleteProductByIdAsync(int id, LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.DeleteProductByIdAsync(id);
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to delete.</param>
        /// <param name="size">The size of the product to delete.</param>
        public async Task DeleteProductByNameAndSizeAsync(string name, string size, LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.DeleteProductByNameAndSizeAsync(name, size);
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A collection of product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                var products = await _productService.GetAllProductsAsync();
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return products;
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        public async Task<ProductDto> GetProductByIdAsync(int id, LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return product;
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to retrieve.</param>
        /// <param name="size">The size of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        public async Task<ProductDto> GetProductByNameAndSizeAsync(string name, string size, LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                var productByNameAndSize = await _productService.GetProductByNameAndSizeAsync(name, size);
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return productByNameAndSize;
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the description of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        public async Task UpdateDescriptionByIdAsync(int id, string newDescription, LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.UpdateDescriptionByIdAsync(id, newDescription);
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the description of a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to update.</param>
        /// <param name="size">The size of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        public async Task UpdateDescriptionByNameAndSizeAsync(string name, string size, string newDescription, LogMessage logMessage)
        {
            var operationPredictor = new OperationPredictor();
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.UpdateDescriptionByNameAndSizeAsync(name, size, newDescription);
            }
            catch (Exception ex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                IMQManagerService mQManagerService = new MQManagerService(
                    _mQProducer, 
                    _operationPredictor, 
                    _logger
                );
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new Exception(ex.Message);
            }
        }
    }
}
