using System.ComponentModel.DataAnnotations;

namespace ServerApp.Dtos
{
    public class UserForUpdateDto
    {
           [Required]
          public string City { get; set; }
          public string Country { get; set; }
          public string Introduction { get; set; }
          public string Hobbies { get; set; }

}}