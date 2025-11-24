using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public FighterBase owner;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(FighterBase owner)
    {
        this.owner = owner;
    }
    // Update is called once per frame
    void Update()
    {
        if(speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Speed has 0");
        }
    }

 

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
