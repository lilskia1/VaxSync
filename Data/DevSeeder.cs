using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VaxSync.Web.Data;
using VaxSync.Web.Models;

public static class DevSeeder
{
    public const int DefaultBatchSize = 1_000;
    private static readonly Random _rng = new();

    public static async Task SeedAsync(ApplicationDbContext db, int targetStudentCount, int schoolCount, int batchSize = DefaultBatchSize)
    {
        if (targetStudentCount <= 0) return;
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        // 0) Fast exit if already seeded near target
        var current = await db.Students.CountAsync();
        if (current >= targetStudentCount * 0.95) return;

        // 1) Vaccines catalog (keep it small & plausible for PVAC-like flows)
        // You can refine codes/names later without touching the generator’s shape.
        if (!await db.Vaccines.AnyAsync())
        {
            db.Vaccines.AddRange(new[]
            {
                new Vaccine { Code = "MMR",   Name = "Measles, Mumps, Rubella" },
                new Vaccine { Code = "POLIO", Name = "Polio (IPV)" },
                new Vaccine { Code = "DTaP",  Name = "Diphtheria, Tetanus, Pertussis" },
                new Vaccine { Code = "Tdap",  Name = "Tetanus, Diphtheria, Pertussis (Adolescent)" },
                new Vaccine { Code = "HEPB",  Name = "Hepatitis B" },
                new Vaccine { Code = "VAR",   Name = "Varicella" },
                new Vaccine { Code = "MCV4",  Name = "Meningococcal ACWY" },
                new Vaccine { Code = "HPV",   Name = "Human Papillomavirus" },
            });
            await db.SaveChangesAsync();
        }

        var vaccineMap = await db.Vaccines.ToDictionaryAsync(v => v.Code, v => v.Id);

        // 2) Tiny “schedule hint” table: dose counts by age band (for generator logic only).
        // This is *not* your compliance engine; it just helps the generator pick doses/dates.
        if (!await db.VaccineSchedules.AnyAsync())
        {
            var vs = new List<VaccineSchedule>();
            void Add(string code, string band, int dose, bool catchup) =>
                vs.Add(new VaccineSchedule { VaccineId = vaccineMap[code], AgeRange = band, DoseNumber = dose, CatchUpEligible = catchup });

            // Example bands — tune as you like
            Add("DTaP", "0–6 years", 1, true); Add("DTaP", "0–6 years", 2, true); Add("DTaP", "0–6 years", 3, true); Add("DTaP", "0–6 years", 4, true);
            Add("Tdap", "11–13 yrs", 1, true);
            Add("POLIO", "0–6 years", 1, true); Add("POLIO", "0–6 years", 2, true); Add("POLIO", "0–6 years", 3, true); Add("POLIO", "4–6 years", 4, true);
            Add("MMR", "1–6 years", 1, true); Add("MMR", "4–6 years", 2, true);
            Add("VAR", "1–6 years", 1, true); Add("VAR", "4–6 years", 2, true);
            Add("HEPB", "0–1 years", 1, true); Add("HEPB", "0–1 years", 2, true); Add("HEPB", "0–1 years", 3, true);
            Add("MCV4", "11–18 yrs", 1, true);
            Add("HPV", "11–18 yrs", 1, true); Add("HPV", "11–18 yrs", 2, true);

            db.VaccineSchedules.AddRange(vs);
            await db.SaveChangesAsync();
        }

        // 3) Schools (≈860). Uses your School model (string Id, Code, Name). :contentReference[oaicite:11]{index=11}
        if (!await db.Schools.AnyAsync())
        {
            var schools = new List<School>(schoolCount);
            for (int i = 1; i <= schoolCount; i++)
            {
                schools.Add(new School
                {
                    Code = $"S-{i:0000}",
                    Name = MakeSchoolName(i)
                });
            }
            db.Schools.AddRange(schools);
            await db.SaveChangesAsync();
        }
        var schoolIds = await db.Schools.Select(s => new { s.Id, s.Code }).ToListAsync();

        // 4) Generate Students in batches to keep memory & transaction time sane.
        int remaining = targetStudentCount - current;
        while (remaining > 0)
        {
            int take = Math.Min(batchSize, remaining);
            var students = new List<Student>(take);
            var vaccines = new List<StudentVaccine>(take * 6); // rough average

            for (int i = 0; i < take; i++)
            {
                var age = RandomAge();                              // 4–18
                var dob = RandomDOB(age);
                var school = schoolIds[_rng.Next(schoolIds.Count)]; // random school

                var student = new Student
                {
                    SchoolId = school.Id,                           // matches Student.SchoolId (string) :contentReference[oaicite:12]{index=12}
                    FirstName = RandomFirstName(),
                    LastName = RandomLastName(),
                    DateOfBirth = dob,
                    IsCompliant = false,                            // can compute later; set after we add doses if you want. :contentReference[oaicite:13]{index=13}
                };

                // Build a plausible immunization history for this student
                vaccines.AddRange(BuildVaccinationHistory(student, dob, vaccineMap));

                students.Add(student);
            }

            db.Students.AddRange(students);
            await db.SaveChangesAsync();

            // Connect vaccine rows (need StudentId after SaveChanges)
            foreach (var v in vaccines)
            {
                // Each v.Student holds a reference object; set foreign key instead
                v.StudentId = v.Student.Id;  // StudentVaccine.StudentId is string FK :contentReference[oaicite:14]{index=14}
                v.Student = null!;
            }
            db.StudentVaccines.AddRange(vaccines);
            await db.SaveChangesAsync();

            remaining -= take;
        }

        // 5) Populate StudentRequiredDose so due/overdue UI can light up later. :contentReference[oaicite:15]{index=15}
        await PopulateStudentRequiredDosesAsync(db);

        // 6) Done. You now have a “live” synthetic DB — UI reads via EF, reports print from rows. :contentReference[oaicite:16]{index=16}
    }

