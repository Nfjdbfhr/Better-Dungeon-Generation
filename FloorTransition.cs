using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorTransition : MonoBehaviour
{

    public CanvasGroup uiImage;
    public GameObject image;
    public float fadeDuration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        image.SetActive(true);
        uiImage = image.GetComponent<CanvasGroup>();
        uiImage.alpha = 0f;

        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeIn()
    {
        StartCoroutine(FadeImage(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeImage(1f, 0f));
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            uiImage.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        uiImage.alpha = endAlpha;
    }
}
