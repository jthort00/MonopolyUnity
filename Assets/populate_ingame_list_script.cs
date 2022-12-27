using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class populate_ingame_list_script : MonoBehaviour
{
    public GameObject prefab;
    public void Populate(string content, int clickable)
    {
        GameObject newobj;
        for (int i = 0; i < 1; i++)
        {
            newobj = (GameObject)Instantiate(prefab, transform);
            newobj.GetComponent<Text>().text = content;
            newobj.GetComponent<text_button_script>().SetClickable(clickable);
        }
    }

    public void Delete()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }
}

