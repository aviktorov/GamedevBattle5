using UnityEngine;
using System.Collections;

/*
 */
public enum EntityType {
	Knight,
	Archer
}

/*
 */
public class Entity : MonoBehaviour {
	public EntityType type = EntityType.Knight;
}
