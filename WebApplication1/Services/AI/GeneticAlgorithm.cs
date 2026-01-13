using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Services.AI
{
    public class GeneticAlgorithm
    {
        public List<DNA> Population { get; private set; }
        private Random _rnd = new Random();

        // Dependencies
        private List<Subject> _subjects;
        private List<TeacherSubject> _teacherMappings;
        private List<TimeSlot> _slots;
        private List<Room> _rooms;
        private HashSet<string> _lockedTeachers;
        private HashSet<string> _lockedRooms;
        private int _batchId;
        private int _batchSize;
        private Dictionary<int, int> _requiredLectures;

        public GeneticAlgorithm(
            List<Subject> subjects,
            List<TeacherSubject> teacherMappings,
            List<TimeSlot> slots,
            List<Room> rooms,
            HashSet<string> lockedTeachers,
            HashSet<string> lockedRooms,
            int batchId,
            int batchSize)
        {
            _subjects = subjects;
            _teacherMappings = teacherMappings;
            _slots = slots;
            _rooms = rooms;
            _lockedTeachers = lockedTeachers;
            _lockedRooms = lockedRooms;
            _batchId = batchId;
            _batchSize = batchSize;
            _requiredLectures = _subjects.ToDictionary(s => s.Id, s => s.WeeklyLectures);

            Population = new List<DNA>();
            for (int i = 0; i < 50; i++)
            {
                var dna = CreateRandomDNA();
                if (dna.Genes.Count > 0) Population.Add(dna);
            }
        }

        public DNA RunEvolution(int generations)
        {
            if (Population == null || Population.Count == 0) return null;

            for (int i = 0; i < generations; i++)
            {
                foreach (var dna in Population) dna.CalculateFitness(_lockedTeachers, _lockedRooms, _batchSize, _requiredLectures);
                Population = Population.OrderByDescending(x => x.Fitness).ToList();

                // If Fitness > 500, we have a valid schedule (No hard conflicts)
                if (Population.First().Fitness > 500) return Population.First();

                var newGen = new List<DNA>();
                newGen.AddRange(Population.Take(5)); // Elitism

                while (newGen.Count < 50)
                {
                    var p1 = Population[_rnd.Next(Math.Min(20, Population.Count))];
                    var p2 = Population[_rnd.Next(Math.Min(20, Population.Count))];

                    var child = Crossover(p1, p2);
                    Mutate(child);
                    newGen.Add(child);
                }
                Population = newGen;
            }
            return Population.OrderByDescending(x => x.Fitness).FirstOrDefault();
        }

        private DNA CreateRandomDNA()
        {
            var dna = new DNA();
            foreach (var sub in _subjects)
            {
                var teachers = _teacherMappings.Where(t => t.SubjectId == sub.Id).ToList();
                if (!teachers.Any()) continue;

                // *** STRATEGY: ROUND ROBIN ***
                int currentDay = _rnd.Next(1, 6);

                for (int i = 0; i < sub.WeeklyLectures; i++)
                {
                    var tm = teachers[_rnd.Next(teachers.Count)];

                    // *** FIX: FILTER ROOMS STRICTLY ***
                    // If Subject is Lab, only pick rooms where isLab == true
                    // If Subject is Theory, only pick rooms where isLab == false
                    var suitableRooms = _rooms.Where(r => r.isLab == sub.IsLab).ToList();

                    // Fallback (just in case)
                    if (!suitableRooms.Any()) suitableRooms = _rooms;

                    var room = suitableRooms[_rnd.Next(suitableRooms.Count)];

                    dna.Genes.Add(new DNA.Gene
                    {
                        SubjectId = sub.Id,
                        SubjectName = sub.Name,
                        IsLab = sub.IsLab,
                        TeacherSubjectId = tm.Id,
                        TeacherId = tm.TeacherId,
                        Day = currentDay,
                        SlotId = _slots[_rnd.Next(_slots.Count)].Id,
                        RoomId = room.Id,
                        RoomCapacity = room.capacity
                    });

                    currentDay++;
                    if (currentDay > 5) currentDay = 1;
                }
            }
            return dna;
        }

        private DNA Crossover(DNA p1, DNA p2)
        {
            var child = new DNA();
            for (int i = 0; i < p1.Genes.Count; i++)
            {
                if (i >= p2.Genes.Count) { child.Genes.Add(CloneGene(p1.Genes[i])); continue; }

                if (_rnd.NextDouble() > 0.5)
                    child.Genes.Add(CloneGene(p1.Genes[i]));
                else
                    child.Genes.Add(CloneGene(p2.Genes[i]));
            }
            return child;
        }

        private DNA.Gene CloneGene(DNA.Gene g)
        {
            return new DNA.Gene
            {
                SubjectId = g.SubjectId,
                SubjectName = g.SubjectName,
                IsLab = g.IsLab,
                TeacherId = g.TeacherId,
                TeacherSubjectId = g.TeacherSubjectId,
                Day = g.Day,
                SlotId = g.SlotId,
                RoomId = g.RoomId,
                RoomCapacity = g.RoomCapacity
            };
        }

        private void Mutate(DNA dna)
        {
            if (dna.Genes.Count == 0) return;

            if (_rnd.NextDouble() < 0.3)
            {
                int idx = _rnd.Next(dna.Genes.Count);
                var gene = dna.Genes[idx];

                // *** STRATEGY: FIND AN EMPTY DAY ***
                var busyDays = dna.Genes
                    .Where(g => g.SubjectId == gene.SubjectId && g != gene)
                    .Select(g => g.Day)
                    .ToHashSet();

                var freeDays = Enumerable.Range(1, 5).Where(d => !busyDays.Contains(d)).ToList();

                if (freeDays.Any())
                {
                    gene.Day = freeDays[_rnd.Next(freeDays.Count)];
                }
                else
                {
                    gene.Day = _rnd.Next(1, 6);
                }

                gene.SlotId = _slots[_rnd.Next(_slots.Count)].Id;

                // *** FIX: FILTER ROOMS STRICTLY HERE TOO ***
                // When mutating, ensure we don't accidentally put a Lab in a Theory room
                var suitableRooms = _rooms.Where(r => r.isLab == gene.IsLab).ToList();
                if (!suitableRooms.Any()) suitableRooms = _rooms;

                var newRoom = suitableRooms[_rnd.Next(suitableRooms.Count)];
                gene.RoomId = newRoom.Id;
                gene.RoomCapacity = newRoom.capacity;
            }
        }
    }
}