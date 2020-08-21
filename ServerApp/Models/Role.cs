using Microsoft.AspNetCore.Identity;

namespace ServerApp.Models
{
    public class Role:IdentityRole<int>
    {
        public int MyProperty { get; set; }
    }
}