namespace YggdrasilDataset.Model.Elements
{
    using System;
    using System.Collections.Generic;

    public class ModuleData
    {
        public DateTime Year { get; set; }

        public List<StudentData> StudentData { get; set; } = new List<StudentData>();

        public override string ToString() => Year.Year.ToString();
    }
}
