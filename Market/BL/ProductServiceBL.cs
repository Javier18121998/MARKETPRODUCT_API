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
        private readonly ILoggerFactory _loggerFactory;

        public ProductServiceBL(
            IProductService productService, 
            IMQProducer mQProducer, 
            OperationPredictor operationPredictor,
            ILoggerFactory loggerFactory)
        {
            _productService = productService;
            _mQProducer = mQProducer;
            _operationPredictor = operationPredictor;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to create.</param>
        /// <returns>The created product DTO.</returns>
        public async Task<ProductDto> CreateProductAsync(
            Product product, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                var productCreated = await _productService.CreateProductAsync(product);
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return productCreated;
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public async Task DeleteProductByIdAsync(
            int id, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.DeleteProductByIdAsync(id);
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Deletes a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to delete.</param>
        /// <param name="size">The size of the product to delete.</param>
        public async Task DeleteProductByNameAndSizeAsync(
            string name, 
            string size, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.DeleteProductByNameAndSizeAsync(name, size);
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A collection of product DTOs.</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                var products = await _productService.GetAllProductsAsync();
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return products;
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        public async Task<ProductDto> GetProductByIdAsync(
            int id, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return product;
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Retrieves a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to retrieve.</param>
        /// <param name="size">The size of the product to retrieve.</param>
        /// <returns>The corresponding product DTO.</returns>
        public async Task<ProductDto> GetProductByNameAndSizeAsync(
            string name, 
            string size, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                var productByNameAndSize = await _productService.GetProductByNameAndSizeAsync(name, size);
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                return productByNameAndSize;
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Updates the description of a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        public async Task UpdateDescriptionByIdAsync(
            int id, 
            string newDescription, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.UpdateDescriptionByIdAsync(id, newDescription);
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }

        /// <summary>
        /// Updates the description of a product by its name and size.
        /// </summary>
        /// <param name="name">The name of the product to update.</param>
        /// <param name="size">The size of the product to update.</param>
        /// <param name="newDescription">The new description for the product.</param>
        public async Task UpdateDescriptionByNameAndSizeAsync(
            string name, 
            string size, 
            string newDescription, 
            LogMessage logMessage)
        {
            IMQManagerService mQManagerService = new MQManagerService(
                _mQProducer, 
                _operationPredictor, 
                _loggerFactory);
            try
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage);
                #endregion
                await _productService.UpdateDescriptionByNameAndSizeAsync(name, size, newDescription);
            }
            catch (CustomException cex)
            {
                #region Broker Message insertions by MQProducerMessageLogger
                await mQManagerService.ConfigureMessageSendingAsync(logMessage, false);
                #endregion
                throw new CustomException(cex.StatusCode, cex.Message, cex.ErrorCode);
            }
        }
    }
}
