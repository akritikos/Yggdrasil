namespace YggdrasilDataset.Model.Elements
{
    using System.Collections.Generic;
    using YggdrasilDataset.Discriminators;

    public class StudentData
    {
        public List<double> Reports { get; set; } = new List<double>();

        public double Exams { get; set; }

        public Outcome Result { get; set; }
    }
}
