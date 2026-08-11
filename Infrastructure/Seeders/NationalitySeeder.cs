using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Seeders
{
    public static class NationalitySeeder
    {
        public static async Task SeedAsync(IUnitOfWork uow)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "nationalities.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed file not found: {filePath}");

            var json = await File.ReadAllTextAsync(filePath);

            var nationalities = JsonSerializer.Deserialize<List<Nationality>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (nationalities == null || !nationalities.Any())
                return;

            var repo = uow.Repository<Nationality>();

            var existingIsoCodes = repo.GetQueryableWithSpec(null).Select(x => x.IsoCode).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newNationalities = nationalities.Where(x => !existingIsoCodes.Contains(x.IsoCode));

            await repo.AddRangeAsync(newNationalities);

            await uow.SaveChangeAsync();
        }
    }
}
