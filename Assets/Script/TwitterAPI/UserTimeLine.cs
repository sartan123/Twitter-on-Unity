using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


namespace Twitter
{
    public class UserTimeLine : MonoBehaviour
    {
        // Start is called before the first frame update
        private string user_name;
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                Debug.Log("Input Q");
                CreateUserTimeLine("sartan_kancore");
            }
        }

        void CreateUserTimeLine(string name)
        {
            if(this.name != name)
            {
                //DeleteTimeLine();
                this.user_name = name;
                CreateTimeLine();
            }
            else
            {
                return;
            }
        }

        void CreateTimeLine()
        {
            Twitter instance = TwitterAPI.twitter;
            if(instance != null)
            {
                string result = instance.GetUserTimeLine(user_name);

                var timeline = JsonConvert.DeserializeObject<List<TimeLine.RootObject>>(result);

                GameObject obj = (GameObject)Resources.Load("TweetController");
                GameObject tweet_controller = Instantiate(obj) as GameObject;
                tweet_controller.transform.parent = transform;

                foreach (var tweet in timeline)
                {
                    if (tweet != null)
                    {
                        tweet_controller.SendMessage("CreateTweetObjectFromID", tweet.id);
                    }
                }
            }
        }

        void DeleteTimeLine()
        {
            foreach (Transform n in transform)
            {
                Destroy(n.gameObject);
            }
        }
    }

}

