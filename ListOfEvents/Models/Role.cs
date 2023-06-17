using System.Collections.Generic;

namespace ListOfEvents.Models
{
    public class Role
    {
        public Role()
        {
            Users = new List<User>();
        }
        public int Id { get; set; }
        public string RoleName { get; set; }
        public List<User> Users { get; set; }

    }
}
