using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*******************************************************
 * Class:           AIConeDetection
 * Description:     Description begin here
 * 
 * Studio Leaves (c) 
 *******************************************************/
[RequireComponent(typeof(CharacterController))]
public class RandomBehavior : MonoBehaviour 
{
    /* Fov Properties */
	private float 		 m_fConeLenght 	              = 5.0f;
	private float 		 m_fAngleOfView	              = 90.0f;
	private float        m_vStartDistanceCone         = 2.0f;
    public  Material     m_matVisibilityCone          = null;
    public  bool 		 m_bHasStartDistance          = true;
	public  LayerMask	 notIgnoreLayermask;
    //public  float       m_fFixedCheckInterval       = 0.5f;
    private float        m_fFixedCheckNexinterpolantForLerpime;

    /* Render Properties */
    public  bool         m_bShowCone                 = true;
	private int   		 m_iConeVisibilityPrecision  = 3;
    //public  float       m_fDistanceForRender        = 600.0f;

    private Mesh         m_mConeMesh;
    private Vector3[]    m_vVertices;
    private Vector2[]    m_vUV;
    private Vector3[]    m_vNormals;
    private int[]	     m_iTriangles;
    private GameObject   m_goVisibilityCone          = null;
    private int 		 m_iVertMax                  = 120;
    private int			 m_iTrianglesMax             = 120;

    private float 		 m_fSpan;
    private float 		 m_fStartRadians;
    private float 		 m_fCurrentRadians;
    //private float 		m_fConeLenghtFixed;
	[HideInInspector]
	public  bool 		 drawCone					 = false;
	private bool		 initialized				 = false;
   
	private ArrayList   m_goGameObjectIntoCone = new ArrayList();
	public  ArrayList   GameObjectIntoCone;


	//test move
	private List<Vector3>    freeDirections;
	public float speed = 15;
	private float maxHeadingChange ;

	CharacterController controller;
	float heading;
	Vector3 targetRotation;
	float changedir ;

	int countForLerp;
	float interpolantForLerp;

	private enum State
	{
		RandomWalk,
		Idle
	}
	private State state;
	private int randomState;

	public float lifeTime = 100.0f;
	private float lT;
	public float changeStateTime = 10.0f;
	private float cST;


	public float angleCone = 120.0f;
	public float startDistCone = 1.0f;
	public float lengthCone = 15.0f;
	//xrisimopoiite gia na psaxni na vrei prota tes pio kontines gonies(gia peristrofi) ke meta tis pio makrines
	public int closeAngle = 2 ;
	public float directionChangeInterval = 1;

	private Animator animator;
	
	private bool changeState;

	//private float distanceToStopMove = 1.5f;
	
	//private Animation animationComponent;

	//public AnimationClip[] IdleAnimations;
	//public AnimationClip[] WalkAnimations;


	void Start () 
	{
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();
		//animationComponent = GetComponent<Animation> ();

		//randomState = Random.Range (0,3);

		//if (randomState == 0) {
		//	state = State.Idle;
		//} 
		//else {
			state = State.RandomWalk;
		//}

		//Debug.Log(state);
		cST = Time.time;

		// Set random initial rotation
		heading = Random.Range(0, 360);
		transform.eulerAngles = new Vector3(0, heading, 0);
		changedir = Time.time;
		setAIConeDetection (angleCone, startDistCone, lengthCone, 50);
		countForLerp = 0;

		changeState = true;

		lT = Time.time;
	}
	
	void Update () 
	{
		if (Time.time - lT >= lifeTime) {

			Destroy(GameObject.Find(this.name + "_VisConeMesh"));
			Destroy(this.gameObject);
		}

		//if (!controller.isGrounded) {
		//	this.transform.position =new Vector3 (this.transform.position.x ,this.transform.position.y - 0.01f, this.transform.position.z);
		//}

		if(drawCone)
		{
			if(!initialized)
			{
				InitAIConeDetection();
				initialized = true;
			}

        	UpdateAIConeDetection();
		}

		//change state
		if (Time.time - cST >= changeStateTime) {
			cST = Time.time;
			randomState = Random.Range (0,3);
			
			if (randomState == 0) {
				state = State.Idle;
			} 
			else if ( randomState == 2 ){
				state = State.RandomWalk;
			}

			changeState = true;
		} 

		if (state == State.Idle) {
			Idle();
		} 
		else if (state == State.RandomWalk) {
			RandomWalk ();
		}

		changeState = false;
	}

	private void Idle(){

		if (changeState) {
			int idleX;
			int idleY;
			do {
				idleX = Random.Range (1, 4) - 2;
				idleY = Random.Range (1, 4) - 2;
			} while((idleX == 0) || (idleY == 0));
		

			animator.SetFloat ("Idle-Walk", 0f);
			animator.SetFloat ("IdleX", idleX);
			animator.SetFloat ("IdleY", idleY);
		}

	}


