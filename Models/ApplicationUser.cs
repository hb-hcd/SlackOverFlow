using Microsoft.AspNetCore.Identity;

namespace NotQuora.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public int Reputation {get; set;} = 0;

        public ApplicationUser()
        {
            Questions= new HashSet<Question>();
            Answers = new HashSet<Answer>();
        }
    }
}
