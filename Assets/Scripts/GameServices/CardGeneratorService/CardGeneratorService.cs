using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CardGeneratorService : ICardGeneratorService 
{
	protected const int MaxRange = 12;
	protected const int MinRange = 1;

	public int GetMaxRange => MaxRange;
	public int GetMinRange => MinRange;

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
			}
		}
	}

	protected CardGeneratorService(DiContainer container, CardGeneratorMode generatorMode)
	{
		_container = container;
		GeneratorMode = generatorMode;
	}
	
	public CardView GenerateCard()
	{
		return _generator.GenerateCard(MinRange, MaxRange);
	}
}
