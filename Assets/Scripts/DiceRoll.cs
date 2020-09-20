using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Board;
using static DataManager;
using static Player;
using static Robot;
using static show_time_of_dice;
using Random = UnityEngine.Random;

public class DiceRoll : MonoBehaviour

{
    public Rigidbody dice1_rb,dice2_rb; //dice Rigidbody
     public ForceMode _forceMode; // type of force
     [Range(0,2000)]public float diceSpeed; // applies speed to dice 
     public Transform Dice1_target,Dice2_target; // the position that dice should reach
     public Renderer Dice1_rend, Dice2_rend;
     [SerializeField]
     private Vector3 Dice1_OffSet,Dice2_OffSet; // offset of target.position - transform.position
     public bool StopRolling; // allow dice to rotation 
     public bool active_show_time; // allow to show the result of dice roll
     private Vector3 Initial_Dice1_target,Initial_Dice2_target,P2Dice1Pos, P2Dice2Pos,P1Dice1Pos, P1Dice2Pos , Dice1_New_Target,Dice2_New_Target;
     [Space]
     public Animator msgbox; // notification messenger animator 
     public Button dice_roll_btn; // dice roll button
     public Text RollDice_msg;
     public GameObject dice1RemoveDiceAlarm, dice2RemoveDiceAlarm;
     public GameObject wall_d1, wall_d2;
     public ParticleSystem particleRemoveDice1Alarm, particleRemoveDice2Alarm;
     private Camera _camera;
     public static DiceRoll DiceRollClass;
     private GameObject XForm;
     private GameObject XForm_out;
     private GameObject XForm_Win;
     public bool Dice1_set, Dice2_set;
     private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    bool allowTappingAgain = true;

     //Attention : if you have any questions feel free to ask. best way to contact is to email us, we will answer and support you for sure. the email address mentioned in the manual.pdf
     
     private void Start()
     {
         XForm_Win = GameObject.Find("XForm_BearOut");
         XForm_out = GameObject.Find("XForm_Burn");
         XForm = GameObject.Find("XForm");

         DiceRollClass = this;
         active_show_time = false;
         StopRolling = true;
        
         StartCoroutine(show_mgsbox());
         _camera = Camera.main;

         //turn of player 1
         P1Dice1Pos = new Vector3(-10f, 1.8f, 24f); // the coordinate that dice will toss on the board (Dice1)
         P1Dice2Pos = new Vector3(-7.5f, 1.8f, 24f); // the coordinate that dice will toss on the board (Dice2)

         P2Dice1Pos = new Vector3(7.5f, 1.8f, -4f);
         P2Dice2Pos = new Vector3(10f, 1.8f, -4f);

         Initial_Dice1_target = Dice1_target.position;
         Initial_Dice2_target = Dice2_target.position;
         
         
         Dice1_New_Target = new Vector3(7.5f, 0f, 8f); // the coordinate that dice will landing on it
         Dice2_New_Target = new Vector3(-7.5f, 0f, 8f);

         if (PlayerPrefs.GetInt("skins") == 1 | PlayerPrefs.GetInt("skins") == 3)
         {
             dice1_rb.GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 0.7f);
             dice1_rb.GetComponent<Renderer>().materials[1].color = Color.black;
           
             dice2_rb.GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 0.7f);
             dice2_rb.GetComponent<Renderer>().materials[1].color = Color.black;
             
             dice1_rb.GetComponent<Renderer>().materials[0].SetColor(EmissionColor,new Color(0.46f, 0.46f, 0.34f));
             dice1_rb.GetComponent<Renderer>().materials[1].SetColor(EmissionColor,Color.black);
             
