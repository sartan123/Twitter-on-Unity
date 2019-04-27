using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;

namespace Twitter
{
    public class UserProfile : MonoBehaviour
    {
        private long user_id;

        [Obsolete]
        public void SetUserID(long id)
        {
            this.user_id = id;
            SetProfileInfomation();
        }

        [Obsolete]
        private void SetProfileInfomation()
        {
            Twitter instance = TwitterAPI.twitter;
            string result = "";

            if (instance != null)
            {
                result = instance.GetUserInfomation(this.user_id.ToString());
            }
            else
            {
                Debug.Log("Failed to get user profile");
                return;
            }

            var user = JsonConvert.DeserializeObject<UserInformation.RootObject>(result);
            GameObject obj = transform.FindChild("Canvas").gameObject;
            foreach (Transform child in obj.transform)
            {
                Text target = null;
                RawImage image = null;
                switch (child.name)
                {
                    case "UserName":
                        target = child.GetComponent<Text>();
                        target.text = user.name;
                        break;
                    case "Follow":
                        target = child.GetComponent<Text>();
                        target.text = "フォロー：" + user.friends_count.ToString();
                        break;
                    case "Follower":
                        target = child.GetComponent<Text>();
                        target.text = "フォロワー：" + user.followers_count.ToString();
                        break;
                    case "TotalTweet":
                        target = child.GetComponent<Text>();
                        target.text = "ツイート数：" + user.statuses_count.ToString();
                        break;
                    case "Description":
                        target = child.GetComponent<Text>();
                        target.text = user.description;
                        break;
                    case "Icon":
                        image = child.GetComponent<RawImage>();
                        image.SendMessage("UpdateTexture", user.profile_image_url);
                        break;
                }
            }
        }
    }
}
