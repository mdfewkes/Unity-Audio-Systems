using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbZoneRectangle : ReverbZone
{
	public float width = 10;
	public float height = 10;

	public override bool IsOverlapping(Vector3 target)
	{
		return (target.x > gameObject.transform.position.x - width / 2 &&
				target.x < gameObject.transform.position.x + width / 2 &&
				target.y > gameObject.transform.position.y - height / 2 &&
				target.y < gameObject.transform.position.y + height / 2);
	}

	public override bool IsOverlapping(GameObject target)
	{
		return IsOverlapping(target.transform.position);
	}

	void OnDrawGizmosSelected()
	{
		Vector3 theSize = new Vector3(width, height, 0f);
		Gizmos.DrawWireCube(gameObject.transform.position, theSize);
	}
}

