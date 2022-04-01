using System.ComponentModel.DataAnnotations;

namespace NotQuora.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public int Upvote { get; set; } = 0;

        public int Downvote {get; set;} = 0;

        public bool IsCorrect { get; set; }

        public virtual ApplicationUser? User {get; set;}

        public int QuestionId { get; set; }
        public DateTime AnswerTime { get; set; }

        public Answer(string content)
        {
            Content = content;
            IsCorrect = false;
            AnswerTime = DateTime.Now.Date;
        }
    }
}
