using System;
using Unity.Services.Core;
using UnityEngine;

public class InitializeService : MonoBehaviour
{
	async void Awake()
	{
		try
		{
			await UnityServices.InitializeAsync();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}
}
