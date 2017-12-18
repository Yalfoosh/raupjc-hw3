using System.Data.Entity;

namespace HW3
{
    public class TodoDbContext : DbContext
    {
        public DbSet<TodoItem> Items { get; set; }
        public DbSet<TodoItemLabel> Labels { get; set; }

        public TodoDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TodoItem>().HasKey(x => x.Id);

            builder.Entity<TodoItem>().Property(x => x.Id).IsRequired();
            builder.Entity<TodoItem>().Property(x => x.UserId).IsRequired();
            builder.Entity<TodoItem>().Property(x => x.Text).IsRequired();
            builder.Entity<TodoItem>().Property(x => x.DateCreated).IsRequired();


            builder.Entity<TodoItemLabel>().HasKey(x => x.Id);
            builder.Entity<TodoItemLabel>().HasKey(x => x.Value);

            builder.Entity<TodoItemLabel>().Property(x => x.Id).IsRequired();
            builder.Entity<TodoItemLabel>().Property(x => x.Value).IsRequired();



            builder.Entity<TodoItem>().HasMany(item => item.Labels)
                                      .WithMany(label => label.LabelTodoItems);

            builder.Entity<TodoItemLabel>().HasMany(label => label.LabelTodoItems)
                                           .WithMany(item => item.Labels);
        }
    }
}