using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class populate_grid_script : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Populate(string test)
    {
        GameObject newobj;
        for (int i = 0; i<1; i++)
        {
            newobj = (GameObject)Instantiate(prefab, transform);
            newobj.GetComponent<Text>().text = test;
        }
    }
}
