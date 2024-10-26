using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.AuthorizationController.AuthServices
{
    public interface IAuthenticationService
    {
        string Authenticate(string username, string password);
    }
}
