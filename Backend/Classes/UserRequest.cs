using Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Classes
{
    [NotMapped]
    public class UserRequest : User
    {
        public string Password { get; set; }

        public byte[] ImageArray { get; set; }
    }
}