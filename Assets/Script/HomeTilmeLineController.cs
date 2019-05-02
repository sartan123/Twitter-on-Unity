using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Twitter
{
    public class HomeTilmeLineController : MonoBehaviour
    {
        // Start is called before the first frame update
        //private long last_get_tweet_id;
        [SerializeField] float interval = 7.5f;
        void Start()
        {
            //last_get_tweet_id = 0;
            Twitter instance = TwitterAPI.twitter;
            if (instance != null)
            {
                GameObject obj = (GameObject)Resources.Load("TweetController");
                GameObject tweet_controller = Instantiate(obj) as GameObject;
                tweet_controller.transform.parent = transform;

                string meta_data = instance.GetHomeTimeLine();
                Debug.Log(meta_data);
                var timeline = JsonConvert.DeserializeObject<List<TimeLine.RootObject>>(meta_data);
                foreach (TimeLine.RootObject tweet in timeline)
                {
                    if (tweet != null)
                    {
                        tweet_controller.SendMessage("CreateTweetObjectFromID", tweet.id);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 temp = GetComponent<Transform>().position;
                temp.y += interval;
                GetComponent<Transform>().position = temp;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector3 temp = GetComponent<Transform>().position;
                temp.y -= interval;
                GetComponent<Transform>().position = temp;
            }
        }
    }
}