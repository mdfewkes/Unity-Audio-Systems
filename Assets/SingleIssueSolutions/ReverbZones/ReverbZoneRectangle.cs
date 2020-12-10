using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbZoneRectangle : ReverbZone
{
	public float width = 10;
	public float height = 10;

	public override bool IsOverlapping(GameObject target)
	{
		return (target.transform.position.x > gameObject.transform.position.x - width / 2 &&
			    target.transform.position.x < gameObject.transform.position.x + width / 2 &&
			    target.transform.position.y > gameObject.transform.position.y - width / 2 &&
			    target.transform.position.y < gameObject.transform.position.y + width / 2);
	}

	void OnDrawGizmosSelected()
	{
		Vector3 theSize = new Vector3(width, height, 0f);
		Gizmos.DrawWireCube(gameObject.transform.position, theSize);
	}
}

