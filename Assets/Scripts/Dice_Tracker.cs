using System.Collections;
using UnityEngine;
using static Player;
public class Dice_Tracker : MonoBehaviour 

{
	public static Dice_Tracker DiceTrackerClass;
	public LayerMask DieCollider ; // layer numbe
	public int diceNumber1; // dice number
	public Rigidbody _rigidbody; // rigidbody component

	[Space][Header("Dice Sound Effect")]
	public AudioClip crashhard; // hard impact of dice wih surface
	public AudioClip crashsoft; // soft impact of dice wih surface
	private  AudioSource Dice_SFX; // audio source

	private float lowPitchRange = 1f; // pitch of sound
	private float highPitchRange = 1.75f;
	private float velToVol = 0.2f; // velocity of sound
	private float velocityClipSplit = 10f; // amount of sound velocity
	float hitVol ;
	public Transform shadow; // fake shadow
	float force; // force that apart dice from each other

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody> ();
		
		DiceTrackerClass = this;
		Dice_SFX = GetComponent<AudioSource>();
		force = 0;
	}

	void Update()
	{
		//after applying gravity to dice ray cast will specifying dice number 
		if (PlayerClass.Dice_number[0] == 0 && !_rigidbody.isKinematic && Physics.Raycast(transform.position, Vector3.up, out var hit, Mathf.Infinity, DieCollider))
		{
			diceNumber1 = hit.collider.GetComponent<SetDiceValue>().Value_Of_Dice; // ray cast will hit with GameObjects that has "number_of_dice" layer

			if (_rigidbody.velocity.magnitude < 0.5f) // if dice no longer moves
            {
				PlayerClass.Dice_number[0] = diceNumber1;
				GetComponent<Photon.Pun.PhotonView>().RPC("RemoveSyncingDice", Photon.Pun.RpcTarget.AllViaServer, null);
			}
				

			if (PlayerClass.Dice_number[0] > 0) // means dice number specified
			{
				DiceRoll.DiceRollClass.Dice1_set = true;
				force = 5f;
			}
		
		}

		if (PlayerClass.Dice_number[0] == 0)
			shadow.transform.position = new Vector3(transform.position.x,3.55f, transform.position.z); //synchronize dice shadow

	}

	[Photon.Pun.PunRPC]
	public void RemoveSyncingDice()
    {
		GetComponent<Photon.Pun.PhotonRigidbodyView>().enabled = false;
    }

	private void OnCollisionEnter(Collision other)
	{
		if (other.collider.CompareTag("Board")) // applies sound effect 
		{
			if (PlayerPrefs.GetInt("sound") == 1)
			{
				Dice_SFX.pitch = Random.Range(lowPitchRange, highPitchRange);
				hitVol = other.relativeVelocity.magnitude * velToVol;

				Dice_SFX.PlayOneShot(other.relativeVelocity.magnitude < velocityClipSplit ? crashsoft : crashhard, hitVol);
			}
		

			StartCoroutine(ForceToSleep()); // after a few sec checks dice is in normal position or not
		}
	
		if (other.collider.CompareTag("wall")) // if dice collide with wall with wall tags the wall the collide of wall will be disable that dice able to keep moving
		{
			if (other.collider.name != "Middle_Wall")
			{
				_rigidbody.velocity = Vector3.zero;
				other.collider.GetComponent<BoxCollider>().enabled = false; // makes collided wall disable till dice have more potential for rolling
			}
	
		}

	}
	
	IEnumerator ForceToSleep()
	{
		yield return new WaitForSecondsRealtime(1f);
		_rigidbody.velocity = Vector3.zero;
		//_rigidbody.Sleep();


	}
	private void OnCollisionStay(Collision other)
	{
		if (other.collider.CompareTag("Dice") && PlayerClass.Dice_number[1] == 0 | PlayerClass.Dice_number[0] == 0)
		{
			force += 0.03f;
			Vector3 forward = transform.TransformDirection(Vector3.back);
			Vector3 right = transform.TransformDirection(Vector3.right);
			Vector3 toOther = other.transform.position - transform.position;

			if (Vector3.Dot(forward, toOther) < 0)
				_rigidbody.AddForce(-_rigidbody.transform.forward * force,ForceMode.Force);
			else
				_rigidbody.AddForce(_rigidbody.transform.forward * force,ForceMode.Force);
			
			if (Vector3.Dot(right, toOther) < 0)
				_rigidbody.AddForce(-_rigidbody.transform.right * force,ForceMode.Force);
			else
				_rigidbody.AddForce(_rigidbody.transform.right * force,ForceMode.Force);
			
			
		}

	}


}
