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

        // 1) Faculties
        const string facultyPhilHistory = "Filologiya-tarix fakültəsi";
        const string facultySocPed = "Sosial-pedoqoji fakültəsi";
        const string facultyInfo = "Infromatika-pedaqoji fakültəsi";

        var facultyNames = new[] { facultyPhilHistory, facultySocPed, facultyInfo };

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

        await db.SaveChangesAsync(ct); // IDs are now set

        // 2) Departments
        var deptNames = new[]
        {
            "Azərbaycan dili və ədəbiyyat kafedrası",
            "Pedaqogika kafedrası",
            "Xarici dillər kafedrası",
            "Riyaziyyat, informatika və təbiət fənləri kafedrası",
            "Psixologiya kafedrası",
            "Tarix kafedrası",
            "İnformatika və proqramlaşdırma kafedrası",
            "Rəqəmsal texnologiyalar kafedrası",
        };

        var deptToFaculty = new Dictionary<string, string>
        {
            ["Azərbaycan dili və ədəbiyyat kafedrası"] = facultyPhilHistory,
            ["Xarici dillər kafedrası"] = facultyPhilHistory,
            ["Tarix kafedrası"] = facultyPhilHistory,
            ["Pedaqogika kafedrası"] = facultySocPed,
            ["Psixologiya kafedrası"] = facultySocPed,
            ["Riyaziyyat, informatika və təbiət fənləri kafedrası"] = facultySocPed,
            ["İnformatika və proqramlaşdırma kafedrası"] = facultyInfo,
            ["Rəqəmsal texnologiyalar kafedrası"] = facultyInfo,
        };

        var facultyIds = facultyByName.Values.Select(f => f.Id).ToList();
        var existingDepts = await db.Departments
            .Where(d => deptNames.Contains(d.Name) && facultyIds.Contains(d.FacultyId))
            .ToListAsync(ct);

        var existingDeptSet = existingDepts
            .Select(d => (d.Name, d.FacultyId))
            .ToHashSet();

        foreach (var deptName in deptNames)
        {
            var faculty = facultyByName[deptToFaculty[deptName]];
            if (!existingDeptSet.Contains((deptName, faculty.Id)))
            {
                db.Departments.Add(new Department
                {
                    Name = deptName,
                    CreatedAt = now,
                    FacultyId = faculty.Id
                });
            }
        }

        await db.SaveChangesAsync(ct); // save before specs

        // 3) Specializations
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

        var infoSpecs = new[]
        {
            "İnformatika müəllimliyi",
            "Kompüter elmləri",
            "Proqram mühəndisliyi",
            "İnformasiya texnologiyaları",
        };

        await UpsertSpecializationsAsync(db, facultyByName[facultyInfo], infoSpecs, now, ct);
        await UpsertSpecializationsAsync(db, facultyByName[facultyPhilHistory], philHistorySpecs, now, ct);
        await UpsertSpecializationsAsync(db, facultyByName[facultySocPed], socPedSpecs, now, ct);

        // 4) Rooms
        var roomNames = GenerateRoomNames(floors: 5, perFloor: 21);
        var existingRoomSet = (await db.Rooms
                .AsNoTracking()
                .Where(r => roomNames.Contains(r.Name))
                .Select(r => r.Name)
                .ToListAsync(ct))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newRooms = roomNames
            .Where(rn => !existingRoomSet.Contains(rn))
            .Select(rn => new Room { Name = rn, Capacity = 20, CreatedAt = now })
            .ToList();

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