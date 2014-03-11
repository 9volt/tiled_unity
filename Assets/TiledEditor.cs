﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TiledEditor : EditorWindow {
	string current_level;
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.23f;

	private XmlDocument doc;
	private GameObject base_sprite;
	private GameObject object_sprite;
	Transform transform;
	private List<Sprite> sprites;

	void load_sprites(string filename){
		Debug.Log("Loading: " + filename);
		Sprite[] ss = Resources.LoadAll<Sprite>(filename);
		sprites.Add(null);
		foreach(Sprite s in ss){
			sprites.Add(s);
		}
	}

	void add_tile(int x, int y, int gid){
		if(gid != 0){
			GameObject g = (GameObject)Instantiate(base_sprite);
			g.transform.parent = transform;
			g.transform.position = new Vector3(transform.position.x + x, transform.position.y - y, 0.0f);
			SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
			sr.sprite = sprites[gid];
		}
	}
	
	void add_object(float x, float y, int gid){
		if(gid != 0){
			GameObject g = (GameObject)Instantiate(object_sprite);
			g.transform.position = new Vector3(transform.position.x + x, transform.position.y - y, 0.0f);
			SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
			sr.sprite = sprites[gid];
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


	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Tiled Editor")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(TiledEditor));
	}

	void OpenLevel(){
		current_level = EditorUtility.OpenFilePanel("Open level file", Application.dataPath, "tmx;xml");
		base_sprite = new GameObject("tile");
		base_sprite.AddComponent("SpriteRenderer");
		object_sprite = new GameObject("object");
		object_sprite.AddComponent("SpriteRenderer");
		doc = new XmlDocument();
		doc.Load(current_level);
		sprites = new List<Sprite>();
		foreach(XmlNode node in doc.GetElementsByTagName("tileset")){
			load_sprites(node.Attributes["name"].Value);
		}
	}

	void DrawBackground(){
		foreach(XmlNode node in doc.GetElementsByTagName("layer")){
			draw_tiles(node);
		}
	}

	void DrawObjects(){
		foreach(XmlNode node in doc.GetElementsByTagName("objectgroup")){
			draw_objects(node);
		}
	}

	void OnGUI()
	{
		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		current_level = EditorGUILayout.TextField ("Current Level", current_level);
		if(GUILayout.Button("Open Tiled File", GUILayout.Width(200))){
			OpenLevel();
		}
		transform = (Transform)EditorGUILayout.ObjectField(transform, typeof(Transform), true);
		if(GUILayout.Button("Create Background", GUILayout.Width(200))){
			DrawBackground();
		}
		if(GUILayout.Button("Create Objects", GUILayout.Width(200))){
			DrawObjects();
		}
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
			myBool = EditorGUILayout.Toggle ("Toggle", myBool);
			myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup ();
	}
}