             dice2_rb.GetComponent<Renderer>().materials[0].SetColor(EmissionColor,new Color(0.46f, 0.46f, 0.34f));
             dice2_rb.GetComponent<Renderer>().materials[1].SetColor(EmissionColor,Color.black);
         }
         
         diceSpeed = 650f; // dice tossing speed. this value should not be more than 1000
         
         Physics.gravity = new Vector3(0, -9.81f, 0); // reset gravity 
   
     }

     private void Update()
     {
         if (ShowTimeOfDice.enabled && !StopRolling && dice1_rb.velocity.magnitude < 0.5f && dice2_rb.velocity.magnitude < 0.5f )
             active_show_time = true; // checks if dice not rolling anymore
        

         if (Input.GetMouseButtonDown(0) )
         {
            Debug.Log("inside here ");
            
             var ray = _camera.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out var hit) && hit.collider.CompareTag("Dice") && PlayerClass.removeDice && Player.PlayerClass.player_number == Player.PlayerClass.myPlayerNumber && allowTappingAgain)
            {
                allowTappingAgain = false;

                if (PlayerPrefs.GetInt("mode") == 3)
                {
                    GetComponent<Photon.Pun.PhotonView>().RPC("ResetDataNetwork", Photon.Pun.RpcTarget.AllViaServer, null);
                }
                else {
                    ResetAllData();
                }
                
            }
                
         }
     }

    [Photon.Pun.PunRPC]
    void ResetDataNetwork()
    {
        ResetAllData();
    }



     public void Roll_Dice_btn() // roll dice button
     {
        if (PlayerPrefs.GetInt("mode") != 3)
        {
            Time.timeScale = 3f;
            dice1_rb.isKinematic = dice2_rb.isKinematic = false;

            dice1_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            dice2_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            dice_roll_btn.gameObject.SetActive(false);

            if (ShowTimeOfDice.enabled)
            {
                ShowTimeOfDice.stopDicesFromRolling = false;
                StartCoroutine(Close_msg());
            }
            else
                StopRolling = true;


            dice1_rb.transform.rotation = Random.rotation;
            dice2_rb.transform.rotation = Random.rotation;


            Dice1_OffSet = Dice1_target.position - dice1_rb.position; // offset (target placed in dice wall)
            Dice2_OffSet = Dice2_target.position - dice2_rb.position; // offset (target placed in dice wall1)

            dice1_rb.velocity = Vector3.up * 2f;
            dice2_rb.velocity = Vector3.up * 2f;

            dice1_rb.AddForceAtPosition(Dice1_OffSet.normalized * diceSpeed, dice1_rb.position,
                _forceMode); // Applies force at position. As a result this will apply a torque and force on object.

            dice2_rb.AddForceAtPosition(Dice2_OffSet.normalized * diceSpeed, dice2_rb.position,
                _forceMode); // Applies force at position. As a result this will apply a torque and force on object. 

            Dice1_rend.enabled = Dice2_rend.enabled = true;

            DataManagerClass.btnSnd.Play();
        }
        else
        {
            //Playing online
            GetComponent<Photon.Pun.PhotonView>().RPC( "Roll_Dice_btn_Online",Photon.Pun.RpcTarget.AllViaServer,null);
        }
         
     }

    [Photon.Pun.PunRPC]
    public void Roll_Dice_btn_Online()
    {
        Time.timeScale = 3f;
        dice1_rb.isKinematic = dice2_rb.isKinematic = false;

        dice1_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        dice2_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        dice1_rb.GetComponent<Photon.Pun.PhotonView>().enabled = true;
        dice2_rb.GetComponent<Photon.Pun.PhotonView>().enabled = true;

        dice_roll_btn.gameObject.SetActive(false);

        if (ShowTimeOfDice.enabled)
        {
            ShowTimeOfDice.stopDicesFromRolling = false;
            StartCoroutine(Close_msg());
        }
        else
            StopRolling = true;


        dice1_rb.transform.rotation = Random.rotation;
        dice2_rb.transform.rotation = Random.rotation;


        Dice1_OffSet = Dice1_target.position - dice1_rb.position; // offset (target placed in dice wall)
        Dice2_OffSet = Dice2_target.position - dice2_rb.position; // offset (target placed in dice wall1)

        dice1_rb.velocity = Vector3.up * 2f;
        dice2_rb.velocity = Vector3.up * 2f;
        
            dice1_rb.AddForceAtPosition(Dice1_OffSet.normalized * diceSpeed, dice1_rb.position,
            _forceMode); // Applies force at position. As a result this will apply a torque and force on object.

            dice2_rb.AddForceAtPosition(Dice2_OffSet.normalized * diceSpeed, dice2_rb.position,
                _forceMode); // Applies force at position. As a result this will apply a torque and force on object. 

        

        Dice1_rend.enabled = Dice2_rend.enabled = true;

        DataManagerClass.btnSnd.Play();
    }

    void PushDice()
    {


    }

     IEnumerator show_mgsbox()
     {
         yield return new WaitForSecondsRealtime(1f);
         msgbox.Play("PopUp_note"); // plays anim of notification 
         RollDice_msg.gameObject.SetActive(true);
     }

     private IEnumerator Close_msg()
     {
         msgbox.Play("PopUp_note_close");

         yield return new WaitForSecondsRealtime(1f);
         for (int i = 0; i < msgbox.gameObject.transform.childCount; i++)
             msgbox.gameObject.transform.GetChild(i).gameObject.SetActive(false);
     }

     public void close_msg_via_button() //circle exit button : close the notification
     {
         msgbox.Play("PopUp_note_close");
         msgbox.gameObject.transform.GetChild(1).gameObject.SetActive(false);
         Invoke(nameof(deactivate_msg), 2f);
         DataManagerClass.btnSnd.Play();
     }

     void deactivate_msg()
     {
         msgbox.gameObject.SetActive(false);
     }

     public void ResetAllData()
     {
        allowTappingAgain = true;
         Physics.gravity = new Vector3(0, -9.81f, 0);
         Dice1_rend.enabled = Dice2_rend.enabled = false;
         PlayerClass.removeDice = Dice1_set = Dice2_set = false;
         DataManagerClass.Undo.gameObject.SetActive(false);
         BoardClass.coin_auto_step = BoardClass.Coin_step_by_step = false;
         Dice_Tracker1.DiceTracker1Class.DontTrackDiceNumber = false;

         for (int i = 0; i < PlayerClass.Dice_number.Length; i++) // Numbers of dice_number array need to be zero
             PlayerClass.Dice_number[i] = 0;

         dice1_rb.transform.rotation = Random.rotation;
         dice2_rb.transform.rotation = Random.rotation;
         PlayerClass.player_number = PlayerClass.player_number == 1 ? 2 : 1;

        dice1_rb.GetComponent<Photon.Pun.PhotonRigidbodyView>().enabled = true;
        dice2_rb.GetComponent<Photon.Pun.PhotonRigidbodyView>().enabled = true;

        if (PlayerClass.player_number == 1)
         {
             dice1_rb.position = P1Dice1Pos;
             dice2_rb.position = P1Dice2Pos;
             
             Dice1_target.position = Initial_Dice1_target;
             Dice2_target.position = Dice2_New_Target;

             BoardClass.WhiteCoinsContainer[0].GetComponent<MeshRenderer>().materials[0].renderQueue = 3000;
             BoardClass.BlackCoinsContainer[0].GetComponent<MeshRenderer>().materials[0].renderQueue = 2450;
         }
         else
         {
             dice1_rb.position = P2Dice1Pos;
             dice2_rb.position = P2Dice2Pos;
         
             Dice1_target.position = Dice1_New_Target;
             Dice2_target.position = Initial_Dice2_target;
             
             BoardClass.WhiteCoinsContainer[0].GetComponent<MeshRenderer>().materials[0].renderQueue = 2450;
             BoardClass.BlackCoinsContainer[0].GetComponent<MeshRenderer>().materials[0].renderQueue = 3000;
     
         }

         for (int i = 0; i < wall_d1.transform.childCount; i++)
         {
             wall_d1.transform.GetChild(i).GetComponent<BoxCollider>().enabled = true;
             wall_d2.transform.GetChild(i).GetComponent<BoxCollider>().enabled = true;
         }

         dice1_rb.angularDrag = 0.05f;
         dice2_rb.angularDrag = 0.05f;
         PlayerClass.doubleCube.layer = 2;

         //*** Undo
         PlayerClass.RListDeliverTower.Clear();
         PlayerClass.RListSelectedTower.Clear();
         PlayerClass.RListBurnTower.Clear();
         PlayerClass.RListBeardOutTower.Clear();
         PlayerClass.UndoMode.Clear();
         //***

          PlayerClass.even_dice = PlayerClass.opposite_dice = false;
         RobotClass.think = RobotClass.SameTower = PlayerClass.playerHasBurntCoin = BoardClass.CoinDefaultColor = false;
         RobotClass.Even_Burn_Activated = RobotClass.active_sgl_move = false; 
         PlayerClass.even_dice = PlayerClass.opposite_dice = false;
         RobotClass.deactive_all = 0;
         
         PlayerClass.diceChecker = 0;

         for (int i = 0; i < 4; i++)
             PlayerClass.DiceCheck[i] = false;

         if (PlayerClass.Container_Of_BC_burnt[0] == null)
             PlayerClass.Num_Of_BC_Burnt = 0;

         if (PlayerClass.Container_Of_WC_burnt[0] == null)
             PlayerClass.Num_Of_WC_Burnt = 0;

         PlayerClass.pauseTime = BoardClass.Count_WC = 0;

         dice1RemoveDiceAlarm.SetActive(false);
         dice2RemoveDiceAlarm.SetActive(false);

         particleRemoveDice1Alarm.Stop();
         particleRemoveDice2Alarm.Stop();

         for (int i1 = 0; i1 < 15; i1++)
             BoardClass.evenDiceCoinIndex[i1] = 0;

         BoardClass.Free_All_legal_Towers = false;

         PlayerClass.xcoin_point = XForm.transform;
         PlayerClass.xcoin_burn = XForm_out.transform;
         PlayerClass.xcoin_bear_out = XForm_Win.transform;

         PlayerClass.BlackBearOut = PlayerClass.WhiteBearOut = false;

         if (PlayerClass.player_number == 1)
         {
             for (int i = 0; i < 15; i++)
                 BoardClass.BlackCoinsContainer[i].GetComponent<Collider>().enabled = false;

             if (PlayerClass.playerHasBurntCoin)
             {
                 for (int i1 = 0; i1 < 15; i1++)
                 {
                     BoardClass.WhiteCoinsContainer[i1].GetComponent<Collider>().enabled = false;
                     PlayerClass.Container_Of_WC_burnt[i1].GetComponent<Collider>().enabled = true;
                 }
             }

             PlayerClass.White_Diceroll_Counter++;
         }
         else
         {
             for (int j = 0; j < 15; j++)
                 BoardClass.WhiteCoinsContainer[j].GetComponent<Collider>().enabled = false;

             if (PlayerClass.playerHasBurntCoin)
             {
                 for (int j1 = 0; j1 < 15; j1++)
                 {
                     BoardClass.BlackCoinsContainer[j1].GetComponent<Collider>().enabled = false;
                     PlayerClass.Container_Of_BC_burnt[j1].GetComponent<Collider>().enabled = true;
                 }
             }

             PlayerClass.Black_Diceroll_Counter++;
         }
         
         Roll_Dice_btn();
     }
     
     
}
