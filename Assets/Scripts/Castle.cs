using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 */
public class Castle : MonoBehaviour {
	
	/*
	 */
	public tk2dSprite selection = null;
	public float selectionOffset = -5.0f;
	
	private tk2dTileMap cachedTileMap = null;
	private GameObject selectedEntity = null;
	
	private GameObject[] entities = null;
	private GameObject[] thieves = null;
	private GameObject[] sheeps = null;
	
	/*
	 */
	private void Awake() {
		cachedTileMap = GetComponent<tk2dTileMap>();
	}
	
	/*
	 */
	private void Start() {
		entities = GameObject.FindGameObjectsWithTag("Entity");
		thieves = GameObject.FindGameObjectsWithTag("Thief");
		sheeps = GameObject.FindGameObjectsWithTag("Sheep");
	}
	
	/*
	 */
	private void Update() {
		if(cachedTileMap == null) return;
		if(selection == null) return;
		
		Vector3 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		int tile_x = 0;
		int tile_y = 0;
		
		if(!cachedTileMap.GetTileAtPosition(mouse_world,out tile_x,out tile_y)) return;
		
		Vector3 tile_world = cachedTileMap.GetTilePosition(tile_x,tile_y);
		tile_world.x += cachedTileMap.data.tileSize.x * 0.5f;
		tile_world.y += cachedTileMap.data.tileSize.y * 0.5f;
		tile_world.z = selectionOffset;
		selection.transform.position = tile_world;
		
		if(selectedEntity != null) {
			tile_world.z = selectedEntity.transform.position.z;
			selectedEntity.transform.position = tile_world;
			
			bool no_thieves = (GetEntityAtTile(tile_x,tile_y,thieves) == null);
			bool no_entities = (GetEntityAtTile(tile_x,tile_y,entities) == null);
			bool no_sheeps = (GetEntityAtTile(tile_x,tile_y,sheeps) == null);
			bool no_block = !IsBlock(tile_x,tile_y);
			
			selection.color = (no_thieves && no_entities && no_sheeps && no_block) ? Color.green : Color.red;
		}
		
		if(Input.GetMouseButtonDown(0)) {
			if(selectedEntity == null) {
				selectedEntity = GetEntityAtTile(tile_x,tile_y,entities);
			}
			else {
				bool no_thieves = (GetEntityAtTile(tile_x,tile_y,thieves) == null);
				bool no_entities = (GetEntityAtTile(tile_x,tile_y,entities) == null);
				bool no_sheeps = (GetEntityAtTile(tile_x,tile_y,sheeps) == null);
				bool no_block = !IsBlock(tile_x,tile_y);
				
				if(no_thieves && no_entities && no_sheeps && no_block) {
					selectedEntity = null;
				}
			}
		}
	}
	
	private bool IsBlock(int x,int y) {
		return (cachedTileMap.GetTile(x,y,2) != -1);
	}
	
	private bool FindPath(Vector2 thief,Vector2 sheep,Queue<Vector3> path) {
		path.Clear();
		if(thief == sheep) return true;
		
		List<Vector3> visited_nodes = new List<Vector3>();
		
		path.Enqueue(new Vector3(thief.x,thief.y,0));
		
		while(path.Count != 0) {
			
			Vector3 node = path.Dequeue();
			visited_nodes.Add(node);
			
			if(node.x == sheep.x && node.y == sheep.y) break;
			
			Vector3 left = new Vector3(node.x - 1,node.y,node.z + 1);
			Vector3 right = new Vector3(node.x + 1,node.y,node.z + 1);
			Vector3 top = new Vector3(node.x,node.y - 1,node.z + 1);
			Vector3 bottom = new Vector3(node.x,node.y + 1,node.z + 1);
			
			if(!IsBlock((int)left.x,(int)left.y) && !visited_nodes.Contains(left)) path.Enqueue(left);
			if(!IsBlock((int)right.x,(int)right.y) && !visited_nodes.Contains(right)) path.Enqueue(right);
			if(!IsBlock((int)top.x,(int)top.y) && !visited_nodes.Contains(top)) path.Enqueue(top);
			if(!IsBlock((int)bottom.x,(int)bottom.y) && !visited_nodes.Contains(bottom)) path.Enqueue(bottom);
		}
		
		// TODO restore path
		return false;
	}
	
	private GameObject GetEntityAtTile(int x,int y,GameObject[] collection) {
		foreach(GameObject obj in collection) {
			if(obj == selectedEntity) continue;
			
			int entity_x = 0;
			int entity_y = 0;
			if(!cachedTileMap.GetTileAtPosition(obj.transform.position,out entity_x,out entity_y)) continue;
			
			if(entity_x != x) continue;
			if(entity_y != y) continue;
			
			return obj;
		}
		
		return null;
	}
}
