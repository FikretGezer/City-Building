using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BuildingManager : MonoBehaviour
{
    [HideInInspector] public static bool isPlaceable;

    [SerializeField] float gridSize;
    [SerializeField] LayerMask ground;
    [SerializeField] GameObject[] spawnObject;
    [SerializeField] GameObject prefab;
    [SerializeField] Material mat_Placeable, mat_NotPlaceable, mat_PlacementOn;
    [SerializeField] GameObject buildingsMenu;
    [SerializeField] float planeSizeValue;
    [SerializeField] float restrictAmount;
    public static float planeSize { get; private set; }//if this is 200 plansize is 200*200
    public static float restrictValue { get; private set; }//Alan? her k??eden ne kadar s?n?rlayaca??n? belirtir.

    GameObject prefabCopy;
    Vector3 prefabCopyPos;
    Material matForPrefab;
    
    Dictionary<GameObject, Material> prefabsMatsOnTheScene = new Dictionary<GameObject, Material>();
    List<GameObject> objectPool = new List<GameObject>();

    bool isThereAnObject;
    bool objPlacementActive;
    float rotateAmount;
    private void Awake()
    {
        planeSize = planeSizeValue;
        restrictValue = restrictAmount;
    }
    private void Update()
    {        
        if(prefabCopy!=null)
        {
            MoveObject();
            RotatePrefab();
            ReleaseObject();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                objectPool.Add(prefabCopy);
                prefabCopy.SetActive(false);
                prefabCopy = null;
                objPlacementActive = false;
            }//Cancel placement for selected object
        }
        PlacementMaterialChange();
    }

    void MoveObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, ground))
        {
            prefabCopyPos = new Vector3(
                RoundToNearPosition(hit.point.x),
                hit.point.y,
                RoundToNearPosition(hit.point.z)
                );
            prefabCopyPos = new Vector3(Mathf.Clamp(prefabCopyPos.x, restrictAmount, planeSize-restrictAmount), prefabCopyPos.y, Mathf.Clamp(prefabCopyPos.z, restrictAmount, planeSize-restrictAmount));
            prefabCopy.transform.position = prefabCopyPos;
            prefabCopy.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            ChangeObjectMaterial();
        }//Move object around
    }
    void ReleaseObject()
    {
        if (Input.GetMouseButtonDown(0) && isPlaceable)
        {
            prefabCopy.GetComponent<Renderer>().sharedMaterial = matForPrefab;
            prefabsMatsOnTheScene.Add(prefabCopy, prefabCopy.GetComponent<Renderer>().material);

            if (prefabCopy.transform.childCount > 0)
                prefabCopy.transform.GetChild(0).gameObject.SetActive(true);//If object has an particle effect actives it

            prefabCopy = null;
            objPlacementActive = false;

            AudioManager.Instance.AudioPlay(AudioManager.Instance.audioClips[0]);//Play a sound when a building placed.            
        }//Placed spawn object
    }
    void RotatePrefab()
    {
        if (Input.GetKeyDown(KeyCode.R))
            rotateAmount += 90;
        prefabCopy.transform.Rotate(Vector3.up, rotateAmount);
        //Rotate Object 90 degree
    }
    void PlacementMaterialChange()//Changes material of all objects white on the scene to see placeable object clear
    {
        Material mat=mat_PlacementOn;
        if (prefabsMatsOnTheScene != null)
        {
            foreach (GameObject go in prefabsMatsOnTheScene.Keys)
            {
                if (objPlacementActive)
                {
                    mat = mat_PlacementOn;
                    // go.transform.GetChild(0)?.gameObject.SetActive(false);
                    if (go.transform.childCount > 0)
                        go.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    mat = prefabsMatsOnTheScene[go];
                    //go.transform.GetChild(0)?.gameObject.SetActive(true);
                    if (go.transform.childCount > 0)
                        go.transform.GetChild(0).gameObject.SetActive(true);
                }

                go.GetComponent<Renderer>().material = mat;
            }
        }
    }
    void ChangeObjectMaterial()//Change object color if it can placeable or not
    {
        if (isPlaceable)
            prefabCopy.GetComponent<Renderer>().material = mat_Placeable;
        else
            prefabCopy.GetComponent<Renderer>().material = mat_NotPlaceable;
    }
    void selectionOutline()
    {
        if (!prefabCopy.GetComponent<Outline>()) prefabCopy.AddComponent<Outline>();
        else prefabCopy.GetComponent<Outline>().enabled = prefabCopy.GetComponent<Outline>().enabled ? false : true;
            
    }
    float RoundToNearPosition(float value)//Creates a grid system
    {
        float Difference = value % gridSize;
        value -= Difference;
        if (Difference > gridSize / 2)
            value += gridSize;
        return value;
    }
    public void ObjectSpawn(int index)//Spawn chosen object
    {
        if (prefabCopy == null)
        {
            foreach (var objFromPool in objectPool)
            {
                if (objFromPool.name == spawnObject[index].name)
                {
                    objFromPool.SetActive(true);
                    prefabCopy = objFromPool;
                    objectPool.Remove(objFromPool);
                    isThereAnObject = true;
                    break;
                }
            }
            if (!isThereAnObject)
            {
                prefabCopy = Instantiate(spawnObject[index], prefabCopyPos, Quaternion.identity);
                prefabCopy.name = spawnObject[index].name;
                selectionOutline();
            }
            matForPrefab = spawnObject[index].GetComponent<Renderer>().sharedMaterial;
            objPlacementActive = true;
            isThereAnObject = false;           
        } 
    }
    public void EnableChildMenu()
    {
        foreach (Transform menuChild in buildingsMenu.transform)
        {
            if (!menuChild.GetComponent<Animator>().GetBool("buildingsOn"))
                menuChild.GetComponent<Animator>().SetBool("buildingsOn", true);
            else
                menuChild.GetComponent<Animator>().SetBool("buildingsOn", false);
        }
    }
   

    //public void EnableChildMenu(GameObject gO)
    //{
    //    GameObject menu = gO.transform.GetChild(0).gameObject;
    //    foreach (Transform menuChild in menu.transform)
    //    {
    //        if(!menuChild.GetComponent<Animator>().GetBool("buildingsOn"))
    //            menuChild.GetComponent<Animator>().SetBool("buildingsOn", true);
    //        else
    //            menuChild.GetComponent<Animator>().SetBool("buildingsOn", false);
    //    }
    //}
}
