using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum zFox{
	POSTION,
	POSTION_X,
	POSTION_Y,
	POSTION_Z,
	LOCALPOSTION,
	LOCALPOSTION_X,
	LOCALPOSTION_Y,
	LOCALPOSTION_Z,
	LOCALROTATION,
	LOCALROTATION_X,
	LOCALROTATION_Y,
	LOCALROTATION_Z,
	LOCALSCALE,
	LOCALSCALE_X,
	LOCALSCALE_Y,
	LOCALSCALE_Z,
	COLOR_R,
	COLOR_G,
	COLOR_B,
	COLOR_A,
	COLOR_RGB,
	COLOR_RGBA,
}
public enum zFox_OPM{
	NON,
	REPEAT,
	PINGPONG,
	SMOOTHSTEP,
	SMOOTHSTEP_PINGPONG,
	SMOOTHDUMP,
	SMOOTHHDUMP_PINGPONG,

	SIN,
	COS,
	TAN,
	RANDOM,
}

public enum zFOX_OUT{
	OVERRIDE,
	ADD,
	SUB,
	ADDxWEITGH,
	SUBxWRiTGH,
}

public enum zFOX_FILTER{
	NON,
	MIN,
	MAX,
	MINMAX,
}

public class Tween : MonoBehaviour {
	[System.Serializable]
	//シリアライズ
	public class TweenItem{
		public bool enable = true;
		public zFox value = zFox.LOCALSCALE;
		public zFox_OPM open = zFox_OPM.NON;
		public zFOX_OUT outMode = zFOX_OUT.ADD;
		public float outWeight = 1.0f;
		public float va = 0.0f;
		public float vb = 1.0f;
		public float speed = 1.0f;
		public float vt = 0.0f;

		public float _smoothDV = 0.0f;
		public float _smoothDMS = 1.0f;

		public zFOX_FILTER filterM = zFOX_FILTER.NON;
		public float fMin = 0.0f;
		public float fMax = 1.0f;
	}
	public TweenItem[] tweenItemL;
	Vector3 orgP;
	Vector3 orgLREA;
	Vector3 orgLS;
	Color orgC;

