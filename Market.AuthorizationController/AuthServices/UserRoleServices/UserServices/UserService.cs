using Market.AuthorizationController.AuthModels;
using Market.AuthorizationController.AuthServices.UserRoleServices.IUserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.AuthorizationController.AuthServices.UserRoleServices.UserServices
{
    public class UserService : IUserService
    {
        public UserRole ValidateUser(string username, string password)
        {
            if (username == "test" && password == "password") 
            {
                return new UserRole { Id = 1, Username = "test", Role = "Admin" };
            }
            return null;
        }
    }
}
