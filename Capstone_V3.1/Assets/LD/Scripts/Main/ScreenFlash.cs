using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour {

    public Image flashImage;
    public float alphaStartValue = 1;       //Starting alpha value, 0 - 1

    bool isFlashing;    //Use to lock out multiple calls on coRoutine, only one flash at a time

    private void Awake()
    {
        //If our flash image is not specified, search for it just in case
        if (flashImage == null)
        {
            flashImage = GetComponent<Image>();
        }
    }

    public void FlashColor(Color colorToFlash, float duration)
    {
        //If we're note already flashing, start the new flash, otherwise continue
        if (!isFlashing)
        {
            //Change the flash panel's color
            flashImage.color = colorToFlash;
            //Start alpha channel animation, Stop previous coroutine before starting anew
            StopCoroutine(FlashImage(duration));
            StartCoroutine(FlashImage(duration));
        }
    }

    IEnumerator FlashImage(float fadeTime)
    {
        //starting coroutine, lock out future flashes until completed
        isFlashing = true;
        //Start the alpha at the alphaStartPoint
        float alpha = alphaStartValue;
        //Change the value over time
        for(float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            //Adjust current to get closer and closer to 0
            Color newColor = flashImage.color;
            newColor.a = Mathf.Lerp(alpha, 0, t);
            flashImage.color = newColor;
            yield return null;
        }
        //Done with coroutine, ready for another flash
        isFlashing = false;
    }
}
