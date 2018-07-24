namespace YggdrasilDataset.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FileHelpers;
    using YggdrasilDataset.Model.Elements;

    public class PliData
    {
        public PliData(string path)
        {
            var engine = new DelimitedFileEngine<PliRow>();
            var res = engine.ReadFile(path);
            foreach (var moduleGroup in res.GroupBy(r => r.Module))
            {
                var module = new Elements.Module { Title = moduleGroup.Key };
                foreach (var yearGroup in moduleGroup.GroupBy(r => r.Year))
                {
                    var moduleData = new ModuleData { Year = yearGroup.Key };
                    foreach (var row in yearGroup)
                    {
                        var datum = new StudentData { Exams = row.MaxExam, Result = row.Result };
                        var vals = typeof(PliRow).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(t => t.PropertyType == typeof(double?) && t.Name.StartsWith("A")).ToList();

                        for (var i = 1; i <= row.MaxReports; i++)
                        {
                            var val = (double?)vals.First(t => t.Name.Equals($"A{i}")).GetValue(row);
                            if (val.HasValue)
                            {
                                datum.Reports.Add(val.Value);
                            }
                        }

                        moduleData.StudentData.Add(datum);
                    }

                    module.Data.Add(moduleData);
                }

                Modules.Add(module);
            }
        }

        public List<Elements.Module> Modules { get; } =
            new List<Elements.Module>();
    }
}
