using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
	public static List<Color> colorBalls;
	public static List<Material> materialBalls;// = new Material(sourceMaterial);
	public static List<Color> colorWalls;
	public static List<Material> materialWalls;// = new Material(sourceMaterial);

	private GameObject player;
	private GameObject wall;

	public void Start() {
		player = GameObject.Find("Sphere");
		wall = GameObject.Find("lvl2");
		Material sourceMaterial;

        sourceMaterial = player.GetComponent<MeshRenderer>().material;
		
		foreach (var c in colorBalls)
		{
			Material m = new Material(sourceMaterial);
			m.color = c;
			materialBalls.Add(m);
		}

		player.GetComponent<MeshRenderer>().material = materialBalls[1];
		player.GetComponent<TrailRenderer>().materials[0].color = materialBalls[1].color;
		wall.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = colorWalls[1];
	}

	public void setColorTo(GameObject other) {
		int r = (int)Mathf.Lerp(1, materialBalls.Count, Random.value);
		other.GetComponent<MeshRenderer>().material = materialBalls[r];
		other.GetComponent<TrailRenderer>().materials[0].color = materialBalls[r].color;
	}
}
