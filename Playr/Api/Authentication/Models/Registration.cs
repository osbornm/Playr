using System.ComponentModel.DataAnnotations;

namespace Playr.Api.Authentication.Models
{
    public class Registration
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}