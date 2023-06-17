using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ListOfEvents.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public string EventOrganizer { get; set; }
        public string EventDescription { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int? UserId { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string RefulationsName { get; set; }
        public string RefulationsPath { get; set; }

    }
}
