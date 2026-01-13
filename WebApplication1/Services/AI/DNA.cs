using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Services.AI
{
    public class DNA
    {
        public List<Gene> Genes { get; set; } = new List<Gene>();
        public double Fitness { get; private set; }

        public class Gene
        {
            public int SubjectId { get; set; }
            public string SubjectName { get; set; }
            public bool IsLab { get; set; }
            public int TeacherId { get; set; }
            public int TeacherSubjectId { get; set; }
            public int Day { get; set; }
            public int SlotId { get; set; }
            public int RoomId { get; set; }
            public int RoomCapacity { get; set; }
        }

        public void CalculateFitness(HashSet<string> lockedTeachers, HashSet<string> lockedRooms, int batchSize, Dictionary<int, int> requiredLectures)
        {
            double score = 10000;
            int hardConflicts = 0;

            // 1. Missing Subjects (Nuclear Penalty)
            var subjectCounts = Genes.GroupBy(g => g.SubjectId).ToDictionary(g => g.Key, g => g.Count());
            foreach (var req in requiredLectures)
            {
                int actual = subjectCounts.ContainsKey(req.Key) ? subjectCounts[req.Key] : 0;
                if (actual < req.Value) score -= (req.Value - actual) * 1000000;
            }

            // 2. Hard Conflicts
            foreach (var g in Genes)
            {
                if (lockedTeachers.Contains($"{g.Day}-{g.SlotId}-{g.TeacherId}")) hardConflicts++;
                if (lockedRooms.Contains($"{g.Day}-{g.SlotId}-{g.RoomId}")) hardConflicts++;
            }

            var teacherCounts = Genes.GroupBy(g => new { g.Day, g.SlotId, g.TeacherId }).Where(x => x.Count() > 1).Sum(x => x.Count());
            var roomCounts = Genes.GroupBy(g => new { g.Day, g.SlotId, g.RoomId }).Where(x => x.Count() > 1).Sum(x => x.Count());
            var slotCounts = Genes.GroupBy(g => new { g.Day, g.SlotId }).Where(x => x.Count() > 1).Sum(x => x.Count());

            hardConflicts += (teacherCounts + roomCounts + slotCounts);
            hardConflicts += Genes.Count(g => g.RoomCapacity < batchSize);

            // 3. Repetition Check (Hard Constraint)
            var subjectGroups = Genes.GroupBy(g => new { g.Day, g.SubjectId });
            foreach (var group in subjectGroups)
            {
                if (group.Count() > 1)
                {
                    // If it's NOT a lab, having it twice in one day is a CRIME
                    if (!group.First().IsLab) hardConflicts++;
                }
            }

            if (hardConflicts > 0)
            {
                Fitness = 1.0 / (hardConflicts + 1);
            }
            else
            {
                // Soft Constraints
                var sortedGenes = Genes.OrderBy(g => g.Day).ThenBy(g => g.SlotId).ToList();
                for (int i = 0; i < sortedGenes.Count - 1; i++)
                {
                    var c = sortedGenes[i];
                    var n = sortedGenes[i + 1];
                    if (c.Day == n.Day && (n.SlotId - c.SlotId == 1))
                    {
                        if (c.RoomId != n.RoomId && !c.IsLab && !n.IsLab) score -= 50;
                    }
                }

                var teacherLoad = Genes.GroupBy(g => new { g.Day, g.TeacherId });
                foreach (var load in teacherLoad)
                {
                    if (load.Count() > 4) score -= 50;
                }

                if (Genes.Select(g => g.Day).Distinct().Count() < 5) score -= 200;

                Fitness = score;
            }
        }
    }
}