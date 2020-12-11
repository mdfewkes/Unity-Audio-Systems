using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbZoneCircle : ReverbZone
{
    public float radius = 10;

	public override bool IsOverlapping(Vector3 target)
	{
		return (Vector3.Distance(gameObject.transform.position, target) < radius);
	}

	public override bool IsOverlapping(GameObject target)
	{
		return IsOverlapping(target.transform.position);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(gameObject.transform.position, radius);
	}
}
