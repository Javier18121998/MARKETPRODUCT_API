using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;

namespace Market.DataValidation.DataValidation
{
    /// <summary>
    /// Indicates that a parameter should be validated as a valid ID.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class IdValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates if the given ID is greater than zero.
        /// </summary>
        /// <param name="id">The ID to validate.</param>
        /// <returns>True if the ID is valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(int id) => id > 0 ? true : throw new Exception();
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class NameValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates if the given value is a non-empty string.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid; otherwise, false.</returns>
        public bool IsValid(object value)
        {
            return value is string str && !string.IsNullOrEmpty(str);
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid size.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class SizeValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates if the given value is a non-empty string.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid; otherwise, false.</returns>
        public bool IsValid(object value)
        {
            return value is string str && !string.IsNullOrEmpty(str);
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid product.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ProductValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates the given product object for required fields and conditions.
        /// </summary>
        /// <param name="value">The product object to validate.</param>
        /// <returns>True if the product is valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            var product = value as Product;
            if (product.Id is 0 or < 0)
            {
                throw new Exception();
            }
            else if (product.Name is not string)
            {
                throw new Exception();
            }
            else if (string.IsNullOrEmpty(product.Name))
            {
                throw new Exception();
            }
            else if (product.Description is not string)
            {
                throw new Exception();
            }
            else if (string.IsNullOrEmpty(product.Description))
            {
                throw new Exception();
            }
            else if (product.Price <= 0)
            {
                throw new Exception();
            }
            else if (product.Size is not string)
            {
                throw new Exception();
            }
            else if (string.IsNullOrEmpty(product.Size))
            {
                throw new Exception();
            }
            return true;
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid new description.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class NewDescriptionValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates if the given value is a non-empty string.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            if (value is not string)
            {
                throw new Exception();
            }
            else if (string.IsNullOrEmpty((string)value))
            {
                throw new Exception();
            }
            return true;
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid update order quantity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class UpdateOrderQuantityValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates the new quantity of an order.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the new quantity is valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            var product = value as UpdateOrderQuantityDto;
            if (product.NewQuantity is 0 or < 0)
            {
                throw new Exception();
            }
            return true;
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid update order quantity by name and size.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class UpdateOrderQuantityByNameAndSizeValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates the request for updating the order quantity based on name and size.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the request is valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            var request = value as UpdateOrderQuantityByNameAndSizeDto;
            if (request.ProductName.Length < 4)
            {
                throw new Exception();
            }
            else if (request.ProductSize.Length < 4)
            {
                throw new Exception();
            }
            else if (request.NewQuantity <= 0)
            {
                throw new Exception();
            }
            return true;
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid create order by product ID.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CreateOrderByProductIdValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates the order details by product ID.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the order details are valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            var order = value as CreateOrderByProductIdDto;
            if (order.ProductId is 0 or < 0)
            {
                throw new Exception();
            }
            else if (order.Quantity is 0 or < 0)
            {
                throw new Exception();
            }
            return true;
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid create order by product name and size.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CreateOrderByProductNameAndSizeValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates the order details by product name and size.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the order details are valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            var order = value as CreateOrderByProductNameAndSizeDto;
            if (order.ProductName.Length == 0 || order.ProductName.Length < 4)
            {
                throw new Exception();
            }
            else if (Convert.ToDouble(order.ProductSize) <= 0)
            {
                throw new Exception();
            }
            else if (order.Quantity <= 0)
            {
                throw new Exception();
            }
            return true;
        }
    }

    /// <summary>
    /// Indicates that a parameter should be validated as a valid delete order by name and size.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class DeleteOrderByNameAndSizeValidationAttribute : Attribute
    {
        /// <summary>
        /// Validates the request for deleting an order based on name and size.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the request is valid; otherwise, an exception is thrown.</returns>
        public bool IsValid(object value)
        {
            var order = value as DeleteOrderByNameAndSizeDto;
            if (order.ProductName.Length == 0 || order.ProductName.Length < 4)
            {
                throw new Exception();
            }
            else if (Convert.ToDouble(order.ProductSize) <= 0)
            {
                throw new Exception();
            }
            return true;
        }
    }
}
