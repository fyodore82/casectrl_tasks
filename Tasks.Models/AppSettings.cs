using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            Jwt = new Jwt();
        }
        public string? DatabaseConnection { get; set; }
        public Jwt Jwt { get; set; }
    }

    public class Jwt
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