	private void RandomWalk(){

		if (changeState) {
			int walk = Random.Range (1, 3) - 1;

			animator.SetFloat ("Idle-Walk", 1f);
			animator.SetFloat ("Walk", walk);
		}

		if (Time.time - changedir >= directionChangeInterval) {
			targetRotation = NewHeadingRoutine ();
			//changedir =false;
			changedir = Time.time;
			countForLerp = Mathf.RoundToInt(directionChangeInterval/Time.deltaTime);
			interpolantForLerp = 1.0f/countForLerp;
		} 
		else {
			if (countForLerp<0)
				targetRotation = new Vector3(0f,0f,0f);
			else
				countForLerp--;
		}
		
		transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + targetRotation, interpolantForLerp);
		transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y ,0f);
		var forward = transform.TransformDirection(Vector3.forward);

		//Vector3 startPos = (this.transform.position + transform.forward * m_vStartDistanceCone) + new Vector3 (0f, 0.2f, 0f);
		//Vector3 directionPos = (startPos + transform.forward) + new Vector3 (0f, 0.2f, 0f);

		//if (!Physics.Raycast (startPos,directionPos, distanceToStopMove)) {
			controller.SimpleMove (forward * speed);
		//} else {
		//	Debug.DrawLine(startPos, directionPos, Color.black);
		//}


	}
	
	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	private Vector3 NewHeadingRoutine ()
	{

		if (freeDirections.Count > 0) {
			List<Vector3> temp ;
			int leptomeria = closeAngle;
			float ang = angleCone / leptomeria ;

			for(int j=1;j<=leptomeria;j++){
				temp = new List<Vector3>(freeDirections);
				while(temp.Count>0){
					int i;
					i = Random.Range (0, temp.Count);

					float tempang = Vector3.Angle (this.transform.forward * 10.0f,   temp[i] );
					float sign = Mathf.Sign(Vector3.Dot(this.transform.forward * 10.0f, Vector3.Cross(Vector3.up, temp[i]) ));
					float finalAngle = sign * tempang;

					if ( Mathf.Abs(finalAngle) <= (ang*j)){

						//Debug.DrawLine(this.transform.position ,(this.transform.position+this.transform.forward * 10.0f ),Color.white,1f);
						//Debug.DrawLine(this.transform.position ,(temp[i] + this.transform.position),Color.blue,1f);
						//Debug.Log("f : " + finalAngle + " t : " +tempang + " s : " +sign );
						return  (new Vector3 (0, finalAngle * (-1), 0));
					}
					else{
						//Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! : " + finalAngle);
						temp.RemoveAt(i);
					}
				}
			}
			//Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
			return (new Vector3 (0, 180.0f , 0));
		} 
		else {
			//Debug.Log("****************************");
			return (new Vector3 (0, 180.0f , 0));
		}
	}	
	
	
	public void setAIConeDetection(float coneAngle, float startDist, float coneLength, int quality = 50)
	{
		m_fAngleOfView = coneAngle;
		maxHeadingChange = coneAngle;
		m_vStartDistanceCone = startDist;
		m_fConeLenght = coneLength;
		m_iConeVisibilityPrecision = quality;
		drawCone = true;
	}

    private void InitAIConeDetection() 
	{
        m_goGameObjectIntoCone  = new ArrayList();
        m_goVisibilityCone      = GameObject.CreatePrimitive( PrimitiveType.Cube );
        Component.Destroy( m_goVisibilityCone.GetComponent<BoxCollider>() );

        m_goVisibilityCone.name                             = this.name + "_VisConeMesh";
		m_goVisibilityCone.transform.parent = transform.parent;
        m_mConeMesh                                         = new Mesh();
        m_goVisibilityCone.GetComponent<MeshFilter>().mesh  = m_mConeMesh;

        m_iVertMax       = m_iConeVisibilityPrecision * 2 + 2;
        m_iTrianglesMax  = m_iConeVisibilityPrecision * 2;

        m_vVertices     = new Vector3[  m_iVertMax          ];
        m_iTriangles    = new int    [  m_iTrianglesMax * 3 ];
        m_vNormals      = new Vector3[  m_iVertMax          ];

        m_mConeMesh.vertices  = m_vVertices;
        m_mConeMesh.triangles = m_iTriangles;
        m_mConeMesh.normals   = m_vNormals;

        m_vUV           = new Vector2[ m_mConeMesh.vertices.Length ];
        m_mConeMesh.uv  = m_vUV;

        m_goVisibilityCone.GetComponent<Renderer>().material = m_matVisibilityCone;

        for ( int i = 0; i < m_iVertMax; ++i ) 
		{
            m_vNormals[ i ] = Vector3.up;
        }

        m_fStartRadians     = ( 360.0f - ( m_fAngleOfView ) ) * Mathf.Deg2Rad;
        m_fCurrentRadians   = m_fStartRadians;
        m_fSpan             = ( m_fAngleOfView ) / m_iConeVisibilityPrecision;
        m_fSpan             *= Mathf.Deg2Rad;
        m_fSpan             *= 2.0f;
        m_fAngleOfView      *= 0.5f;
        //m_fConeLenghtFixed  = m_fConeLenght * m_fConeLenght;
    }

    private void UpdateAIConeDetection() 
	{
        DrawVisibilityCone2();
    }

    public void DisableCone() 
	{
        m_mConeMesh.Clear();
    }

    private RaycastHit  m_rcInfo;
    private Ray         m_rayDir = new Ray();

    private void DrawVisibilityCone2() 
	{
        m_goGameObjectIntoCone.Clear();
        m_fCurrentRadians           = m_fStartRadians;
        Vector3 CurrentVector 		= this.transform.forward;
        Vector3 DrawVectorCurrent 	= this.transform.forward;

		//
		freeDirections = new List<Vector3>();

        int index = 0;
        for ( int i = 0; i < m_iConeVisibilityPrecision + 1; ++i ) 
		{
            float newX = CurrentVector.x * Mathf.Cos( m_fCurrentRadians ) - CurrentVector.z * Mathf.Sin( m_fCurrentRadians );
            float newZ = CurrentVector.x * Mathf.Sin( m_fCurrentRadians ) + CurrentVector.z * Mathf.Cos( m_fCurrentRadians );
            float newY = CurrentVector.y * Mathf.Sin( m_fCurrentRadians ) + CurrentVector.z * Mathf.Cos( m_fCurrentRadians );

            DrawVectorCurrent.x = newX;
			DrawVectorCurrent.y = 0.05f;
            DrawVectorCurrent.z = newZ;

            //float Angle       = 90.0f;
            //DrawVectorCurrent = Quaternion.Euler( 0.0f, 0.0f, Angle ) * DrawVectorCurrent;

            m_fCurrentRadians += m_fSpan;

            /* Calcoliamo dove arriva il Ray */
            float FixedLenght = m_fConeLenght;
            bool  bFoundWall  = false;
            /* AdainterpolantForLerpiamo la mesh alla superfice sulla quale tocca */
            
			m_rayDir.origin = (this.transform.position + DrawVectorCurrent.normalized * m_vStartDistanceCone) + new Vector3(0f,0.2f,0f);
            //**m_rayDir.origin = this.transform.position;
            m_rayDir.direction = DrawVectorCurrent.normalized;
			if ( Physics.Raycast( m_rayDir, out m_rcInfo, Mathf.Infinity, notIgnoreLayermask ) ) 
			{
                if ( m_rcInfo.distance < m_fConeLenght ) 
				{
                    bFoundWall = true;
                    FixedLenght = m_rcInfo.distance;

                    bool bGOFound = false;
                    foreach ( GameObject go in m_goGameObjectIntoCone ) 
					{
                        if ( go.GetInstanceID() == m_rcInfo.collider.gameObject.GetInstanceID() ) 
						{
                            bGOFound = true;
                            break;
                        } 
                    }
                    if ( !bGOFound ) 
					{
                        m_goGameObjectIntoCone.Add( m_rcInfo.collider.gameObject );

                    }
                }
            }

			GameObjectIntoCone = m_goGameObjectIntoCone;
            
            if ( m_bHasStartDistance ) 
			{
				m_vVertices[ index ] = m_rayDir.origin ;//this.transform.position + DrawVectorCurrent.normalized * m_vStartDistanceCone;
            }
            else 
			{
                m_vVertices[ index ] = this.transform.position;
            }

            m_vVertices[ index + 1 ]    = this.transform.position + DrawVectorCurrent.normalized * FixedLenght;
            //m_vVertices[ index + 1 ].y  = this.transform.position.y;

            Color color;
            if ( bFoundWall )
			{
                color = Color.red;
			}
            else
			{
                color = Color.yellow;
				//direction without "wall" from the player's position to the target's.
				freeDirections.Add( m_vVertices[ index + 1 ] -this.transform.position );
			}
			

			Debug.DrawLine( m_rayDir.origin/*this.transform.position + DrawVectorCurrent.normalized * m_vStartDistanceCone*/, m_vVertices[ index + 1 ], color );
			//**Debug.DrawLine( this.transform.position, m_vVertices[ index + 1 ], color );
            index += 2;
        }

        if ( m_bShowCone ) 
		{
            int localIndex = 0;
            for ( int j = 0; j < m_iTrianglesMax * 3; j = j + 6 ) {
                m_iTriangles[ j     ] = localIndex;
                m_iTriangles[ j + 1 ] = localIndex + 3;
                m_iTriangles[ j + 2 ] = localIndex + 1;

                m_iTriangles[ j + 3 ] = localIndex + 2;
                m_iTriangles[ j + 4 ] = localIndex + 3;
                m_iTriangles[ j + 5 ] = localIndex;

                localIndex += 2;
            }

            m_mConeMesh.Clear();
            m_mConeMesh.vertices  = m_vVertices;
            m_mConeMesh.triangles = m_iTriangles;
            m_mConeMesh.normals   = m_vNormals;
            m_mConeMesh.RecalculateNormals();
            m_mConeMesh.Optimize();
        }
        else 
		{
            m_mConeMesh.Clear();
        }
        
    }
}
