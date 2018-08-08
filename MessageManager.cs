using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Animations;


/// <summary>
/// Class to control all de Messages popups.
/// </summary>
public class MessageManager : MonoBehaviour
{
	private static MessageManager instance;
	static Text panelText;		   // Main text of the popup
	static Text panelTitle;		  // Title of the popup
	static Button cancelButton;	  // Reference of the cancel Button on the popUp
	static Button actionButton;	  // Reference of the action Button on the popUp
	static Button okButton;		  // Reference of the OK button on the popUp
	static bool popUpIsOpen;		 // is the popUp open?
	static UnityAction extraAction;  //Additional action added with the method AddActionTobutton
	static bool transIni;			// does the transition started?
	static Image[] uiImages;		 // array of images that the the popup window contains
	static Text[] uiText;			// array of texts that the popup window contains
	static float transitionSpeed = .05f;  // speed of the panel for apearing or disapearing
	static bool show;				// boolean to define if the panel will be shown or hidden
	static List<float> alphasImgs;   // The alpha value of the images to maintain them after fadein
	static List<float> alphasText;   // The alpha value of the texts to maintain them after fadein
	static float currentTime;		// time used for the transition
	static string cancelText = "Cancel";   //Variable created if the text of the cancel buttons requiere to be changed to other language of word

	static Transform popUpPanel;	//Main Transform of the popUpPanel
	static Queue<Messages> messagesQueue = new Queue<Messages>();  //List of Messages in Queue


	//Event trigered when All messages in the queue are closed
	public delegate void AllMessagesClosedHandler();
	static event AllMessagesClosedHandler AllMessagesClosed;

	//Event trigered when transition ends
	public delegate void TransitionFinishedHandler();
	public event TransitionFinishedHandler TransitionFinished;

