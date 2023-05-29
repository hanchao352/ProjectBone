using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshComb : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> list = new List<GameObject>();
 
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[list.Count];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine,true,true);
        transform.gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            bool vis = list[0].activeSelf;
            list[0].SetActive(!vis);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            bool vis = list[1].activeSelf;
            list[1].SetActive(!vis);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            bool vis = list[2].activeSelf;
            list[2].SetActive(!vis);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            bool vis = transform.gameObject.activeSelf;
            transform.gameObject.SetActive(!vis);
        }
    }
}
