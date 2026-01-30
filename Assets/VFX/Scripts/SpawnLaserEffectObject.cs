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
        if (HitSpawnTransform == null)
        {
            GameObject go = new GameObject("HitSpawnPoint");
            HitSpawnTransform = go.transform;
            HitSpawnTransform.SetParent(transform); // optional but recommended
        }
      
        planeScale = 100f;//determine the rnage of the laser
                      
        Vector3 origin = transform.position;
        origin.y += 1.2f;
        // Vector3 forward = new Vector3(SpawnTransform.forward.x, 0f, SpawnTransform.forward.z).normalized;
        Vector3 forward = transform.forward;

        //if (Physics.Raycast(origin, forward, out RaycastHit hitInfo, 100f, layerMaskRayCastTest))
        if (Physics.Raycast(origin, forward, out RaycastHit hit, 100f, layerMask))
        {
            Debug.Log("<color=ray cast complete player detected</color>");
            Debug.Log("hit.collider.gameObject.layer:"+hit.collider.gameObject.layer);
            Debug.Log("hit.collider.gameObject.layer:"+ LayerMask.LayerToName(hit.collider.gameObject.layer));

            HitSpawnTransform = new GameObject("HitSpawnPoint").transform;
           
            HitSpawnTransform.position = hit.point;
            HitSpawnTransform.rotation = Quaternion.LookRotation(SpawnTransform.forward, SpawnTransform.up);
            Debug.DrawRay(origin, forward * 100f, Color.red, 3f);
        }
        else
        {
            Debug.Log("=====ray cast failed no player detected=======");
         
            HitSpawnTransform.position = origin + forward * planeScale;
            Debug.DrawRay(origin, forward * 100f, Color.blue, 3f);


        }

        //int playerLayer = LayerMask.NameToLayer("Player");
        //layerMask = 1 << playerLayer;

        //if (Physics.Raycast(origin, forward, out hit, distance, layerMask, QueryTriggerInteraction.Ignore))
        //{
        //    Debug.Log("<color=red>Player hit</color>");

        //    //HitSpawnTransform.position = hit.point;
        //}
        //else
        //{
        //    Debug.Log("<color=red> does not hit player </color>");
        //    Debug.Log("hit.collider.gameObject.layer:" + hit.collider.gameObject.layer);
        //    Debug.Log("hit.collider.gameObject.layer:" + LayerMask.LayerToName(hit.collider.gameObject.layer));
        //}

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
