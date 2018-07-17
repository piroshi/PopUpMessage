using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessages : MonoBehaviour 
{
	public Transform popUpTrans;
	public AudioSource myAudioS;

	void Start () 
	{
		MessageManager.Instance.IniPopUpPanel(popUpTrans);
		//MessageManager.Instance.OpenPopUp("Warning","Window 1",true);
		//MessageManager.Instance.OpenPopMessage("Hello world");
	}


	public void PopMessage()
	{
		MessageManager.Instance.OpenPopMessage("Simple message window");
	}

	public void ActionPopUp()
	{
		MessageManager.Instance.OpenPopUp("Alert", "Press the button to play a sound", false,"Play Sound", PlaySound,true,null);
	}
	public void CompletePopUp()
	{
		MessageManager.Instance.OpenPopUp("Alert", "Press the button to play a sound", true, "Play Sound", PlaySound, true,PlaySound);
	}
    
	public void PlaySound()
	{
		myAudioS.Play();
	}

}