    private static IEnumerable<StudentVaccine> BuildVaccinationHistory(Student s, DateTime dob, Dictionary<string, int> vid)
    {
        // We’ll create doses only up to the student’s current age window,
        // randomly miss some (to simulate non-compliance / catch-up).
        var list = new List<StudentVaccine>();

        void Add(string code, int doseNumber, DateTime when)
        {
            // ~15% chance the dose is “missing” to create realistic gaps
            if (_rng.NextDouble() < 0.15) return;
            list.Add(new StudentVaccine
            {
                Student = s,
                VaccineId = vid[code],
                DoseNumber = doseNumber,
                DateGiven = when
            });
        }

        int age = GetAge(dob, DateTime.Today);

        // Infant/early childhood sets (if age allows)
        if (age >= 1)
        {
            Add("HEPB", 1, dob.AddMonths(1 + _rng.Next(0, 2)));
            Add("HEPB", 2, dob.AddMonths(2 + _rng.Next(0, 2)));
            Add("HEPB", 3, dob.AddMonths(6 + _rng.Next(0, 2)));

            Add("MMR", 1, dob.AddYears(1).AddDays(_rng.Next(0, 120)));
            Add("VAR", 1, dob.AddYears(1).AddDays(_rng.Next(0, 120)));
        }
        if (age >= 4)
        {
            // preschool boosters
            Add("DTaP", 3, dob.AddYears(3).AddMonths(_rng.Next(0, 6)));
            Add("POLIO", 3, dob.AddYears(3).AddMonths(_rng.Next(0, 6)));
        }
        if (age >= 5)
        {
            // kindergarten boosters
            Add("MMR", 2, dob.AddYears(5).AddMonths(_rng.Next(0, 6)));
            Add("VAR", 2, dob.AddYears(5).AddMonths(_rng.Next(0, 6)));
            Add("POLIO", 4, dob.AddYears(5).AddMonths(_rng.Next(0, 6)));
            Add("DTaP", 4, dob.AddYears(5).AddMonths(_rng.Next(0, 6)));
        }
        if (age >= 11)
        {
            Add("Tdap", 1, dob.AddYears(11).AddMonths(_rng.Next(0, 6)));
            Add("MCV4", 1, dob.AddYears(11).AddMonths(_rng.Next(0, 6)));
            // HPV: 2-dose series; randomize completion
            Add("HPV", 1, dob.AddYears(12).AddMonths(_rng.Next(0, 6)));
            Add("HPV", 2, dob.AddYears(12).AddMonths(6 + _rng.Next(0, 4)));
        }

        return list;
    }

