using LMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// ======= User =======
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique(); /// دوه یوزره نباید دوه ایمیل ولری
                entity.Property(u => u.Role).HasConversion<int>(); // په عدد باید ذخیره شی 
            });


            /// ======= Course =======

            //modelBuilder.Entity<Course>(entity =>
            //{
            //    /// Teacher -> Courses (one - to - Many)
            //    entity.HasOne(c => c.Teacher)
            //          .WithMany(u => u.Course)
            //          .OnDelete(DeleteBehavior.Restrict); // که استاد دلیت شی خو کورسونه یی نباید دلیت شی
            //    // Category \-> Courses (one - to - Many)

            //    entity.HasOne(c => c.Teacher)
            //          .WithMany(u => u.Course)
            //          .HasForeignKey(c => c.TeacherId)
            //          .OnDelete(DeleteBehavior.Restrict);// که کتګوری دلیت شی خو کورسونه یی نیاید دلیت شی
            //    entity.Property(c => c.Price).HasColumnType("decimal(10,2)");// د قیمت لپاره د اعشاری دقت 

            //});
            modelBuilder.Entity<Course>(entity =>
            {
                // Teacher -> Courses (One-to-Many)
                entity.HasOne(c => c.Teacher)
                      .WithMany(u => u.Course)
                      .HasForeignKey(c => c.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Category -> Courses (One-to-Many)
                entity.HasOne(c => c.Category)
                      .WithMany(cat => cat.Courses)
                      .HasForeignKey(c => c.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Price).HasColumnType("decimal(10,2)");
            });

            /// ======= Lesson =======

            modelBuilder.Entity<Lesson>(entity => {
                // Course -> Lesson (one to many)
                entity.HasOne(l => l.Course)
                      .WithMany(c => c.Lessons)
                      .HasForeignKey(c => c.CourseId)
                      .OnDelete(DeleteBehavior.Cascade); // که کورس دلیت شو نو درس دی هم دلیت شی 

            });

            /// ======= LessonProgerss =======

            modelBuilder.Entity<LessonProgress>(entity => {
                
                entity.HasOne(lp => lp.Student)
                        
                      .WithMany(u => u.LessonProgresses)
                      .HasForeignKey(lp => lp.StudentId)
                      .OnDelete(DeleteBehavior.Restrict); // که کورس دلیت شو نو درس دی هم دلیت شی 

                

                entity.HasOne(lp  => lp.Lesson)
                      .WithMany(l => l.LessonProgresses)
                      .HasForeignKey(lp => lp.LessionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(lp => new { lp.StudentId, lp.LessionId }).IsUnique();
            });

            modelBuilder.Entity<Enrollment>(entity => {
                // Student -> Enrolment
                entity.HasOne(e => e.Student)
                      .WithMany(u => u.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict); // که کورس دلیت شو نو درس دی هم دلیت شی 

                // Course -> Enrolments

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            });

            // ===== Materials ====
            modelBuilder.Entity<Material>(enity => {
                enity.HasOne(m => m.Lesson)
                     .WithMany(l => l.Materials)
                     .HasForeignKey(m => m.LessonId)
                     .OnDelete(DeleteBehavior.Cascade); ; // که درس دلیت شو نو متریال دی هم دلیت شی 

            });

            // ====== RefreshToken ==== 
            modelBuilder.Entity<RefreshToken>(enity =>
            {
                enity.HasOne(rt => rt.User)
                     .WithMany(u => u.RefreshTokens)
                     .HasForeignKey(rf => rf.UserId)
                     .OnDelete(DeleteBehavior.Cascade); //  که یوزر دلیت شی نو توکن دی هم دلیت شی 

            });
             
            // ===== For Payment =====
            //modelBuilder.Entity<Enrollment>()
            //.HasIndex(e => new { e.StudentId, e.CourseId })
            //.IsUnique();
        }
    }
}
