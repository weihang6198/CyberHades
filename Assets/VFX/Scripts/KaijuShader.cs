using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuShader : MonoBehaviour
{
    List<Material> materials = new List<Material>();
    [SerializeField] Color skinColor;
    [SerializeField] float speed = 4f;
    [SerializeField] float maxValue = 10f;
    [SerializeField] float minValue = 1f;

    void Awake()
    {
        RegisterMaterialsFromRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var m in materials)
        {
            float pulse = (Mathf.Cos(speed * Time.time) + 1f) * 0.5f * (maxValue - minValue) + minValue;

            m.SetFloat("_EmissiveOffset", pulse);
            m.SetColor("_AlbedoColor", skinColor);
            m.SetColor("_EmissiveColor", skinColor);
        }
    }

    void RegisterMaterialsFromRenderer()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in rends)
        {
            foreach (var m in r.materials)
            {
                materials.Add(m);
            }
        }
    }
}