    private static readonly Dictionary<string, Dictionary<int, int>> DoseDueMonths = new(StringComparer.OrdinalIgnoreCase)
    {
        ["DTaP"] = new Dictionary<int, int> { [1] = 2, [2] = 4, [3] = 6, [4] = 60 },
        ["POLIO"] = new Dictionary<int, int> { [1] = 2, [2] = 4, [3] = 6, [4] = 60 },
        ["MMR"] = new Dictionary<int, int> { [1] = 12, [2] = 60 },
        ["VAR"] = new Dictionary<int, int> { [1] = 12, [2] = 60 },
        ["HEPB"] = new Dictionary<int, int> { [1] = 0, [2] = 1, [3] = 6 },
        ["Tdap"] = new Dictionary<int, int> { [1] = 132 },
        ["MCV4"] = new Dictionary<int, int> { [1] = 132 },
        ["HPV"] = new Dictionary<int, int> { [1] = 144, [2] = 150 }
    };

    private record ScheduleInfo(int Id, int VaccineId, string VaccineCode, int DoseNumber, string AgeRange);

    private record StudentStub(string Id, DateTime Dob);

    private static async Task PopulateStudentRequiredDosesAsync(ApplicationDbContext db)
    {
        if (await db.StudentRequiredDoses.AnyAsync()) return;

        var schedules = await db.VaccineSchedules
            .AsNoTracking()
            .Include(vs => vs.Vaccine)
            .ToListAsync();

        var scheduleInfos = schedules
            .Select(vs => new ScheduleInfo(vs.Id, vs.VaccineId, vs.Vaccine.Code, vs.DoseNumber, vs.AgeRange))
            .ToList();

        if (scheduleInfos.Count == 0) return;

        var students = await db.Students
            .AsNoTracking()
            .Select(s => new StudentStub(s.Id, s.DateOfBirth))
            .ToListAsync();

        if (students.Count == 0) return;

        var vaccineRecords = await db.StudentVaccines
            .AsNoTracking()
            .Select(v => new { v.StudentId, v.VaccineId, v.DoseNumber })
            .ToListAsync();

        var vaccineLookup = vaccineRecords
            .GroupBy(v => v.StudentId)
            .ToDictionary(g => g.Key, g => g.Select(x => (x.VaccineId, x.DoseNumber)).ToHashSet());

        var today = DateTime.Today;
        var buffer = new List<StudentRequiredDose>(DefaultBatchSize);
        var compliance = new Dictionary<string, bool>(students.Count);

        foreach (var student in students)
        {
            var owned = vaccineLookup.TryGetValue(student.Id, out var set)
                ? set
                : new HashSet<(int VaccineId, int DoseNumber)>();

            compliance[student.Id] = true;

            foreach (var schedule in scheduleInfos)
            {
                var completed = owned.Contains((schedule.VaccineId, schedule.DoseNumber));
                var dueDate = CalculateDueDate(student.Dob, schedule);

                if (!completed && dueDate <= today)
                {
                    compliance[student.Id] = false;
                }

                buffer.Add(new StudentRequiredDose
                {
                    StudentId = student.Id,
                    VaccineScheduleId = schedule.Id,
                    DoseNumber = schedule.DoseNumber,
                    DueDate = dueDate,
                    Completed = completed
                });

                if (buffer.Count >= DefaultBatchSize)
                {
                    await db.StudentRequiredDoses.AddRangeAsync(buffer);
                    await db.SaveChangesAsync();
                    buffer.Clear();
                }
            }
        }

        if (buffer.Count > 0)
        {
            await db.StudentRequiredDoses.AddRangeAsync(buffer);
            await db.SaveChangesAsync();
        }

        await UpdateComplianceAsync(db, compliance);
    }

