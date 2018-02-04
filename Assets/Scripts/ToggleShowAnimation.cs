using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShowAnimation : MonoBehaviour {

    public bool animationEnCours = false;
    private bool toUp = true;
    private Vector3 pos;
    
	public void LancerAnimation()
    {
        animationEnCours = true;
        pos = this.transform.position;
        InvokeRepeating("Animate", 0.1f, 0.1f);
    }

    public void Update()
    {
        if(animationEnCours)
        {
            if (toUp)
                this.transform.position += (Vector3.up * Time.deltaTime);
            else
                this.transform.position -= (Vector3.up * Time.deltaTime);
        }
    }

    public void Animate()
    {
        toUp = !toUp;
    }

    public void StopperAnimation()
    {
        animationEnCours = false;
        CancelInvoke();
        this.transform.position = pos;
    }
}
