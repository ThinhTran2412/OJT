using Laboratory_Service.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // --- DbSet Properties ---
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<TestOrder> TestOrders { get; set; }
        //public DbSet<TestResult> TestResults { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<MedicalRecordHistory> MedicalRecordHistories { get; set; } = null!;

        // --- Constructor ---
        /// <summary>
        /// Initializes a new instance of the AppDbContext class using the specified options.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Model Configuration ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------- Patient configuration --------------------
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId);
                entity.Property(e => e.IdentifyNumber).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);

                entity.HasIndex(e => e.IdentifyNumber).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.PhoneNumber);
            });

            // -------------------- MedicalRecord configuration --------------------
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.MedicalRecordId);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // -------------------- TestOrder configuration --------------------
            modelBuilder.Entity<TestOrder>(entity =>
            {
                entity.HasKey(e => e.TestOrderId);

                entity.Property(e => e.OrderCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Priority)
                    .HasMaxLength(20);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Note)
                    .HasMaxLength(1000);

                entity.Property(e => e.PatientName)
                    .HasMaxLength(200);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.TestType)
                    .HasMaxLength(100);

                entity.Property(e => e.TestResults)
                    .HasMaxLength(2000);

                entity.Property(e => e.CreatedBy)
                    .IsRequired();

                // Relationship with Patient
                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.TestOrders)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with MedicalRecord
                entity.HasOne(e => e.MedicalRecord)
                    .WithMany()
                    .HasForeignKey(e => e.MedicalRecordId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OrderCode).IsUnique();
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);
            });

            // -------------------- TestResult configuration --------------------
            //modelBuilder.Entity<TestResult>(entity =>
            //{
            //    entity.HasKey(e => e.TestResultId);

            //    // 1-1 TestOrder <-> TestResult via TestResult.TestOrderId
            //    entity.HasOne(e => e.TestOrder)
            //          .WithOne(o => o.TestResult)
            //          .HasForeignKey<TestResult>(e => e.TestOrderId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.Property(e => e.TestCode).HasMaxLength(100);
            //    entity.Property(e => e.Parameter).HasMaxLength(200);
            //    entity.Property(e => e.Unit).HasMaxLength(50);
            //    entity.Property(e => e.ReferenceRange).HasMaxLength(200);
            //    entity.Property(e => e.Instrument).HasMaxLength(100);

            //    entity.HasIndex(e => e.TestOrderId).IsUnique();
            //});

            // -------------------- EventLog configuration --------------------
            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasKey(e => e.EventLogId);

                entity.Property(e => e.EventId)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.EventLogMessage)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.OperatorName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.EntityType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedOn)
                    .IsRequired();

                entity.HasIndex(e => e.CreatedOn);
            });
        }
    }
}
