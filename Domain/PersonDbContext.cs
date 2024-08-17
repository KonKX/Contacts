using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Person> Persons => Set<Person>();
        public DbSet<Country> Countries => Set<Country>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Bind DB tables to classes
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Read data from json files
            var countriesJson = File.ReadAllText("countries.json");
            var personsJson = File.ReadAllText("persons.json");

            //Deserialize json files
            var countries = JsonSerializer.Deserialize<Country[]>(countriesJson);
            var persons = JsonSerializer.Deserialize<Person[]>(personsJson);

            //Seed
            modelBuilder.Entity<Country>().HasData(countries!);
            modelBuilder.Entity<Person>().HasData(persons!);
        }
    }
}