	SpriteRenderer sprite;
	bool cameraV = false;
	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
		orgP = transform.position;
		orgLREA = transform.localRotation.eulerAngles;
		orgLS = transform.localScale;
		if (sprite != null) {
			orgC = GetComponent<SpriteRenderer> ().color;
		}
	}

	void OnBecameVisible() {
		cameraV = true;
	}

	void OnBecameInvisible(){
		cameraV = false;
}
	void Update(){
		float position_x = orgP.x;
		float position_y = orgP.y;
		float position_z = orgP.z;
		float localR_x = orgLREA.x;
		float localR_y = orgLREA.y;
		float localR_z = orgLREA.z;
		float localS_x = orgLS.x;
		float localS_y = orgLS.y;
		float localS_z = orgLS.z;

		float color_r = orgC.r;
		float color_g = orgC.g;
		float color_b = orgC.b;
		float color_a = orgC.a;

		if(!cameraV || tweenItemL.Length <= 0){
			return;
	}
		foreach(TweenItem tw in tweenItemL){
			if(!tw.enable){
				continue;
		}
			switch(tw.value){
			case zFox.POSTION_X:
				position_x = TweenFloat (tw, position_x, orgP.x);
				break;
			case zFox.POSTION_Y:
				position_y = TweenFloat(tw,position_y, orgP.y);
				break;
			case zFox.POSTION_Z :
				position_z = TweenFloat(tw,position_z,orgP.z);
				break;
			case zFox.POSTION:
				position_x = TweenFloat (tw, position_x, localR_x);
				position_y = TweenFloat (tw, position_y, localR_y);
				position_z = TweenFloat (tw, position_z, localR_z);
				break;
			case zFox.LOCALROTATION_X:
				localR_x = TweenFloat (tw, localR_x, localR_x);
				break;
			case zFox.LOCALROTATION_Y:
				localR_y = TweenFloat (tw, localR_y, localR_y);
				break;
			case zFox.LOCALROTATION_Z:
				localR_z = TweenFloat (tw, localR_z, localR_z);
				break;
			case zFox.LOCALROTATION:
				localR_x = TweenFloat (tw, localR_x, localR_x);
				localR_y = TweenFloat (tw, localR_y, localR_y);
				localR_z = TweenFloat (tw, localR_z, localR_z);
				break;
			case zFox.LOCALSCALE_X:
				localS_x += TweenFloat(tw,localS_x,orgLS.x);
				break;
			case zFox.LOCALSCALE_Y:
				localS_y += TweenFloat(tw,localS_y,orgLS.y);
				break;
			case zFox.LOCALSCALE_Z:
				localS_z += TweenFloat(tw,localS_z,orgLS.z);
				break;
			case zFox.LOCALSCALE:
				localS_x += TweenFloat(tw,localS_x,orgLS.x);
				localS_y += TweenFloat(tw,localS_y,orgLS.y);
				localS_z += TweenFloat(tw,localS_z,orgLS.z);
				break;
			case zFox.COLOR_R :
				color_r = TweenFloat(tw,color_r,orgC.r);
				break;
			case zFox.COLOR_G:
				color_g = TweenFloat(tw,color_g,orgC.g);
				break;
			case zFox.COLOR_B:
				color_b = TweenFloat(tw,color_b,orgC.b);
				break;
			case zFox.COLOR_A:
				color_a = TweenFloat(tw,color_a,orgC.a);
				break;
			case zFox.COLOR_RGB:
				color_r = TweenFloat(tw,color_r,orgC.r);
				color_g = TweenFloat(tw,color_g,orgC.g);
				color_b = TweenFloat(tw,color_b,orgC.b);
				break;
			case zFox.COLOR_RGBA:
				color_r = TweenFloat(tw,color_r,orgC.r);
				color_g = TweenFloat(tw,color_g,orgC.g);
				color_b = TweenFloat(tw,color_b,orgC.b);
				color_a = TweenFloat(tw,color_a,orgC.a);
				break;
			}
			transform.position = new Vector3(position_x,position_y,position_z);
			transform.localRotation = Quaternion.Euler(localR_x,localR_y,localR_z);
			transform.localScale = new Vector3(localS_x,localS_y,localS_z);
			if(sprite != null){
				sprite.color = new Color(color_r,color_g,color_b,color_a);
			}
		}
	}

	float TweenFloat(TweenItem tw,float nowN,float orgN){
		float n = tw.va;
		float v1 = tw.vb - tw.va;
		float t = Time.time * tw.speed + tw.vt;

		switch(tw.open){
		case zFox_OPM.REPEAT:
			n = tw.va + Mathf.Repeat (t, v1);
			break;
		case zFox_OPM.PINGPONG:
			n = tw.va + Mathf.PingPong (t, v1);
			break;
		case zFox_OPM.SMOOTHSTEP:
			n = tw.va + Mathf.SmoothStep (tw.va, tw.vb, Mathf.Repeat (t, 1.0f));
			break;
		case zFox_OPM.SMOOTHSTEP_PINGPONG:
			n = tw.va + Mathf.SmoothStep (tw.va, tw.vb, Mathf.PingPong (t, 1.0f));
			break;
		case zFox_OPM.SMOOTHDUMP:
			n = tw.va + Mathf.SmoothDamp (tw.va, tw.vb, ref tw._smoothDV, tw._smoothDMS, Mathf.Repeat (t, 1.0f));
			break;
		case zFox_OPM.SMOOTHHDUMP_PINGPONG:
			n = tw.va + Mathf.SmoothDamp (tw.va, tw.vb, ref tw._smoothDV, tw._smoothDMS, Mathf.PingPong (t, 1.0f));
			break;
		case zFox_OPM.SIN:
			n = tw.va + (v1 / 2.0f) * Mathf.Sin(t) + (v1 / 2.0f);
			break;
		case zFox_OPM.COS:
			n = tw.va + (v1 / 2.0f) * Mathf.Cos(t) + (v1 / 2.0f);
			break;
		case zFox_OPM.TAN:
			n = tw.va + (v1 / 2.0f) * Mathf.Tan(t) + (v1 / 2.0f);
			break;
		case zFox_OPM.RANDOM:
			n = Random.Range (tw.va, tw.vb);
			break;
		}

		switch(tw.outMode){
		case zFOX_OUT.OVERRIDE:
			nowN = orgN; break;
		case zFOX_OUT.ADD:
			n = n;
			break;
		case zFOX_OUT.SUB:
			n = -n;
			break;
		case zFOX_OUT.ADDxWEITGH:
			n = +n * tw.outWeight;
			break;
		case zFOX_OUT.SUBxWRiTGH:
			n = -n * tw.outWeight;
			break;
		}
		switch(tw.filterM){
		case zFOX_FILTER.MIN:
			n = Mathf.Min (n, tw.fMin);break;
		case zFOX_FILTER.MAX:
			n = Mathf.Max (n, tw.fMax);break;
		case zFOX_FILTER.MINMAX:
			n = Mathf.Clamp (n, tw.fMin, tw.fMax);break;

		}
		return nowN +n;
	}
}