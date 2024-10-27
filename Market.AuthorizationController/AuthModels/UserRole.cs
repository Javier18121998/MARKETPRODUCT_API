using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.AuthorizationController.AuthModels
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
