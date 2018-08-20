namespace Kritikos.MachineLearning.ForestTests
{
	using Kritikos.MachineLearning.Forest.Helpers;
	using Xunit;

	public class Dummy
	{
		[Fact]
		public void True()
		{
			var i = Yggdrasil.Rnd.Next(0, 100);
			Assert.InRange(i, 0, 100);
		}
	}
}
