using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweetController : MonoBehaviour
{
    // Start is called before the first frame update
    private int tweet_exist_count = 0;
    private float prev_offset_y = 0.0f;
    private float prev_position_y = 0.0f;
    private float base_offset_y = -256;

    void CreateTweetObjectFromID(long id)
    {
        GameObject obj = (GameObject)Resources.Load("Tweet");
        GameObject instance = Instantiate(obj) as GameObject;
        instance.SetActive(true);
        instance.SendMessage("CreateTweetObjectFromID", id);

        instance.transform.parent = transform;
        tweet_exist_count++;

        GameObject canvas_obj = instance.transform.Find("Canvas").gameObject;
        GameObject panel_obj  = canvas_obj.transform.Find("Panel").gameObject;

        if (tweet_exist_count != 1)
        {
            Vector3 temp = instance.transform.position;
            float y = prev_position_y  + prev_offset_y + base_offset_y;
            temp.y = y;
            instance.transform.position = temp;
        }
        else
        {
            instance.transform.position = Vector3.zero;
        }

        prev_offset_y = panel_obj.GetComponent<RectTransform>().offsetMin.y;
        prev_position_y = instance.transform.position.y;
    }

}
