using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void GoToMainScene()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void StartGameFromTheBeginning()
    {
        SceneManager.LoadScene("Prologue", LoadSceneMode.Single);
    }
}
