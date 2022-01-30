using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TheOracle2.DataClasses;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent;

public class EFContext : DbContext
{
    public EFContext(DbContextOptions<EFContext> options) : base(options)
    {
        //Database.EnsureCreated();
    }

    public DbSet<OracleGuild> OracleGuilds { get; set; }
    public DbSet<GuildPlayer> GuildPlayers { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<OracleCategory> OracleCategories { get; set; }
    public DbSet<OracleTable> OracleTables { get; set; }
    public DbSet<OracleTableRow> OracleTableRows { get; set; }
    public DbSet<Ability> AssetAbilities { get; set; }
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }

    public async Task RecreateDB()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();

        var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
        var file = baseDir.GetFiles("assets.json").FirstOrDefault();

        string text = file.OpenText().ReadToEnd();
        var root = JsonConvert.DeserializeObject<List<Asset>>(text);

        foreach (var asset in root)
        {
            asset.Id = asset.Name;
            var counter = 1;
            foreach (var ability in asset.Abilities)
            {
                ability.Id = $"{asset.Id}#{counter}";
                counter++;
            }
            Assets.Add(asset);
        }
        await SaveChangesAsync();

        file = baseDir.GetFiles("moves.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var moveRoot = JsonConvert.DeserializeObject<MovesInfo>(text);

        foreach (var move in moveRoot.Moves)
        {
            move.Id = move.Name;
            Moves.Add(move);
        }
        await SaveChangesAsync();

        file = baseDir.GetFiles("oracles_next.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var oracleCatList = JsonConvert.DeserializeObject<List<OracleCategory>>(text);
        foreach (var oracleCat in oracleCatList)
        {
            Add(oracleCat);
        }

        await SaveChangesAsync();
        var thing = this;
    }
    public bool HasTables() => Database.GetService<IRelationalDatabaseCreator>().HasTables();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("NOCASE");

        var stringArrayToCSVConverter = new ValueConverter<IList<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IList<string>>(v)
            );

        var valueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
            );

        var requiresConverter = new ValueConverter<IDictionary<string, string[]>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<IDictionary<string, string[]>>(v)
            );

        var requiresComparer = new ValueComparer<IDictionary<string, string[]>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c
            );

        //TheOracle Stuff
        modelBuilder.Entity<PlayerCharacter>().Property(pc => pc.Impacts).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<GuildPlayer>().HasKey(guildPlayer => new { guildPlayer.UserId, guildPlayer.DiscordGuildId });

        //Dataforged stuff
        modelBuilder.Entity<Ability>().Property(a => a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        // for table rows
        // modelBuilder.Entity<OracleTableRow>().HasKey(row => row.Path);

        // modelBuilder.Entity<OracleTable>().HasKey(table => table.Path);

        // modelBuilder.Entity<Oracle>().HasKey();

        // Assets
        modelBuilder.Entity<Asset>().Property(a =>
            a.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Asset>().Property(a =>
            a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<AssetStatOptions>().Property(s => s.Legacies).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetStatOptions>().Property(s => s.Resources).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<AssetStatOptions>().Property(s => s.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        // modelBuilder.Entity<Attributes>().Property(a => a.DerelictType).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        // modelBuilder.Entity<Attributes>().Property(a => a.Location).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        // modelBuilder.Entity<Attributes>().Property(a => a.Type).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleTableRow>().Property(c =>
            c.Assets).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleTableRow>().Property(c =>
            c.OracleRolls).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleTableRow>().Property(c =>
            c.PartOfSpeech).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);


        modelBuilder.Entity<Requires>().Property(c => c.Paths).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Requires>().Property(c => c.Results).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<Suggestions>().Property(c => c.OracleRolls).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<ConditionMeter>().Property(c => c.Conditions).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Legacies).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Progress).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Oracle>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Select>().Property(a => a.Options).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<OracleCategory>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleCategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleCategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            //.UseNpgsql("Host=TheOracle;Database=GameData;")
            .UseSqlite("Data Source=GameContent.db;Cache=Shared")
            .UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
}
