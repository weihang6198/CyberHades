using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrailEffect : MonoBehaviour
{

    bool isTrailActive = false;
    public float activeTime = 1.0f;

    [Header("MeshRelated")]
    public float meshRefreshRate = 0.1f;
    public float lifetime;
    public Transform positionToSpawn;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    [Header("ShaderRelated")]
    public Material material;
    public string ShaderVarRef;
    public float ShaderVarRate = 0.1f;
    public float ShaderVarRefreshRate = 0.05f;

    void Update()
    {

    }

    public void Execute()
    {
        isTrailActive = true;
        if (isTrailActive)
        {
            //Debug.Log("MeshTrail has Executed");
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float time)
    {

        while (time >0) {

            time -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                GameObject go = new GameObject("TrailMesh");
                go.transform.position = skinnedMeshRenderers[i].transform.position;
                go.transform.rotation = skinnedMeshRenderers[i].transform.rotation;

                MeshFilter mf = go.AddComponent<MeshFilter>();
                mf.mesh = mesh;

                MeshRenderer mr = go.AddComponent<MeshRenderer>();

                int subMeshCount = mesh.subMeshCount;
                Material[] mats = new Material[subMeshCount];
                for (int j = 0; j < subMeshCount; j++)
                    mats[j] = material;

                mr.materials = mats;
                for (int k = 0; k < mr.materials.Length; k++)
                {
                    StartCoroutine(AnimateMaterial(mr.materials[k], 0, ShaderVarRate, ShaderVarRefreshRate));
                }

                Destroy(go, lifetime);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

 

        isTrailActive = false;
    }

    IEnumerator AnimateMaterial(Material mat,float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(ShaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(ShaderVarRef, valueToAnimate);

            yield return new WaitForSeconds(refreshRate);
        }

    }

}
