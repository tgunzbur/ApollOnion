using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linksButton : MonoBehaviour
{
    public string websiteLink;
    public string twitterLink;

    public void goToTwitter() {
        Application.OpenURL(twitterLink);
    }

    public void goToWebsite() {
        Application.OpenURL(websiteLink);
    }
}
