namespace LMS.Entities
{
    public class LessonProgress
    {
        public int Id { get; set; }
        public  int StudentId { get; set; }
        public int LessionId { get; set; }
        public bool IsCompletedAt { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

           // Navigations
        public User Student { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;

    }
}
