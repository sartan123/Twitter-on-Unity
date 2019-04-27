using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveTweetScreen : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Move Tweet Scene");
        SceneManager.LoadScene("TweetScreen");
    }
}

