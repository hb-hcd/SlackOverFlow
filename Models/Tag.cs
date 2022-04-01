using System.ComponentModel.DataAnnotations;

namespace NotQuora.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Question> Questions { get; set; }

        public Tag()
        {
            Questions = new HashSet<Question>();
        }
    }
}
