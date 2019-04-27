using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Twitter
{
    public class HomeTilmeLineController : MonoBehaviour
    {
        // Start is called before the first frame update
        //private long last_get_tweet_id;
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
                foreach (var tweet in timeline)
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

        }
    }
}