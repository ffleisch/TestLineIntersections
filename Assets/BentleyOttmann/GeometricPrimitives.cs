using System.Collections;
using System.Collections.Generic;
using UnityEngine;

int ComparePts(Vector2 a, Vector2 b)
{
	if (a.x < b.x)
	{
		return 0;
	}
	if (a.x > b.x)
	{
		return 1;
	}

	if (a.x == b.x)
	{
		if (a.y < b.y)
		{
			return 0;
		}
		else
		{
			return 1;
		}
	}

	//hsould never be reached
	return 0;

}



