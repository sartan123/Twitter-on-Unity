using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Linq;

namespace Twitter
{
    public class Tweet : MonoBehaviour
    {
        public void OnUserAction()
        {
            transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("Move TweetDetail Scene");
            SceneManager.LoadScene("TweetDetail");
        }


        void CreateTweetObjectFromID(long tweet_id)
        {
            Twitter instance = TwitterAPI.twitter;

            string result = "";
            if (instance != null)
            {
                result = instance.GetTweetFromID(tweet_id.ToString());
            }
            else
            {
                Debug.Log("Failed to get tweet object");
                return;
            }

            if(result.Contains("\"retweeted\":true"))
            {
                if (result.Contains("media"))
                {
                    CreateMediaRetweetInfo(result);
                }
                else
                {
                    CreateRetweetInfo(result);
                }
            }
            else
            {
                if (result.Contains("media"))
                {
                    CreateMediaTweetInfo(result);
                }
                else
                {
                    CreateNormalTweetInfo(result);
                }
            }
        }

        void AdjustPanelSize(string tweet)
        {
            GameObject obj = transform.Find("Canvas").gameObject;
            GameObject obj2 = obj.transform.Find("Panel").gameObject;
            var lineCount = tweet.ToList().Where(c => c.Equals('\n')).Count() + 1;
            float x = obj2.GetComponent<RectTransform>().offsetMin.x;
            float y = obj2.GetComponent<RectTransform>().offsetMin.y;

            if(lineCount >= 4)
            {
                y = y - (20 * (lineCount - 4));
            }
            obj2.GetComponent<RectTransform>().offsetMin = new Vector2(x, y);
        }


        void CreateNormalTweetInfo(string tweet_info)
        {
            var tweet = JsonConvert.DeserializeObject<NormalTweet.TweetObject>(tweet_info);
            if (tweet == null) return;
            GameObject obj = transform.Find("Canvas").gameObject;

            foreach (Transform child in obj.transform)
            {
                Text target = null;
                RawImage image = null;
                Button button = null;
                switch (child.name)
                {
                    case "UserName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = tweet.user.name;
                        break;
                    case "ScreenName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = "@" + tweet.user.screen_name;
                        break;
                    case "Context":
                        target = child.GetComponent<Text>();
                        target.text = tweet.full_text;
                        break;
                    case "CreatedTime":
                        target = child.GetComponent<Text>();
                        target.text = tweet.created_at;
                        break;
                    case "Icon":
                        image = child.GetComponent<RawImage>();
                        image.SendMessage("UpdateTexture", tweet.user.profile_image_url);
                        break;
                    case "TouchEvent":
                        button = child.GetComponent<Button>();
                        Debug.Log("Added Onclink");
                        button.onClick.AddListener(OnUserAction);
                        break;
                }
            }

            AdjustPanelSize(tweet.full_text);
        }

        void CreateRetweetInfo(string tweet_info)
        {
            var tweet = JsonConvert.DeserializeObject<Retweet.TweetObject>(tweet_info);
            if (tweet == null) return;
            GameObject obj = transform.Find("Canvas").gameObject;

            foreach (Transform child in obj.transform)
            {
                Text target = null;
                RawImage image = null;
                Button button = null;
                switch (child.name)
                {
                    case "UserName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = tweet.user.name;
                        break;
                    case "ScreenName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = "@" + tweet.user.screen_name;
                        break;
                    case "Context":
                        target = child.GetComponent<Text>();
                        target.text = tweet.retweeted_status.full_text;
                        break;
                    case "CreatedTime":
                        target = child.GetComponent<Text>();
                        target.text = tweet.retweeted_status.created_at;
                        break;
                    case "Icon":
                        image = child.GetComponent<RawImage>();
                        image.SendMessage("UpdateTexture", tweet.retweeted_status.user.profile_image_url);
                        break;
                    case "TouchEvent":
                        button = child.GetComponent<Button>();
                        button.onClick.AddListener(OnUserAction);
                        break;
                }
            }

            AdjustPanelSize(tweet.full_text);
        }


        void CreateMediaTweetInfo(string tweet_info)
        {
            var tweet = JsonConvert.DeserializeObject<TweetIncludeImage.TweetObject>(tweet_info);
            if (tweet == null) return;
            GameObject obj = transform.Find("Canvas").gameObject;

            foreach (Transform child in obj.transform)
            {
                Text target = null;
                RawImage image = null;
                RawImage image2 = null;
                Button button = null;
                switch (child.name)
                {
                    case "UserName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = tweet.user.name;
                        break;
                    case "ScreenName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = "@" + tweet.user.screen_name;
                        break;
                    case "Context":
                        target = child.GetComponent<Text>();
                        target.text = tweet.full_text;
                        break;
                    case "CreatedTime":
                        target = child.GetComponent<Text>();
                        target.text = tweet.created_at;
                        break;
                    case "Icon":
                        image = child.GetComponent<RawImage>();
                        image.SendMessage("UpdateTexture", tweet.user.profile_image_url);
                        break;
                    case "Image":
                        if (tweet.extended_entities != null)
                        {
                            if (tweet.extended_entities.media != null)
                            {
                                image2 = child.GetComponent<RawImage>();
                                image2.gameObject.SetActive(true);
                                image2.SendMessage("UpdateTexture", tweet.extended_entities.media[0].media_url);
                            }
                        }
                        break;
                    case "TouchEvent":
                        button = child.GetComponent<Button>();
                        button.onClick.AddListener(OnUserAction);
                        break;
                }
            }

            AdjustPanelSize(tweet.full_text);
        }

        void CreateMediaRetweetInfo(string tweet_info)
        {
            var tweet = JsonConvert.DeserializeObject<RetweetIncludeImage.TweetObject>(tweet_info);

            if (tweet == null) return;
            GameObject obj = transform.Find("Canvas").gameObject;

            foreach (Transform child in obj.transform)
            {
                Text target = null;
                RawImage image = null;
                RawImage image2 = null;
                Button button = null;
                switch (child.name)
                {
                    case "UserName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = tweet.user.name;
                        break;
                    case "ScreenName":
                        button = child.GetComponent<Button>();
                        target = button.GetComponentInChildren<Text>();
                        target.text = "@" + tweet.user.screen_name;
                        break;
                    case "Context":
                        target = child.GetComponent<Text>();
                        target.text = tweet.retweeted_status.full_text;
                        break;
                    case "CreatedTime":
                        target = child.GetComponent<Text>();
                        target.text = tweet.retweeted_status.created_at;
                        break;
                    case "Icon":
                        image = child.GetComponent<RawImage>();
                        image.SendMessage("UpdateTexture", tweet.retweeted_status.user.profile_image_url);
                        break;
                    case "Image":
                        if (tweet.extended_entities != null)
                        {
                            if (tweet.extended_entities.media != null)
                            {
                                image2 = child.GetComponent<RawImage>();
                                image2.gameObject.SetActive(true);
                                image2.SendMessage("UpdateTexture", tweet.retweeted_status.extended_entities.media[0].media_url);
                            }
                        }
                        break;
                    case "TouchEvent":
                        button = child.GetComponent<Button>();
                        button.onClick.AddListener(OnUserAction);
                        break;
                }
            }

            AdjustPanelSize(tweet.full_text);
        }
    }

}