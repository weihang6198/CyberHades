using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public ParticleSystem Slash;
    public GameObject SlashVFX;
    
    public void SpawnEffect(Transform SlashSpawnTransform)
    {
        var slash = Instantiate(SlashVFX.GetComponentInChildren<ParticleSystem>(), SlashSpawnTransform.position, SlashSpawnTransform.rotation);
        slash.Play();
        //Debug.DrawLine(SlashSpawnTransform.position, new Vector3(0, 0, 0));
        float SlashLifeTime = slash.main.duration + slash.main.startLifetime.constantMax;
        Destroy(slash.gameObject, slash.main.duration);
    }
  
    public Quaternion GetCalculatedSlashRotation(Animator animator,AnimationClip attackClip,Transform handTransform, AttackData attackData)
    {
        attackClip.SampleAnimation(animator.gameObject, attackData.ImpactStartTime);
        Vector3 startPos = handTransform.position;

        attackClip.SampleAnimation(animator.gameObject, attackData.ImpactEndTime);
        Vector3 endPos = handTransform.position;

        Vector3 dir = endPos - startPos;
        if (dir == Vector3.zero) { return Quaternion.LookRotation(Vector3.zero); }

         Quaternion rotation = Quaternion.LookRotation(dir);
        return rotation;
    }
}
