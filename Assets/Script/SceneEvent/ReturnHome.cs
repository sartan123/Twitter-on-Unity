using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnHome : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Return Home Scene");
        SceneManager.LoadScene("HomeMenu");
    }
}
