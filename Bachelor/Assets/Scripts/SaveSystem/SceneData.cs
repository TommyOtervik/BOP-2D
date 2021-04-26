using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    private string sceneName;
    private List<GameObject> environmentalObjects;

    public SceneData(SceneData sceneData)
    {
        this.sceneName = sceneData.sceneName;
        this.environmentalObjects = sceneData.environmentalObjects;
    }

    public SceneData(string sceneName, List<GameObject> environmentalObjects)
    {
        this.sceneName = sceneName;
        this.environmentalObjects = environmentalObjects;
    }


    public void AddToEnvironmentalList(GameObject environmentalModel)
    {
        this.environmentalObjects.Add(environmentalModel);
    }



}
