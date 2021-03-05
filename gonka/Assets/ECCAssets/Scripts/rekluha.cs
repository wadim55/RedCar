using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Advertisements;


public class rekluha : MonoBehaviour
{

    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize("3965401", false);
        }
    }


    public void rolik()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("video");
        }

    }
 

   
}