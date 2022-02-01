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
    public DbSet<Encounter> Encounters { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<OracleCategory> OracleCategories { get; set; }
    public DbSet<OracleInfo> OracleInfo { get; set; }
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
        var assetRoot = JsonConvert.DeserializeObject<List<Asset>>(text);

        foreach (var asset in assetRoot)
        {
            for (var i = 0; i < asset.Abilities.Count; i++)
            {
                var ability = asset.Abilities[i];
                ability.Id = $"{asset}#{i + 1}";
            }
        }
        await Assets.AddRangeAsync(assetRoot);
        await SaveChangesAsync();

        file = baseDir.GetFiles("moves.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var moveRoot = JsonConvert.DeserializeObject<MovesInfo>(text);
        // foreach (var move in moveRoot.Moves)
        // {
        //     move.Id = move.Name;
        // }
        await Moves.AddRangeAsync(moveRoot.Moves);
        await SaveChangesAsync();

        file = baseDir.GetFiles("oracles_next.json").FirstOrDefault();
        text = file.OpenText().ReadToEnd();
        var oracleCatList = JsonConvert.DeserializeObject<List<OracleCategory>>(text);

        for (var i2 = 0; i2 < oracleCatList.Count; i2++)
        {
            var oracleCat = oracleCatList[i2];
            oracleCat = await CrawlOracles(oracleCat);
        }
        await OracleCategories.AddRangeAsync(oracleCatList);

        OracleInfo.Include(oracle => oracle.Table).ToList();
        // OracleTables.Include(tbl => tbl.Select(row => row.Ceiling)).ToList();

        SaveChanges();
    }

    public async Task<OracleCategory> CrawlOracles(OracleCategory oracleCat)
    {
        // oracleCat.Id = oracleCat.Path;
        if (oracleCat.Oracles != null)
        {
            for (var i = 0; i < oracleCat.Oracles.Count; i++)
            {
                var oracleInfo = oracleCat.Oracles[i];
                oracleInfo.CategoryId = oracleCat.Id;
                oracleInfo = await CrawlOracles(oracleInfo);
                // await OracleInfo.AddAsync(oracleInfo);
            }
        }
        if (oracleCat.Categories != null)
        {
            for (var i2 = 0; i2 < oracleCat.Categories.Count; i2++)
            {
                var oracleSubcat = oracleCat.Categories[i2];
                // oracleSubcat.Id = oracleSubcat.Path;
                oracleSubcat.CategoryId = oracleCat.Id;
                oracleSubcat = await CrawlOracles(oracleSubcat);
                // await OracleCategories.AddAsync(oracleSubcat);
            }
        }
        return oracleCat;
        // await OracleCategories.AddAsync(oracleCat);
    }
    public async Task<OracleInfo> CrawlOracles(OracleInfo oracleInfo)
    {
        oracleInfo.Id = $"{oracleInfo.Path} / Info";
        if (oracleInfo.Table != null)
        {
            var table = oracleInfo.Table;
            table.Path = oracleInfo.Path;
            // table.OracleInfo = oracleInfo;
            table.OracleInfoId = oracleInfo.Id;
            // table.Category = oracleInfo.Category;
            table.CategoryId = oracleInfo.CategoryId;
            table = await CrawlOracles(table);
            await OracleTables.AddAsync(table);
        }
        if (oracleInfo.Oracles != null)
        {
            for (var i = 0; i < oracleInfo.Oracles.Count; i++)
            {
                var subOracle = oracleInfo.Oracles[i];
                // subOracle.MemberOf = oracleInfo;
                subOracle.MemberOfId = oracleInfo.Id;
                // subOracle.Category = oracleInfo.Category;
                subOracle.CategoryId = oracleInfo.CategoryId;
                subOracle = await CrawlOracles(subOracle);
                // await OracleInfo.AddAsync(subOracle);
            }
        }
        return oracleInfo;
    }
    public async Task<OracleTable> CrawlOracles(OracleTable table)
    {
        table.Id = table.Path;
        for (var i = 0; i < table.Count; i++)
        {
            var row = table[i];
            // row.RowOf = table;
            var rangeString = row.Ceiling != row.Floor ? $"{row.Floor}-{row.Ceiling}" : $"{row.Ceiling}";
            row.Id = $"{table.Id} / {rangeString}";
            row.TableId = table.Id;
            // row.Category = table.Category;
            row.CategoryId = table.CategoryId;
            // row.OracleInfo = table.OracleInfo;
            row.OracleInfoId = table.OracleInfoId;

            if (row.Subtable != null)
            {
                row = await CrawlOracles(row);
                // await CrawlOracles(row.Table);
            }

            await OracleTableRows.AddAsync(row);
        }
        return table;

        // await OracleTables.AddAsync(table);
    }

    public async Task<OracleTableRow> CrawlOracles(OracleTableRow row)
    {
        row.Subtable.EmbeddedIn = row;
        // row.Subtable.RowId = row.Id;
        row.Subtable.Path = row.Path;
        row.Subtable.OracleInfoId = row.OracleInfoId;
        row.Subtable.CategoryId = row.CategoryId;
        var rowName = row.Result.Replace("▶️", "");
        row.Subtable.Id = $"{row.TableId} / {rowName}";
        // OracleTables.Add(row.Table);
        return row;
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

        // Oracles
        // TODO: do as a double key, table.Path + index?

        modelBuilder.Entity<OracleCategory>()
            .HasMany(oracleCat => oracleCat.Categories)
            .WithOne(oracleCat => oracleCat.Category)
            .HasPrincipalKey(oracleCat => oracleCat.Id)
            .HasForeignKey(oracleCat => oracleCat.CategoryId)
            ;
        modelBuilder.Entity<OracleCategory>()
            .HasMany(oracleCat => oracleCat.Oracles)
            .WithOne(oracleInfo => oracleInfo.Category)
            .HasPrincipalKey(oracleCat => oracleCat.Id)
            .HasForeignKey(oracleInfo => oracleInfo.CategoryId);

        modelBuilder.Entity<OracleInfo>()
            .HasMany(oracleInfo => oracleInfo.Table)
            .WithOne(row => row.OracleInfo)
            .HasPrincipalKey(oracleInfo => oracleInfo.Id)
            .HasForeignKey(row => row.OracleInfoId);
        modelBuilder.Entity<OracleInfo>()
            .HasMany(oracleInfo => oracleInfo.Oracles)
            .WithOne(oracleInfo => oracleInfo.MemberOf)
            .HasPrincipalKey(oracleInfo => oracleInfo.Id)
            .HasForeignKey(row => row.MemberOfId);

        modelBuilder.Entity<OracleTable>()
            .HasOne(table => table.OracleInfo)
            .WithOne(info => info.Table)
            .HasPrincipalKey<OracleTable>(table => table.Id)
            .HasForeignKey<OracleInfo>(info => info.TableId)
            ;
        // modelBuilder.Entity<OracleTable>()
        //     .HasOne(table => table.EmbeddedIn)
        //     .WithOne(row => row.Subtable)
        //     .HasPrincipalKey<OracleTableRow>(row => row.SubtableId)
        //     .HasForeignKey<OracleTable>(table => table.RowId);
        modelBuilder.Entity<OracleTableRow>()
            .HasOne(row => row.Subtable)
            .WithOne(table => table.EmbeddedIn)
            .HasForeignKey<OracleTable>(table => table.RowId);
        modelBuilder.Entity<OracleTableRow>()
            .HasOne(row => row.Category)
            .WithMany()
            .HasForeignKey(row => row.CategoryId);
        modelBuilder.Entity<OracleTableRow>()
            .HasOne(row => row.RowOf)
            .WithMany()
            .HasForeignKey(row => row.TableId);
        // Assets
        modelBuilder.Entity<Asset>().Property(a =>
            a.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Asset>().Property(a =>
            a.Input).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        // Moves
        modelBuilder.Entity<MoveStatOptions>().Property(s => s.Legacies).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(s => s.Resources).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(s => s.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
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
        // modelBuilder.Entity<MoveStatOptions>().Property(a => a.Progress).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<MoveStatOptions>().Property(a => a.Stats).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleInfo>().Property(o => o.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<Select>().Property(a => a.Options).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<OracleCategory>().Property(s => s.Aliases).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleCategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
        modelBuilder.Entity<OracleCategory>().Property(s => s.SampleNames).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);

        modelBuilder.Entity<Encounter>().Property(e => e.Drives).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer); modelBuilder.Entity<Encounter>().Property(e => e.Features).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer); modelBuilder.Entity<Encounter>().Property(e => e.Tactics).HasConversion(stringArrayToCSVConverter).Metadata.SetValueComparer(valueComparer);
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
