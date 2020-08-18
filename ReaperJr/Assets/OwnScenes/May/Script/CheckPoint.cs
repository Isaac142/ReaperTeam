using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CheckPoint : ReaperJr
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _GAME.checkPoints.Add(transform.position);
            StartCoroutine(FadeText());
        }
    }

    IEnumerator FadeText ()
    {
        _UI.checkPointInfo.DOFade(0, 0);
        _UI.checkPointInfo.rectTransform.DOMove(this.transform.position, 0);
        _UI.checkPointInfo.DOFade(0.3f, 0.3f);
        yield return new WaitForSeconds(0.5f);
        _UI.checkPointInfo.DOFade(0, 0.3f);
    }
}
