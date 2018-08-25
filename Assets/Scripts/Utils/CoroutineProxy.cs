using System.Collections;
using UnityEngine;

public class CoroutineProxy
{
	private MonoBehaviour _proxy;

	public CoroutineProxy(MonoBehaviour proxy)
	{
		_proxy = proxy;
	}

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		return _proxy.StartCoroutine(routine);
	}

	public void StopCoroutine(IEnumerator routine)
	{
		_proxy.StopCoroutine(routine);
	}
	
	public void StopCoroutine(Coroutine coroutine)
	{
		_proxy.StopCoroutine(coroutine);
	}
}
