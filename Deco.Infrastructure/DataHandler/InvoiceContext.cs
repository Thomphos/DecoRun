
using Deco.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Infrastructure.Entities
{
    public class InvoiceContext : DbContext
    {
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
  
        }

        public void Configure(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<InvoiceContext>();
                context.Database.EnsureCreated();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InvoiceHeader>(entity =>
            {
                entity.ToTable("InvoiceHeader");

                entity.HasKey(e => e.InvoiceId)
                      .HasName("PK_InvoiceHeader");

                entity.Property(e => e.InvoiceId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.InvoiceNumber)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(e => e.InvoiceDate)
                      .HasColumnType("date");

                entity.Property(e => e.Address)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.InvoiceTotal)
                      .HasColumnType("float");
            });

            modelBuilder.Entity<InvoiceLine>(entity =>
            {
                entity.ToTable("InvoiceLines");

                entity.HasKey(e => e.LineId)
                      .HasName("PK_InvoiceLines");

                entity.Property(e => e.LineId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.InvoiceNumber)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasColumnType("varchar(100)");

                entity.Property(e => e.Quantity)
                      .HasColumnType("float");

                entity.Property(e => e.UnitSellingPriceExVAT)
                      .HasColumnType("float");

       
                modelBuilder.Entity<InvoiceLine>()
                    .HasOne(il => il.InvoiceHeader)
                    .WithMany(ih => ih.InvoiceLines)
                    .HasForeignKey(il => il.InvoiceNumber)
                    .HasPrincipalKey(ih => ih.InvoiceNumber)
                    .OnDelete(DeleteBehavior.Cascade); // Adjust based on needs
            });
        }

    }

}

