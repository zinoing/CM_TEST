using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using ColorMemory.DTO;

namespace ColorMemory.Data
{
    public class Player
    {
        [Key]
        [Required]
        public string PlayerId { get; set; }
        public string Name { get; set; }

        public int IconId { get; set; }
        public int Score { get; set; }
        public int Money { get; set; }
        public ICollection<PlayerArtwork> PlayerArtworks { get; set; } = new List<PlayerArtwork>();
    }

    public class Artwork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArtworkId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Artist { get; set; }
        [Required]
        public string FileName { get; set; }
    }

    public class PlayerArtwork
    {
        [Required]
        public string PlayerId { get; set; }
        [Required]
        public Player Player { get; set; }
        public int ArtworkId { get; set; }
        [Required]
        public Artwork Artwork { get; set; }
        public int TotalMistakesAndHints { get; set; }
        public int TotalHints { get; set; }
        public int TotalMistakes { get; set; }

        [Required]
        [Column(TypeName = "json")]
        public string Stages { get; set; } = JsonConvert.SerializeObject(
            Enumerable.Range(0, 15).ToDictionary(i => i, i => new StageDTO
            {
                Status = StageStauts.Lock,
                Rank = Rank.NONE,
                HintUsage = -1,
                IncorrectCnt = -1
            })
        );
        public Rank Rank { get; set; }
        public bool HasIt { get; set; } = false;
        public DateTime? ObtainedDate { get; set; }
    }

    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {}

        public DbSet<Player> Players { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<PlayerArtwork> PlayerArtworks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.IconId).HasDefaultValue(0);
                entity.Property(p => p.Score).HasDefaultValue(0);
                entity.Property(p => p.Money).HasDefaultValue(0);

                entity.HasMany(p => p.PlayerArtworks)
                      .WithOne(pa => pa.Player)
                      .HasForeignKey(pa => pa.PlayerId);
            });

            modelBuilder.Entity<Artwork>(entity =>
            {
                entity.Property(a => a.ArtworkId)
                        .ValueGeneratedNever();

                entity.Property(a => a.Title).IsRequired();
                entity.Property(a => a.Artist).IsRequired();
                entity.Property(a => a.FileName).IsRequired();
            });

            modelBuilder.Entity<PlayerArtwork>(entity =>
            {
                entity.HasKey(pa => new { pa.PlayerId, pa.ArtworkId });

                entity.HasOne(pa => pa.Artwork)
                      .WithMany()
                      .HasForeignKey(pa => pa.ArtworkId);

                entity.Property(pa => pa.TotalMistakesAndHints).HasDefaultValue(0);
                entity.Property(pa => pa.TotalHints).HasDefaultValue(0);
                entity.Property(pa => pa.TotalMistakes).HasDefaultValue(0);

                entity.Property(pa => pa.Stages)
                      .IsRequired()
                      .HasColumnType("json");

                entity.Property(pa => pa.Rank)
                      .HasConversion<int>();

                entity.Property(pa => pa.HasIt)
                      .HasDefaultValue(false);

                entity.Property(pa => pa.ObtainedDate)
                      .IsRequired(false)
                      .HasDefaultValueSql(null);
            });
        }
    }
}
