using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmileTimeNET_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SmileTimeNET_API.src.Domain.Models;
using System.Reflection.Emit;
using SmileTimeNET.Domain.Entities.Dentist;

namespace SmileTimeNET_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carousels> Carousels { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<DentalAppointment> DentalAppointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Carousels
            builder.Entity<Carousels>()
                .HasIndex(c => c.Titulo);

            // Conversation
            builder.Entity<Conversation>(entity =>
            {
                entity.HasKey(c => c.ConversationId);
                entity.Property(c => c.Type).HasMaxLength(20);
            });

            // ConversationParticipant
            builder.Entity<ConversationParticipant>(entity =>
            {
                entity.HasKey(cp => new { cp.ConversationId, cp.UserId });
                entity.HasOne(cp => cp.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(cp => cp.ConversationId);
                entity.HasOne(cp => cp.User)
                    .WithMany(u => u.ConversationParticipants)
                    .HasForeignKey(cp => cp.UserId);
            });

            // Message
            builder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.MessageId);
                entity.Property(m => m.MessageType).HasMaxLength(20);
                entity.HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId);
                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.SenderId);
            });

            // MessageStatus 
            builder.Entity<MessageStatus>(entity =>
            {
                entity.HasKey(ms => new { ms.MessageId, ms.UserId });
                entity.Property(ms => ms.Status).HasMaxLength(20);
                entity.HasOne(ms => ms.Message)
                    .WithMany(m => m.MessageStatuses)
                    .HasForeignKey(ms => ms.MessageId);
                entity.HasOne(ms => ms.User)
                    .WithMany(u => u.MessageStatuses)
                    .HasForeignKey(ms => ms.UserId);
            });

            // Attachment
            builder.Entity<Attachment>(entity =>
            {
                entity.HasKey(a => a.AttachmentId);
                entity.HasOne(a => a.Message)
                    .WithMany(m => m.Attachments)
                    .HasForeignKey(a => a.MessageId);
            });

            // DentalAppointment
            builder.Entity<DentalAppointment>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(u => u.PatientAppointments)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Dentist)
                    .WithMany(u => u.DentistAppointments)
                    .HasForeignKey(d => d.DentistId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}