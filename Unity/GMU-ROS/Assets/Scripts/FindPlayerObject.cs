using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayerObject : MonoBehaviour
{

    public bool notFound = true;
    public GameObject Head;
    public GameObject L_Hand;
    public GameObject R_Hand;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    
    IEnumerator FindPlayer()
    {
        while (notFound)
        {
            Head = GameObject.FindGameObjectWithTag("Head");
            L_Hand = GameObject.FindGameObjectWithTag("L_Hand");
            R_Hand = GameObject.FindGameObjectWithTag("R_Hand");

            
            if (Head == null || L_Hand == null || R_Hand == null)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                notFound = false;  
            }

        }
    }
}
