using Microsoft.EntityFrameworkCore;
using TextProcessor.Models;

namespace TextProcessor.Data
{
    public sealed class TextProcessDbContext : DbContext
    {
        public TextProcessDbContext(DbContextOptions<TextProcessDbContext> options): base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<WordDetector> Words { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WordDetector>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(p => p.Word)
                    .HasMaxLength(20)
                    .HasColumnType("varchar")
                    //.UseCollation("Latin1_General_100_CI_AS_UTF8") //don't work
                    ;
                builder.HasIndex(x => x.Word).IsUnique();
                //for concurrency
                //https://learn.microsoft.com/ru-ru/ef/core/saving/concurrency?tabs=data-annotations
                builder.Property(p => p.Version).IsRowVersion();
            });
        }
    }
}