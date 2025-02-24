using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SkiServiceManagement.Models
{
    public class LoginRequest
    {
        public string username { get; set; }

        public string password { get; set; }
    }
}
