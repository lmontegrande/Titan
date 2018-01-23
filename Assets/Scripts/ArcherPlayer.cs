using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherPlayer : MonoBehaviour {

    public GameObject screenFlashImage;
    public int healthPoints = 10;

    public void GetHit()
    {
        GetHit(1);
        StartCoroutine(ScreenFlash());
    }

    public void GetHit(int damage)
    {
        healthPoints--;
        StartCoroutine(ScreenFlash());
    }

    public IEnumerator ScreenFlash()
    {
        screenFlashImage.SetActive(true);
        yield return new WaitForSeconds(.5f);
        screenFlashImage.SetActive(false);
    }
}
