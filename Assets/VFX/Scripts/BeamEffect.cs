using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeamEffect : MonoBehaviour
{
    [SerializeField] public GameObject BeamChargeVFX;
    [SerializeField] public GameObject BeamLaserVFX;
    Transform SpawnTransform;
    Transform ChestTransform;
    bool isSpawned = false;

    public bool isEnd = true;
    public float duration = 3.0f;
    public float planeScale = 3.0f;

    private void CatchSpawnPointTransform()
    {
        ChestTransform = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest);     


        float yRotation = ChestTransform.rotation.eulerAngles.y;
        ChestTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        SpawnTransform = transform;

        CalcRayLength();
    }    
    void CalcRayLength()
    {
        Vector3 origin = SpawnTransform.position;

        Vector3 forward = new Vector3(SpawnTransform.forward.x, 0f, SpawnTransform.forward.z).normalized;
        Ray ray = new Ray(origin, forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 300.0f))
        {

            planeScale = Vector3.Distance(ray.origin, hit.point) / 10f;
        }

        SpawnTransform.rotation = Quaternion.Euler(0f, SpawnTransform.rotation.eulerAngles.y, 0f);

    }

    // Update is called once per frame
    void Update()
    {

        if (!isSpawned && Input.GetKeyDown(KeyCode.R))
        {
            isEnd = false;
            StartCoroutine(StartBeam());
            isSpawned = true;
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            isSpawned = false;
        }
    }


    IEnumerator StartBeam()
    {
        CatchSpawnPointTransform();

        GameObject objCharge = Instantiate(BeamChargeVFX, SpawnTransform.position, SpawnTransform.rotation);

        ParticleSystem ps = objCharge.GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        float lifetime = main.startLifetime.constant; 

        yield return new WaitForSeconds(55f/60f);

        GameObject objLaser =Instantiate(BeamLaserVFX, SpawnTransform.position, SpawnTransform.rotation);

        MeshFilter[] meshes = objLaser.GetComponentsInChildren<MeshFilter>();
            
    

        Renderer[] renderers = objLaser.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.sharedMaterial;

            if (mat.HasProperty("_LaserDissolve"))
            {
                renderer.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, planeScale);
                renderer.gameObject.transform.localPosition = new Vector3(0,0, planeScale*5f);

                renderer.material = mat; 
                StartCoroutine(Fade(mat, 1.103f, 1.0f, 0.2f, "_LaserDissolve"));
            }
            else
            {
                Debug.Log("Has not laserDis");
            }
        }



        yield return new WaitForSeconds(duration);

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;

            if (mat.HasProperty("_LaserDissolve"))
            {
                renderer.material = mat;
                yield return Fade(mat, 1.0f ,1.103f, 0.4f, "_LaserDissolve");
            }
            else
            {
                Debug.Log("Has not laserDis");
            }
        }

        Destroy(objCharge);
        Destroy(objLaser);
        isEnd = true;
    }

    private IEnumerator Fade(Material adjust, float from, float to, float duration,string name, float waitForSeconds = 0.0f)
    {
        float elapsedTime = 0f;

        yield return new WaitForSecondsRealtime(waitForSeconds);

        while (elapsedTime < duration)
        {

            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            float progress = Mathf.Lerp(from, to, t);
            adjust.SetFloat(name, progress);
            yield return null;
        }

        adjust.SetFloat(name, to);
    }
}