	//Instace to create a singleton
	public static MessageManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject obj = GameObject.Find("MessageManager");
				if (obj == null)
				{
					obj = new GameObject("MessageManager");
					instance = obj.AddComponent<MessageManager>();
					DontDestroyOnLoad(obj);
				}
			}
			return instance;
		}
	}

	void Update()
	{
	  
		if (transIni)
		{
			Fade();
		}
	}

	/// <summary>
	/// Fade in or out the panel at the selected speed
	/// </summary>
	void Fade()
	{
		bool finished = true;

			// Transition for the images
			for (int i = 0; i < uiImages.Length; i++)
			{
				float a;
				a = NewAlpha(uiImages[i].color.a, transitionSpeed, show);
				uiImages[i].color = new Color(uiImages[i].color.r, uiImages[i].color.g, uiImages[i].color.b, Mathf.Clamp(a, 0, alphasImgs[i]));

					if ((uiImages[i].color.a > 0 && !show) || (uiImages[i].color.a <= alphasImgs[i] && show))
					{
						finished = false;
						OnTransitionFinished();
					}
			}
			// Transition for the texts
			for (int i = 0; i < uiText.Length; i++)
			{
				float a;
				a = NewAlpha(uiText[i].color.a, transitionSpeed, show);
				uiText[i].color = new Color(uiText[i].color.r, uiText[i].color.g, uiText[i].color.b, Mathf.Clamp(a, 0, alphasText[i]));

					if (uiText[i].color.a > 0 && !show || (uiText[i].color.a <= alphasText[i] && show))
					{
						finished = false;
					}
			}
		
			if (finished)
			{
				transIni = false;

					if (!show)
						gameObject.SetActive(false);
			}
	}


	/// <summary>
	/// addition or subtraction of a variable. Used to simplify the Fade method
	/// </summary>
	/// <returns>El valor que se modifica</returns>
	/// <param name="iniValue">Initian value.</param>
	/// <param name="change">how mutch changes.</param>
	/// <param name="plus">if is<c>true</c> returns  iniValue + Change </param>
	float NewAlpha(float iniValue, float change, bool plus)
	{
		if (plus)
		{
			return iniValue + change;
		}
		else
		{
			return iniValue - change;
		}
	}
	void IniFade(bool iShow)
	{
		currentTime = Time.time;
		transIni = true;
		show = iShow;
	}

	public void OnTransitionFinished()
	{
		if (TransitionFinished != null)
			TransitionFinished();
	}
	
	/// <summary>
	/// Assign all the variables and objects related to the Popup
	/// </summary>
	public void IniPopUpPanel(Transform popUpTransform)
	{

		InitStateFade(popUpTransform);

		popUpPanel = popUpTransform;
		panelTitle = popUpPanel.Find("Title").GetComponent<Text>();
		panelText = popUpPanel.Find("MainText").GetComponent<Text>();
		cancelButton = popUpPanel.Find("PanelButtons").Find("CancelButton").GetComponent<Button>();
		actionButton = popUpPanel.Find("PanelButtons").Find("ActionButton").GetComponent<Button>();
		okButton = popUpPanel.Find("PanelButtons").Find("OkButton").GetComponent<Button>();
		okButton.onClick.AddListener(ClosePopup);
		cancelButton.onClick.AddListener(ClosePopup);
	}
	/// <summary>
	/// Inits the state fade.
	/// </summary>
	/// <param name="popUpTransform">Pop up transform.</param>
	void InitStateFade(Transform popUpTransform)
	{
		float a;

		alphasImgs = new List<float>();
		alphasText = new List<float>();
		uiImages = popUpTransform.GetComponentsInChildren<Image>(true);
		uiText = popUpTransform.GetComponentsInChildren<Text>(true);


			for (int i = 0; i < uiImages.Length; i++)
			{
				alphasImgs.Add(uiImages[i].color.a);
			}

			for (int i = 0; i < uiText.Length; i++)
			{
				alphasText.Add(uiText[i].color.a);
			}


			for (int i = 0; i < uiImages.Length; i++)
			{
				a = 0;
				uiImages[i].color = new Color(uiImages[i].color.r, uiImages[i].color.g, uiImages[i].color.b, a);

			}
			for (int i = 0; i < uiText.Length; i++)
			{
				 a = 0;
				uiText[i].color = new Color(uiText[i].color.r, uiText[i].color.g, uiText[i].color.b, a);
			}

		gameObject.SetActive(false);
	}


	/// <summary>
	/// Opens the popUp only to show a message.
	/// </summary>
	/// <param name="message">Message to show</param>
	public void OpenPopMessage(string message)
	{
		Debug.Log("popUpIsOpen: "+ popUpIsOpen);

		if (popUpIsOpen)
		{
			Messages newMessage = new Messages();
			newMessage.messageText = message;
			newMessage.showOkButton = true;
			newMessage.action = null;
			messagesQueue.Enqueue(newMessage);
		}
		else
		{
			popUpIsOpen = true;

			popUpPanel.gameObject.SetActive(true);
			IniFade(true);

			panelTitle.text = "";
			okButton.transform.gameObject.SetActive(true);
			actionButton.transform.gameObject.SetActive(false);
			cancelButton.transform.gameObject.SetActive(false);					
			panelText.alignment = TextAnchor.MiddleCenter;
			panelText.text = message;
		}	  
	}

	/// <summary>
	/// Closes the popup with a transition
	/// </summary>
	public void ClosePopup()
	{

		if (messagesQueue.Count > 0)
		{
			OpenMessageFromStack();
		}
		else
		{
			IniFade(false);
			TransitionFinished += OpenMessageFromStack;
			popUpIsOpen = false;
			OnAllMessagesClosed();
		}
		
	}

	/// <summary>
	/// Opens pop up. if it is already not opened
	/// </summary>
	/// <param name="titleText">PopUp Title .</param>
	/// <param name="messageText">PopUp Message .</param>
	/// <param name="actionButonText">Action buton text.</param>
	/// <param name="action">UnityAction asigned to action button.</param>
	/// <param name="showOKButton">If set to <c>true</c> show OK Buton.</param>
	/// <param name="showCancelButton">If set to <c>true</c> show cancel buton.</param>
	/// <param name="cancel">UnityAction asigned to cancel button.</param>
	public void OpenPopUp(string titleText, string messageText,bool showOKButton = false, string actionButonText = "OK", UnityAction action = null, bool showCancelButton = false, UnityAction cancel = null)
	{

		Debug.Log("popUpIsOpen: "+ popUpIsOpen);
		if (popUpIsOpen)
		{
			Messages newMessage = new Messages();

			newMessage.titleText = titleText;
			newMessage.messageText = messageText;
			newMessage.actionButonText = actionButonText;
			newMessage.action = action;
			newMessage.showOkButton = showOKButton;
			newMessage.showCancelButton = showCancelButton;
			newMessage.cancel = cancel;
			messagesQueue.Enqueue(newMessage);
		}
		else
		{
			OpenActionPopUp(titleText, messageText,showOKButton, actionButonText, action, showCancelButton, cancel);
		}
	}


	

	/// <summary>
	/// Opens an action pop up. passing all parameters
	/// </summary>
	/// <param name="titleText">PopUp Title .</param>
	/// <param name="messageText">PopUp Message .</param>
	/// <param name="actionButonText">Action buton text.</param>
	/// <param name="action">UnityAction asigned to action button.</param>
	/// <param name="showOKButton">If set to <c>true</c> show OK Buton.</param>
	/// <param name="showCancelButton">If set to <c>true</c> show cancel buton.</param>
	/// <param name="cancel">UnityAction asigned to cancel button.</param>
	void OpenActionPopUp(string titleText, string messageText,bool showOKButton, string actionButonText, UnityAction action , bool showCancelButton, UnityAction cancel = null)
	{
	
		InitMessage(titleText, messageText, showOKButton, showCancelButton, (action != null));

		//Other

			if (action != null)
			{
				actionButton.onClick.AddListener(action);
				actionButton.gameObject.SetActive(true);
				actionButton.transform.Find("Text").GetComponent<Text>().text = actionButonText;
			}

		cancelButton.onClick.RemoveAllListeners();
		cancelButton.onClick.AddListener(ClosePopup);

			if (cancel != null)
				cancelButton.onClick.AddListener(cancel);
		
	}
   
	/// <summary>
	/// Inits the popup message.
	/// </summary>
	/// <param name="titleText">Title text.</param>
	/// <param name="messageText">Message text.</param>
	/// <param name="showOkButton">If set to <c>true</c> show ok button.</param>
	/// <param name="showCancelButton">If set to <c>true</c> show cancel button.</param>
	/// <param name="showActionButton">If set to <c>true</c> show action button.</param>
	void InitMessage(string titleText, string messageText, bool showOkButton, bool showCancelButton, bool showActionButton)
	{

		popUpIsOpen = true;

		//Texts
		panelTitle.text = titleText;
		panelText.text = messageText;
	   
		actionButton.transform.Find("Text").GetComponent<Text>().text = "OK";
		cancelButton.transform.Find("Text").GetComponent<Text>().text = cancelText;


		//Buttons
		okButton.gameObject.SetActive(showOkButton);
		cancelButton.gameObject.SetActive(showCancelButton);
		actionButton.gameObject.SetActive(showActionButton);

		popUpPanel.gameObject.SetActive(true);
		IniFade(true);

		actionButton.onClick.RemoveAllListeners();

			if (extraAction != null)
			{
				actionButton.onClick.AddListener(extraAction);
				extraAction = null;
			}
	}

	/// <summary>
	/// Adds the action to Actionbutton.
	/// </summary>
	/// <param name="action">Action.</param>
	public void AddActionToButton(UnityAction action)
	{
		actionButton.onClick.AddListener(action);
	}

	/// <summary>
	/// Adds the action to button to next popUp message.
	/// </summary>
	/// <param name="action">Action.</param>
	public void AddActionToButtonToNextMessage(UnityAction action)
	{
		extraAction = action;
	}

	/// <summary>
	/// Removes the extra action.
	/// </summary>
	public void RemoveExtraAction()
	{
		extraAction = null;
	}

	/// <summary>
	/// Hides all the buttons of the popUp
	/// </summary>
	public void HideButtons()
	{
		actionButton.gameObject.SetActive(false);
		okButton.gameObject.SetActive(false);
		cancelButton.gameObject.SetActive(false);
	}

	/// <summary>
	/// Opens the message from  message stack.
	/// </summary>
	void OpenMessageFromStack()
	{

		TransitionFinished -= OpenMessageFromStack;

		Messages nextMessage = messagesQueue.Dequeue();
		OpenActionPopUp(nextMessage.titleText, nextMessage.messageText,nextMessage.showOkButton, nextMessage.actionButonText,nextMessage.action,nextMessage.showCancelButton, nextMessage.cancel);
		

	}

	/// <summary>
	/// Adds the suscriber to event AllMessagesClosed.
	/// </summary>
	/// <param name="action">Action.</param>
	public void AddSuscriberToEvent(AllMessagesClosedHandler action)
	{
		AllMessagesClosed += action;
	}
	/// <summary>
	/// Removes the suscriber to event AllMessagesClosed.
	/// </summary>
	/// <param name="action">Action.</param>
	public void RemoveSuscriberToEvent(AllMessagesClosedHandler action)
	{
		AllMessagesClosed -= action;
	}
	/// <summary>
	/// calls AllMessagesClosed suscribed methods
	/// </summary>
	void OnAllMessagesClosed()
	{
		if (AllMessagesClosed != null)
		{
			//Debug.Log("NOt null");
			AllMessagesClosed();
		}
	}

}


/// <summary>
/// Class that storage popup messages' info added to a stack
/// </summary>
class Messages
{
	public string titleText;
	public string messageText;
	public string actionButonText;
	public UnityAction action;
	public UnityAction cancel;
	public Sprite extraImage;
	public bool showOkButton;
	public bool showCancelButton;
	public int param;
	public int quantity;
}


