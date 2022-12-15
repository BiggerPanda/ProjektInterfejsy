using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneScriptableList", menuName = "ProjektInterfejsy/SceneScriptableList", order = 0)]
public class SceneScriptableList : ScriptableObject
{
    public List<SceneData> sceneList = new List<SceneData>();

    public SceneData findScene(string name)
    {
        foreach (SceneData scene in sceneList)
        {
            if (scene.sceneName == name)
            {
                return scene;
            }
        }
        return null;
    }

    public SceneData findScene(int index)
    {
        foreach (SceneData scene in sceneList)
        {
            if (scene.sceneIndex == index)
            {
                return scene;
            }
        }
        return null;
    }
}
