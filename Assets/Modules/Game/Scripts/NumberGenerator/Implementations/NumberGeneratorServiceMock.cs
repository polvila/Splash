using Zenject;

namespace Modules.Game
{
	public class NumberGeneratorServiceMock : INumberGeneratorService
	{
		public readonly static int MaxRange = 13;
		public readonly static int MinRange = 1;

		private RandomGenerator _generator;
		private DiContainer _container;

		private CardGeneratorMode _generatorMode;

		public CardGeneratorMode GeneratorMode
		{
			get { return _generatorMode; }
			set
			{
				_generatorMode = value;
				switch (_generatorMode)
				{
					case CardGeneratorMode.Random:
						_generator = _container.Resolve<RandomGenerator>();
						break;
					case CardGeneratorMode.RandomExcluding:
						_generator = _container.Resolve<RandomExcludingGenerator>();
						break;
					case CardGeneratorMode.FTUE:
						_generator = _container.Resolve<FTUEGenerator>();
						break;
				}
				_generator.Reset();
			}
		}

		protected NumberGeneratorServiceMock(DiContainer container)
		{
			_container = container;
			GeneratorMode = _generatorMode;
		}

		public int GetNumber()
		{
			return _generator.GenerateNumber(MinRange, MaxRange);
		}
	}
}