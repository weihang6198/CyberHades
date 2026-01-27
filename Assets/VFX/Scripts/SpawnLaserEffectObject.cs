using System.Collections;
using UnityEngine;

public class SpawnLaserEffectObject : MonoBehaviour
{
    [SerializeField] public GameObject BeamChargeVFX;
    [SerializeField] public GameObject BeamLaserVFX;
    [SerializeField] public GameObject BeamHitVFX;
    [SerializeField] public Transform SpawnTransform;
    [SerializeField] public LayerMask layerMask;
    Transform HitSpawnTransform;
    bool isSpawned = false;
    
    public bool isEnd = true;
    [SerializeField]  public float duration = 3.0f;
    [SerializeField]  public float distance = 300.0f;
    float planeScale = 3.0f;

    public FighterBase owner;
    private void CatchSpawnPointTransform()
    {

        CalcRayLength();
    }    
    void CalcRayLength()
    {
        Vector3 origin = SpawnTransform.position;

        Vector3 forward = new Vector3(SpawnTransform.forward.x, 0f, SpawnTransform.forward.z).normalized;
        Ray ray = new Ray(origin, forward);

        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("Player");
        Debug.DrawRay(origin, forward * distance, Color.red, 3f);
        planeScale = 100f;//determine the rnage of the laser
        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
           

            Debug.Log("ray cast complete player detected");
          

            HitSpawnTransform = new GameObject("HitSpawnPoint").transform;
           
            HitSpawnTransform.position = hit.point;
            HitSpawnTransform.rotation = Quaternion.LookRotation(SpawnTransform.forward, SpawnTransform.up);
        }
        else
        {
            HitSpawnTransform.position = origin + forward * planeScale;
          
            Debug.Log("ray cast failed no player detected");
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


    public IEnumerator StartBeam()
    {
        CatchSpawnPointTransform();

        GameObject objCharge =
            Instantiate(BeamChargeVFX, SpawnTransform.position, SpawnTransform.rotation);

        ParticleSystem ps = objCharge.GetComponentInChildren<ParticleSystem>();
        var main = ps.main;

        yield return new WaitForSeconds(55f / 60f);

        if (BeamLaserVFX == null) Debug.LogError("BeamLaserVFX is NULL!");
        if (BeamHitVFX == null) Debug.LogError("BeamHitVFX is NULL!");
        if (SpawnTransform == null) Debug.LogError("SpawnTransform is NULL!");
        if (HitSpawnTransform == null) Debug.LogError("HitSpawnTransform is NULL!");

        // ===============================
        // SPAWN LASER VFX
        // ===============================
        GameObject objLaser =
            Instantiate(BeamLaserVFX, SpawnTransform.position, SpawnTransform.rotation);

        // ===============================
        // SPAWN HIT POINT + DAMAGE TRIGGER
        // ===============================
        GameObject objHit =
            Instantiate(BeamHitVFX, HitSpawnTransform.position, HitSpawnTransform.rotation);

        // 🔥 IMPORTANT: parent to this laser object
        objHit.transform.SetParent(this.transform, true);

        // Required for OnTriggerEnter
        objHit.tag = "HitBox";

        SphereCollider hitCol = objHit.AddComponent<SphereCollider>();
        hitCol.isTrigger = true;
        hitCol.radius = 0.5f;

        Rigidbody hitRb = objHit.AddComponent<Rigidbody>();
        hitRb.isKinematic = true;
        hitRb.useGravity = false;

        // ===============================
        // LASER VISUAL SETUP
        // ===============================
        Renderer[] renderers = objLaser.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.sharedMaterial;

            if (mat.HasProperty("_LaserDissolve"))
            {
                renderer.transform.localScale =
                    new Vector3(0.1f, 0.1f, planeScale);

                renderer.transform.localPosition =
                    new Vector3(0, 0, planeScale * 5f);

                renderer.material = mat;
                StartCoroutine(Fade(mat, 1.103f, 1.0f, 0.2f, "_LaserDissolve"));
            }
        }

        // ===============================
        // BEAM ACTIVE DURATION
        // ===============================
        yield return new WaitForSeconds(duration);

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;

            if (mat.HasProperty("_LaserDissolve"))
            {
                yield return Fade(mat, 1.0f, 1.103f, 0.4f, "_LaserDissolve");
            }
        }

        Destroy(objCharge);
        Destroy(objLaser);
        Destroy(objHit);

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
