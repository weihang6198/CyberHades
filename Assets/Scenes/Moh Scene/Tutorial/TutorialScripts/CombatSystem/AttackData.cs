using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat System/create a new attack")]
public class AttackData : ScriptableObject
{
    [field:SerializeField] public string AnimName {  get;private set; }
    [field:SerializeField] public AttackHitbox HitBoxToUse {  get;private set; }
    [field:SerializeField] public float ImpactStartTime {  get;private set; }
    [field:SerializeField] public float ImpactEndTime {  get;private set; }

    
    
}

public enum AttackHitbox { LeftHand,RightHand ,LeftFoot, RightFoot,Sword};