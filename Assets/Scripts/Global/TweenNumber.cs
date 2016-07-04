using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("NGUI/Tween/Count")]
public class TweenNumber : UITweener {

	#region Public Instance Variables
	public float from = 0.0f;
	public float to = 0.0f;
	#endregion

	#region Public Instance Properties
	public float value
	{
		get
		{
			return this._value;
		}
		set
		{
			this._value = value;
		}
	}
	#endregion

	#region Private Instance Variables
	/// <summary>
	/// 끝났는지 확인 여부
	/// </summary>
	private bool isFinish = false;
	private float _value;
	#endregion

	/// <summary>
	/// Occurs when on update tween event.
	/// </summary>
	public string callWhenTween = "";

	#region Implementation ( OnUpdate )
	protected override void OnUpdate (float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
		if (callWhenTween != "")
			eventReceiver.SendMessage(callWhenTween, value);
	}
	#endregion

	#region Implementation ( Begin )
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	public static TweenNumber Begin(GameObject go, float duration, float to)
	{
		TweenNumber comp = UITweener.Begin<TweenNumber>(go, duration);
		comp.from = comp.value;
		comp.to = to;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
	#endregion

	#region Implementation ( Set Start and End Current Value )
	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
	#endregion
}
