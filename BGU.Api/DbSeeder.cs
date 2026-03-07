namespace BGU.Api;

using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var now = DateTime.UtcNow;

        // ----------------------------
        // 1) Faculties
        // ----------------------------
        const string facultyPhilHistory = "Filologiya-tarix fakültəsi";
        const string facultySocPed = "Sosial-pedoqoji fakültəsi";

        var facultyNames = new[] { facultyPhilHistory, facultySocPed };

        var existingFaculties = await db.Faculties
            .Where(f => facultyNames.Contains(f.Name))
            .ToListAsync(ct);

        var facultyByName = existingFaculties.ToDictionary(f => f.Name);

        foreach (var name in facultyNames)
        {
            if (!facultyByName.ContainsKey(name))
            {
                var f = new Faculty { Name = name, CreatedAt = now };
                db.Faculties.Add(f);
                facultyByName[name] = f;
            }
        }

        // ----------------------------
        // 2) Departments
        // ----------------------------
        var deptNames = new[]
        {
            "Azərbaycan dili və ədəbiyyat kafedrası",
            "Pedaqogika kafedrası",
            "Xarici dillər kafedrası",
            "Riyaziyyat, informatika və təbiət fənləri kafedrası",
            "Psixologiya kafedrası",
            "Tarix kafedrası"
        };

        var deptToFaculty = new Dictionary<string, string>
        {
            // Filologiya-tarix
            ["Azərbaycan dili və ədəbiyyat kafedrası"] = facultyPhilHistory,
            ["Xarici dillər kafedrası"] = facultyPhilHistory,
            ["Tarix kafedrası"] = facultyPhilHistory,

            // Sosial - pedoqoji
            ["Pedaqogika kafedrası"] = facultySocPed,
            ["Psixologiya kafedrası"] = facultySocPed,
            ["Riyaziyyat, informatika və təbiət fənləri kafedrası"] = facultySocPed
        };

        var existingDepts = await db.Departments
            .Where(d => deptNames.Contains(d.Name))
            .ToListAsync(ct);

        foreach (var deptName in deptNames)
        {
            var facultyName = deptToFaculty[deptName];
            var faculty = facultyByName[facultyName];

            var alreadyExists = existingDepts.Any(d =>
                d.Name == deptName &&
                (!string.IsNullOrWhiteSpace(faculty.Id)
                    ? d.FacultyId == faculty.Id
                    : facultyByName[facultyName].Name == facultyName));

            if (!alreadyExists)
            {
                db.Departments.Add(new Department
                {
                    Name = deptName,
                    CreatedAt = now,
                    Faculty = faculty
                });
            }
        }

        // ----------------------------
        // 3) Specializations
        // ----------------------------
        var philHistorySpecs = new[]
        {
            "Azərbaycan dili və ədəbiyyatı müəllimliyi",
            "Tarix müəllimliyi",
            "Xarici dil müəllimliyi",
            "Coğrafiya müəllimliyi",
        };

        var socPedSpecs = new[]
        {
            "Ibtidai sinif müəllimliyi",
            "Məktəbəqədər təhsil",
            "Təhsildə sosial-psixoloji xidmət",
            "Psixologiya",
            "Riyaziyyat və informatika müəllimliyi",
            "Pedaqogika"
        };

        await UpsertSpecializationsAsync(db, facultyByName[facultyPhilHistory], philHistorySpecs, now, ct);
        await UpsertSpecializationsAsync(db, facultyByName[facultySocPed], socPedSpecs, now, ct);

        // ----------------------------
        // 4) Rooms: 4 floors * 21 rooms
        // ----------------------------
        var roomNames = GenerateRoomNames(floors: 4, perFloor: 21); // 101-121, 201-221, ...
        var existingRoomNames = await db.Rooms
            .AsNoTracking()
            .Where(r => roomNames.Contains(r.Name))
            .Select(r => r.Name)
            .ToListAsync(ct);

        var existingRoomSet = existingRoomNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newRooms = new List<Room>();
        foreach (var rn in roomNames)
        {
            if (existingRoomSet.Contains(rn)) continue;

            newRooms.Add(new Room
            {
                Name = rn,
                Capacity = 20,
                CreatedAt = now
            });
        }

        if (newRooms.Count > 0)
            db.Rooms.AddRange(newRooms);

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    private static async Task UpsertSpecializationsAsync(
        AppDbContext db,
        Faculty faculty,
        IEnumerable<string> specNames,
        DateTime now,
        CancellationToken ct)
    {
        var names = specNames.ToArray();

        List<Specialization> existing = new();
        if (!string.IsNullOrWhiteSpace(faculty.Id))
        {
            existing = await db.Specializations
                .Where(s => s.FacultyId == faculty.Id && names.Contains(s.Name))
                .ToListAsync(ct);
        }

        var existingSet = existing.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            if (existingSet.Contains(name)) continue;

            db.Specializations.Add(new Specialization
            {
                Name = name,
                CreatedAt = now,
                Faculty = faculty
            });
        }
    }

    private static List<string> GenerateRoomNames(int floors, int perFloor)
    {
        var names = new List<string>(floors * perFloor);

        for (int floor = 1; floor <= floors; floor++)
        {
            for (int i = 1; i <= perFloor; i++)
            {
                int roomNumber = floor * 100 + i; // 101..121, 201..221 ...
                names.Add($"Otaq {roomNumber}");
            }
        }

        return names;
    }
}