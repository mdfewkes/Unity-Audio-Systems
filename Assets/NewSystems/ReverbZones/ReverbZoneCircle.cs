using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbZoneCircle : ReverbZone
{
    public float radius = 10;

	public override bool IsOverlapping(GameObject target)
	{
		return (Vector3.Distance(gameObject.transform.position, target.transform.position) < radius);
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(gameObject.transform.position, radius);
	}
}
