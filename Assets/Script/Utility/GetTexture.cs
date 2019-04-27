using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Twitter
{
    public class GetTexture : MonoBehaviour
    {
        [Obsolete]
        IEnumerator UpdateTexture(string url)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                RawImage rawImage = GetComponent<RawImage>();
                rawImage.texture = www.textureNonReadable;
            }
        }
    }
}
