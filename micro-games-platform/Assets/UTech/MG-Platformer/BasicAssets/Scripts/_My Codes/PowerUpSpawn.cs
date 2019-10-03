using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] powerUps;

    private void Awake()
    {
        Instantiate(powerUps[Random.Range(0,powerUps.Length)], transform);
    }

}
