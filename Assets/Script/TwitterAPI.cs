using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Twitter
{
    public class TwitterAPI : MonoBehaviour
    {
        private static TwitterAPI instance;
        public static Twitter twitter;

        public static TwitterAPI Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = (TwitterAPI)FindObjectOfType(typeof(TwitterAPI));
                    if (null == instance)
                    {
                        Debug.Log(" DataManager Instance Error ");
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            using (StreamReader r = new StreamReader("Assets/Resources/Config.json"))
            {
                string json = r.ReadToEnd();
                AccessTokenObject items = JsonConvert.DeserializeObject<AccessTokenObject>(json);
                string cK = items.ConsumerKey;
                string cS = items.ConsumerSecret;
                string aT = items.AccessToken;
                string aTS = items.AccessTokenSecret;

                twitter = new Twitter(cK, cS, aT, aTS);

                Debug.Log("Created TwitterAPI Instance");
            }
        }

    }

    public class Twitter
    {
        /// <summary>
        /// Consumer Key (API Key)
        /// </summary>
        public string ConsumerKey;

        /// <summary>
        /// Consumer Secret (API Secret)
        /// </summary>
        public string ConsumerSecret;

        /// <summary>
        /// Access Token
        /// </summary>
        public string AccessToken;

        /// <summary>
        /// Access Token Secret
        /// </summary>
        public string AccessTokenSecret;

        /// <summary>
        /// OAuth Token
        /// </summary>
        public string OAuthToken;

        /// <summary>
        /// OAuth Token Secret
        /// </summary>
        public string OAuthTokenSecret;

        private enum METHOD { GET, POST };

        private const string ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";
        private const string AUTHORIZE = "https://api.twitter.com/oauth/authorize";
        private const string REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";
        private const string UPDATE_STATUS = "https://api.twitter.com/1.1/statuses/update.json";

        private const string GET_USER_TIMELINE = "https://api.twitter.com/1.1/statuses/user_timeline.json";
        private const string GET_TWEET_FROM_ID = "https://api.twitter.com/1.1/statuses/show.json";
        private const string GET_FOLLOWE_IDS = "https://api.twitter.com/1.1/friends/ids.json";
        private const string GET_FOLLOWERS_IDS = "https://api.twitter.com/1.1/followers/ids.json";
        private const string GET_USER_INFO = "https://api.twitter.com/1.1/users/show.json";

        private const string OAUTH_CONSUMER_KEY = "oauth_consumer_key";
        private const string OAUTH_NONCE = "oauth_nonce";
        private const string OAUTH_SIGNATURE = "oauth_signature";
        private const string OAUTH_SIGNATURE_METHOD = "oauth_signature_method";
        private const string OAUTH_TIMESTAMP = "oauth_timestamp";
        private const string OAUTH_TOKEN = "oauth_token";
        private const string OAUTH_TOKEN_SECRET = "oauth_token_secret";
        private const string OAUTH_VERIFIER = "oauth_verifier";
        private const string OAUTH_VERSION = "oauth_version";
        private const string OAUTH_VERSION_VALUE = "1.0";

        /// <summary>
        /// <see cref="EasyTweet.Twitter"/> オブジェクトを生成します。
        /// </summary>
        public Twitter() : this("", "", "", "") { }

        /// <summary>
        /// <see cref="EasyTweet.Twitter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="consumerKey">Consumer Key (API Key)</param>
        /// <param name="consumerSecret">Consumer Secret (API Secret)</param>
        public Twitter(string consumerKey, string consumerSecret) : this(consumerKey, consumerSecret, "", "") { }

        /// <summary>
        /// <see cref="EasyTweet.Twitter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="consumerKey">Consumer Key (API Key)</param>
        /// <param name="consumerSecret">Consumer Secret (API Secret)</param>
        /// <param name="accessToken">Access Token</param>
        /// <param name="accessTokenSecret">Access Token Secret</param>
        public Twitter(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
        }

        /// <summary>
        /// 認証 URL を取得します。
        /// </summary>
        /// <returns>認証 URL</returns>
        public string GetAuthorizeUrl()
        {
            string authorizeUrl = "";
            string response = OAuthWebRequest(METHOD.GET, REQUEST_TOKEN, "", "");
            System.Collections.Generic.SortedDictionary<string, string> parameters = QueryDecode(response);

            if (parameters.ContainsKey(OAUTH_TOKEN) && parameters.ContainsKey(OAUTH_TOKEN_SECRET))
            {
                this.OAuthToken = parameters[OAUTH_TOKEN];
                this.OAuthTokenSecret = parameters[OAUTH_TOKEN_SECRET];

                authorizeUrl = System.String.Format("{0}?{1}={2}", AUTHORIZE, OAUTH_TOKEN, this.OAuthToken);
            }

            return authorizeUrl;
        }

        /// <summary>
        /// アクセストークン及びそのシークレットを取得します。
        /// </summary>
        /// <param name="pin">PIN コード</param>
        /// <returns>正常に取得できた場合は true</returns>
        public bool GetTokens(string pin)
        {
            if (System.String.IsNullOrEmpty(pin))
            {
                return false;
            }

            bool result = false;
            this.AccessToken = this.OAuthToken;
            string response = OAuthWebRequest(METHOD.GET, ACCESS_TOKEN, "", pin);
            System.Collections.Generic.SortedDictionary<string, string> parameters = QueryDecode(response);

            if (parameters.ContainsKey(OAUTH_TOKEN) && parameters.ContainsKey(OAUTH_TOKEN_SECRET))
            {
                this.AccessToken = parameters[OAUTH_TOKEN];
                this.AccessTokenSecret = parameters[OAUTH_TOKEN_SECRET];
                result = true;
            }

            return result;
        }

        /// <summary>
        /// ツイートを送信します。
        /// </summary>
        /// <param name="status">ツイート内容</param>
        /// <returns>ツイートを送信出来た場合は true</returns>
        public bool Tweet(string status)
        {
            return Tweet(status, "");
        }

        /// <summary>
        /// ステータス ID を指定して、ツイートを送信します。
        /// </summary>
        /// <param name="status">ツイート内容</param>
        /// <param name="statusId">ステータス ID</param>
        /// <returns>ツイートを送信出来た場合は true</returns>
        public bool Tweet(string status, string statusId)
        {
            if (System.String.IsNullOrEmpty(status))
            {
                return false;
            }

            string query = System.String.Format("?status={0}", UrlEncode(status));

            if (!System.String.IsNullOrEmpty(statusId))
            {
                query += System.String.Format("&in_reply_to_status_id={0}", statusId);
            }

            return OAuthWebRequest(METHOD.POST, UPDATE_STATUS, query, "") != "";
        }

        public string GetUserTimeLine(string screen_name)
        {
            if (System.String.IsNullOrEmpty(screen_name))
            {
                return "";
            }

            string query = System.String.Format("?screen_name={0}&exclude_replies=false", UrlEncode(screen_name));

            return OAuthWebRequest(METHOD.GET, GET_USER_TIMELINE + query, "", "");
        }

        public string GetTweetFromID(string id)
        {
            if (System.String.IsNullOrEmpty(id))
            {
                return "";
            }

            string query = System.String.Format("?id={0}&include_entities=false&contributor_details=false&include_rts=false&tweet_mode=extended", UrlEncode(id));

            return OAuthWebRequest(METHOD.GET, GET_TWEET_FROM_ID + query, "", "");
        }

        /// <summary>
        /// ステータス ID を指定して、フォローリストを返します。
        /// </summary>
        /// <param name="status">ユーザー名</param>
        /// <returns>Jsonを返した場合は true</returns>
        public string GetFollowIDList(string status)
        {
            if (System.String.IsNullOrEmpty(status))
            {
                return "";
            }

            string query = System.String.Format("?screen_name={0}", UrlEncode(status));

            return OAuthWebRequest(METHOD.GET, GET_FOLLOWE_IDS + query, "", "");
        }

        public string GetUserInfomation(string user_id)
        {
            if (System.String.IsNullOrEmpty(user_id))
            {
                return "";
            }
            string query = System.String.Format("?user_id={0}", UrlEncode(user_id));

            return OAuthWebRequest(METHOD.GET, GET_USER_INFO + query, "", "");
        }

        /// <summary>
        /// OAuth1.0 を用いて、リクエストを送信します。
        /// </summary>
        /// <param name="method">HTTP メソッド</param>
        /// <param name="url">URL</param>
        /// <param name="query">クエリ</param>
        /// <param name="pin">PIN コード</param>
        /// <returns>レスポンスデータ</returns>
        private string OAuthWebRequest(METHOD method, string url, string query, string pin)
        {
            if (System.String.IsNullOrEmpty(url))
            {
                return "";
            }

            System.Uri uri = (method == METHOD.GET) ? new System.Uri(url) : new System.Uri(url + query);

            Debug.Log(uri);

            string normalizedUrl = System.String.Format("{0}://{1}", uri.Scheme, uri.Host) + uri.AbsolutePath;
            string normalizedQuery = GenerateQuery(method, normalizedUrl, uri.Query, pin);

            if (method == METHOD.GET)
            {
                normalizedUrl = System.String.Format("{0}?{1}", normalizedUrl, normalizedQuery);
            }

            System.Net.HttpWebRequest webRequest = System.Net.WebRequest.Create(normalizedUrl) as System.Net.HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;

            if (method == METHOD.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (System.IO.StreamWriter requestWriter = new System.IO.StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(normalizedQuery);
                }
            }

            string response = "";
            System.IO.Stream resStream = null;
            System.IO.StreamReader resReader = null;

            try
            {
                resStream = webRequest.GetResponse().GetResponseStream();
                resReader = new System.IO.StreamReader(resStream);
                response = resReader.ReadToEnd();
            }
            catch
            {
                response = "";
            }
            finally
            {
                if (resReader != null)
                {
                    resReader.Dispose();
                }

                if (resStream != null)
                {
                    resStream.Dispose();
                }
            }

            return response;
        }

        /// <summary>
        /// リクエスト用のクエリを生成します。
        /// </summary>
        /// <param name="method">HTTP メソッド</param>
        /// <param name="url">URL</param>
        /// <param name="query">クエリ</param>
        /// <param name="pin">PIN コード</param>
        /// <returns>生成されたリクエスト用のクエリ</returns>
        private string GenerateQuery(METHOD method, string url, string query, string pin)
        {
            if (System.String.IsNullOrEmpty(url))
            {
                return "";
            }

            System.Collections.Generic.SortedDictionary<string, string> parameters = QueryDecode(query);
            parameters.Add(OAUTH_CONSUMER_KEY, this.ConsumerKey);
            parameters.Add(OAUTH_NONCE, GenerateNonce());
            parameters.Add(OAUTH_SIGNATURE_METHOD, "HMAC-SHA1");
            parameters.Add(OAUTH_TIMESTAMP, GenerateTimestamp());
            parameters.Add(OAUTH_TOKEN, this.AccessToken);
            parameters.Add(OAUTH_VERSION, OAUTH_VERSION_VALUE);

            if (!System.String.IsNullOrEmpty(pin))
            {
                parameters.Add(OAUTH_VERIFIER, pin);
            }

            string normalizedQuery = QueryEncode(parameters);
            string signature = GenerateSignature(method, url, normalizedQuery);

            return System.String.Format("{0}&{1}={2}", normalizedQuery, OAUTH_SIGNATURE, UrlEncode(signature));
        }

        /// <summary>
        /// OAuth1.0 の署名を作成します。
        /// </summary>
        /// <param name="method">HTTP メソッド</param>
        /// <param name="url">URL</param>
        /// <param name="query">クエリ</param>
        /// <returns>生成された署名</returns>
        private string GenerateSignature(METHOD method, string url, string query)
        {
            if (System.String.IsNullOrEmpty(url) || System.String.IsNullOrEmpty(query))
            {
                return "";
            }

            byte[] key = System.Text.Encoding.ASCII.GetBytes(System.String.Format("{0}&{1}", UrlEncode(this.ConsumerSecret), UrlEncode(this.AccessTokenSecret)));
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(System.String.Format("{0}&{1}&{2}", method.ToString(), UrlEncode(url), UrlEncode(query)));

            using (System.Security.Cryptography.HMACSHA1 hmacsha1 = new System.Security.Cryptography.HMACSHA1(key))
            {
                return System.Convert.ToBase64String(hmacsha1.ComputeHash(buffer));
            }
        }

        /// <summary>
        /// 文字列を RFC3986 に準拠して URL エンコードします。
        /// </summary>
        /// <param name="srcString">変換する文字列</param>
        /// <returns>URL エンコードされた <paramref name="srcString"/></returns>
        private string UrlEncode(string srcString)
        {
            if (System.String.IsNullOrEmpty(srcString))
            {
                return "";
            }

            string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] srcByte = System.Text.Encoding.UTF8.GetBytes(srcString);

            System.Text.StringBuilder dstStr = new System.Text.StringBuilder();

            for (int i = 0; i < srcByte.Length; i++)
            {
                if (srcByte[i] < 0x80 && unreserved.IndexOf((char)srcByte[i]) != -1)
                {
                    dstStr.Append((char)srcByte[i]);
                }
                else
                {
                    dstStr.Append('%' + System.String.Format("{0:X2}", (int)srcByte[i]));
                }
            }

            return dstStr.ToString();
        }

        /// <summary>
        /// URL エンコードされた文字列をデコードします。
        /// </summary>
        /// <param name="srcString"></param>
        /// <returns>URL デコードされた <paramref name="srcString"/></returns>
        private string UrlDecode(string srcString)
        {
            if (System.String.IsNullOrEmpty(srcString))
            {
                return "";
            }

            return System.Uri.UnescapeDataString(srcString);
        }

        /// <summary>
        /// System.Collections.Generic.SortedDictionary&lt;string, string&gt; を key1=value1&amp;key2=value2&amp;... の文字列に変換します。
        /// </summary>
        /// <param name="parameters">System.Collections.Generic.SortedDictionary&lt;string, string&gt;</param>
        /// <returns>key1=value1&amp;key2=value2&amp;... に変換された <paramref name="parameters"/></returns>
        private string QueryEncode(System.Collections.Generic.SortedDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return "";
            }

            int count = 0;
            System.Text.StringBuilder normalizedQuery = new System.Text.StringBuilder();

            foreach (string key in parameters.Keys)
            {
                if (count != 0)
                {
                    normalizedQuery.Append("&");
                }

                normalizedQuery.AppendFormat("{0}={1}", key, parameters[key]);

                count++;
            }

            return normalizedQuery.ToString();
        }

        /// <summary>
        /// key1=value1&amp;key2=value2&amp;... の文字列を System.Collections.Generic.SortedDictionary&lt;string, string&gt; に変換します。
        /// </summary>
        /// <param name="query">key1=value1&amp;key2=value2&amp;... の文字列</param>
        /// <returns>System.Collections.Generic.SortedDictionary&lt;string, string&gt; に変換された key1=value1&amp;key2=value2&amp;...</returns>
        private System.Collections.Generic.SortedDictionary<string, string> QueryDecode(string query)
        {
            System.Collections.Generic.SortedDictionary<string, string> parameters = new System.Collections.Generic.SortedDictionary<string, string>();

            if (System.String.IsNullOrEmpty(query))
            {
                return parameters;
            }

            if (query.StartsWith("?"))
            {
                query = query.Remove(0, 1);
            }

            string[] p = query.Split('&');

            foreach (string q in p)
            {
                if (q.IndexOf('=') > -1)
                {
                    string[] temp = q.Split('=');
                    parameters.Add(temp[0], temp[1]);
                }
                else
                {
                    parameters.Add(q, "");
                }
            }

            return parameters;
        }

        /// <summary>
        /// ワンタイムトークンを生成します。
        /// </summary>
        /// <returns>ワンタイムトークン</returns>
        private string GenerateNonce()
        {
            return new System.Random().Next(100000, 999999).ToString();
        }

        /// <summary>
        /// タイムスタンプを生成します。
        /// </summary>
        /// <returns>タイムスタンプ</returns>
        private string GenerateTimestamp()
        {
            System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            long unixtime = (long)(System.DateTime.Now.ToUniversalTime() - epoch).TotalSeconds;

            return unixtime.ToString();
        }
    }

}