//==============================================================
// PopupText
//==============================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupText : MonoBehaviour
{
	public static PopupText Instance;
	private Text popupText;
	private Coroutine PopupTextCoroutine;
	private bool CoroutineRun;

	//==============================================================
	// Awake
	//==============================================================
	void Awake()
	{
		Instance = this;
		popupText = GetComponent<Text>();
	}

	//==============================================================
	// Enter popupText
	//==============================================================
	public void Popup(string textString, float time, float delay) // Text, fade in/out, delay
	{
		if (CoroutineRun)
		{
			StopCoroutine(PopupTextCoroutine);
			popupText.text = "";
		}
		PopupTextCoroutine = StartCoroutine(FadeInOut(textString, time, delay));
	}

	//==============================================================
	// Fade in/out text..
	//==============================================================
	public IEnumerator FadeInOut(string textString, float time, float delay) // Text, fade in/out, delay
	{
		CoroutineRun = true;

		popupText.text = textString;
		popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 0);
		while (popupText.color.a < 1.0f)
		{
			popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, popupText.color.a + (Time.deltaTime / time));
			yield return null;
		}
		yield return new WaitForSeconds(delay);

		popupText.text = textString;
		popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 1);
		while (popupText.color.a > 0.0f)
		{
			popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, popupText.color.a - (Time.deltaTime / time));
			yield return null;
		}

		CoroutineRun = false;
	}
}