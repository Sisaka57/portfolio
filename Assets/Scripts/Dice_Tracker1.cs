using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Dice_Tracker;
using static DiceRoll;
using static Player;
using static show_time_of_dice;
using Random = UnityEngine.Random;

public class Dice_Tracker1 : MonoBehaviour
{
	public static Dice_Tracker1 DiceTracker1Class;
	public LayerMask DieCollider; // layer mask that raycast will detect
	public int diceNumber2; // number of dice
	public Image[] Dince_Monitor = new Image[24]; // which image should show up
	public CanvasGroup D1, D2, D3, D4;
	private Rigidbody _rigidbody;

	[Space] [Header("Dice Sound Effect")] 
	public AudioClip crashhard; 
	public AudioClip crashsoft;

	private  AudioSource Dice_SFX;

	private float lowPitchRange = 1f;
	private float highPitchRange = 1.75f;
	private float velToVol = 0.2f;
	private float velocityClipSplit = 10f;
	
	
	float hitVol;
	public Image dice_panel;
	public Transform shadow; // fake shadow
	public bool DontTrackDiceNumber;
	float force;
	public float gravitydouble;
	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		DiceTracker1Class = this;
		Dice_SFX = GetComponent<AudioSource>();
		DontTrackDiceNumber = false;

		force = 0;

