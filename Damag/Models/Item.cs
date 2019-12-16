using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Damag.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required][MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(2048)]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public bool Approved { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }

        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string CurrentUserId;
    }
}
