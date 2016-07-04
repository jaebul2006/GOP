using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyEffectScaler : MonoBehaviour {
	public List<ParticleScaler> effects;

	public float f_x = 0.15f;

	void Start(){
		for(int i=0; i<effects.Count; i++){
			effects[i].particleScale = transform.lossyScale.x*(1f/f_x);
		}
	}
}
