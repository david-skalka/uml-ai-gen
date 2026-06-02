#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TodoAppApi.Migrations;

[DbContext(typeof(ApplicationDbContext))]
internal class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "9.0.8");

        modelBuilder.Entity("TodoAppApi.Models.Alarm", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<DateTime>("Time")
                .HasColumnType("TEXT");

            b.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("Alarms");
        });

        modelBuilder.Entity("TodoAppApi.Models.TodoList", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            b.Property<string>("Description")
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnType("TEXT");

            b.Property<bool>("IsArchived")
                .HasColumnType("INTEGER");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("TodoLists");
        });
#pragma warning restore 612, 618
    }
}