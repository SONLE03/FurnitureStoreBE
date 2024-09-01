﻿using FurnitureStoreBE.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace FurnitureStoreBE.Models
{
    [Table("User")]
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public ICollection<RefreshToken>? Tokens { get; set; }
        public ICollection<Notification>? Notifications { get; set; }

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Question>? Question { get; set; }
        public ICollection<Reply>? Reply { get; set; }
        public ICollection<UserUsedCoupon>? UserUsedCoupon { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public Cart Cart { get; set; }
    }
}
