using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Twitter
{
    public class FollerIDs
    {
        public List<object> ids { get; set; }
        public long next_cursor { get; set; }
        public string next_cursor_str { get; set; }
        public long previous_cursor { get; set; }
        public string previous_cursor_str { get; set; }
    }

    public class UserProfileController : MonoBehaviour
    {
        private List<long> UserIDList { get; set; }
        private List<GameObject> UserProfileList { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            UserIDList = new List<long>();
            UserProfileList = new List<GameObject>();

            SetUserIDList();
            SetUserProfileList();
        }

        // Update is called once per frame
        void Update()
        {
        }

        void SetUserIDList()
        {
            Twitter instance = TwitterAPI.twitter;
            if (instance != null)
            {

                string meta_data = instance.GetFollowIDList("sartan_kancore");
                var id_list = JsonConvert.DeserializeObject<FollerIDs>(meta_data);

                foreach (long id in id_list.ids)
                {
                    Debug.Log(id);
                    this.UserIDList.Add(id);
                }
            }
        }

        void SetUserProfileList()
        {
            //foreach (long id in this.UserIDList)
            for(int i = 0; i < 5; i++)
            {
                long id = this.UserIDList[i];
                GameObject obj = (GameObject)Resources.Load("UserProfile");
                GameObject instance = Instantiate(obj) as GameObject;
                instance.SetActive(true);
                instance.SendMessage("SetUserID", id);

                Vector3 temp = instance.transform.position;
                temp.y += i * 120.0f;
                instance.transform.position = temp;

                UserProfileList.Add(instance);
            }
        }


    }
}
