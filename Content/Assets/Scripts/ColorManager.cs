using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
	public List<Color> colorBalls;
	public List<Material> materialBalls;// = new Material(sourceMaterial);
	public List<Color> colorWalls;
	public List<Material> materialWalls;// = new Material(sourceMaterial);

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

		player.GetComponent<MeshRenderer>().material = materialBalls[0];
		player.GetComponent<TrailRenderer>().materials[0].color = materialBalls[0].color;
		wall.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = colorWalls[0];
	}

	public void setColorTo(GameObject other) {
		int r = (int)Mathf.Lerp(1, materialBalls.Count, Random.value);
		other.GetComponent<MeshRenderer>().material = materialBalls[r];
		other.GetComponent<TrailRenderer>().materials[0].color = materialBalls[r].color;
	}
}
