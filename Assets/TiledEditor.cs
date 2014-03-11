using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TiledEditor : EditorWindow {
	public string level_name;
	private XmlDocument doc;
	public Texture2D[] textures;
	private GameObject base_sprite;
	private GameObject object_sprite;
	private List<Sprite> sprites;
	public GameObject base_node;

	void print_children(XmlNode node){
		foreach(XmlNode child in node){
			Debug.Log(child.Value);
		}
	}

	void load_sprites(string filename){
		Debug.Log("Loading: " + filename);
		Sprite[] ss = Resources.LoadAll<Sprite>(filename);
		foreach(Sprite s in ss){
			sprites.Add(s);
		}
	}

	void add_tile(int x, int y, int gid){
		if(gid != 0){
			GameObject g = (GameObject)Instantiate(base_sprite);
			g.transform.position = new Vector3(base_node.transform.position.x + x, base_node.transform.position.y - y, 0.0f);
			SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
			sr.sprite = sprites[gid-2];
		}
	}

	void add_object(float x, float y, int gid){
		if(gid != 0){
			GameObject g = (GameObject)Instantiate(object_sprite);
			g.transform.position = new Vector3(base_node.transform.position.x + x, base_node.transform.position.y - y, 0.0f);
			SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
			sr.sprite = sprites[gid-2];
			sr.sortingOrder = 1;
		}
	}

	void draw_tiles(XmlNode x){
		Debug.Log("Drawing tiles");
		Debug.Log("width: " + x.Attributes["width"].Value + " height: " + x.Attributes["height"].Value);
		//add_tile(1,1,0);
		int height = int.Parse(x.Attributes["height"].Value);
		int width = int.Parse(x.Attributes["width"].Value);
		int i = 0;
		foreach(XmlNode child in x.ChildNodes[0].ChildNodes){
			add_tile(i%width, i/height, int.Parse(child.Attributes["gid"].Value));
			i++;
		}
	}

	void draw_objects(XmlNode node){
		Debug.Log("Drawing Objects");
		foreach(XmlNode child in node.ChildNodes){
			float x = int.Parse(child.Attributes["x"].Value) / 32.0f;
			float y = (int.Parse(child.Attributes["y"].Value) - 32) / 32.0f;
			add_object(x, y, int.Parse(child.Attributes["gid"].Value));
		}
	}

	// Use this for initialization
	void nothing () {
		base_sprite = new GameObject("tile");
		base_sprite.AddComponent("SpriteRenderer");
		object_sprite = new GameObject("object");
		object_sprite.AddComponent("SpriteRenderer");
		doc = new XmlDocument();
		doc.Load(Application.dataPath + "/Resources/" + level_name);
		sprites = new List<Sprite>();
		foreach(XmlNode node in doc.GetElementsByTagName("tileset")){
			load_sprites(node.Attributes["name"].Value);
		}
		foreach(XmlNode node in doc.GetElementsByTagName("layer")){
			draw_tiles(node);
		}
		foreach(XmlNode node in doc.GetElementsByTagName("objectgroup")){
			draw_objects(node);
		}
	}

	[MenuItem ("Window/Tiled Editor")]
	void Init(){
		TiledEditor window = (TiledEditor)EditorWindow.GetWindow (typeof (TiledEditor));
	}
	
	// Update is called once per frame
	void OnGUI () {
		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		level_name = EditorGUILayout.TextField ("Text Field", level_name);
		
		//groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		//myBool = EditorGUILayout.Toggle ("Toggle", myBool);
		//myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup ();
	}
}