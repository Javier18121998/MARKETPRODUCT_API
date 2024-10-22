using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataValidation.DataValidation
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class IdValidationAttribute : Attribute
    {
        public bool IsValid(int id) => id > 0 ? true : throw new Exception();
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class NameValidationAttribute : Attribute
    {
        public bool IsValid(object value)
        {
            return value is string str && !string.IsNullOrEmpty(str);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class SizeValidationAttribute : Attribute
    {
        public bool IsValid(object value)
        {
            return value is string str && !string.IsNullOrEmpty(str);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ProductValidationAttribute : Attribute
    {
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

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class NewDescriptionValidationAttribute : Attribute
    {
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

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class UpdateOrderQuantityValidationAttribute : Attribute
    {
        public bool isValid(object value)
        {
            var product = value as DataModels.DTos.UpdateOrderQuantityDto;
            if (product.NewQuantity is 0 or < 0)
            {
                throw new Exception();
            }
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class UpdateOrderQuantityByNameAndSizeValidationAttribute : Attribute 
    { 
        public bool isValid(object value)
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

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CreateOrderByProductIdValidationAttribute : Attribute
    {
        public bool isValid(object value)
        {
            var order = value as CreateOrderByProductIdDto;
            if(order.ProductId is 0 or < 0)
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

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CreateOrderByProductNameAndSizeValidationAttribute : Attribute
    {
        public bool isValid(object value)
        {
            var order = value as CreateOrderByProductNameAndSizeDto;
            if(order.ProductName.Length == 0 || order.ProductName.Length < 4 )
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

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class DeleteOrderByNameAndSizeValidationAttribute : Attribute
    {
        public bool isValid(object value) 
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
