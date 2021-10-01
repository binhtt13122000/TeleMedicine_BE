using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Infrastructure.Models
{
    public partial class TeleMedicineContext : DbContext
    {
        public TeleMedicineContext()
        {
        }

        public TeleMedicineContext(DbContextOptions<TeleMedicineContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Certification> Certifications { get; set; }
        public virtual DbSet<CertificationDoctor> CertificationDoctors { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<DiseaseGroup> DiseaseGroups { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Drug> Drugs { get; set; }
        public virtual DbSet<DrugType> DrugTypes { get; set; }
        public virtual DbSet<HealthCheck> HealthChecks { get; set; }
        public virtual DbSet<HealthCheckDisease> HealthCheckDiseases { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<HospitalDoctor> HospitalDoctors { get; set; }
        public virtual DbSet<Major> Majors { get; set; }
        public virtual DbSet<MajorDoctor> MajorDoctors { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<Symptom> Symptoms { get; set; }
        public virtual DbSet<SymptomHealthCheck> SymptomHealthChecks { get; set; }
        public virtual DbSet<TimeFrame> TimeFrames { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=52.221.193.237;Database=TeleMedicine;Username=postgres;Password=thanhbinh");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Email, "Account_Email_key")
                    .IsUnique();

                entity.Property(e => e.FacebookId);

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.City).IsRequired();

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.IsMale).HasDefaultValueSql("true");

                entity.Property(e => e.LastName).IsRequired();

                entity.Property(e => e.Locality).IsRequired();

                entity.Property(e => e.Phone).IsRequired();

                entity.Property(e => e.PostalCode).IsRequired();

                entity.Property(e => e.RegisterTime).HasColumnType("timestamp with time zone");

                entity.Property(e => e.StreetAddress).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_role");
            });

            modelBuilder.Entity<Certification>(entity =>
            {
                entity.ToTable("Certification");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CertificationDoctor>(entity =>
            {
                entity.ToTable("CertificationDoctor");

                entity.Property(e => e.DateOfIssue).HasColumnType("date");

                entity.Property(e => e.Evidence).IsRequired();

                entity.HasOne(d => d.Certification)
                    .WithMany(p => p.CertificationDoctors)
                    .HasForeignKey(d => d.CertificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_certificate_user");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.CertificationDoctors)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_certificate");
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.ToTable("Disease");

                entity.HasIndex(e => e.DiseaseCode, "Disease_DiseaseCode_key")
                    .IsUnique();

                entity.Property(e => e.DiseaseCode).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.DiseaseGroup)
                    .WithMany(p => p.Diseases)
                    .HasForeignKey(d => d.DiseaseGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_disease_group");
            });

            modelBuilder.Entity<DiseaseGroup>(entity =>
            {
                entity.ToTable("DiseaseGroup");

                entity.Property(e => e.GroupName).IsRequired();
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor");

                entity.HasIndex(e => e.CertificateCode, "Doctor_CertificateCode_key")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "Doctor_Email_key")
                    .IsUnique();

                entity.Property(e => e.CertificateCode).IsRequired();

                entity.Property(e => e.DateOfCertificate).HasColumnType("date");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.IsVerify).HasDefaultValueSql("false");

                entity.Property(e => e.NumberOfConsultants).HasDefaultValueSql("0");

                entity.Property(e => e.PlaceOfCertificate).IsRequired();

                entity.Property(e => e.PractisingCertificate).IsRequired();

                entity.Property(e => e.Rating).HasDefaultValueSql("0");

                entity.Property(e => e.ScopeOfPractice).IsRequired();
            });

            modelBuilder.Entity<Drug>(entity =>
            {
                entity.ToTable("Drug");

                entity.Property(e => e.DrugForm).IsRequired();

                entity.Property(e => e.DrugOrigin).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Producer).IsRequired();

                entity.HasOne(d => d.DrugType)
                    .WithMany(p => p.Drugs)
                    .HasForeignKey(d => d.DrugTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_drug_detail");
            });

            modelBuilder.Entity<DrugType>(entity =>
            {
                entity.ToTable("DrugType");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<HealthCheck>(entity =>
            {
                entity.ToTable("HealthCheck");

                entity.Property(e => e.Status).IsRequired();

                entity.Property(e => e.CanceledTime).HasColumnType("timestamp with time zone");

                entity.Property(e => e.CreatedTime).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Token).IsRequired();

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.HealthChecks)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_health_checl");
            });

            modelBuilder.Entity<HealthCheckDisease>(entity =>
            {
                entity.ToTable("HealthCheckDisease");

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.HealthCheckDiseases)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_disease_health_check");

                entity.HasOne(d => d.HealthCheck)
                    .WithMany(p => p.HealthCheckDiseases)
                    .HasForeignKey(d => d.HealthCheckId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_health_check_disease");
            });

            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.ToTable("Hospital");

                entity.HasIndex(e => e.HospitalCode, "Hospital_HospitalCode_key")
                    .IsUnique();

                entity.Property(e => e.HospitalCode).IsRequired();

                entity.Property(e => e.Lat).IsRequired();

                entity.Property(e => e.Long).IsRequired();
            });

            modelBuilder.Entity<HospitalDoctor>(entity =>
            {
                entity.ToTable("HospitalDoctor");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.HospitalDoctors)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_hospital");

                entity.HasOne(d => d.Hospital)
                    .WithMany(p => p.HospitalDoctors)
                    .HasForeignKey(d => d.HospitalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_hospital_user");
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.ToTable("Major");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<MajorDoctor>(entity =>
            {
                entity.ToTable("MajorDoctor");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.MajorDoctors)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_major");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.MajorDoctors)
                    .HasForeignKey(d => d.MajorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_major_user");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreatedDate).HasColumnType("timestamp with time zone");

                entity.Property(e => e.IsSeen).HasDefaultValueSql("false");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_notification");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");

                entity.HasIndex(e => e.Email, "Patient_Email_key")
                    .IsUnique();

                entity.Property(e => e.Email).IsRequired();
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("Prescription");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Drug)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.DrugId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_drug_prescription");

                entity.HasOne(d => d.HealthCheck)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.HealthCheckId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_health_check_prescription");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Slot>(entity =>
            {
                entity.ToTable("Slot");

                entity.Property(e => e.AssignedDate).HasColumnType("date");

                entity.Property(e => e.EndTime).HasColumnType("time without time zone");

                entity.Property(e => e.StartTime).HasColumnType("time without time zone");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_slot");

                entity.HasOne(d => d.HealthCheck)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.HealthCheckId)
                    .HasConstraintName("fk_health_check_slot");
            });

            modelBuilder.Entity<Symptom>(entity =>
            {
                entity.ToTable("Symptom");

                entity.HasIndex(e => e.SymptomCode, "Symptom_SymptomCode_key")
                    .IsUnique();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.SymptomCode).IsRequired();
            });

            modelBuilder.Entity<SymptomHealthCheck>(entity =>
            {
                entity.ToTable("SymptomHealthCheck");

                entity.Property(e => e.Evidence).IsRequired();

                entity.HasOne(d => d.HealthCheck)
                    .WithMany(p => p.SymptomHealthChecks)
                    .HasForeignKey(d => d.HealthCheckId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_health_check_symptom");

                entity.HasOne(d => d.Symptom)
                    .WithMany(p => p.SymptomHealthChecks)
                    .HasForeignKey(d => d.SymptomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_symptom_health_check");
            });

            modelBuilder.Entity<TimeFrame>(entity =>
            {
                entity.ToTable("TimeFrame");

                entity.Property(e => e.EndTime).HasColumnType("time without time zone");

                entity.Property(e => e.StartTime).HasColumnType("time without time zone");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
