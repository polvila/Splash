using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private const int SecondsInMinute = 60;
    private float _targetTime;
    private int _seconds;
    private bool _onGoing;

    public Action TimerEnded;
    public Action<string> SecondsUpdated;

    public void Init(int seconds)
    {
        _targetTime = _seconds = seconds;
        SecondsUpdated?.Invoke(_seconds.ToString());
    }

    public void StartTimer()
    {
        _onGoing = true;
    }

    public void StopTimer()
    {
        _onGoing = false;
    }

    void Update()
    {
        if (!_onGoing) return;
        
        _targetTime -= Time.deltaTime;

        if (_targetTime <= _seconds)
        {
            _seconds = (int) _targetTime;
            Debug.Log("Timer: seconds:" + (_seconds + 1) + " Target time: " + _targetTime);
            SecondsUpdated?.Invoke(GetTimeInMinutesAndSeconds());
        }
        
        if (_targetTime <= 0.0f)
        {
            SecondsUpdated?.Invoke("0:00");
            _onGoing = false;
            TimerEnded?.Invoke();
        }
    }

    private string GetTimeInMinutesAndSeconds()
    {
        var minutes = (_seconds + 1) / SecondsInMinute;
        var seconds = _seconds + 1 - minutes * SecondsInMinute;

        return $"{minutes}:{seconds:00}";
    }
}
