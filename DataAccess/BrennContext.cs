namespace Opuno.Brenn.DataAccess
{
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.IO;

    using EFProviderWrapperToolkit;
    using EFTracingProvider;

    using Opuno.Brenn.Models;

    public class BrennContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Trip.ExpenseId<->Expense
            modelBuilder.Entity<Trip>()
                .HasMany(t => t.Expenses)
                .WithRequired(e => e.Trip)
                .HasForeignKey(e => e.TripId);

            // Trip.DisplayName
            modelBuilder.Entity<Trip>().Property(p => p.DisplayName).IsRequired();

            // Expense<->Receivers
            modelBuilder.Entity<Expense>()
                .HasMany(e => e.Receivers)
                .WithMany(p => p.ExpensesUsed);

            // Expense.DisplayName
            modelBuilder.Entity<Expense>().Property(p => p.DisplayName).IsRequired();

            // Expense.Sender<>Person
            modelBuilder.Entity<Expense>()
                .HasRequired(a => a.Sender)
                .WithMany(p => p.ExpensesPaid)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public BrennContext()
            : base("BrennConnectionString")
        {
        }

        public BrennContext(DbConnection connection)
            : base(connection, true)
        {
        }
    }
}
