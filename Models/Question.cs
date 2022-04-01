using System.ComponentModel.DataAnnotations;

namespace NotQuora.Models
{
    public class Question
    {
        [Key]
        public int Id {get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public ICollection<Answer> Answers {get; set;}

        public Question()
        {
            Answers = new HashSet<Answer>();
        }
    }
}
