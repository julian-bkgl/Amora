using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amora.Resources
{
    public class User
    {
        public int Id { get; set; } // Primärschlüssel der DB
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Sollte in der echten Anwendung natürlich gehasht sein
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
