using System;
using System.ComponentModel.DataAnnotations;

namespace LibApp.Dtos {
    public class CustomerDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public bool HasNewsletterSubscribed { get; set; }
        public byte MembershipTypeId { get; set; }
        public MembershipTypeDto MembershipType { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}
