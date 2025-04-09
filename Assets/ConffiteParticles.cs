using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConffiteParticles : MonoBehaviour
{
    public static ConffiteParticles Instance;

    private void Awake()
    {
        Instance = this;
    }
    public GameObject[] Conffiteparticle;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i<Conffiteparticle.Length;i++)
        {
            Conffiteparticle[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
