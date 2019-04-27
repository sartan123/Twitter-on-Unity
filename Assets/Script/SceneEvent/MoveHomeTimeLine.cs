using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveHomeTimeLine : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Move Home TimeLine");
        SceneManager.LoadScene("HomeTimeLine");
    }
}
