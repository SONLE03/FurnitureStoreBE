﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Staff")]
    public class Staff
    {
        [Key]
        public Guid id { get; set; }

        public required string Position { get; set; }
    }
}
