namespace LMS.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Descriptions { get; set; }

        // Navigation Property  ==> one category -> many course(oner to many)
        public ICollection<Course> Courses { get; set; } = new List<Course>(); /// One to Many 
    }
}
