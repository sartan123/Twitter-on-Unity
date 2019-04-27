using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnTimeLine : MonoBehaviour
{
    private Button testButton;
    private Text buttonText;
    // Start is called before the first frame update
    void Start()
    {
        testButton = GetComponent<Button>();
        buttonText = testButton.transform.GetChild(0).GetComponent<Text>();
        buttonText.text = "test1";
        testButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        Debug.Log("Return Scene");
        SceneManager.LoadScene("SampleScene");
    }

}
