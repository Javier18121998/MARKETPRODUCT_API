namespace MARKETPRODUCT_API.Data.ModelValidations.IDataModelValidations
{
    public interface IProductModelValidations
    {
        bool ValidateProductId(int id);
        bool ValidateProductName(string name);
        bool ValidateProductPrice(decimal price);
    }
}
