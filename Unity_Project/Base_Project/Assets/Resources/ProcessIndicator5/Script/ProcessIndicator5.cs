using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProcessIndicator5 : MonoBehaviour {

	[Range(1,10)]
	public int animationSpeed;
	public bool showAllCircles;
	
	private int frame;
	private int counter;
	private const int FrameMax = 16;
	
	private Texture2D[] texture;
	private RawImage img;

	// Use this for initialization
	void Start () {
		frame = 0;
		counter = 0;
		
		img = (RawImage)GetComponent<RawImage>();
		
		texture = new Texture2D[FrameMax];

		if (showAllCircles) {
			texture[0] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img1");
			texture[1] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img2");
			texture[2] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img3");
			texture[3] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img4");
			texture[4] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img5");
			texture[5] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img6");
			texture[6] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img7");
			texture[7] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img8");
			texture[8] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img9");
			texture[9] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img10");
			texture[10] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img11");
			texture[11] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img12");
			texture[12] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img13");
			texture[13] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img14");
			texture[14] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img15");
			texture[15] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img16");
		}
		else {
			texture[0] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img1b");
			texture[1] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img2b");
			texture[2] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img3b");
			texture[3] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img4b");
			texture[4] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img5b");
			texture[5] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img6b");
			texture[6] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img7b");
			texture[7] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img8b");
			texture[8] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img9b");
			texture[9] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img10b");
			texture[10] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img11b");
			texture[11] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img12b");
			texture[12] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img13b");
			texture[13] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img14b");
			texture[14] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img15b");
			texture[15] = (Texture2D)Resources.Load("ProcessIndicator5/Image/img16b");
		}
	}
	
	// Update is called once per frame
	void Update () {
		counter++;
		if (counter == animationSpeed) {
			counter = 0;
			frame++;
			if (frame == FrameMax) {
				frame = 0;
			}
			//Change Texture of the GameObject
			img.texture  = texture[frame];
		}
	}

}