		gravitydouble = 5f;
	}

	void Update()
	{
		//ray cast will notice the specific layer which is "DiceNumber" and will returns the dice number based on number you written in Value_Of_Dice variable
	if (!DontTrackDiceNumber && !_rigidbody.isKinematic && Physics.Raycast(transform.position, Vector3.up, out var hit, Mathf.Infinity, DieCollider))
		{
			
			diceNumber2 = hit.collider.GetComponent<SetDiceValue>().Value_Of_Dice;
			if (_rigidbody.velocity.magnitude < 0.5f)
			{
				PlayerClass.Dice_number[1] = diceNumber2;

				GetComponent<Photon.Pun.PhotonView>().RPC("RemoveSyncingDice", Photon.Pun.RpcTarget.AllViaServer, null);

				foreach (var t in Dince_Monitor)
					t.enabled = false;
				
				D1.alpha = D2.alpha = D3.alpha = D4.alpha = 1f;
			
			}
		}

		if (!DontTrackDiceNumber)
		{
			if (PlayerClass.Dice_number[1] > 0)
			{
				DiceRollClass.Dice2_set = true;
				force = 5f;
			}
		
			if (DiceRollClass.Dice2_set && DiceRollClass.Dice1_set)
			{
				DontTrackDiceNumber = true;
				StartCoroutine(ShowDiceNumber_Icons()); //needs half seconds till dice1 and dice2 digits gets specify
				Physics.gravity = new Vector3(0, -9.81f, 0f);
				DiceRollClass.dice1_rb.Sleep();
				DiceRollClass.dice2_rb.Sleep();
				Time.timeScale = 1f;
			}
			
			
		}
		
		if (PlayerClass.Dice_number[1] == 0)
			shadow.transform.position = new Vector3(transform.position.x,3.55f, transform.position.z); // distance of fake shadow from the dice y: 3.4f
	}


	[Photon.Pun.PunRPC]
	public void RemoveSyncingDice()
	{
		GetComponent<Photon.Pun.PhotonRigidbodyView>().enabled = false;
	}

	IEnumerator ShowDiceNumber_Icons()
	{
		yield return new WaitForSeconds(0.5f); // before was 1f
		if (DiceTrackerClass.diceNumber1 != DiceTracker1Class.diceNumber2)
		{
			dice_panel.rectTransform.sizeDelta = new Vector2(116.6f,30f);
			PlayerClass.opposite_dice = true;
		
			switch (DiceTrackerClass.diceNumber1) //shows the image of specific dice1 number
			{
				case 1:
					for (int i = 0; i < 6; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[0].enabled = true;
					break;

				case 2:
					for (int i = 0; i < 6; i++)
						Dince_Monitor[i].enabled = false;

					Dince_Monitor[1].enabled = true;
					break;

				case 3:
					for (int i = 0; i < 6; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[2].enabled = true;
					break;

				case 4:

					for (int i = 0; i < 6; i++)
						Dince_Monitor[i].enabled = false;
					Dince_Monitor[3].enabled = true;
					break;
				case 5:
					for (int i = 0; i < 6; i++)
						Dince_Monitor[i].enabled = false;
					Dince_Monitor[4].enabled = true;
					break;

				case 6:
					for (int i = 0; i < 6; i++)
						Dince_Monitor[i].enabled = false;
					Dince_Monitor[5].enabled = true;
					break;
			}

			switch (diceNumber2) //shows the image of specific dice2 number
			{

				case 1:
					for (int i = 6; i < 12; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[6].enabled = true;
					break;

				case 2:
					for (int i = 6; i < 12; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[7].enabled = true;
					break;

				case 3:

					for (int i = 6; i < 12; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[8].enabled = true;
					break;

				case 4:

					for (int i = 6; i < 12; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[9].enabled = true;
					break;

				case 5:
					for (int i = 6; i < 12; i++)
						Dince_Monitor[i].enabled = false;
				
					Dince_Monitor[10].enabled = true;
					break;

				case 6:
					for (int i = 6; i < 12; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[11].enabled = true;
					break;
			}
		}
		else
		{
			dice_panel.rectTransform.sizeDelta = new Vector2(116.6f,55f);
			PlayerClass.even_dice = true;
			switch (diceNumber2) //shows the image of specific dice1 number
			{
				case 1:
					for (int i = 12; i < Dince_Monitor.Length; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[0].enabled = true;
					Dince_Monitor[6].enabled = true;
					Dince_Monitor[12].enabled = true;
					Dince_Monitor[18].enabled = true;
					break;

				case 2:
					for (int i = 12; i < Dince_Monitor.Length; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[1].enabled = true;
					Dince_Monitor[7].enabled = true;
					Dince_Monitor[13].enabled = true;
					Dince_Monitor[19].enabled = true;

					break;

				case 3:
					for (int i = 12; i < Dince_Monitor.Length; i++)
						Dince_Monitor[i].enabled = false;

					Dince_Monitor[2].enabled = true;
					Dince_Monitor[8].enabled = true;
					Dince_Monitor[14].enabled = true;
					Dince_Monitor[20].enabled = true;

					break;

				case 4:
					for (int i = 12; i < Dince_Monitor.Length; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[3].enabled = true;
					Dince_Monitor[9].enabled = true;
					Dince_Monitor[15].enabled = true;
					Dince_Monitor[21].enabled = true;
					break;

				case 5:
					for (int i = 12; i < Dince_Monitor.Length; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[4].enabled = true;
					Dince_Monitor[10].enabled = true;
					Dince_Monitor[16].enabled = true;
					Dince_Monitor[22].enabled = true;
					break;
				case 6:
					for (int i = 12; i < Dince_Monitor.Length; i++)
						Dince_Monitor[i].enabled = false;
					
					Dince_Monitor[5].enabled = true;
					Dince_Monitor[11].enabled = true;
					Dince_Monitor[17].enabled = true;
					Dince_Monitor[23].enabled = true;
					break;
			}

		}
		
		if (ShowTimeOfDice.PlayerDetectedAtFrstRoll)
			StartCoroutine(PlayerClass.SwitchPlayer()); //calls
		
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.collider.CompareTag("Board"))
		{
			if (PlayerPrefs.GetInt("sound") == 1)
			{
				Dice_SFX.pitch = Random.Range(lowPitchRange, highPitchRange);
				hitVol = other.relativeVelocity.magnitude * velToVol;

				Dice_SFX.PlayOneShot(other.relativeVelocity.magnitude < velocityClipSplit ? crashsoft : crashhard, hitVol);
			}
		
			DiceRollClass.StopRolling = false;
			StartCoroutine(MakeDiceHeavy());
		}
	
		if (other.collider.CompareTag("wall"))
		{
			if (other.collider.name != "Middle_Wall")
			{
				_rigidbody.velocity = Vector3.zero;
				_rigidbody.angularDrag = 0.5f;
				other.collider.GetComponent<BoxCollider>().enabled = false; // makes collided wall disable till dice have more potential for rolling
			}
		
		}
	
	}

	   IEnumerator MakeDiceHeavy()
	   {
		  yield return new WaitForSecondsRealtime(0.5f);
		   Physics.gravity = new Vector3(0, -9.81f * gravitydouble, 0); // makes dice stop at normal time
	
		   yield return new WaitForSecondsRealtime(0.5f);
		   _rigidbody.velocity = Vector3.zero;
	   }
	   
	private void OnCollisionStay(Collision other)
	{
		if (other.collider.CompareTag("Dice") && PlayerClass.Dice_number[1] == 0 | PlayerClass.Dice_number[0] == 0)
		{
			force += 0.04f;
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





