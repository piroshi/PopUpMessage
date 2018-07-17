Class to control popup messages in unity it comes with an example scene and and additional code to tested.

Usage

- Create a UI popUp panel with tittle, message and 3 buttons , Cancel, Ok, Action (Action button is used tu assign and special method to execute when pressed
- The path and names of the UI elements are hardcoded so if you want to create a different path change the values in IniPopUpPanel method in line 150 of MessageManager.cs
- Attach MessageManager.cs to the popUp panel
- Initialize the MessageManager in another class using:
  		MessageManager.Instance.IniPopUpPanel(popUpTrans);
 	where popUpTrans is the popUpPanel where MessageManager.cs is attached
- open the message using:
	MessageManager.Instance.OpenPopMessage   OR
	MessageManager.Instance.OpenPopUp