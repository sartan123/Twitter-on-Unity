using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Twitter
{
    public class TweetPost : MonoBehaviour
    {
        [SerializeField] Text inputText;
        public void OnClick()
        {
            Twitter instance = TwitterAPI.twitter;
            string test = inputText.text;

            if(instance != null)
            {
                if(test.Length != 0)
                {
                    bool result = instance.Tweet(test);
                    if(result)
                    {
                        Debug.Log("Post Success!");
                    }
                    else
                    {
                        Debug.Log("Post Failed...");
                    }
                }
                else
                {
                    Debug.Log("Tweet Text Empty");
                }

            }
            else
            {
                Debug.Log("Don't Exist TweetAPI instance");
            }

        }
    }
}