using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShineAlpha : MonoBehaviour
{

    TextMeshProUGUI PushToStartText;
    // Start is called before the first frame update
    void Start()
    {
        FindText();
    }

    // Update is called once per frame
    void Update()
    {
        if (PushToStartText != null)
        {
            float sinValue = (Mathf.Sin(Time.time* 2.0f));

            float alpha = (sinValue + 1.0f) * 0.5f + 0.2f;

            Color c = PushToStartText.color;
            c.a = alpha;
            PushToStartText.color = c;
        }
    }

    private void FindText()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            if (text == null) continue;

            if (text.name == "PushEnterToStart")
            {
                PushToStartText = text;
                break;
            }
        }
    }
}
