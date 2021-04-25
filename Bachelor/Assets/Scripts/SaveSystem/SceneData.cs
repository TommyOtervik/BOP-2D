using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    private string sceneName;
    private List<EnvironmentalModel> environmentalObjects;

    public SceneData(SceneData sceneData)
    {
        this.sceneName = sceneData.sceneName;
        this.environmentalObjects = sceneData.environmentalObjects;
    }

    public SceneData(string sceneName, List<EnvironmentalModel> environmentalObjects)
    {
        this.sceneName = sceneName;
        this.environmentalObjects = environmentalObjects;
    }


    public void AddToEnvironmentalList(EnvironmentalModel environmentalModel)
    {
        this.environmentalObjects.Add(environmentalModel);
    }



}
