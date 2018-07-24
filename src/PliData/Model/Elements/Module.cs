namespace YggdrasilDataset.Model.Elements
{
    using System.Collections.Generic;

    public class Module
    {
        public string Title { get; set; }

        public List<ModuleData> Data { get; set; } = new List<ModuleData>();

        public override string ToString() => Title;
    }
}
