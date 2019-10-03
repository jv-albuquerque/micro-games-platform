using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnimation : MonoBehaviour
{
    [SerializeField] private float minScale = .5f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float speed = 0.1f;

    private bool growing;
    
    // Update is called once per frame
    void Update()
    {
        // it will make the star growing for a little time than shrink for the same amount of time

        //part that make the star grow
        if(growing)
        {
            transform.localScale = new Vector3(transform.localScale.x + speed * Time.deltaTime, transform.localScale.y + speed * Time.deltaTime, transform.localScale.z);
            if (transform.localScale.x >= maxScale)
                growing = false;
        }
        //part that make the star shrink
        else
        {
            transform.localScale = new Vector3(transform.localScale.x - speed * Time.deltaTime, transform.localScale.y - speed * Time.deltaTime, transform.localScale.z);
            if (transform.localScale.x <= minScale)
                growing = true;
        }
    }
}
