using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class DeadScene : MonoBehaviour
{
    public MeleeFighter playerMeleeFighterClass;
    public Volume GlobalVol;
    public CanvasGroup DeadSceneCanvasGroup;
    public CanvasGroup InGameUICanvasGroup;
    public CanvasGroup DeadUICanvasGroup;
    public CanvasGroup DeadImageCanvasGroup;
    public AnimationCurve DeadImageAnimationCurve;
    bool hasDead = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMeleeFighterClass.isDead && !hasDead)
        {
            playerMeleeFighterClass.OnDead += () =>
            {
              
                Debug.Log("PlayerDead");

                OnDead();
                hasDead = true;
            };
           
        }
    }

    void OnDead()
    {
        if (GlobalVol.profile.TryGet<ColorAdjustments>(out var ca))
        {
           StartCoroutine(FadeFilterByColorAdj(ca, 0, -10, 2f));

        }
        else
        {
            Debug.Log("CantGet ColorAdjustments");
        }

        if (DeadUICanvasGroup != null)
        {
            DeadUICanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeFilterByCanvas(DeadUICanvasGroup, 0, 1, 0.5f, 5.0f));
        }

    }


    private IEnumerator FadeFilterByColorAdj(ColorAdjustments adjust,float from, float to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            float newColor = Mathf.Lerp(from, to, t);
            InGameUICanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            DeadImageCanvasGroup.gameObject.SetActive(true);
            DeadImageCanvasGroup.alpha = DeadImageAnimationCurve.Evaluate(t);
            adjust.postExposure.value = newColor;
            yield return null;
        }

        adjust.postExposure.value = to;
    }

    private IEnumerator FadeFilterByCanvas(CanvasGroup adjust, float from, float to, float duration ,float waitForSeconds = 0.0f)
    {
        float elapsedTime = 0f;

        yield return new WaitForSecondsRealtime(waitForSeconds);

        while (elapsedTime < duration)
        {

            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            float newColor = Mathf.Lerp(from, to, t);
            adjust.alpha = newColor;
            yield return null;
        }

        adjust.alpha = to;
    }
}