    private static async Task UpdateComplianceAsync(ApplicationDbContext db, Dictionary<string, bool> compliance)
    {
        if (compliance.Count == 0) return;

        const int chunkSize = 1_000;
        var ids = compliance.Keys.ToList();

        for (int i = 0; i < ids.Count; i += chunkSize)
        {
            var slice = ids.Skip(i).Take(chunkSize).ToList();
            var students = await db.Students
                .Where(s => slice.Contains(s.Id))
                .ToListAsync();

            foreach (var student in students)
            {
                if (compliance.TryGetValue(student.Id, out var isCompliant))
                {
                    student.IsCompliant = isCompliant;
                }
            }

            await db.SaveChangesAsync();
        }
    }

    private static DateTime CalculateDueDate(DateTime dob, ScheduleInfo schedule)
    {
        var months = ResolveDueMonths(schedule);
        var due = dob.AddMonths(months);

        // Jitter a bit so tables show varied dates
        due = due.AddDays(_rng.Next(-15, 16));

        // Prevent impossible dates (before birth)
        if (due < dob.AddDays(30))
            due = dob.AddDays(30);

        return due;
    }

    private static int ResolveDueMonths(ScheduleInfo schedule)
    {
        if (DoseDueMonths.TryGetValue(schedule.VaccineCode, out var doses) &&
            doses.TryGetValue(schedule.DoseNumber, out var months))
        {
            return months;
        }

        var (start, end) = ParseAgeRange(schedule.AgeRange);
        return end != 0 ? end : Math.Max(start, 6);
    }

    private static (int startMonths, int endMonths) ParseAgeRange(string ageRange)
    {
        if (string.IsNullOrWhiteSpace(ageRange)) return (0, 0);

        var normalized = ageRange.Replace('–', '-').ToLowerInvariant();
        var numbers = Regex.Matches(normalized, "\\d+")
            .Select(m => int.Parse(m.Value, CultureInfo.InvariantCulture))
            .ToArray();

        var usesMonths = normalized.Contains("month");
        var multiplier = usesMonths ? 1 : 12;

        int start = numbers.Length > 0 ? numbers[0] : 0;
        int end = numbers.Length > 1 ? numbers[1] : start;

        return (start * multiplier, end * multiplier);
    }

    // ——— helpers ———

    private static int RandomAge() => _rng.Next(4, 19); // 4–18 inclusive

    private static DateTime RandomDOB(int age)
    {
        var today = DateTime.Today;
        var start = today.AddYears(-(age + 1));
        var end = today.AddYears(-age);
        return RandomDate(start, end);
    }

    private static DateTime RandomDate(DateTime start, DateTime end)
    {
        var range = (end - start).Days;
        return start.AddDays(_rng.Next(range + 1));
    }

    private static int GetAge(DateTime dob, DateTime on) =>
        (on.Year - dob.Year) - (on.Date < dob.Date.AddYears(on.Year - dob.Year) ? 1 : 0);

    private static string MakeSchoolName(int i)
    {
        // Sprinkle a few Spanish names for PR vibe; keep it synthetic.
        string[] baseNames = {
            "Escuela Superior",
            "Escuela Intermedia",
            "Escuela Elemental",
            "Colegio Regional",
            "Academia Municipal"
        };
        string[] surnames = { "Betances", "Hostos", "Marín", "Sotomayor", "Balseiro", "Cordero", "Piñero", "Valentín", "Rivera", "Pérez" };

        var kind = baseNames[_rng.Next(baseNames.Length)];
        var who = surnames[_rng.Next(surnames.Length)];
        return $"{kind} {who} #{i:0000}";
    }

    private static string RandomFirstName()
    {
        string[] options = { "Ana", "Carlos", "María", "Luis", "Sofía", "Javier", "Valeria", "José", "Camila", "Diego",
                             "Paola", "Héctor", "Andrea", "Isabela", "Daniel", "Gabriela", "Angel", "Lucía", "Mateo", "Alexa" };
        return options[_rng.Next(options.Length)];
    }

    private static string RandomLastName()
    {
        string[] options = { "Rivera", "Pérez", "Santiago", "González", "Rodríguez", "Martínez", "Hernández", "López", "Torres", "Vega",
                             "Morales", "Cruz", "Ortiz", "Ramírez", "Figueroa", "Vázquez", "Díaz", "Colón", "Nieves", "Cordero" };
        return options[_rng.Next(options.Length)];
    }
}
