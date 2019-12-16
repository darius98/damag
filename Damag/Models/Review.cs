using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Damag.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Rating { get; set; }

        [MaxLength(1024)]
        public String Text { get; set; }

        public string UserId { get; set; }

        public int ItemId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Item Item { get; set; }
    }
}