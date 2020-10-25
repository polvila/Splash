using TMPro;
using UnityEngine;
using Zenject;

public class LeftBarView : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _enemyCounterText;
	[SerializeField] private TextMeshProUGUI _timerText;
	[SerializeField] private TextMeshProUGUI _humanCounterText;
	public Timer Timer;
	
	private Presenter<LeftBarView> _presenter;

	private int _enemyCounter;
	public int EnemyCounter
	{
		get { return _enemyCounter; }
		set
		{
			_enemyCounter = value;
			SetEnemyCounter(_enemyCounter);
		}
	}
	
	private int _humanCounter;
	public int HumanCounter
	{
		get { return _humanCounter; }
		set
		{
			_humanCounter = value;
			SetHumanCounter(_humanCounter);
		}
	}

	[Inject]
	void Init(Presenter<LeftBarView> presenter)
	{
		_presenter = presenter;
		_presenter.RegisterView(this);
		Timer.SecondsUpdated += OnSecondsUpdated;
	}
	
	private void SetEnemyCounter(int value)
	{
		_enemyCounterText.text = value.ToString();
	}
	
	private void SetHumanCounter(int value)
	{
		_humanCounterText.text = value.ToString();
	}

	public void OnSecondsUpdated(string timeInMinutesAndSeconds)
	{
		_timerText.text = timeInMinutesAndSeconds;
	}
	
	private void OnDestroy()
	{
		Timer.SecondsUpdated -= OnSecondsUpdated;
		_presenter?.Dispose();
	}
}
