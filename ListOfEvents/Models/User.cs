using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListOfEvents.Models
{
    public class User
    {
        public User()
        {
            Events = new List<Event>();
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public int? RoleId { get; set; }
        public List<Event> Events { get; set; }
    }
}
