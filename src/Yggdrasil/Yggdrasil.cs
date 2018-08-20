namespace Kritikos.MachineLearning.Forest
{
	using Kritikos.MachineLearning.Forest.Helpers;

	public sealed class Yggdrasil<T> : Yggdrasil
	{
		/// <summary>
		/// Prevents a default instance of the <see cref="Yggdrasil{T}"/> class from being created.
		/// </summary>
		private Yggdrasil()
		{

		}

		public T[] Dataset { get; }
	}
}
