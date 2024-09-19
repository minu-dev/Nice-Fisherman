using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    private AndroidJavaObject curActivity = null;

	private void Awake()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}
	public void doToast(string content)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			curActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				AndroidJavaObject toast = new AndroidJavaObject("android.widget.Toast", curActivity);
			    toast.CallStatic<AndroidJavaObject>("makeText", curActivity, content, 0).Call("show");
			}));
		}
	}
}
