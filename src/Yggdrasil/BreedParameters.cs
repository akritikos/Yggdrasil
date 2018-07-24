namespace Yggdrasil
{
    public class BreedParameters
    {
        public int StartingPopulation { get; set; }

        public int GenerationCap { get; set; }

        public int LargeTreePenalty { get; set; }

        public int FoldToParts { get; set; } = 1;

        public double CrossoverChance { get; set; }

        public double MutationChance { get; set; }

        public double ElitismPercent { get; set; }
    }
}
