using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Models;
using savorfolio_backend.Models.enums;

namespace savorfolio_backend.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CustomTag> CustomTags { get; set; }

    public virtual DbSet<CustomTagRecipe> CustomTagRecipes { get; set; }

    public virtual DbSet<IngredientList> IngredientLists { get; set; }

    public virtual DbSet<IngredientType> IngredientTypes { get; set; }

    public virtual DbSet<IngredientVariant> IngredientVariants { get; set; }

    public virtual DbSet<Instruction> Instructions { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeSection> RecipeSections { get; set; }

    public virtual DbSet<RecipeTag> RecipeTags { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRecipe> UserRecipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<CuisineTag>();
        modelBuilder.HasPostgresEnum<DietaryTag>();
        modelBuilder.HasPostgresEnum<MealTag>();
        modelBuilder.HasPostgresEnum<RecipeTypeTag>();
        modelBuilder.HasPostgresEnum<TempUnitsTag>();

        modelBuilder.Entity<CustomTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Custom_Tags_pkey");

            entity.ToTable("Custom_Tags");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.TagName)
                .HasMaxLength(20)
                .HasColumnName("tag_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.CustomTags)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_fk");
        });

        // modelBuilder.Ignore<CustomTagRecipe>();

        modelBuilder.Entity<CustomTagRecipe>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Custom_Tag_Recipes");

            entity.Property(e => e.CustomTagId).HasColumnName("custom_tag_id");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");

            entity.HasOne(d => d.CustomTag).WithMany() //p => p.CustomTagRecipes
                .HasForeignKey(d => d.CustomTagId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("custom_tag_fk");

            entity.HasOne(d => d.Recipe).WithMany()
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("recipe_fk");
        });

        modelBuilder.Entity<IngredientList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ingredient_Lists_pkey");

            entity.ToTable("Ingredient_Lists");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Quantity)
                .HasMaxLength(10)
                .HasColumnName("quantity");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.IngredientOrder).HasColumnName("ingredient_order");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.Qualifier).HasColumnName("qualifier");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.IngredientLists)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("ingredient_fk");

            entity.HasOne(d => d.Recipe).WithMany(p => p.IngredientLists)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipe_fk");

            entity.HasOne(d => d.Unit).WithMany(p => p.IngredientLists)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("unit_fk");
        });

        modelBuilder.Entity<IngredientType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ingredient_Types_pkey");

            entity.ToTable("Ingredient_Types");

            entity.HasIndex(e => e.Name, "Ingredient_Types_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<IngredientVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ingredient_Variants_pkey");

            entity.ToTable("Ingredient_Variants");

            entity.HasIndex(e => e.Name, "Ingredient_Variants_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(400)
                .HasColumnName("name");
            entity.Property(e => e.PluralName)
                .HasMaxLength(400)
                .HasColumnName("plural_name");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Type).WithMany(p => p.IngredientVariants)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("ingredient_type_fk");
        });

        modelBuilder.Entity<Instruction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Instructions_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.InstructionText).HasColumnName("instruction_text");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.StepNumber).HasColumnName("step_number");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Instructions)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipe_fk");

            entity.HasOne(d => d.Section).WithMany(p => p.Instructions)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("section_fk");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notes_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Note1).HasColumnName("note");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Notes)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipe_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Notes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_fk");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Recipe_pkey");

            entity.ToTable("Recipe");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BakeTemp).HasColumnName("bake_temp");
            entity.Property(e => e.CookTime)
                .HasMaxLength(20)
                .HasColumnName("cook_time");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.PrepTime)
                .HasMaxLength(20)
                .HasColumnName("prep_time");
            entity.Property(e => e.Servings)
                .HasMaxLength(20)
                .HasColumnName("servings");
            entity.Property(e => e.Temp_unit).HasColumnName("temp_unit")
                .HasConversion<string>();
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<RecipeSection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Recipe_Sections_pkey");

            entity.ToTable("Recipe_Sections");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.SectionName)
                .HasMaxLength(40)
                .HasColumnName("section_name");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeSections)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipe_fk");
        });

        modelBuilder.Entity<RecipeTag>(entity =>
        {
            entity
                .ToTable("Recipe_Tags")
                .HasKey(e => e.RecipeId);

            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.Meal).HasColumnName("meal_tag")
                .HasConversion<string>();
            entity.Property(e => e.Recipe_type).HasColumnName("type_tag")
                .HasConversion<string>();
            entity.Property(e => e.Cuisine).HasColumnName("cuisine_tag")
                .HasConversion<string>();
            entity.Property(e => e.Dietary).HasColumnName("dietary_tag")
                .HasColumnType("text[]");

            entity.HasOne(d => d.Recipe).WithOne(r => r.RecipeTags)
                .HasForeignKey<RecipeTag>(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("recipe_fk");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Units_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(10)
                .HasColumnName("abbreviation");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.PluralName)
                .HasMaxLength(20)
                .HasColumnName("plural_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "User_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "User_username_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserRecipe>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("User_Recipes");

            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Recipe).WithMany()
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("recipe_fk");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
