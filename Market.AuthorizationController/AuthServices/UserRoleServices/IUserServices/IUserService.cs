using Market.AuthorizationController.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.AuthorizationController.AuthServices.UserRoleServices.IUserServices
{
    public interface IUserService
    {
        UserRole ValidateUser(string username, string password);
    }
}
