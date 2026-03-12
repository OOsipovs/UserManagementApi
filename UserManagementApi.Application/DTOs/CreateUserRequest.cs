using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Application.DTOs
{
    public class CreateUserRequest
    {
        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;
         
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]  
        public DateTime DateOfBirth { get; set; }    
    }
}
