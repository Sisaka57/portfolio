using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.String;
//classes
using static Board;
using static DataManager;
using static DiceRoll;
using static Robot;

//© 2020 (RVBinary)

public class Player : MonoBehaviour
{
    public bool Black_Bot; // AI will be active in single mode
    public bool removeDice; // player is able to remove dice for roll
    public bool White_ofr_dbl, Black_ofr_dbl; // situation of offering double of each player. true means player offered to double the score of game 

    // values of game such as number of dice roll and overall score for each player 
    public int White_ScoreOfCurrentMatch, White_Diceroll_Counter, num_double, WhiteOverAllScore, White_Double_Offer_Counter;
    public int Black_ScoreOfCurrentMatch, Black_Diceroll_Counter, BlackOverAllScore, Black_Double_Offer_Counter;
    public bool[] SwitchBearOutTwr = new bool[3]; // controls moving of checkers to bear out section 
    public int No_Bear_out_WC_Twr_18_23; // number of White checkers that exist in 18th till 23th towers if reached to 15 player is able to bearout its checkers
    public int No_Bear_out_BC_Twr_5_0;
    public int[] Bearout_Delivery_Tower = new int[4]; // with this array backgammon finds out how many move has player in bear out mode
    public bool[] TransferCheckMade = new bool[4]; // manages movement of checkers along the board by making it false and true
    public int[] Coin_Legal_Tower = new int[4]; // possible moves of selected checker
    public int[] temp_coin_legal = new int[4]; // relative to burned checkers by this array backgammon determines which dice played  

    [Header("*** Dice Section ***")]
    public int[] Dice_number = new int[7]; // value of dice stores in this array
    public int player_number, diceChecker, myPlayerNumber=1; // player_number : 1 (White) / : 2 (black). diceChecker : index of dice check
    public bool even_dice, opposite_dice, burn_event, Undo_Bearout; //equal_dice: if DiceNumbers is equal * opposite_dice: if Dice Numbers is not equal 
    public bool[] DiceCheck = new bool[4]; // by this array we could limit number of checkers
    public int Num_Of_WC_Burnt; // stores number of burned Checkers ,by this value we could bring back burnt checkers in order
    public GameObject[] Container_Of_WC_burnt = new GameObject[15]; // stores white burnt checkers
    public int Num_Of_BC_Burnt; // stores number of burned checkers
    public GameObject[] Container_Of_BC_burnt = new GameObject[15]; // stores black burnt checkers
    public bool WhiteBearOut, BlackBearOut; // players are able to bear out its checkers
    public int W_win; // number of beard out checkers (white)
    public GameObject[] White_beardout_Coin = new GameObject[15]; // stores beard out checkers
    public int B_win; // number of beard out checkers (black)
    public GameObject[] Black_Beardout_coins = new GameObject[15];
    public Transform[] All_Possible_Coin_Positions = new Transform[360]; // the exact position that Checker will get place on it
    public GameObject[] White_burn_points = new GameObject[10]; // stores burnt checker (for white)
    public GameObject[] Black_burn_points = new GameObject[10]; // stores burnt checker (for black)
    public GameObject[] White_Bearout_points = new GameObject[15];// in bearout mode. the position that each white Checker get place on it
    public GameObject[] Black_Bearout_points = new GameObject[15];
    public string click; // a part of automatic move of checker if its value equal to "Locked" means auto move is not possible
    public string Who = Empty; // specify the against and current player it will fill with "White" or "Black"
    public bool playerHasBurntCoin; // if playerHaveBurnCoin == true means: The player has a burnt checker to enter it on the board
    private GameObject Selected_obj; //stores touched object 
    public GameObject Tower_obj; // the tower that were selected
    public GameObject doubleCube; // Cube of Double the score
    public int Tower_Number; // tower number : 0 to 26
    public int SelectedTowerNumber, DeliveryTower; // SelectedTowerNumber : tower number of selected tower --- DeliveryTower : the tower that checker will get in it
    public int Coin_Index; // index of checker in the tower that were selected
    public int Final_Position_OF_Coin; // the position that checker will get on it
    public Transform SelectedCoin; // the checker that were selected
    public Transform xcoin_point; // the checker that will transfer to its next position
    public Transform xcoin_burn; // the checker that will transfer to burnt section (middle of each backgammon)
    public Transform xcoin_bear_out; // the checker that will transfer to  bear out section
    CameraShake cam_shake; // shaking camera class
    public int White_dbl_chk; // if its value equal to 1 means, Player already offered to double score 
    public int Black_dbl_chk;
    public float pauseTime; //Interruption that is necessary for transferring checker to Burn section 
    private bool White_dbl_wins, Black_dbl_wins; // = true means player won the match
    public int white_coin_move_counter = 167; // number of move that player needs to do for wining the game
    public int black_coin_move_counter = 167;
    public int which_dice;

    public static Player PlayerClass;
    public TextMesh Coin_index_label; // shows number of checker 

    private Camera _camera;
    private GameObject Trf_XForm; // Secondary object for preventing of null error
    private GameObject Trf_XForm_burn; // Secondary object for preventing of null error
    private GameObject Trf_XForm_BearOut; // Secondary object for preventing of null error

    private Rigidbody Dbl_cube_rb; // Rigidbody component of Double Cube 

    [Header("*** Undo Section ***")]
    [Space] //record all index of select checker,tower for Undoing current move
    public List<int> RListDeliverTower = new List<int>(); // stores the selected tower (next place of checker) number.
    public List<int> RListSelectedTower = new List<int>(); //stores the selected tower number.
    public List<int> RListBurnTower = new List<int>(); // the tower that Checker got burn
    public List<int> RListBeardOutTower = new List<int>(); // the tower that Checker got bear out
    public List<string> UndoMode = new List<string>(); // based on Undo mode Checker will get back to previous position
    public bool UndoAction, Undo_BurntCoin, Regular_Undo_BurntCoin, DontRunRegualrBurn; // determines how undo gets happen

    private Transform startPos, startPos_burn; // the position that Checker will transfer to next position / burn section
    private Transform endPos, endPos_burn; // target position of checker
    public bool makebuttons_Deactivate, DisconnectAutoMove;

    public ParticleSystem[] EvenDice_particles;
    public GameObject Checker_shadow;

    [Header("lerp Process")]
    [Space]
    private Vector3 centerPoint, centerPoint_burn;
    Vector3 startRelCenter, startRelCenter_burn;
    Vector3 endRelCenter, endRelCenter_burn;

    [Header("Slerp Process")]
    [Space]
    public Vector3 Direction;
    public Image p1_label, p2_label;
    private float speed, Degree;

    bool allowTapAgain = true;

    //Attention : if you have any questions feel free to ask. best way to contact is to email us, we will answer and support you for sure. the email address mentioned in the manual.pdf

    private void Start()
    {
        Dbl_cube_rb = doubleCube.GetComponent<Rigidbody>();
        makebuttons_Deactivate = false;

        //** A Checker on the board can be placed at 359 the point (if you search : WBC_TransformPoints in the hierarchy you can find them).
        //A Checker moves with a simple formula on the board : All_Possible_Coin_Positions = SelectedTowerNumber * 15 + Coin_Index;
        // All_Possible_Coin_Positions(place of the Checker in the Delivery tower) *** SelectedTowerNumber: index of The tower that selected by the player * 15 (each tower has 15 row) +
        //Coin_Index(numbers of coins that available in the selected tower)
        //the points of All_Possible_Coin_Positions array must not change and they must be in order

        for (int index = 0; index < 360; index++) //finds and store all points (possible positions for each Checker)
            All_Possible_Coin_Positions[index] = GameObject.Find("Point_" + index).transform;
        //**
        if (Photon.Pun.PhotonNetwork.IsConnected)
            if (Photon.Pun.PhotonNetwork.IsMasterClient)
            {

                myPlayerNumber = 1;
            }
            else
            {
                myPlayerNumber = 2;
            }
        else
        {
            myPlayerNumber = 1;
        }
        _camera = Camera.main;
        PlayerClass = this;
        cam_shake = FindObjectOfType<CameraShake>(); // camera shake class

        even_dice = opposite_dice = false; // even_dice : Dice 1 = 2 Dice 2 = 2 --- opposite_dice: Dice 1 = 2 Dice 2 = 6

        xcoin_point = GameObject.Find("XForm").transform; //moves the coins a log the board
        xcoin_burn = GameObject.Find("XForm_Burn").transform; //taking out the coins from the board to the burn section of player
        xcoin_bear_out = GameObject.Find("XForm_BearOut").transform; //bears out the coins from the board to the win floor section of player

        Trf_XForm = xcoin_point.gameObject;
        Trf_XForm_BearOut = xcoin_bear_out.gameObject;
        Trf_XForm_burn = xcoin_burn.gameObject;

        num_double = 1; //in the beginning it need to be 1 for doubling score of the match

        White_Diceroll_Counter = Black_Diceroll_Counter = 0;

        WhiteOverAllScore = PlayerPrefs.GetInt("w_overall_score");
        BlackOverAllScore = PlayerPrefs.GetInt("b_overall_score");

        White_Diceroll_Counter = PlayerPrefs.GetInt("w_overall_dice_roll");
        Black_Diceroll_Counter = PlayerPrefs.GetInt("b_overall_dice_roll");

        Black_Double_Offer_Counter = PlayerPrefs.GetInt("b_overall_dbl");
        White_Double_Offer_Counter = PlayerPrefs.GetInt("w_overall_dbl");

        if (PlayerPrefs.GetInt("mode") == 2 || PlayerPrefs.GetInt("mode") == 3)
            Black_Bot = false;

        Degree = 1f; // makes checker move in wave motion

    }

    private void Update()
    {
        ObjectDetector(); // specify the type of selected object

        // when the tag of selected tower were "tower" or xcoin_point value filled by a Checker or it is turns of robot to make move
        if ((Tower_obj != null && Tower_obj.CompareTag("Tower") | xcoin_point != Trf_XForm.transform) |
            Black_Bot) //check for robot 
        {
            if (PlayerPrefs.GetInt("MoveMode") == 0)
                lerp_XCoins(); // using vector3.lerp to transfer Checker along the board
            else
                Slerp_XCoins();
        }

        if (burn_event | Undo_BurntCoin | Regular_Undo_BurntCoin)
            lerp_XCoins_burn(Who); // using vector3.lerp to transfer Checker to Burn section

        if (Undo_Bearout | (Tower_obj != null && Tower_obj.CompareTag("W_win_tower") | Tower_obj.CompareTag("B_win_tower")) | Black_Bot && xcoin_bear_out != Trf_XForm_BearOut.transform)
            lerp_XCoins_bearout(Who); // using vector3.lerp to transfer Checker to bear ou section

        //if There is an opportunity for the player to move its Checkers. this function will show the right Checkers to select
        if (W_win != 15 && B_win != 15 && !BoardClass.CoinDefaultColor && PlayerPrefs.GetInt("legalmoves") == 1 && player_number == 1 | (player_number == 2 && (PlayerPrefs.GetInt("mode") == 2) || PlayerPrefs.GetInt("mode") == 3))
            BoardClass.FlashTouchableCoins();

        if (WhiteBearOut | (BlackBearOut && !Black_Bot))
            Lock_BeardOut_Coins(); // locks the Checkers that are placed in Bear out Section

        if (PlayerClass.W_win == 15 | PlayerClass.B_win == 15 && removeDice) // makes dice untouchable when a player wins the game
        {
            removeDice = false;
            DiceRollClass.dice1RemoveDiceAlarm.SetActive(false);
            DiceRollClass.dice2RemoveDiceAlarm.SetActive(false);
        }

    }

    public IEnumerator SwitchPlayer() // this function will specify which player should start/continue the game
    {
        float timeCloseMsgAnim;
        if (even_dice) // calculate Dice_number[] array values. in even_dice mode player has four tower to select
        {
            Dice_number[2] = Dice_number[3] = Dice_number[0];
            Dice_number[4] = Dice_number[0] + Dice_number[1];
            Dice_number[5] = Dice_number[4] + Dice_number[1];
            Dice_number[6] = Dice_number[5] + Dice_number[1];


            EvenDice_particles[0].transform.position = Dice_Tracker.DiceTrackerClass.transform.position;
            EvenDice_particles[1].transform.position = Dice_Tracker1.DiceTracker1Class.transform.position;
            EvenDice_particles[0].Play();
            EvenDice_particles[1].Play();
            EvenDice_particles[0].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            EvenDice_particles[1].transform.GetChild(0).GetComponent<ParticleSystem>().Play();

            if (PlayerPrefs.GetInt("sound") == 1)
                DataManagerClass.evendice_Snd.Play();
        }

        if (PlayerPrefs.GetInt("mode") == 1)
            Black_Bot = true;

        if (opposite_dice) // calculate Dice_number[] array values. in opposite_dice mode player has three tower to select
            Dice_number[2] = Dice_number[0] + Dice_number[1];

        DiceRollClass.dice1_rb.Sleep(); // make dice sleep and no longer moves
        DiceRollClass.dice2_rb.Sleep();

        removeDice = false; // player cant touch dice for roll
        DiceRollClass.particleRemoveDice1Alarm.Stop(); // particles that orbit around the dice will be deactivated
        DiceRollClass.particleRemoveDice2Alarm.Stop();


        if (PlayerPrefs.GetInt("Amateur") == 0) // if its first time that game is run
        {
            yield return new WaitForSecondsRealtime(2f); // after two seconds
            DiceRollClass.msgbox.Play("PopUp_note"); // this msg will show up
            DiceRollClass.msgbox.transform.GetChild(0).gameObject.SetActive(true);
            DiceRollClass.msgbox.transform.GetChild(1).gameObject.SetActive(true);
            DiceRollClass.msgbox.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1.0f;

            for (int i = 2; i < DiceRollClass.msgbox.transform.childCount; i++)
                DiceRollClass.msgbox.transform.GetChild(i).gameObject.SetActive(false);

            timeCloseMsgAnim = 3f; // after 3 sec player is able to select its checkers, its up to you how to manage the game sequence

        }
        else
            timeCloseMsgAnim = 0f;

        if (PlayerPrefs.GetInt("Amateur") == 0)
        {

            yield return new WaitForSecondsRealtime(timeCloseMsgAnim);
            PlayerPrefs.SetInt("Amateur", 1);

            if (Dice_number[0] > Dice_number[1]) //player the white need to start the game
            {
                Who = "White";

                for (int i = 0; i < 15; i++) // each player has 15 checker to play with them

                {
                    BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = true;
                    BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = false; // Checkers of against player will be deactivate
                }

                SelectedCoin = BoardClass.WhiteCoinsContainer[1].transform; // prevent of null error

            }
            else
            {
                Who = "Black";

                if (PlayerPrefs.GetInt("mode") == 1)
                {
                    Black_Bot = true; // robot get the control of black Checkers
                    RobotClass.think = true; // checks possibilities for making move
                }

                for (int i = 0; i < 15; i++)
                {
                    if (!Black_Bot)
                        BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = true;

                    BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
                }

                SelectedCoin = BoardClass.BlackCoinsContainer[1].transform;

                if (RobotClass.think) // AI
                    RobotClass.CalculateCoinsPosition(); //calculates the exact Checker moves for rbt

            }

        }
        else
        {
            if (PlayerPrefs.GetInt("mode") == 1) // if mode key equal to 1 means: single player and / equal to 2 means two player
            {
                if (player_number == 2)
                {
                    Black_Bot = true; // robot get the control of black Checkers
                    RobotClass.think = true; // checks possibilities for making move
                }

            }

            if (player_number == 1)
            {
                for (int i = 0; i < 15; i++)
                {
                    BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = true;
                    BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
                }
                SelectedCoin = BoardClass.WhiteCoinsContainer[1].transform; //WhiteCoinsContainer[] array consist of all white checkers
                Who = "White";

                p1_label.color = new Color(0f, 0f, 0f, 1f);
                p2_label.color = new Color(0f, 0f, 0f, 0.6f);

            }
            else
            {
                for (int i = 0; i < 15; i++)
                {
                    if (!Black_Bot)
                        BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = true;

                    BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
                }
                SelectedCoin = BoardClass.BlackCoinsContainer[1].transform;
                Who = "Black";

                p1_label.color = new Color(0f, 0f, 0f, 0.6f);
                p2_label.color = new Color(0f, 0f, 0f, 1f);
            }

            if (BoardClass.NumberOfBurntCoins(Who) > 0)  // Number Of BurntCoins based on Whose value determines whether player has burnt Checker or not
                playerHasBurntCoin = true;

            BearOutPossible(Who); // task of this function is to determine that player is able to bear out it's Checker.

            if (RobotClass.think) // AI
                RobotClass.CalculateCoinsPosition(); //calculates the exact Checker moves for rbt
        }

        if (timeCloseMsgAnim > 0.5f)
            DiceRollClass.msgbox.transform.GetChild(1).GetComponent<Animator>().Play("close_msg_btn");


        BoardClass.FindLegalTowers(); // shows those legal towers that player has permission to select

    }

    #region Double The Score of match

    public void OfferDouble_btn() // activates the Double cube
    {
        removeDice = false; // player cant touch dice for roll
        DiceRollClass.particleRemoveDice1Alarm.Stop(); // particles that orbit around the dice will be deactivated
        DiceRollClass.particleRemoveDice2Alarm.Stop();

        //DataManagerClass.offerDouble.Play("OfferDouble"); // recently gets comment

        if (PlayerPrefs.GetInt("undobtn") == 1)
            DataManagerClass.Undo.gameObject.SetActive(false);

        switch (player_number)
        {
            case 1:
                White_dbl_chk = 0;
                Black_dbl_chk = 1; // player 2 offered the double
                break;
            case 2:
                White_dbl_chk = 1; //player 1 offered the double
                Black_dbl_chk = 0;
                break;
        }

        if (player_number == 2 && Black_Bot)
            StartCoroutine(RobotClass.chk_dbl_black()); // robot will offer double

        DataManagerClass.offerDouble.Play("OfferDouble");
        DataManagerClass.offerDouble.transform.GetChild(1).GetComponent<Text>().text = (num_double * 2).ToString();
        DataManagerClass.btnSnd.Play();

    }
    public void TakeDoubleOffer() // takes the Double offer
    {
        doubleCube.transform.rotation = Quaternion.identity;

        if (num_double < 64) // double number should be under 64 
        {
            num_double *= 2; //doubles the current score
            for (int i = 0; i < doubleCube.transform.childCount; i++)
                doubleCube.transform.GetChild(i).GetComponent<TextMeshPro>().text = num_double.ToString(); //makes score double 

            //DataManagerClass.offerDouble.transform.GetChild(1).GetComponent<Text>().text = num_double.ToString(); // recently comments
        }

        if (player_number == 1)
        {
            Black_Double_Offer_Counter++;  // number of double offer
            Black_ofr_dbl = true;

            if (num_double > 2) // push double cube 
                Dbl_cube_rb.AddForce(doubleCube.transform.forward * 650f, ForceMode.Acceleration);
            else
                Dbl_cube_rb.AddForce(doubleCube.transform.forward * 500f, ForceMode.Acceleration);
            Dbl_cube_rb.rotation = Quaternion.identity;
        }
        else
        {
            White_Double_Offer_Counter++;
            White_ofr_dbl = true;

            if (num_double > 2)
                Dbl_cube_rb.AddForce(-doubleCube.transform.forward * 650f, ForceMode.Acceleration);
            else
                Dbl_cube_rb.AddForce(-doubleCube.transform.forward * 500f, ForceMode.Acceleration);
            Dbl_cube_rb.rotation = Quaternion.identity;
        }

        if (PlayerPrefs.GetInt("dblcube") == 1)
            doubleCube.layer = 2; // makes double cube untouchable (ignore raycast layer)

        DataManagerClass.offerDouble.Play("OfferDouble_close");

        DiceRollClass.ResetAllData(); // reset all necessary data for next diceroll
    }
    public void PassDoubleOffer() //pass the Double offer
    {
        if (player_number == 2) // player 2 passed the double offer when Player 1 offered
        {
            White_dbl_wins = true;
            DataManagerClass.winnerMsg.transform.GetChild(0).gameObject.SetActive(true);
            Black_Bot = false;
        }

        else // player 1 passed the double offer when Player 2 offered
        {
            Black_dbl_wins = true;
            DataManagerClass.winnerMsg.transform.GetChild(1).gameObject.SetActive(true);
            Black_Bot = false;
        }

        DataManagerClass.offerDouble.Play("OfferDouble_close");
        DataManagerClass.winnerMsg.Play("WinnerIs");

        doubleCube.layer = 2;
        removeDice = false;
        if (PlayerPrefs.GetInt("undobtn") == 1)
            DataManagerClass.Undo.gameObject.SetActive(false);
    }

    #endregion

    #region Score Table
    public void Calculate_Score() //calculates the final scores of the game
    {
        foreach (var clap in DataManagerClass.claps)
            clap.Play();

        int score = 0, marsMode = 0;

        if (W_win == 15 | White_dbl_wins)
        {
            for (int i1 = 18; i1 <= 23; i1++)
                if (BoardClass.boardArray[i1, 0].CompareTag("Black_coin") && B_win == 0 && W_win == 15)
                {
                    score = 2;
                    marsMode = 1;
                }

            if (W_win == 15 && B_win == 0 && marsMode == 0)
            {
                score = 1;
            }

            if (W_win == 15 && B_win == 0)
            {
                if (White_ofr_dbl | Black_ofr_dbl) // if one of players offered to double score of match
                {
                    White_ScoreOfCurrentMatch = num_double + score;
                    WhiteOverAllScore += num_double + score;
                }
                else
                {
                    White_ScoreOfCurrentMatch = 1 + score;
                    WhiteOverAllScore += 1 + score;
                }

            }
            else // if player won the match with offering double to against and he/she pass the offer
            {
                if (White_ofr_dbl | Black_ofr_dbl)
                {
                    White_ScoreOfCurrentMatch = num_double;
                    WhiteOverAllScore += num_double;
                }
                else
                {
                    White_ScoreOfCurrentMatch = 1;
                    WhiteOverAllScore += 1;
                }

            }

        }

        if (B_win == 15 | Black_dbl_wins)
        {
            for (int i1 = 0; i1 <= 5; i1++)
                if (BoardClass.boardArray[i1, 0].CompareTag("White_coin") && B_win == 15 && W_win == 0)
                {
                    score = 2;
                    marsMode = 1;
                }

            if (B_win == 15 && W_win == 0 && marsMode == 0)
                score = 1;


            if (B_win == 15 && W_win == 0)
            {
                if (White_ofr_dbl | Black_ofr_dbl)
                {
                    Black_ScoreOfCurrentMatch = num_double + score;
                    BlackOverAllScore += num_double + score;
                }
                else
                {
                    Black_ScoreOfCurrentMatch = 1 + score;
                    BlackOverAllScore += 1 + score;
                }
            }
            else
            {
                if (White_ofr_dbl | Black_ofr_dbl)
                {
                    Black_ScoreOfCurrentMatch = num_double;
                    BlackOverAllScore += num_double;
                }
                else
                {
                    Black_ScoreOfCurrentMatch = 1;
                    BlackOverAllScore += 1;
                }
            }
        }

        // stores game scores and data
        DataManagerClass.White_Overall_double_Offer += White_Double_Offer_Counter;
        DataManagerClass.Black_Overall_double_Offer += Black_Double_Offer_Counter;
        DataManagerClass.OverAllScoreForWhite += WhiteOverAllScore;
        DataManagerClass.OverAllScoreForBlack += BlackOverAllScore;

        DataManagerClass.Overall_DiceRoll_White += White_Diceroll_Counter;
        DataManagerClass.Overall_DiceRoll_Black += Black_Diceroll_Counter;

        PlayerPrefs.SetInt("w_overall_dbl", DataManagerClass.White_Overall_double_Offer);
        PlayerPrefs.SetInt("b_overall_dbl", DataManagerClass.Black_Overall_double_Offer);

        PlayerPrefs.SetInt("w_overall_score", DataManagerClass.OverAllScoreForWhite);
        PlayerPrefs.SetInt("b_overall_score", DataManagerClass.OverAllScoreForBlack);

        PlayerPrefs.SetInt("w_overall_dice_roll", DataManagerClass.Overall_DiceRoll_White);
        PlayerPrefs.SetInt("b_overall_dice_roll", DataManagerClass.Overall_DiceRoll_Black);


        StartCoroutine(show_Score_tables());

        if (PlayerPrefs.GetInt("undobtn") == 1)
            DataManagerClass.Undo.gameObject.SetActive(false);
    }
    private IEnumerator show_Score_tables() // shows the Score table
    {
        //white
        DataManagerClass.scoreTableTexts[0].text = White_ScoreOfCurrentMatch.ToString();
        DataManagerClass.scoreTableTexts[1].text = PlayerPrefs.GetInt("w_overall_dice_roll").ToString(); // diceroll
        DataManagerClass.scoreTableTexts[2].text = PlayerPrefs.GetInt("w_overall_dbl").ToString(); // double offer
        DataManagerClass.scoreTableTexts[3].text = PlayerPrefs.GetInt("w_overall_score").ToString(); //over all score

        //black
        DataManagerClass.scoreTableTexts[4].text = Black_ScoreOfCurrentMatch.ToString();
        DataManagerClass.scoreTableTexts[5].text = PlayerPrefs.GetInt("b_overall_dice_roll").ToString();
        DataManagerClass.scoreTableTexts[6].text = PlayerPrefs.GetInt("b_overall_dbl").ToString();
        DataManagerClass.scoreTableTexts[7].text = PlayerPrefs.GetInt("b_overall_score").ToString();

        DataManagerClass.scoreTables.Play("score_table");

        yield return new WaitForSecondsRealtime(1f);

        removeDice = false;
        doubleCube.layer = 2;
    }

    #endregion

    public void BearOutPossible(string who) //specifies that the player is able to Bear out his Checkers
    {
        int numOfCoinsB4BearoutSection = 0;

        if (who == "White")
        {
            BlackBearOut = false;
            No_Bear_out_WC_Twr_18_23 = 0;
            if (BoardClass.NumberOfBurntCoins(who) == 0)
            {
                for (int i = 18; i < 24; i++) // checks how many checker player 1 has in 18th tower till 23th tower
                    for (int j = 0; j < 15; j++)
                        if (BoardClass.boardArray[i, j].CompareTag("White_coin"))
                            No_Bear_out_WC_Twr_18_23++;

                for (int i = 0; i < 18; i++) // checks how many checker player 1 has before 18towers
                    for (int j = 0; j < 15; j++)
                        if (BoardClass.boardArray[i, j].CompareTag("White_coin"))
                            numOfCoinsB4BearoutSection++;



            }

            if (No_Bear_out_WC_Twr_18_23 == 15 | (White_beardout_Coin[0] != null && W_win < 15 && numOfCoinsB4BearoutSection == 0 && !playerHasBurntCoin)) // in this condition player is able to bear out its Checkers
            {
                WhiteBearOut = true;
            }
            else
            {
                No_Bear_out_WC_Twr_18_23 = 0;
                WhiteBearOut = false;
            }

        }

        if (who == "Black")
        {
            WhiteBearOut = false;
            No_Bear_out_BC_Twr_5_0 = 0;
            if (BoardClass.NumberOfBurntCoins(who) == 0)
            {
                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < 15; j++)
                        if (BoardClass.boardArray[i, j].CompareTag("Black_coin"))
                            No_Bear_out_BC_Twr_5_0++;

                for (int i = 23; i > 5; i--)
                    for (int j = 0; j < 15; j++)
                        if (BoardClass.boardArray[i, j].CompareTag("Black_coin"))
                            numOfCoinsB4BearoutSection++;

            }

            if (No_Bear_out_BC_Twr_5_0 == 15 | (Black_Beardout_coins[0] != null && B_win < 15 && numOfCoinsB4BearoutSection == 0 && !playerHasBurntCoin))
                BlackBearOut = true;

            else
            {
                No_Bear_out_BC_Twr_5_0 = 0;
                BlackBearOut = false;
            }
        }
    }
    private void ShowIndexOfSelectedTower() //if player touched one of his coins, this function will Selects the highest coin in that tower
    {
        if (BoardClass.NumberOfBurntCoins(Who) == 0 | (UndoAction && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "burn"))
        {
            for (int indexTwr = 0; indexTwr < 15; indexTwr++)

                if (BoardClass.boardArray[SelectedTowerNumber, indexTwr] == BoardClass.blank)
                {
                    Coin_Index = indexTwr - 1;
                    SelectedCoin = BoardClass.boardArray[SelectedTowerNumber, Coin_Index].transform; //the highest checker in tower column
                    Coin_index_label.transform.position = new Vector3(SelectedCoin.position.x, SelectedCoin.position.y + 0.03f, SelectedCoin.position.z);
                    SelectedCoin.GetComponent<MeshRenderer>().materials[0].renderQueue = 3000;
                    Checker_shadow.transform.parent = SelectedCoin.transform;// transfer fake shadow to position of SelectedCoin
                    Checker_shadow.transform.localPosition = new Vector3(0f, 0f, 0f);

                    if (!UndoAction)
                        Coin_index_label.text = indexTwr.ToString(); //contains the numbers of coins in the selected tower
                    else
                        Coin_index_label.text = "";
                    break;

                }

        }

        if (!UndoAction)
        {
            if (BoardClass.NumberOfBurntCoins(Who) > 0 && Selected_obj.CompareTag("White_coin"))
                SelectedCoin = Container_Of_WC_burnt[Num_Of_WC_Burnt].transform; // filled with last burnt Checker


            if (BoardClass.NumberOfBurntCoins(Who) > 0 && Selected_obj.CompareTag("Black_coin"))
                SelectedCoin = Container_Of_BC_burnt[Num_Of_BC_Burnt].transform;

        }

        Coin_index_label.color = player_number == 1 ? Color.black : Color.white;

        if (PlayerPrefs.GetInt("skins") == 3)
            Coin_index_label.color = Color.white;

    }

    [Photon.Pun.PunRPC]
    void DetectTapOnCoins(string Selected_OBJ_Str)
    {


        Selected_obj = GameObject.Find(Selected_OBJ_Str);
        UndoAction = false;

        allowTapAgain = true;
        BoardClass.CoinDefaultColor = true; // checkers no longer gets flashing

        for (int bod = 0; bod < 4; bod++)
            Bearout_Delivery_Tower[bod] = 0; //records the index of selected tower when player moves his Checker to Bear out section

        for (int sbo = 0; sbo < 3; sbo++)
            SwitchBearOutTwr[sbo] = true; //specifies the movement of coin to Bear out section


        for (int i = 0; i < 24; i++)
            for (int j = 0; j < 15; j++)
                if (BoardClass.boardArray[i, j] == Selected_obj) //finds the situation of coin in the board_array(in the board of backgammon)
                {
                    SelectedCoin = Selected_obj.transform;
                    SelectedTowerNumber = i;
                    break;
                }

        ShowIndexOfSelectedTower(); // select the highest Checker in the selected tower


        for (int deactiveTowers = 0; deactiveTowers < 26; deactiveTowers++)
            BoardClass.greenTowers[deactiveTowers].SetActive(false);

        BearOutPossible(Who); // task of this function is to determine that player is able to bear out it's checkers.
        BoardClass.FindLegalTowers(); // info in board scripts 
        BoardClass.ShowLegalTowers(); // info in board scripts (different from flash legal towers functions)


        if (PlayerPrefs.GetInt("automove") == 1 && !DisconnectAutoMove)
            StartCoroutine(AutoMoveCoins()); //if one move remained this function will makes the last move

        xcoin_point = Trf_XForm.transform;
        Time.timeScale = 1f;
    }

    [Photon.Pun.PunRPC]
    void DetectTapOnTower(string Selected_OBJ_Str)
    {
        allowTapAgain = true;


        Selected_obj = GameObject.Find(Selected_OBJ_Str);
        UndoAction = false;


        BoardClass.CoinDefaultColor = true;

        Tower_obj = Selected_obj;

        //returns the Material of Player Checkers to Default material
        if (player_number == 1)
            for (int i = 0; i < 15; i++)
                BoardClass.WhiteCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.WhiteCoinDefultColor;
        else
            for (int i2 = 0; i2 < 15; i2++)
                BoardClass.BlackCoinsContainer[i2].GetComponent<Renderer>().material = BoardClass.BlackCoinDefultColor;

        UnTouchable_The_Coins(); // will locks all coins in the board when a coins starts to move

        for (int twr = 0; twr < 24; twr++)
            if (BoardClass.greenTowers[twr] == Tower_obj)
            {
                xcoin_point = SelectedCoin;
                BoardClass.Coin_step_by_step = BoardClass.coin_auto_step = false; // it could be useless
                DeliveryTower = twr; //The index of the tower which the Checker will be place in it
                Tower_Number = twr; //index of selected tower
                break;
            }

        if (Tower_obj.CompareTag("White_coin") | Tower_obj.CompareTag("Black_coin") && PlayerPrefs.GetInt("undobtn") == 1)
            Invoke(nameof(Show_undo_btn), 1f);

        Coin_index_label.text = "";

        for (int gt = 0; gt < 26; gt++)
            BoardClass.greenTowers[gt].SetActive(false); // after selecting the right tower all towers will be deactive  

        Calculate_Dice_check(); // task of this function is to specify how many move the Player has

        MoveCounter_167(); //Counting the remaining moves of the Checkers (167 till 0)

        if (Selected_obj.CompareTag("B_win_tower") | Selected_obj.CompareTag("W_win_tower"))
            StartCoroutine(Transfer_BearOutCoin()); // his function has the task of transfers Checkers to bear out section

        if (Selected_obj.CompareTag("Tower"))
            StartCoroutine(Transfer_Coin_To_DeliveryTower()); // this function has the task of transferring Checkers along the board 

        if (even_dice) // more time need to update the board array && DeliveryTower == SelectedTowerNumber + Dice_number[4]
            Invoke(nameof(invoke_FindLegalTowers), 2f); // after 1 sec calculate next legal tower 
        else
            Invoke(nameof(invoke_FindLegalTowers), 1f); // after 1 sec calculate next legal tower 
    }

    private void ObjectDetector() //all process of selecting Checkers and towers 
    {
        if (Input.GetMouseButtonDown(0) && DataManagerClass.menuIsClose /* here by Saad*/ && Player.PlayerClass.player_number == Player.PlayerClass.myPlayerNumber/*here by Saad*/)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider != null)
            {
                Selected_obj = hit.collider.gameObject;
                UndoAction = false;

                object[] parameters = new object[1];
                parameters[0] = Selected_obj.name;

                if (hit.collider.gameObject.CompareTag("White_coin") | hit.collider.gameObject.CompareTag("Black_coin"))
                {
                    allowTapAgain = false;
                    if (PlayerPrefs.GetInt("mode") == 3)
                    {
                        
                        GetComponent<Photon.Pun.PhotonView>().RPC("DetectTapOnCoins", Photon.Pun.RpcTarget.AllViaServer, Selected_obj.name);
                        //DetectTapOnCoins();
                    }
                    else
                    {
                        allowTapAgain = true;

                        BoardClass.CoinDefaultColor = true; // checkers no longer gets flashing

                        for (int bod = 0; bod < 4; bod++)
                            Bearout_Delivery_Tower[bod] = 0; //records the index of selected tower when player moves his Checker to Bear out section

                        for (int sbo = 0; sbo < 3; sbo++)
                            SwitchBearOutTwr[sbo] = true; //specifies the movement of coin to Bear out section


                        for (int i = 0; i < 24; i++)
                            for (int j = 0; j < 15; j++)
                                if (BoardClass.boardArray[i, j] == Selected_obj) //finds the situation of coin in the board_array(in the board of backgammon)
                                {
                                    SelectedCoin = Selected_obj.transform;
                                    SelectedTowerNumber = i;
                                    break;
                                }

                        ShowIndexOfSelectedTower(); // select the highest Checker in the selected tower


                        for (int deactiveTowers = 0; deactiveTowers < 26; deactiveTowers++)
                            BoardClass.greenTowers[deactiveTowers].SetActive(false);

                        BearOutPossible(Who); // task of this function is to determine that player is able to bear out it's checkers.
                        BoardClass.FindLegalTowers(); // info in board scripts 
                        BoardClass.ShowLegalTowers(); // info in board scripts (different from flash legal towers functions)


                        if (PlayerPrefs.GetInt("automove") == 1 && !DisconnectAutoMove)
                            StartCoroutine(AutoMoveCoins()); //if one move remained this function will makes the last move

                        xcoin_point = Trf_XForm.transform;
                        Time.timeScale = 1f;
                    }

                }

                if (Selected_obj.CompareTag("Tower") | Selected_obj.CompareTag("W_win_tower") | Selected_obj.CompareTag("B_win_tower"))
                {
                    allowTapAgain = false;
                    if (PlayerPrefs.GetInt("mode")==3){
                        GetComponent<Photon.Pun.PhotonView>().RPC("DetectTapOnTower", Photon.Pun.RpcTarget.AllViaServer, Selected_obj.name);
                        //DetectTapOnTower();
                    }
                    else
                    {
                        allowTapAgain = true;
                        BoardClass.CoinDefaultColor = true;

                        Tower_obj = Selected_obj;

                        //returns the Material of Player Checkers to Default material
                        if (player_number == 1)
                            for (int i = 0; i < 15; i++)
                                BoardClass.WhiteCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.WhiteCoinDefultColor;
                        else
                            for (int i2 = 0; i2 < 15; i2++)
                                BoardClass.BlackCoinsContainer[i2].GetComponent<Renderer>().material = BoardClass.BlackCoinDefultColor;

                        UnTouchable_The_Coins(); // will locks all coins in the board when a coins starts to move

                        for (int twr = 0; twr < 24; twr++)
                            if (BoardClass.greenTowers[twr] == Tower_obj)
                            {
                                xcoin_point = SelectedCoin;
                                BoardClass.Coin_step_by_step = BoardClass.coin_auto_step = false; // it could be useless
                                DeliveryTower = twr; //The index of the tower which the Checker will be place in it
                                Tower_Number = twr; //index of selected tower
                                break;
                            }

                        if (Tower_obj.CompareTag("White_coin") | Tower_obj.CompareTag("Black_coin") && PlayerPrefs.GetInt("undobtn") == 1)
                            Invoke(nameof(Show_undo_btn), 1f);

                        Coin_index_label.text = "";

                        for (int gt = 0; gt < 26; gt++)
                            BoardClass.greenTowers[gt].SetActive(false); // after selecting the right tower all towers will be deactive  

                        Calculate_Dice_check(); // task of this function is to specify how many move the Player has

                        MoveCounter_167(); //Counting the remaining moves of the Checkers (167 till 0)

                        if (Selected_obj.CompareTag("B_win_tower") | Selected_obj.CompareTag("W_win_tower"))
                            StartCoroutine(Transfer_BearOutCoin()); // his function has the task of transfers Checkers to bear out section

                        if (Selected_obj.CompareTag("Tower"))
                            StartCoroutine(Transfer_Coin_To_DeliveryTower()); // this function has the task of transferring Checkers along the board 

                        if (even_dice) // more time need to update the board array && DeliveryTower == SelectedTowerNumber + Dice_number[4]
                            Invoke(nameof(invoke_FindLegalTowers), 2f); // after 1 sec calculate next legal tower 
                        else
                            Invoke(nameof(invoke_FindLegalTowers), 1f); // after 1 sec calculate next legal tower 
                    }
                    
                }

                //To be implemented 
                if (Selected_obj.CompareTag("Double_cube"))
                    OfferDouble_btn(); // if player touches the double cube the function of it will be run

            }
        }

    }
    void invoke_FindLegalTowers() //gives time to FindLegalTowers function gets update 
    {
        if (BoardClass.NumberOfBurntCoins(Who) == 0)
            playerHasBurntCoin = false;

        BearOutPossible(Who); // task of this function is to determine that player is able to bear out it's checkers.
        BoardClass.FindLegalTowers();
    }
    private void Check_if_W_Or_B_Could_Be_winner()
    {
        if (W_win == 15) //if white player wins the game this code will run
        {
            DataManagerClass.winnerMsg.Play("WinnerIs");
            DataManagerClass.winnerMsg.transform.GetChild(0).gameObject.SetActive(true);

            if (Black_Bot)
                Black_Bot = false;

            for (int i = 0; i < 15; i++)
            {
                BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
                BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
            }

            makebuttons_Deactivate = true;

            doubleCube.SetActive(false);

            DiceRollClass.dice1RemoveDiceAlarm.SetActive(false);
            DiceRollClass.dice2RemoveDiceAlarm.SetActive(false);
            removeDice = false;
        }

        if (B_win == 15) //if black player wins the game this code will run
        {

            DataManagerClass.winnerMsg.Play("WinnerIs");
            DataManagerClass.winnerMsg.transform.GetChild(1).gameObject.SetActive(true);


            if (Black_Bot)
                Black_Bot = false;

            for (int i = 0; i < 15; i++)

            {
                BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
                BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
            }

            makebuttons_Deactivate = true; // undo button gets deactivate

            doubleCube.SetActive(false);

            removeDice = false;
        }

    }

    #region  Transfer Coin in the board

    private void lerp_XCoins() // vector3.lerp will transfer Checkers from point A(selected tower) to Point B(delivery tower)
    {
        startPos = xcoin_point;

        if (Final_Position_OF_Coin < 360)
            endPos = All_Possible_Coin_Positions[Final_Position_OF_Coin];

        xcoin_point.position = Vector3.Lerp(startPos.position, endPos.position, DataManagerClass.AlternativeOfTime * Time.deltaTime);
    }

    private void Slerp_XCoins()
    {
        startPos = xcoin_point; // checker that moves
        endPos = All_Possible_Coin_Positions[Final_Position_OF_Coin]; // target of checker
        var offset = endPos.position - startPos.position; // exact position that checker will using to reach the destination
        var dis = Vector3.Distance(startPos.position, endPos.position); // based on distance of checker in its way to destination tower movement speed will calculate
        centerPoint = (startPos.position + offset) * Degree; // changes the direction of checkers vector
        centerPoint -= Direction; // new direction of checkers vector
        startRelCenter = startPos.position - centerPoint;
        endRelCenter = endPos.position - centerPoint;

        speed += Time.deltaTime; // movement speed

        xcoin_point.position = Vector3.Slerp(startRelCenter, endRelCenter, speed);
        xcoin_point.position += centerPoint;

        if (dis <= 0.01f)
            speed = 0f;

    }

    private void lerp_XCoins_burn(string who)
    {
        startPos_burn = xcoin_burn;
        if (Final_Position_OF_Coin < 360 && Regular_Undo_BurntCoin) // back to board from burn section
            endPos_burn = All_Possible_Coin_Positions[Final_Position_OF_Coin];

        else
        {
            if (Undo_BurntCoin) // brings back coins from the board part to burn section
            {
                if (who == "Black")
                    endPos_burn = Black_burn_points[Num_Of_BC_Burnt].transform;
                else
                    endPos_burn = White_burn_points[Num_Of_WC_Burnt].transform;
            }
            else // transfers to burn section
            {
                if (who == "White")
                    endPos_burn = Black_burn_points[Num_Of_BC_Burnt].transform;
                else
                    endPos_burn = White_burn_points[Num_Of_WC_Burnt].transform;
            }
        }

        xcoin_burn.position = Vector3.Lerp(startPos_burn.position, endPos_burn.position, DataManagerClass.AlternativeOfTime_burn * Time.deltaTime);

    }
    private void lerp_XCoins_bearout(string who)
    {
        xcoin_point = Trf_XForm.transform;
        startPos = xcoin_bear_out;

        if (Undo_Bearout)
            endPos = All_Possible_Coin_Positions[Final_Position_OF_Coin];
        else
        {
            if (who == "White")
                endPos = White_Bearout_points[W_win].transform;
            else
                endPos = Black_Bearout_points[B_win].transform;
        }

        xcoin_bear_out.position = Vector3.Lerp(xcoin_bear_out.position, endPos.position, DataManagerClass.AlternativeOfTime * Time.deltaTime);

    }


    private IEnumerator Transfer_Coin_To_DeliveryTower()
    {
        if (PlayerPrefs.GetInt("undobtn") == 1)
            DataManagerClass.Undo.gameObject.SetActive(false);

        for (int check = 0; check < 4; check++) // TransferCheckMade[] will be false for each time player transfer a Checker
            TransferCheckMade[check] = false;

        if (playerHasBurntCoin)
        {
            //in this case when a Checker leaves burn section and getting back to the board the DeliveryTower will be the selectedTower
            Final_Position_OF_Coin = DeliveryTower * 15 + BoardClass.IndexOfCoinInSelectedTower(DeliveryTower) + 1; // prevent of going to tower zero

            Update_board(false, SelectedCoin.gameObject); //when the first argument of function is false it going to update the burn array of coins in the board
        }

        if (!playerHasBurntCoin)
        {
            Final_Position_OF_Coin = SelectedTowerNumber * 15 + Coin_Index; //Coin_Index : row of the tower

            if (!UndoAction)
                RListSelectedTower.Add(SelectedTowerNumber);

            Update_board(true, SelectedCoin.gameObject); //this function will update the situation of coins in the board
        }

        if (player_number == 1)
        {
            // Is_The_Delivery_Tower_Free : will checks the destination tower, if there were more than one Checker(against player) the result of this function will be false
            if (BoardClass.Is_The_Delivery_Tower_Free(BoardClass.Best_Tower_To_deliver_Coin) | (even_dice && BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1])) | playerHasBurntCoin)
            {
                if (DeliveryTower == Coin_Legal_Tower[0])
                {
                    StartCoroutine(BurnTheCoin(Coin_Legal_Tower[0], Who)); //will burn black player coins

                    yield return new WaitForSecondsRealtime(pauseTime);

                    Update_TargetTower(Tower_Number); // updates the destination tower with new arrived Checker

                    //Coin_Legal_Tower [0] : index of destination tower * 15(each tower has 15 row) + index of highest Checker of destination tower
                    Final_Position_OF_Coin = Coin_Legal_Tower[0] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[0]);

                    StartCoroutine(Touchable_The_Coins());
                    Invoke(nameof(Show_undo_btn), 1f);

                    if (!UndoAction)
                    {
                        RListDeliverTower.Add(Coin_Legal_Tower[0]);
                        UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                    }
                    DataManagerClass.coinImpactSnd.Play();
                }
                else
                {
                    if (even_dice) // if dice were equal
                    {
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[0], Who)); // by default will check the destination tower

                        //by adding digit 1 to "BoardClass.IndexOfSelectedCoin (Coin_Legal_Place [0])" the selected coin will skips
                        //from fifteenth rows of previous tower and that's will prevent from overlapping with coins of previous tower
                        Final_Position_OF_Coin = Coin_Legal_Tower[0] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[0]) + 1;
                        if (DeliveryTower == Coin_Legal_Tower[1] | DeliveryTower == Coin_Legal_Tower[2] | DeliveryTower == Coin_Legal_Tower[3])
                            TransferCheckMade[1] = true; // unlock the next move

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[0]); // stores the index of destination tower for undo process
                            RListSelectedTower.Add(Coin_Legal_Tower[0]); // stores the index of selected tower for undo process
                            UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            //if the player knocked out a checker, undoMode will fill by "burn" so if player requested the undo
                            // function at first checks the undoMode then runs the process of undo
                        }

                        if (BoardClass.NumberOfBurntCoins(Who) == 0)
                            playerHasBurntCoin = false;

                        DataManagerClass.coinImpactSnd.Play();
                    }

                    else if (DeliveryTower == Coin_Legal_Tower[2])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        if (BoardClass.boardArray[Coin_Legal_Tower[0], 0].CompareTag("Black_coin") && BoardClass.boardArray[Coin_Legal_Tower[0], 1] == BoardClass.blank)
                        {
                            StartCoroutine(BurnTheCoin(Coin_Legal_Tower[0], Who));
                            Final_Position_OF_Coin = Coin_Legal_Tower[0] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[0]) + 1;
                            if (!UndoAction)
                            {
                                RListDeliverTower.Add(Coin_Legal_Tower[0]);
                                RListSelectedTower.Add(Coin_Legal_Tower[0]);
                                UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            }
                        }
                        else
                        if (BoardClass.boardArray[Coin_Legal_Tower[1], 0].CompareTag("Black_coin") && BoardClass.boardArray[Coin_Legal_Tower[1], 1] == BoardClass.blank)
                        {
                            StartCoroutine(BurnTheCoin(Coin_Legal_Tower[1], Who));
                            Final_Position_OF_Coin = Coin_Legal_Tower[1] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[1]) + 1;
                            if (!UndoAction)
                            {
                                RListDeliverTower.Add(Coin_Legal_Tower[1]);
                                RListSelectedTower.Add(Coin_Legal_Tower[1]);
                                UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            }

                            if (BoardClass.NumberOfBurntCoins(Who) == 0)
                                playerHasBurntCoin = false;
                        }
                        else
                        {
                            // Best_Tower_To_deliver_Coin : The best intermediate tower for transferring checker to destination tower
                            Final_Position_OF_Coin = BoardClass.Best_Tower_To_deliver_Coin * 15 + BoardClass.IndexOfCoinInSelectedTower(BoardClass.Best_Tower_To_deliver_Coin) + 1;
                            if (!UndoAction)
                            {
                                RListDeliverTower.Add(BoardClass.Best_Tower_To_deliver_Coin);
                                RListSelectedTower.Add(BoardClass.Best_Tower_To_deliver_Coin);
                                UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            }

                            if (BoardClass.NumberOfBurntCoins(Who) == 0)
                                playerHasBurntCoin = false;

                        }

                        DataManagerClass.coinImpactSnd.Play();
                        TransferCheckMade[2] = true;

                    }

                }

            }

            if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1]) && opposite_dice && DeliveryTower == Coin_Legal_Tower[1])
                TransferCheckMade[1] = true;

            if (TransferCheckMade[1])
            {
                if (even_dice) // in even dice situation 0.5f times gives opportunity that all transfers of coin in order gets done 
                    yield return new WaitForSecondsRealtime(0.5f);

                if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1]) | (even_dice && BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[2])))
                {
                    if (DeliveryTower == Coin_Legal_Tower[1])
                    {
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[1], Who));

                        yield return new WaitForSecondsRealtime(pauseTime);

                        Update_TargetTower(Tower_Number);

                        Final_Position_OF_Coin = Coin_Legal_Tower[1] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[1]);

                        StartCoroutine(Touchable_The_Coins());

                        DataManagerClass.coinImpactSnd.Play();

                        Invoke(nameof(Show_undo_btn), 1f);
                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[1]);
                            UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                        }

                    }

                    else if (even_dice)
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[1], Who));

                        Final_Position_OF_Coin = Coin_Legal_Tower[1] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[1]) + 1;
                        if (DeliveryTower == Coin_Legal_Tower[2] | DeliveryTower == Coin_Legal_Tower[3])
                            TransferCheckMade[2] = true;

                        DataManagerClass.coinImpactSnd.Play();

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[1]);
                            RListSelectedTower.Add(Coin_Legal_Tower[1]);
                            UndoMode.Add("regular");
                        }
                    }

                    if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[2]) && opposite_dice && DeliveryTower == Coin_Legal_Tower[2])
                        TransferCheckMade[2] = true;
                }
            }

            if (TransferCheckMade[2])
            {
                yield return new WaitForSecondsRealtime(0.5f);
                if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[2]))
                {
                    if (DeliveryTower == Coin_Legal_Tower[2])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[2], Who));

                        Update_TargetTower(Tower_Number);

                        Final_Position_OF_Coin = Coin_Legal_Tower[2] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[2]);
                        StartCoroutine(Touchable_The_Coins());

                        Invoke(nameof(Show_undo_btn), 1f);

                        DataManagerClass.coinImpactSnd.Play();
                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[2]);

                            if (!playerHasBurntCoin)
                                UndoMode.Add("regular");
                        }
                    }
                    else
                    if (DeliveryTower == Coin_Legal_Tower[3])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[2], Who));

                        Final_Position_OF_Coin = Coin_Legal_Tower[2] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[2]) + 1;
                        DataManagerClass.coinImpactSnd.Play();
                        TransferCheckMade[3] = true;

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[2]);
                            UndoMode.Add("regular");

                            if (even_dice)
                                RListSelectedTower.Add(Coin_Legal_Tower[2]);
                        }
                    }

                }

            }

            if (TransferCheckMade[3])
            {
                yield return new WaitForSecondsRealtime(0.5f);
                if (even_dice && BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[3]))
                {
                    yield return new WaitForSecondsRealtime(pauseTime);
                    StartCoroutine(BurnTheCoin(Coin_Legal_Tower[3], Who));

                    Update_TargetTower(Tower_Number);

                    Final_Position_OF_Coin = Coin_Legal_Tower[3] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[3]);
                    DataManagerClass.coinImpactSnd.Play();

                    if (!UndoAction)
                    {
                        RListDeliverTower.Add(Coin_Legal_Tower[3]);
                        UndoMode.Add("regular");
                    }

                    StartCoroutine(Touchable_The_Coins());
                }
            }

            if (even_dice)
            {
                if (DiceCheck[3]) // all the moves of player just done
                {
                    if (!Black_Bot)
                        removeDice = true;

                    BoardClass.CoinDefaultColor = true;
                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    Invoke(nameof(Show_undo_btn), 1f);

                    //against player could offer to double score 
                    if (Black_dbl_chk == 0 && PlayerPrefs.GetInt("mode") == 2 && num_double != 64)
                    {
                        if (PlayerPrefs.GetInt("dblcube") == 1 && num_double != 64)
                            doubleCube.layer = 9; // Dice Layer
                    }
                    else if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2; // ignore ray cast Layer

                    if (Black_Bot && num_double != 64)
                    {
                        yield return new WaitForSecondsRealtime(1.5f);
                        if (PlayerPrefs.GetInt("dblcube") == 1)
                            StartCoroutine(RobotClass.Offer_dbl_black());
                        else
                            DiceRollClass.ResetAllData();

                    }

                }

                if (!DiceCheck[0])
                {
                    if (PlayerPrefs.GetInt("undobtn") == 1)
                        DataManagerClass.Undo.gameObject.SetActive(false);
                }

            }

            if (opposite_dice)
            {
                if ((DiceCheck[0] && DiceCheck[1]) | DiceCheck[2])
                {
                    if (!Black_Bot)
                        removeDice = true;

                    BoardClass.CoinDefaultColor = true;
                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    if (Black_dbl_chk == 0 && PlayerPrefs.GetInt("mode") == 2)
                    {
                        if (PlayerPrefs.GetInt("dblcube") == 1 && num_double != 64)
                            doubleCube.layer = 9;
                    }
                    else
                    if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                    if (Black_Bot && player_number == 1 && num_double != 64)
                    {
                        yield return new WaitForSecondsRealtime(1.5f);
                        if (PlayerPrefs.GetInt("dblcube") == 1)
                            StartCoroutine(RobotClass.Offer_dbl_black());
                        else
                            DiceRollClass.ResetAllData();

                    }
                    Invoke(nameof(Show_undo_btn), 1f);
                }

            }


        }

        //--------black zone---> -----------------> ---------------------------------------------------------------------------------------

        if (player_number == 2 && !Black_Bot)
        {
            if (BoardClass.Is_The_Delivery_Tower_Free(BoardClass.Best_Tower_To_deliver_Coin) | (even_dice && BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1])) | playerHasBurntCoin) // | playerHasBurntCoin : new code for an error added
            {
                if (DeliveryTower == Coin_Legal_Tower[0])
                {

                    yield return new WaitForSecondsRealtime(pauseTime);
                    StartCoroutine(BurnTheCoin(Coin_Legal_Tower[0], Who));

                    Update_TargetTower(Tower_Number);

                    Final_Position_OF_Coin = Coin_Legal_Tower[0] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[0]);
                    StartCoroutine(Touchable_The_Coins());


                    Invoke(nameof(Show_undo_btn), 1f);

                    if (!UndoAction)
                    {
                        RListDeliverTower.Add(Coin_Legal_Tower[0]);
                        UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                    }
                    DataManagerClass.coinImpactSnd.Play();
                }

                else
                {
                    if (even_dice)
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[0], Who));

                        Final_Position_OF_Coin = Coin_Legal_Tower[0] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[0]) + 1;
                        DataManagerClass.coinImpactSnd.Play();

                        if (DeliveryTower == Coin_Legal_Tower[1] | DeliveryTower == Coin_Legal_Tower[2] | DeliveryTower == Coin_Legal_Tower[3])
                            TransferCheckMade[1] = true;

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[0]);
                            RListSelectedTower.Add(Coin_Legal_Tower[0]);
                            UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                        }

                        if (BoardClass.NumberOfBurntCoins(Who) == 0)
                            playerHasBurntCoin = false;

                    }

                    else if (DeliveryTower == Coin_Legal_Tower[2])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        if (BoardClass.boardArray[Coin_Legal_Tower[0], 0].CompareTag("White_coin") && BoardClass.boardArray[Coin_Legal_Tower[0], 1] == BoardClass.blank)
                        {
                            StartCoroutine(BurnTheCoin(Coin_Legal_Tower[0], Who));
                            Final_Position_OF_Coin = Coin_Legal_Tower[0] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[0]) + 1;

                            if (!UndoAction)
                            {
                                RListDeliverTower.Add(Coin_Legal_Tower[0]);
                                RListSelectedTower.Add(Coin_Legal_Tower[0]);
                                UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            }

                        }

                        else

                        if (BoardClass.boardArray[Coin_Legal_Tower[1], 0].CompareTag("White_coin") && BoardClass.boardArray[Coin_Legal_Tower[1], 1] == BoardClass.blank)
                        {
                            StartCoroutine(BurnTheCoin(Coin_Legal_Tower[1], Who));
                            Final_Position_OF_Coin = Coin_Legal_Tower[1] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[1]) + 1;

                            if (!UndoAction)
                            {
                                RListDeliverTower.Add(Coin_Legal_Tower[1]);
                                RListSelectedTower.Add(Coin_Legal_Tower[1]);
                                UndoMode.Add("regular");
                                UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            }

                            if (BoardClass.NumberOfBurntCoins(Who) == 0)
                                playerHasBurntCoin = false;

                        }
                        else
                        {
                            Final_Position_OF_Coin = BoardClass.Best_Tower_To_deliver_Coin * 15 + BoardClass.IndexOfCoinInSelectedTower(BoardClass.Best_Tower_To_deliver_Coin) + 1;

                            if (!UndoAction)
                            {
                                RListDeliverTower.Add(BoardClass.Best_Tower_To_deliver_Coin);
                                RListSelectedTower.Add(BoardClass.Best_Tower_To_deliver_Coin);
                                UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                            }

                            if (BoardClass.NumberOfBurntCoins(Who) == 0)
                                playerHasBurntCoin = false;
                        }

                        DataManagerClass.coinImpactSnd.Play();
                        TransferCheckMade[2] = true;
                    }

                }
            }

            if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1]) && Dice_number[0] != Dice_number[1] && DeliveryTower == Coin_Legal_Tower[1])
                TransferCheckMade[1] = true;

            if (TransferCheckMade[1])
            {
                if (even_dice)
                    yield return new WaitForSecondsRealtime(0.5f); // in even dice situation 0.5f times gives opportunity that all transfers of coin in order gets done 

                if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1]) | (even_dice && BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[2])))
                {
                    if (DeliveryTower == Coin_Legal_Tower[1])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[1], Who));

                        Update_TargetTower(Tower_Number);

                        Final_Position_OF_Coin = Coin_Legal_Tower[1] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[1]);

                        StartCoroutine(Touchable_The_Coins());

                        DataManagerClass.coinImpactSnd.Play();


                        Invoke(nameof(Show_undo_btn), 1f);

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[1]);
                            UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                        }

                    }
                    else if (even_dice)
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[1], Who));

                        Final_Position_OF_Coin = Coin_Legal_Tower[1] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[1]) + 1;

                        if (DeliveryTower == Coin_Legal_Tower[2] | DeliveryTower == Coin_Legal_Tower[3])
                            TransferCheckMade[2] = true;

                        DataManagerClass.coinImpactSnd.Play();

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[1]);
                            RListSelectedTower.Add(Coin_Legal_Tower[1]);
                            UndoMode.Add("regular");
                        }
                    }

                    if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[2]) && Dice_number[0] != Dice_number[1] && DeliveryTower == Coin_Legal_Tower[2])
                        TransferCheckMade[2] = true;
                }
            }
            if (TransferCheckMade[2])
            {
                yield return new WaitForSecondsRealtime(0.5f);
                if (BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[2]))
                {
                    if (DeliveryTower == Coin_Legal_Tower[2])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[2], Who));

                        Update_TargetTower(Tower_Number);

                        Final_Position_OF_Coin = Coin_Legal_Tower[2] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[2]);

                        StartCoroutine(Touchable_The_Coins());


                        Invoke(nameof(Show_undo_btn), 1f);

                        DataManagerClass.coinImpactSnd.Play();

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[2]);
                            UndoMode.Add(playerHasBurntCoin ? "burn" : "regular");
                        }
                    }
                    else
                    if (DeliveryTower == Coin_Legal_Tower[3])
                    {
                        yield return new WaitForSecondsRealtime(pauseTime);
                        StartCoroutine(BurnTheCoin(Coin_Legal_Tower[2], Who));

                        Final_Position_OF_Coin = Coin_Legal_Tower[2] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[2]) + 1;

                        DataManagerClass.coinImpactSnd.Play();
                        TransferCheckMade[3] = true;

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Coin_Legal_Tower[2]);
                            UndoMode.Add("regular");

                            if (even_dice)
                                RListSelectedTower.Add(Coin_Legal_Tower[2]);
                        }
                    }

                }

            }
            if (TransferCheckMade[3])
            {
                yield return new WaitForSecondsRealtime(0.5f);
                if (even_dice && BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[3]))
                {
                    yield return new WaitForSecondsRealtime(pauseTime);
                    StartCoroutine(BurnTheCoin(Coin_Legal_Tower[3], Who));

                    Update_TargetTower(Tower_Number);

                    Final_Position_OF_Coin = Coin_Legal_Tower[3] * 15 + BoardClass.IndexOfCoinInSelectedTower(Coin_Legal_Tower[3]);

                    DataManagerClass.coinImpactSnd.Play();

                    StartCoroutine(Touchable_The_Coins());

                    if (!UndoAction)
                    {
                        RListDeliverTower.Add(Coin_Legal_Tower[3]);
                        UndoMode.Add("regular");
                    }

                }

            }

            if (even_dice)
            {
                if (DiceCheck[3])
                {
                    removeDice = true;

                    BoardClass.CoinDefaultColor = true;
                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    Invoke(nameof(Show_undo_btn), 1f);

                    if (White_dbl_chk == 0)
                    {
                        if (PlayerPrefs.GetInt("dblcube") == 1 && num_double != 64)
                            doubleCube.layer = 9;
                    }
                    else
                    if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                }

                if (!DiceCheck[0])
                {
                    if (PlayerPrefs.GetInt("undobtn") == 1)
                        DataManagerClass.Undo.gameObject.SetActive(false);
                }
            }

            if (opposite_dice)
            {
                if ((DiceCheck[0] && DiceCheck[1]) | DiceCheck[2])
                {
                    removeDice = true;

                    BoardClass.CoinDefaultColor = true;
                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    Invoke(nameof(Show_undo_btn), 1f);

                    if (White_dbl_chk == 0 && num_double != 64)
                        doubleCube.layer = 9;
                    else if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                }

            }

        }


    }
    private IEnumerator Transfer_BearOutCoin() // makes Checkers to move in BearOut section
    {
        if (PlayerPrefs.GetInt("undobtn") == 1)
            DataManagerClass.Undo.gameObject.SetActive(false);

        if (PlayerPrefs.GetInt("dblcube") == 1)
            doubleCube.layer = 2;

        xcoin_bear_out = Trf_XForm_BearOut.transform;

        if (!playerHasBurntCoin)
            Final_Position_OF_Coin = SelectedTowerNumber * 15 + Coin_Index; // synchronize the position of moving coin into bear out section

        Update_board(true, SelectedCoin.gameObject); //this function will update the situation of Checkers in the backgammon (board)

        if (BoardClass.Coin_step_by_step) // Checkers just get one move
        {
            if (!UndoAction)
            {
                UndoMode.Add("bearout");
                RListBeardOutTower.Add(SelectedTowerNumber); // stores the index of selected tower for bear out
            }

            xcoin_bear_out = SelectedCoin.transform;

            if (player_number == 1)
            {
                White_beardout_Coin[W_win] = SelectedCoin.gameObject; // White_beardout_Coin[W_win(0)] equals to last beard out Checker W_win will increase each time player bear out a Checker
                White_beardout_Coin[W_win].GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                Black_Beardout_coins[B_win] = SelectedCoin.gameObject;
                Black_Beardout_coins[B_win].GetComponent<BoxCollider>().enabled = false;
            }

            StartCoroutine(ToQueue());

            if (DiceCheck[0] | DiceCheck[1] | DiceCheck[2] | DiceCheck[3])
                Invoke(nameof(Show_undo_btn), 1f);

            StartCoroutine(Touchable_The_Coins());

            //---------------------------------------->turn_ON_OFF

            if (opposite_dice && (DiceCheck[0] && DiceCheck[1]) | DiceCheck[2])
            {
                yield return new WaitForSecondsRealtime(1f); //0.5f

                xcoin_burn = Trf_XForm_burn.transform;
                xcoin_bear_out = Trf_XForm_BearOut.transform;

                if ((player_number == 1 && !Black_Bot) | player_number == 2)
                    removeDice = true;

                DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
                DiceRollClass.particleRemoveDice1Alarm.Play();
                DiceRollClass.particleRemoveDice2Alarm.Play();

                if ((player_number == 1 && !Black_Bot && Black_dbl_chk == 0) | (player_number == 2 && White_dbl_chk == 0) && num_double != 64)
                    doubleCube.layer = 9;
                else
                if (PlayerPrefs.GetInt("dblcube") == 1)
                    doubleCube.layer = 2;

                if (Black_Bot)
                {
                    yield return new WaitForSecondsRealtime(1.5f);
                    DiceRollClass.ResetAllData();
                }

            }

            if (even_dice && DiceCheck[3])
            {
                yield return new WaitForSecondsRealtime(1f);

                xcoin_burn = Trf_XForm_burn.transform;
                xcoin_bear_out = Trf_XForm_BearOut.transform;

                if ((player_number == 1 && !Black_Bot) | player_number == 2)
                    removeDice = true;

                DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
                DiceRollClass.particleRemoveDice1Alarm.Play();
                DiceRollClass.particleRemoveDice2Alarm.Play();

                if (Black_Bot)
                    DiceRollClass.ResetAllData();

                if ((player_number == 1 && !Black_Bot && Black_dbl_chk == 0) | (player_number == 2 && White_dbl_chk == 0) && num_double != 64)
                    doubleCube.layer = 9;
                else if (PlayerPrefs.GetInt("dblcube") == 1)
                    doubleCube.layer = 2;
            }

            DataManagerClass.coinImpactSnd.Play();
        }

        //coin_auto_step --------------------------------------------------------------------------------------

        if (BoardClass.coin_auto_step) //all the steps of Checker move in one step automatically ://coin_auto_step will use Transfer_Coin_To_DeliveryTower functions too for delivering Checker to bear out section
        {
            RListSelectedTower.Add(SelectedTowerNumber);

            if (player_number == 1)
            {
                xcoin_point = SelectedCoin.transform;

                if (opposite_dice)
                {
                    Tower_Number = BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1]) ? Coin_Legal_Tower[1] : Coin_Legal_Tower[0];

                    StartCoroutine(BurnTheCoin(Tower_Number, Who));

                    yield return new WaitForSecondsRealtime(pauseTime);
                    Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;

                    if (!UndoAction)
                    {
                        RListDeliverTower.Add(Tower_Number);
                        UndoMode.Add("regular");

                        RListBeardOutTower.Add(Tower_Number);
                        UndoMode.Add("bearout");
                    }

                    StartCoroutine(Touchable_The_Coins());

                    DataManagerClass.coinImpactSnd.Play();
                }

                //--------------------------------------------------------------------------------------------------------------------------

                if (even_dice)
                {
                    if (Dice_number[0] == 1)
                    {
                        //SwitchBearOutTwr : just like TransferCheckMade[] Controls
                        //the movement of Checker,in each time specifies that Checker should pass from which tower
                        if (SelectedTowerNumber == 22) //SelectedTower Number determines which tower must be temporarily get used for transferring the checker
                            SwitchBearOutTwr[1] = SwitchBearOutTwr[2] = false;

                        if (SelectedTowerNumber == 21)
                            SwitchBearOutTwr[2] = false;
                    }

                    if (Dice_number[0] == 2)
                    {
                        if (SelectedTowerNumber == 20)
                            SwitchBearOutTwr[1] = SwitchBearOutTwr[2] = false;

                        if (SelectedTowerNumber == 18)
                            SwitchBearOutTwr[2] = false;
                    }

                    if (Dice_number[0] == 3 && SelectedTowerNumber == 18)
                        SwitchBearOutTwr[1] = SwitchBearOutTwr[2] = false;

                    if (SwitchBearOutTwr[0])
                    {
                        yield return new WaitForSecondsRealtime(0.1f);
                        Tower_Number = Coin_Legal_Tower[0];

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Tower_Number);
                            UndoMode.Add("regular");

                            if (!SwitchBearOutTwr[1])
                            {
                                RListBeardOutTower.Add(Tower_Number);
                                UndoMode.Add("bearout");
                            }
                            else
                                RListSelectedTower.Add(Tower_Number);
                        }

                        StartCoroutine(BurnTheCoin(Tower_Number, Who));

                        yield return new WaitForSecondsRealtime(pauseTime);
                        Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;
                        DataManagerClass.coinImpactSnd.Play();
                    }

                    if (SwitchBearOutTwr[1])
                    {
                        yield return new WaitForSecondsRealtime(0.2f);
                        Tower_Number = Coin_Legal_Tower[1];

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Tower_Number);
                            UndoMode.Add("regular");

                            if (!SwitchBearOutTwr[2])
                            {
                                RListBeardOutTower.Add(Tower_Number);
                                UndoMode.Add("bearout");
                            }
                            else
                                RListSelectedTower.Add(Tower_Number);

                        }

                        StartCoroutine(BurnTheCoin(Tower_Number, Who));

                        yield return new WaitForSecondsRealtime(pauseTime);
                        Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;
                        DataManagerClass.coinImpactSnd.Play();
                    }

                    if (SwitchBearOutTwr[2])
                    {
                        yield return new WaitForSecondsRealtime(0.3f);
                        Tower_Number = Coin_Legal_Tower[2];
                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Tower_Number);
                            RListBeardOutTower.Add(Tower_Number);
                            UndoMode.Add("regular");
                            UndoMode.Add("bearout");
                        }
                        StartCoroutine(BurnTheCoin(Tower_Number, Who));

                        yield return new WaitForSecondsRealtime(pauseTime);
                        Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;
                        DataManagerClass.coinImpactSnd.Play();
                    }

                }

                yield return new WaitForSecondsRealtime(0.5f); // Interruption for each transfer

                xcoin_point = Trf_XForm.transform;
                xcoin_bear_out = SelectedCoin.transform;
                White_beardout_Coin[W_win] = SelectedCoin.gameObject;

                White_beardout_Coin[W_win].GetComponent<BoxCollider>().enabled = false;

                StartCoroutine(ToQueue());
                StartCoroutine(Touchable_The_Coins());

                if (DiceCheck[0] | DiceCheck[1] | DiceCheck[2] | DiceCheck[3])
                    yield return new WaitForSecondsRealtime(0.5f);

                Invoke(nameof(Show_undo_btn), 1f);

                if (even_dice && DiceCheck[3])
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    xcoin_point = Trf_XForm.transform;
                    xcoin_burn = Trf_XForm_burn.transform;
                    xcoin_bear_out = Trf_XForm_BearOut.transform;

                    if (PlayerPrefs.GetInt("dblcube") == 1 && Black_dbl_chk == 0 && num_double != 64)
                        doubleCube.layer = 9;
                    else
                    if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                    if ((player_number == 1 && !Black_Bot) | player_number == 2)
                        removeDice = true;

                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    yield return new WaitForSecondsRealtime(1.5f);
                    if (Black_Bot)
                        DiceRollClass.ResetAllData();
                }

                if (opposite_dice && (DiceCheck[0] && DiceCheck[1]) | DiceCheck[2])
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    xcoin_point = Trf_XForm.transform;
                    xcoin_burn = Trf_XForm_burn.transform;
                    xcoin_bear_out = Trf_XForm_BearOut.transform;

                    if (PlayerPrefs.GetInt("dblcube") == 1 && Black_dbl_chk == 0 && num_double != 64)
                        doubleCube.layer = 9;
                    else
                    if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                    if ((player_number == 1 && !Black_Bot) | player_number == 2)
                        removeDice = true;

                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    yield return new WaitForSecondsRealtime(1.5f);
                    if (Black_Bot)
                        DiceRollClass.ResetAllData();

                }
            }

            //---------------------------Black

            if (player_number == 2 && Tower_obj.CompareTag("B_win_tower"))
            {
                xcoin_point = SelectedCoin.transform;
                if (opposite_dice)
                {
                    Tower_Number = BoardClass.Is_The_Delivery_Tower_Free(Coin_Legal_Tower[1]) ? Coin_Legal_Tower[1] : Coin_Legal_Tower[0];

                    StartCoroutine(BurnTheCoin(Tower_Number, Who));

                    yield return new WaitForSecondsRealtime(pauseTime);
                    Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;

                    if (!UndoAction)
                    {
                        RListDeliverTower.Add(Tower_Number);
                        UndoMode.Add("regular");

                        RListBeardOutTower.Add(Tower_Number);
                        UndoMode.Add("bearout");
                    }


                    DataManagerClass.coinImpactSnd.Play();
                    StartCoroutine(Touchable_The_Coins());

                }

                //--------------------------------------------------------------------------------------------------------------------------

                if (even_dice)
                {
                    if (Dice_number[0] == 1)
                    {
                        if (SelectedTowerNumber == 1)
                            SwitchBearOutTwr[1] = SwitchBearOutTwr[2] = false;

                        if (SelectedTowerNumber == 2)
                            SwitchBearOutTwr[2] = false;

                    }

                    if (Dice_number[0] == 2)
                    {

                        if (SelectedTowerNumber == 3)
                            SwitchBearOutTwr[1] = SwitchBearOutTwr[2] = false;

                        if (SelectedTowerNumber == 5)
                            SwitchBearOutTwr[2] = false;

                    }

                    if (Dice_number[0] == 3 && SelectedTowerNumber == 5)
                    {
                        SwitchBearOutTwr[1] = SwitchBearOutTwr[2] = false;
                    }


                    if (SwitchBearOutTwr[0])
                    {
                        yield return new WaitForSecondsRealtime(0.1f);
                        Tower_Number = Coin_Legal_Tower[0];

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Tower_Number);
                            UndoMode.Add("regular");

                            if (!SwitchBearOutTwr[1])
                            {
                                RListBeardOutTower.Add(Tower_Number);
                                UndoMode.Add("bearout");
                            }
                            else
                                RListSelectedTower.Add(Tower_Number);
                        }

                        StartCoroutine(BurnTheCoin(Tower_Number, Who));

                        yield return new WaitForSecondsRealtime(pauseTime);
                        Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;
                        //	GameObject.Find ("Coin_move").GetComponent<AudioSource> ().Play ();
                    }

                    if (SwitchBearOutTwr[1])
                    {
                        yield return new WaitForSecondsRealtime(0.2f);
                        Tower_Number = Coin_Legal_Tower[1];

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Tower_Number);
                            UndoMode.Add("regular");

                            if (!SwitchBearOutTwr[2])
                            {
                                RListBeardOutTower.Add(Tower_Number);
                                UndoMode.Add("bearout");
                            }
                            else
                                RListSelectedTower.Add(Tower_Number);
                        }

                        StartCoroutine(BurnTheCoin(Tower_Number, Who));

                        yield return new WaitForSecondsRealtime(pauseTime);
                        Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;
                        DataManagerClass.coinImpactSnd.Play();
                    }

                    if (SwitchBearOutTwr[2])
                    {
                        yield return new WaitForSecondsRealtime(0.3f);
                        Tower_Number = Coin_Legal_Tower[2];

                        if (!UndoAction)
                        {
                            RListDeliverTower.Add(Tower_Number);
                            RListBeardOutTower.Add(Tower_Number);
                            UndoMode.Add("regular");
                            UndoMode.Add("bearout");
                        }

                        StartCoroutine(BurnTheCoin(Tower_Number, Who));

                        yield return new WaitForSecondsRealtime(pauseTime);
                        Final_Position_OF_Coin = Tower_Number * 15 + BoardClass.IndexOfCoinInSelectedTower(Tower_Number) + 1;

                        DataManagerClass.coinImpactSnd.Play();
                    }

                }

                yield return new WaitForSecondsRealtime(0.5f);

                xcoin_point = Trf_XForm.transform;
                xcoin_bear_out = SelectedCoin.transform;

                Black_Beardout_coins[B_win] = SelectedCoin.gameObject;

                Black_Beardout_coins[B_win].GetComponent<BoxCollider>().enabled = false;

                StartCoroutine(ToQueue());
                StartCoroutine(Touchable_The_Coins());


                //				if (DiceCheck [0] | DiceCheck [1] | DiceCheck [2]  | DiceCheck [3] ) // before it was in use
                //					yield return new WaitForSecondsRealtime (0.4f);

                Invoke(nameof(Show_undo_btn), 1f);

                //Turn_ON_OFF
                yield return new WaitForSecondsRealtime(0.5f);

                if (even_dice && DiceCheck[3])
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    xcoin_point = Trf_XForm.transform;
                    xcoin_burn = Trf_XForm_burn.transform;
                    xcoin_bear_out = Trf_XForm_BearOut.transform;

                    if ((player_number == 1 && !Black_Bot) | player_number == 2)
                        removeDice = true;

                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    yield return new WaitForSecondsRealtime(1.5f);

                    if (PlayerPrefs.GetInt("dblcube") == 1 && !Black_Bot && White_dbl_chk == 0 && num_double != 64)
                        doubleCube.layer = 9;
                    else
                    if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                }

                if (opposite_dice && (DiceCheck[0] && DiceCheck[1]) | DiceCheck[2])
                {
                    xcoin_point = Trf_XForm.transform;
                    xcoin_burn = Trf_XForm_burn.transform;
                    xcoin_bear_out = Trf_XForm_BearOut.transform;

                    if ((player_number == 1 && !Black_Bot) | player_number == 2)
                        removeDice = true;

                    DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
                    DiceRollClass.particleRemoveDice1Alarm.Play();
                    DiceRollClass.particleRemoveDice2Alarm.Play();

                    yield return new WaitForSecondsRealtime(1.5f);
                    if (PlayerPrefs.GetInt("dblcube") == 1 && White_dbl_chk == 0 && num_double != 64)
                        doubleCube.layer = 9;
                    else
                    if (PlayerPrefs.GetInt("dblcube") == 1)
                        doubleCube.layer = 2;

                }
            }
        }
    }
    public IEnumerator BurnTheCoin(int hotTower, string who) // process of burning the checkers
    {
        var playerOpposite = who;

        if (player_number == 1 | (player_number == 2 && !Black_Bot)) // It is necessary to change the "who" variable so that it is possible to burn the opponent's Checker
            playerOpposite = who == "White" ? "Black" : "White"; // because "Who" variable in Switch Players function is equals to nature of the current Player

        if (playerOpposite == "Black")
        {
            // checks everything is alright for burn
            if (hotTower < 24 && hotTower > -1 && BoardClass.boardArray[hotTower, 0].CompareTag("Black_coin") && BoardClass.boardArray[hotTower, 1] == BoardClass.blank)
            {
                pauseTime = 0.5f;
                xcoin_burn = BoardClass.boardArray[hotTower, 0].transform; // transfers checker to the Burn section
                for (int i = 0; i < 15; i++)
                    if (Container_Of_BC_burnt[i] == null)
                    {
                        Num_Of_BC_Burnt = i; // index of burn array
                        break;
                    }

                if (!UndoAction)
                    RListBurnTower.Add(hotTower);

                Container_Of_BC_burnt[Num_Of_BC_Burnt] = xcoin_burn.gameObject;
                BoardClass.boardArray[hotTower, 0] = BoardClass.blank;
                cam_shake.canUpdate = cam_shake.isPlaying = true;
                black_coin_move_counter += 24 - hotTower;
                DataManagerClass.blackMoveCounterTxt.text = black_coin_move_counter.ToString();
                burn_event = true;
                DataManagerClass.noMoveSnd.Play();
                if (BoardClass.boardArray[hotTower, 0] == BoardClass.blank)
                {
                    yield return new WaitForSecondsRealtime(1f);
                    xcoin_burn = Trf_XForm_burn.transform;
                    burn_event = false;
                }

            }
            else
                pauseTime = 0;
        }

        else if (playerOpposite == "White")
        {
            if (hotTower < 24 && hotTower > -1 && BoardClass.boardArray[hotTower, 0].CompareTag("White_coin") && BoardClass.boardArray[hotTower, 1] == BoardClass.blank)
            {

                burn_event = true;
                pauseTime = 0.5f;
                xcoin_burn = BoardClass.boardArray[hotTower, 0].transform;

                for (int i = 0; i < 15; i++)
                    if (Container_Of_WC_burnt[i] == null)
                    {
                        Num_Of_WC_Burnt = i;
                        break;
                    }

                if (!UndoAction)
                    RListBurnTower.Add(hotTower);

                Container_Of_WC_burnt[Num_Of_WC_Burnt] = xcoin_burn.gameObject;

                BoardClass.boardArray[hotTower, 0] = BoardClass.blank;

                cam_shake.canUpdate = cam_shake.isPlaying = true;

                white_coin_move_counter += hotTower + 1;

                DataManagerClass.whiteMoveCounterTxt.text = white_coin_move_counter.ToString();
                DataManagerClass.noMoveSnd.Play();

                if (BoardClass.boardArray[hotTower, 0] == BoardClass.blank)
                {
                    yield return new WaitForSecondsRealtime(1f);
                    xcoin_burn = Trf_XForm_burn.transform;
                    burn_event = false;
                }

            }
            else
                pauseTime = 0;
        }
    }
    public IEnumerator ToQueue() // flipping checkers in Bear out section
    {

        yield return new WaitForSecondsRealtime(0.3f);
        xcoin_bear_out.GetComponent<Renderer>().enabled = false;
        UnTouchable_The_Coins(); // it happens less than 1 sec
        xcoin_bear_out = Trf_XForm_BearOut.transform;
        StartCoroutine(Touchable_The_Coins()); //checkers that are not in bear out section gets touchable

        if (player_number == 1)
        {
            White_Bearout_points[W_win].GetComponent<Renderer>().enabled = true;
            W_win++;
        }
        else
        {
            Black_Bearout_points[B_win].GetComponent<Renderer>().enabled = true;
            B_win++;

        }

        Check_if_W_Or_B_Could_Be_winner();

    }
    private void UnTouchable_The_Coins()
    {
        for (int i = 0; i < 15; i++)
        {
            BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
            BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = false;
        }
    }
    IEnumerator Touchable_The_Coins()
    {
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < 15; i++)
        {
            if (player_number == 1)
                BoardClass.WhiteCoinsContainer[i].GetComponent<BoxCollider>().enabled = true;

            if (player_number == 2 && !Black_Bot)
                BoardClass.BlackCoinsContainer[i].GetComponent<BoxCollider>().enabled = true;
        }
    }
    private void Lock_BeardOut_Coins() // will locks those checkers that are placed in Bear out
    {
        int brk;
        if (player_number == 1 && White_beardout_Coin[0] != null)

        {
            for (brk = 0; brk < 15; brk++)
                if (White_beardout_Coin[brk] == null)
                    break;

            for (int i = 0; i < brk; i++)
                White_beardout_Coin[i].GetComponent<BoxCollider>().enabled = false;

        }

        if (player_number == 2 && Black_Beardout_coins[0] != null)
        {
            for (brk = 0; brk < 15; brk++)
                if (Black_Beardout_coins[brk] == null)
                    break;

            for (int i = 0; i < brk; i++)
                Black_Beardout_coins[i].GetComponent<BoxCollider>().enabled = false;

        }

    }
    #endregion
    public void Update_board(bool allowUpdate, GameObject coin) //will update all coins situation in the board by updating board_array
    {
        if (allowUpdate)
            for (int i = 0; i < 15; i++)
            {
                if (BoardClass.boardArray[SelectedTowerNumber, i] == coin) //finds the place of the coin in the SelectedTower then fill it by Blank object
                {
                    BoardClass.boardArray[SelectedTowerNumber, i] = BoardClass.blank;
                    break;
                }
            }

        if (!allowUpdate) // burn position
        {
            if (player_number == 1)
            {
                Container_Of_WC_burnt[Num_Of_WC_Burnt] = null;

                for (int i = 14; i >= 0; i--)
                    if (Container_Of_WC_burnt[i] != null)
                    {
                        Num_Of_WC_Burnt = i;
                        break;
                    }
            }

            if (player_number == 2)
            {
                Container_Of_BC_burnt[Num_Of_BC_Burnt] = null;

                for (int i = 14; i >= 0; i--)
                    if (Container_Of_BC_burnt[i] != null)
                    {
                        Num_Of_BC_Burnt = i;
                        break;
                    }
            }
        }
    }
    public void Update_TargetTower(int tower) // rvb
    {
        for (int targetTwr = 0; targetTwr < 15; targetTwr++)
            if (BoardClass.boardArray[tower, targetTwr].CompareTag("blank")) // finds the row of tower that filled by blank game object
            {
                BoardClass.boardArray[tower, targetTwr] = SelectedCoin.gameObject;
                break;
            }

        //	speed = 0f;
    }
    private void Calculate_Dice_check() // shows to the player how many move can make
    {
        //Rolling a dice can have 6 outcomes, 1,2,3,4,5,6 as each number is written on different
        //sides of the cubic shaped dice. The likelihood of getting any number is equal as
        //the all sides of a dice is equal in area. Hence, all sides are equally likely to come when a dice is rolled.

        int numberOfDices, sumDice = 0, sumDiceBurnMode = 0, sumDiceBearoutMode = 0;

        numberOfDices = even_dice ? 3 : 2; // if dice are equal in value so player have 4 moves, otherwise 3 has moves

        if (playerHasBurntCoin && even_dice)
        {
            for (int i = 0; i < Coin_Legal_Tower.Length; i++) // by temp_coin_legal[] array we could limit number of player moves
                temp_coin_legal[i] = Coin_Legal_Tower[i];

            if (DiceCheck[0] && !DiceCheck[1] && !DiceCheck[2]) // DiceCheck[0] == true means: player made its first move
            {
                // now possible moves will be limited to 3 tower (you can check this array in play mode when even_dice is true)
                temp_coin_legal[2] = temp_coin_legal[1];
                temp_coin_legal[1] = temp_coin_legal[0];
            }

            if (DiceCheck[0] && DiceCheck[1] && !DiceCheck[2])
            {
                //now possible moves will be limited to 2 tower
                temp_coin_legal[3] = temp_coin_legal[2];
                temp_coin_legal[2] = temp_coin_legal[1] = temp_coin_legal[0];
            }

            if (DiceCheck[0] && DiceCheck[1] && DiceCheck[2])
                temp_coin_legal[3] = temp_coin_legal[2] = temp_coin_legal[1] =
                    temp_coin_legal[0];
        }

        for (diceChecker = 0; diceChecker < numberOfDices; diceChecker++)
        {
            if (opposite_dice && playerHasBurntCoin) //relative to burn coins 
                sumDiceBurnMode = player_number == 1 ? Dice_number[diceChecker] - 1 : 24 - Dice_number[diceChecker]; // determines delivery tower. if player 2 wants to bring back a

            // coin from burn section to the its home (18th tower till 23) for example dice are D1: 2, D2,3 and wants to choose the tower that relative to D2 value 24 - 3 = 21 
            //so Dice_number[diceChecker(1)] equals to 21

            if (even_dice && playerHasBurntCoin) //relative to burn coins
                sumDiceBurnMode = temp_coin_legal[diceChecker];


            if (WhiteBearOut && Selected_obj.CompareTag("W_win_tower") && !DiceCheck[diceChecker])
                sumDiceBearoutMode = Bearout_Delivery_Tower[BoardClass.tb_W];

            if (BlackBearOut && Selected_obj.CompareTag("B_win_tower") && !DiceCheck[diceChecker])
                sumDiceBearoutMode = Bearout_Delivery_Tower[BoardClass.tb_B];


            if (!UndoAction) //regular mode of the game
            {
                if (!DiceCheck[diceChecker] && opposite_dice)
                    sumDice = Dice_number[diceChecker];

                if (!DiceCheck[diceChecker] && even_dice)
                    sumDice += Dice_number[diceChecker];
            }
            else // undo mode of the game : 
            {
                if (DiceCheck[diceChecker] && opposite_dice)
                    sumDice = Dice_number[diceChecker]; //it makes value of each dice check index to false

                if (DiceCheck[diceChecker] && even_dice)
                    sumDice += Dice_number[diceChecker];

            }

            if (player_number == 1)
            {
                //determines which index of dice check must be equal to true

                //specifies which tower selected for next destination of  Checker. actually this code will block the previous delivery tower for next chooses
                if (!UndoAction && sumDice + SelectedTowerNumber == DeliveryTower && !DiceCheck[diceChecker] && Tower_obj.CompareTag("Tower") && BoardClass.NumberOfBurntCoins(Who) == 0)
                    break;

                //it going to uncheck the relative DiceCheck of selected tower
                if (UndoAction && sumDice == SelectedTowerNumber - DeliveryTower && DiceCheck[diceChecker] && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "regular")
                    break;

                //relative to burn checker
                if (!UndoAction && sumDiceBurnMode == DeliveryTower && !DiceCheck[diceChecker] && Tower_obj.CompareTag("Tower") && BoardClass.NumberOfBurntCoins(Who) > 0)
                    break;

                if (UndoAction && sumDiceBurnMode == SelectedTowerNumber && DiceCheck[diceChecker] && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                    break;


                //relative to bear out
                if (!UndoAction && sumDiceBearoutMode == Bearout_Delivery_Tower[diceChecker] && !DiceCheck[diceChecker] && Tower_obj.CompareTag("W_win_tower"))
                    break;

                if (UndoAction && 24 - SelectedTowerNumber == Dice_number[diceChecker] && DiceCheck[diceChecker] && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "bearout")
                    break;

            }
            else
            {
                if (!UndoAction && SelectedTowerNumber - sumDice == DeliveryTower && !DiceCheck[diceChecker] && Tower_obj.CompareTag("Tower") && BoardClass.NumberOfBurntCoins(Who) == 0)
                    break;

                if (UndoAction && sumDice == DeliveryTower - SelectedTowerNumber && DiceCheck[diceChecker] && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "regular")
                    break;

                if (!UndoAction && sumDiceBurnMode == DeliveryTower && !DiceCheck[diceChecker] && Tower_obj.CompareTag("Tower") && BoardClass.NumberOfBurntCoins(Who) > 0)
                    break;


                if (UndoAction && sumDiceBurnMode == SelectedTowerNumber && DiceCheck[diceChecker] && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                    break;


                if (!UndoAction && sumDiceBearoutMode == Bearout_Delivery_Tower[diceChecker] && !DiceCheck[diceChecker] && Tower_obj.CompareTag("B_win_tower"))
                    break;

                if (UndoAction && SelectedTowerNumber - Dice_number[diceChecker] == -1 && DiceCheck[diceChecker] && UndoMode.Count > 0 && UndoMode.ElementAt(UndoMode.Count - 1) == "bearout")
                    break;

            }
        }

        if (!UndoAction)
        {
            if (even_dice)
            {
                for (int check = 0; check <= diceChecker; check++)
                    DiceCheck[check] = true;
            }
            else
            {
                if (diceChecker <= 1)
                    DiceCheck[diceChecker] = true;
                else
                    for (int check = 0; check <= diceChecker; check++)
                        DiceCheck[check] = true;
            }
        }
        else
        {
            if (even_dice)
            {
                for (int check = numberOfDices; check >= 0; check--)
                {
                    if (DiceCheck[check])
                    {
                        DiceCheck[check] = false;
                        break;
                    }
                }
            }
            else
            {
                if (DiceCheck[2])
                    DiceCheck[2] = false;

                if (diceChecker <= 1)
                    DiceCheck[diceChecker] = false;
                else
                    for (int check = 0; check <= diceChecker; check++)
                        DiceCheck[check] = false;
            }
        }

        if (!UndoAction)
        {
            if (DiceCheck[0])
                GameObject.Find("D1").GetComponent<CanvasGroup>().alpha = 0.3f;

            if (DiceCheck[1])
                GameObject.Find("D2").GetComponent<CanvasGroup>().alpha = 0.3f;

            if (even_dice)
            {
                if (DiceCheck[2])
                    GameObject.Find("D3").GetComponent<CanvasGroup>().alpha = 0.3f;

                if (DiceCheck[3])
                    GameObject.Find("D4").GetComponent<CanvasGroup>().alpha = 0.3f;
            }
        }
        else
        {
            if (!DiceCheck[0])
                GameObject.Find("D1").GetComponent<CanvasGroup>().alpha = 1f;

            if (!DiceCheck[1])
                GameObject.Find("D2").GetComponent<CanvasGroup>().alpha = 1f;

            if (even_dice)
            {
                if (!DiceCheck[2])
                    GameObject.Find("D3").GetComponent<CanvasGroup>().alpha = 1f;

                if (!DiceCheck[3])
                    GameObject.Find("D4").GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }
    private void MoveCounter_167() //  calculate number of remains moves for each player
    {
        if (player_number == 1)
        {
            if (opposite_dice)
            {
                if (!UndoAction)
                {
                    if (WhiteBearOut && Tower_obj.CompareTag("W_win_tower"))
                        which_dice = 24 - SelectedTowerNumber; // counting number of moves for beard out checker
                    else // before counting number of moves checks is there any burnt checker if playerHasBurntCoin return true result deliver tower number will increase
                        which_dice = playerHasBurntCoin ? DeliveryTower + 1 : DeliveryTower - SelectedTowerNumber;

                    if (which_dice == Dice_number[2])
                        white_coin_move_counter -= which_dice; // which_dice:  DeliveryTower - SelectedTowerNumber

                    if (which_dice == Dice_number[0])
                        white_coin_move_counter -= Dice_number[0];

                    if (which_dice == Dice_number[1])
                        white_coin_move_counter -= Dice_number[1];

                    if (WhiteBearOut && which_dice != Dice_number[0] && which_dice != Dice_number[1] && which_dice != Dice_number[2] && !UndoAction)
                        white_coin_move_counter -= which_dice; // which_dice: 24 - SelectedTowerNumber
                }

                else // undo action

                {
                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "bearout")
                        white_coin_move_counter += 24 - RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1); // bearout tower index for white starts from 18th tower till 23th tower as you know backgammon has 24 tower

                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                        white_coin_move_counter += DeliveryTower + 1; // deliver tower for WC burnt checkers starts from 0 tower till 5th tower


                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular")
                    {
                        which_dice = SelectedTowerNumber - DeliveryTower; // opposite of regular move 

                        if (which_dice == Dice_number[0])
                            white_coin_move_counter += Dice_number[0];

                        if (which_dice == Dice_number[1])
                            white_coin_move_counter += Dice_number[1];

                        if (which_dice == Dice_number[2])
                            white_coin_move_counter += which_dice;
                    }
                }
            }
            if (even_dice)
            {
                if (!UndoAction)
                {
                    if (WhiteBearOut && Tower_obj.CompareTag("W_win_tower")) //same to opposite dice
                        white_coin_move_counter -= 24 - SelectedTowerNumber;
                    else
                    {
                        which_dice = playerHasBurntCoin ? DeliveryTower + 1 : DeliveryTower - SelectedTowerNumber;

                        if (Tower_obj.CompareTag("Tower"))
                            if (which_dice == Dice_number[0])
                                white_coin_move_counter -= Dice_number[0];
                            else if (which_dice == Dice_number[4])
                                white_coin_move_counter -= Dice_number[4];
                            else if (which_dice == Dice_number[5])
                                white_coin_move_counter -= Dice_number[5];
                            else if (which_dice == Dice_number[6])
                                white_coin_move_counter -= Dice_number[6];
                    }
                }
                else // undo action
                {

                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "bearout")
                        white_coin_move_counter += 24 - RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1); // 24 - the latest tower that player removed its checkers

                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular" | UndoMode.ElementAt(UndoMode.Count - 1) == "burn" && RListBurnTower.Count > 0 &&
                        RListBurnTower.ElementAt(RListBurnTower.Count - 1) == RListDeliverTower.ElementAt(RListDeliverTower.Count - 1))
                    {
                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "burn") // back to board
                        {
                            black_coin_move_counter -= 24 - RListBurnTower.ElementAt(RListBurnTower.Count - 1);
                            DataManagerClass.blackMoveCounterTxt.text = black_coin_move_counter.ToString();
                        }

                        //regular move 
                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular" && RListBurnTower.ElementAt(RListBurnTower.Count - 1)
                            != RListDeliverTower.ElementAt(RListDeliverTower.Count - 1))
                            white_coin_move_counter += RListDeliverTower[RListDeliverTower.Count - 1] - RListSelectedTower[RListSelectedTower.Count - 1];
                        else
                        {
                            if (playerHasBurntCoin) // updates number of moves when checker returned to burn section
                                white_coin_move_counter += RListDeliverTower[RListDeliverTower.Count - 1] + 1;
                            else
                                white_coin_move_counter += RListDeliverTower[RListDeliverTower.Count - 1] - RListSelectedTower[RListSelectedTower.Count - 1];


                        }
                    }
                    else
                    {
                        //count number of moves for white when its checker returned to burn section
                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                            white_coin_move_counter += RListDeliverTower[RListDeliverTower.Count - 1] + 1;

                        //count regular move for white 
                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular")
                            white_coin_move_counter += RListDeliverTower[RListDeliverTower.Count - 1] - RListSelectedTower[RListSelectedTower.Count - 1];
                    }
                }
            }

            // result will display by text
            DataManagerClass.whiteMoveCounterTxt.text = white_coin_move_counter.ToString();
        }

        if (player_number == 2)
        {
            if (opposite_dice)
            {
                if (!UndoAction)
                {
                    if (BlackBearOut && Tower_obj.CompareTag("B_win_tower"))
                        which_dice = SelectedTowerNumber + 1; // tower of 0 till 5th
                    else
                        which_dice = playerHasBurntCoin ? 24 - DeliveryTower : SelectedTowerNumber - DeliveryTower;

                    if (which_dice == Dice_number[2])
                        black_coin_move_counter -= which_dice;

                    if (which_dice == Dice_number[0])
                        black_coin_move_counter -= Dice_number[0];

                    if (which_dice == Dice_number[1])
                        black_coin_move_counter -= Dice_number[1];

                    if (BlackBearOut && which_dice != Dice_number[0] && which_dice != Dice_number[1] && which_dice != Dice_number[2] && !UndoAction)
                        black_coin_move_counter -= which_dice;
                }
                else
                {
                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "bearout")
                        black_coin_move_counter += RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1) + 1;

                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                        black_coin_move_counter += 24 - SelectedTowerNumber;


                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular")
                    {
                        which_dice = DeliveryTower - SelectedTowerNumber;
                        if (which_dice == Dice_number[0])
                            black_coin_move_counter += Dice_number[0];

                        if (which_dice == Dice_number[1])
                            black_coin_move_counter += Dice_number[1];

                        if (which_dice == Dice_number[2])
                            black_coin_move_counter += which_dice;
                    }

                }
            }
            if (even_dice)
            {
                if (!UndoAction)
                {
                    if (BlackBearOut && Tower_obj.CompareTag("B_win_tower"))
                        black_coin_move_counter -= SelectedTowerNumber + 1;
                    else
                    {
                        which_dice = playerHasBurntCoin ? 24 - DeliveryTower : SelectedTowerNumber - DeliveryTower;

                        if (Tower_obj.CompareTag("Tower"))
                            if (which_dice == Dice_number[0])
                                black_coin_move_counter -= Dice_number[0];
                            else if (which_dice == Dice_number[4])
                                black_coin_move_counter -= Dice_number[4];
                            else if (which_dice == Dice_number[5])
                                black_coin_move_counter -= Dice_number[5];
                            else if (which_dice == Dice_number[6])
                                black_coin_move_counter -= Dice_number[6];
                    }
                }
                else
                {

                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "bearout")
                        black_coin_move_counter += RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1) + 1;

                    if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular" | UndoMode.ElementAt(UndoMode.Count - 1) == "burn" && RListBurnTower.Count > 0 &&
                        RListBurnTower.ElementAt(RListBurnTower.Count - 1) == RListDeliverTower.ElementAt(RListDeliverTower.Count - 1))
                    {
                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                        {
                            white_coin_move_counter -= RListBurnTower.ElementAt(RListBurnTower.Count - 1) + 1;
                            DataManagerClass.whiteMoveCounterTxt.text = white_coin_move_counter.ToString();
                        }

                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular" && RListBurnTower.ElementAt(RListBurnTower.Count - 1)
                            != RListDeliverTower.ElementAt(RListDeliverTower.Count - 1))
                            black_coin_move_counter += RListSelectedTower[RListSelectedTower.Count - 1] - RListDeliverTower[RListDeliverTower.Count - 1];
                        else
                        {
                            if (playerHasBurntCoin)
                                black_coin_move_counter += 24 - RListDeliverTower[RListDeliverTower.Count - 1];
                            else
                                black_coin_move_counter += RListSelectedTower[RListSelectedTower.Count - 1] - RListDeliverTower[RListDeliverTower.Count - 1];

                        }

                    }
                    else
                    {
                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "burn")
                            black_coin_move_counter += 24 - RListDeliverTower[RListDeliverTower.Count - 1];

                        if (UndoMode.ElementAt(UndoMode.Count - 1) == "regular")
                            black_coin_move_counter += RListSelectedTower[RListSelectedTower.Count - 1] - RListDeliverTower[RListDeliverTower.Count - 1];

                    }
                }
            }
            DataManagerClass.blackMoveCounterTxt.text = black_coin_move_counter.ToString();
        }
    }

    #region Undo
    private void Show_undo_btn()
    {
        if (!makebuttons_Deactivate && PlayerPrefs.GetInt("undobtn") == 1 && UndoMode.Count > 0) // undo button is active
        {
            if (player_number == 1)
            {
                if ((UndoAction && UndoMode.Count > 0) | !UndoAction) // if player made any move undo button is available
                    DataManagerClass.Undo.gameObject.SetActive(true);

                if (Black_Bot && (even_dice && DiceCheck[3]) | (opposite_dice && DiceCheck[2] | (DiceCheck[0] && DiceCheck[1]))) // if match is in single mode (against robot)
                    DataManagerClass.Undo.gameObject.SetActive(false); // if player made its all possible moves undo button will be deactivate
            }

            if (player_number == 2)
            {
                if (!Black_Bot | (UndoAction && UndoMode.Count > 0) | !UndoAction)
                    DataManagerClass.Undo.gameObject.SetActive(true);
            }

        }

        if (player_number == 1)
            for (int i = 0; i < BoardClass.WhiteCoinsContainer.Length; i++)
                BoardClass.WhiteCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.WhiteCoinDefultColor;
        else
            for (int i = 0; i < BoardClass.BlackCoinsContainer.Length; i++)
                BoardClass.BlackCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.BlackCoinDefultColor;


    }

    public void Undo()
    {
        xcoin_point = Trf_XForm.transform;
        xcoin_bear_out = Trf_XForm_BearOut.transform;
        xcoin_burn = Trf_XForm_burn.transform;
        BoardClass.CoinDefaultColor = true;

        DontRunRegualrBurn = false;
        pauseTime = 0f;
        if (PlayerPrefs.GetInt("dblcube") == 1)
            doubleCube.layer = 2;

        removeDice = false;

        UnTouchable_The_Coins();

        DiceRollClass.particleRemoveDice1Alarm.Stop();
        DiceRollClass.particleRemoveDice2Alarm.Stop();

        if (UndoMode.Count > 0)
        {
            UndoAction = true;
            switch (UndoMode.ElementAt(UndoMode.Count - 1))
            {
                case "regular": // means no burn action done
                    {

                        for (int i = 0; i < TransferCheckMade.Length; i++)
                            Coin_Legal_Tower[i] = -1; // resets Coin_Legal_Tower[] array

                        Coin_Legal_Tower[0] = RListSelectedTower.ElementAt(RListSelectedTower.Count - 1); //each time undo function get runs Coin_Legal_Tower[0] equals to last move that player made

                        Tower_Number = DeliveryTower = RListSelectedTower.ElementAt(RListSelectedTower.Count - 1);
                        BoardClass.Best_Tower_To_deliver_Coin = SelectedTowerNumber = RListDeliverTower.ElementAt(RListDeliverTower.Count - 1);

                        ShowIndexOfSelectedTower();

                        Final_Position_OF_Coin = Tower_Number * 15 + Coin_Index;
                        xcoin_point = SelectedCoin;

                        Calculate_Dice_check();
                        StartCoroutine(Transfer_Coin_To_DeliveryTower());
                        MoveCounter_167();


                        if (RListBurnTower.Count > 0 && RListBeardOutTower.Count > 0)
                        {
                            if (RListBurnTower.ElementAt(RListBurnTower.Count - 1) == RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1))
                                DontRunRegualrBurn = true;


                            for (int i = 0; i < RListSelectedTower.Count; i++)
                            {
                                if (RListBurnTower.ElementAt(RListBurnTower.Count - 1) == RListSelectedTower.ElementAt(i))
                                {
                                    DontRunRegualrBurn = true;
                                    break;
                                }

                            }

                        }

                        if (!DontRunRegualrBurn && RListBurnTower.Count > 0 && (player_number == 1 && !BoardClass.boardArray[RListBurnTower.ElementAt(RListBurnTower.Count - 1), 0].CompareTag("White_coin")) |
                            (player_number == 2 && !BoardClass.boardArray[RListBurnTower.ElementAt(RListBurnTower.Count - 1), 0].CompareTag("Black_coin")) &&
                            RListDeliverTower.ElementAt(RListDeliverTower.Count - 1) == RListBurnTower.ElementAt(RListBurnTower.Count - 1))
                            StartCoroutine(Regular_UndoBurn()); // this function when gets run that there is not any Checker of white in the destination tower because burnt Checker must return to that tower


                        RListSelectedTower.RemoveAt(RListSelectedTower.Count - 1); // updates RListSelectedTower list
                        RListDeliverTower.RemoveAt(RListDeliverTower.Count - 1); // updates RListDeliverTower list

                        break;
                    }
                case "burn":
                    StartCoroutine(UndoBurn());
                    break;
                case "bearout":
                    StartCoroutine(UndoBearOut());
                    break;
            }

            UndoMode.RemoveAt(UndoMode.Count - 1);
        }

        if (PlayerPrefs.GetInt("undobtn") == 1)
            DataManagerClass.Undo.gameObject.SetActive(false);

        DataManagerClass.UndoSnd.Play();

    }
    private IEnumerator Regular_UndoBurn() // returns checker from burn section to board
    {
        yield return new WaitForSecondsRealtime(0.6f);

        xcoin_point = Trf_XForm.transform;
        xcoin_burn = Trf_XForm_burn.transform;

        if (player_number == 1)
        {
            xcoin_burn = Container_Of_BC_burnt[Num_Of_BC_Burnt].transform;
            Final_Position_OF_Coin = RListBurnTower.ElementAt(RListBurnTower.Count - 1) * 15 +
                                     BoardClass.IndexOfCoinInSelectedTower(RListBurnTower.ElementAt(RListBurnTower.Count - 1)) + 1;

            SelectedCoin = Container_Of_BC_burnt[Num_Of_BC_Burnt].transform;
            Update_TargetTower(RListBurnTower.ElementAt(RListBurnTower.Count - 1));

            Container_Of_BC_burnt[Num_Of_BC_Burnt] = null;
            for (int i = 14; i >= 0; i--)
                if (Container_Of_BC_burnt[i] != null)
                {
                    Num_Of_BC_Burnt = i;
                    break;
                }

            black_coin_move_counter -= 24 - RListBurnTower.ElementAt(RListBurnTower.Count - 1); // new code
            DataManagerClass.blackMoveCounterTxt.text = black_coin_move_counter.ToString();
        }
        else
        {
            xcoin_burn = Container_Of_WC_burnt[Num_Of_WC_Burnt].transform;
            Final_Position_OF_Coin = RListBurnTower.ElementAt(RListBurnTower.Count - 1) * 15 +
                                     BoardClass.IndexOfCoinInSelectedTower(RListBurnTower.ElementAt(RListBurnTower.Count - 1)) + 1;

            SelectedCoin = Container_Of_WC_burnt[Num_Of_WC_Burnt].transform;
            Update_TargetTower(RListBurnTower.ElementAt(RListBurnTower.Count - 1));

            Container_Of_WC_burnt[Num_Of_WC_Burnt] = null;
            for (int i = 14; i >= 0; i--)
                if (Container_Of_WC_burnt[i] != null)
                {
                    Num_Of_WC_Burnt = i;
                    break;
                }

            white_coin_move_counter -= RListBurnTower.ElementAt(RListBurnTower.Count - 1) + 1; // new code
            DataManagerClass.whiteMoveCounterTxt.text = white_coin_move_counter.ToString();

        }



        Regular_Undo_BurntCoin = true;
        RListBurnTower.RemoveAt(RListBurnTower.Count - 1);

        yield return new WaitForSecondsRealtime(0.5f);
        Regular_Undo_BurntCoin = false;
        xcoin_burn = Trf_XForm_burn.transform;

    }
    private IEnumerator UndoBurn() // returns coin from board to burn section
    {
        SelectedTowerNumber = RListDeliverTower.ElementAt(RListDeliverTower.Count - 1);

        Calculate_Dice_check();
        ShowIndexOfSelectedTower();
        xcoin_burn = SelectedCoin;
        Update_board(true, SelectedCoin.gameObject);
        Undo_BurntCoin = playerHasBurntCoin = true;
        MoveCounter_167();

        if (player_number == 1)
            for (int i = 0; i <= 14; i++)
            {
                if (Container_Of_WC_burnt[i] == null)
                {
                    Num_Of_WC_Burnt = i;
                    Container_Of_WC_burnt[i] = SelectedCoin.gameObject;
                    break;
                }
            }

        if (player_number == 2)
            for (int i = 0; i <= 14; i++)
            {
                if (Container_Of_BC_burnt[i] == null)
                {
                    Num_Of_BC_Burnt = i;
                    Container_Of_BC_burnt[i] = SelectedCoin.gameObject;
                    break;
                }
            }
        BoardClass.FindLegalTowers();

        StartCoroutine(Touchable_The_Coins());

        yield return new WaitForSecondsRealtime(0.5f);
        Undo_BurntCoin = false;
        xcoin_burn = Trf_XForm_burn.transform;

        //for prevent of returning against player coin at the same time in to the board whiling returning player coins we have to make sure that the deliver tower it has same coin or it's empty
        if (RListBurnTower.Count > 0 && (player_number == 1 && !BoardClass.boardArray[RListBurnTower.ElementAt(RListBurnTower.Count - 1), 0].CompareTag("White_coin"))
                                     | (player_number == 2 && !BoardClass.boardArray[RListBurnTower.ElementAt(RListBurnTower.Count - 1), 0].CompareTag("Black_coin"))
            && RListDeliverTower.ElementAt(RListDeliverTower.Count - 1) == RListBurnTower.ElementAt(RListBurnTower.Count - 1))

            StartCoroutine(Regular_UndoBurn());

        RListDeliverTower.RemoveAt(RListDeliverTower.Count - 1);
        Invoke(nameof(Show_undo_btn), 0.5f);

    }
    private IEnumerator UndoBearOut()
    {
        Undo_Bearout = true;
        xcoin_point = Trf_XForm.transform;

        Final_Position_OF_Coin = RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1) * 15 +
                                 BoardClass.IndexOfCoinInSelectedTower(RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1)) + 1;

        SelectedTowerNumber = RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1);
        Calculate_Dice_check();
        MoveCounter_167();
        SelectedCoin = player_number == 1 ? White_beardout_Coin[W_win - 1].transform : Black_Beardout_coins[B_win - 1].transform;
        SelectedCoin.GetComponent<BoxCollider>().enabled = true;
        xcoin_bear_out = SelectedCoin;
        xcoin_bear_out.GetComponent<Renderer>().enabled = true;

        Update_TargetTower(RListBeardOutTower.ElementAt(RListBeardOutTower.Count - 1));

        if (player_number == 1)
        {
            W_win--;
            White_Bearout_points[W_win].GetComponent<Renderer>().enabled = false;
            White_beardout_Coin[W_win] = null;
        }
        else
        {
            B_win--;
            Black_Bearout_points[B_win].GetComponent<Renderer>().enabled = false;
            Black_Beardout_coins[B_win] = null;
        }

        RListBeardOutTower.RemoveAt(RListBeardOutTower.Count - 1);

        StartCoroutine(Touchable_The_Coins());

        yield return new WaitForSecondsRealtime(0.5f);
        Undo_Bearout = false;
        xcoin_bear_out = Trf_XForm_BearOut.transform;

        Invoke(nameof(Show_undo_btn), 0.5f);
    }

    #endregion

    public void DoubleCube()
    {
        if (player_number == 1)
        {
            if (Black_dbl_chk == 0 && PlayerPrefs.GetInt("mode") == 2)
            {
                if (PlayerPrefs.GetInt("dblcube") == 1 && num_double != 64)
                    doubleCube.layer = 9;
            }
            else if (PlayerPrefs.GetInt("dblcube") == 1)
                doubleCube.layer = 2;
        }

        else
        {
            if (White_dbl_chk == 0)
            {
                if (PlayerPrefs.GetInt("dblcube") == 1 && num_double != 64)
                    doubleCube.layer = 9;
            }
            else if (PlayerPrefs.GetInt("dblcube") == 1)
                doubleCube.layer = 2;
        }

    }

    private IEnumerator AutoMoveCoins() // this function will make the last remain move or The only available move for the Player automatically
    {
        // next update player could transfer more than one checker at same time by selecting checkers from common tower and process will be like stack
        if (BoardClass.NumOfPlayerMoves > -1 && !playerHasBurntCoin && (player_number == 1 && !WhiteBearOut) | (player_number == 2 && !BlackBearOut))
        {
            if (player_number == 1)
            {
                if (opposite_dice)
                {
                    if (!DiceCheck[0])
                    {
                        //based on these conditions calculates the exact delivery tower (destination tower)
                        if (SelectedTowerNumber + Dice_number[0] < 24 && !BoardClass.boardArray[SelectedTowerNumber + Dice_number[0], 1].CompareTag("Black_coin")
                            && (SelectedTowerNumber + Dice_number[1] < 24 && BoardClass.boardArray[SelectedTowerNumber + Dice_number[1], 1].CompareTag("Black_coin")
                            && !DiceCheck[1]) | DiceCheck[1] | SelectedTowerNumber + Dice_number[1] > 23
                            && (SelectedTowerNumber + Dice_number[0] + Dice_number[1] < 24
                            && BoardClass.boardArray[SelectedTowerNumber + Dice_number[0] + Dice_number[1], 1].CompareTag("Black_coin"))
                            | SelectedTowerNumber + Dice_number[0] + Dice_number[1] > 23 | DiceCheck[1])

                        {
                            DeliveryTower = SelectedTowerNumber + Dice_number[0];
                            click = "Tower " + (SelectedTowerNumber + Dice_number[0]);
                        }
                        else
                            click = "Locked";

                    }

                    if (!DiceCheck[1] && click == "Locked" | DiceCheck[0])
                    {
                        //calculates the exact delivery tower (destination tower)
                        if (SelectedTowerNumber + Dice_number[1] < 24 && !BoardClass.boardArray[SelectedTowerNumber + Dice_number[1], 1].CompareTag("Black_coin")
                            && (SelectedTowerNumber + Dice_number[0] < 24 && BoardClass.boardArray[SelectedTowerNumber + Dice_number[0], 1].CompareTag("Black_coin") && !DiceCheck[0]) | DiceCheck[0] | SelectedTowerNumber + Dice_number[0] > 23
                            && (SelectedTowerNumber + Dice_number[0] + Dice_number[1] < 24 && BoardClass.boardArray[SelectedTowerNumber + Dice_number[0] + Dice_number[1], 1].CompareTag("Black_coin"))
                            | SelectedTowerNumber + Dice_number[0] + Dice_number[1] > 23 | DiceCheck[0])
                        {
                            DeliveryTower = SelectedTowerNumber + Dice_number[1];
                            click = "Tower " + (SelectedTowerNumber + Dice_number[1]);
                        }
                        else
                            click = "Locked";
                    }
                }

                if (even_dice)
                {

                    //calculates the exact delivery tower (destination tower)
                    if ((SelectedTowerNumber + Dice_number[0] < 24 && !BoardClass.boardArray[SelectedTowerNumber + Dice_number[0], 1].CompareTag("Black_coin")
                        && SelectedTowerNumber + Dice_number[4] > 23) | (SelectedTowerNumber + Dice_number[4] < 24 && BoardClass.boardArray[SelectedTowerNumber + Dice_number[4], 1].CompareTag("Black_coin")
                        && SelectedTowerNumber + Dice_number[0] < 24 && !BoardClass.boardArray[SelectedTowerNumber + Dice_number[0], 1].CompareTag("Black_coin"))
                        | (DiceCheck[2] && SelectedTowerNumber + Dice_number[0] < 24 && !BoardClass.boardArray[SelectedTowerNumber + Dice_number[0], 1].CompareTag("Black_coin")))
                    {
                        DeliveryTower = SelectedTowerNumber + Dice_number[0];
                        click = "Tower " + (SelectedTowerNumber + Dice_number[0]);
                    }
                    else
                        click = "Locked";
                }
            }

            if (player_number == 2)
            {
                if (opposite_dice)
                {
                    if (!DiceCheck[0])
                    {
                        //calculates the exact delivery tower (destination tower)
                        if (SelectedTowerNumber - Dice_number[0] > -1 && !BoardClass.boardArray[SelectedTowerNumber - Dice_number[0], 1].CompareTag("White_coin")
                            && (SelectedTowerNumber - Dice_number[1] > -1 && BoardClass.boardArray[SelectedTowerNumber - Dice_number[1], 1].CompareTag("White_coin")
                            && !DiceCheck[1]) | DiceCheck[1] | SelectedTowerNumber - Dice_number[1] < 0 && (SelectedTowerNumber - (Dice_number[0] + Dice_number[1]) > -1
                            && BoardClass.boardArray[SelectedTowerNumber - (Dice_number[0] + Dice_number[1]), 1].CompareTag("White_coin"))
                            | SelectedTowerNumber - (Dice_number[0] + Dice_number[1]) < 0 | DiceCheck[1])

                        {
                            DeliveryTower = SelectedTowerNumber - Dice_number[0];
                            click = "Tower " + (SelectedTowerNumber - Dice_number[0]);
                        }
                        else
                            click = "Locked";
                    }

                    if (!DiceCheck[1] && click == "Locked" | DiceCheck[0])
                    {
                        //calculates the exact delivery tower (destination tower)
                        if (SelectedTowerNumber - Dice_number[1] > -1 && !BoardClass.boardArray[SelectedTowerNumber - Dice_number[1], 1].CompareTag("White_coin")
                            && (SelectedTowerNumber - Dice_number[0] > -1 && BoardClass.boardArray[SelectedTowerNumber - Dice_number[0], 1].CompareTag("White_coin")
                            && !DiceCheck[0]) | DiceCheck[0] | SelectedTowerNumber - Dice_number[0] < 0 && (SelectedTowerNumber - (Dice_number[0] + Dice_number[1]) > -1
                            && BoardClass.boardArray[SelectedTowerNumber - (Dice_number[0] + Dice_number[1]), 1].CompareTag("White_coin")) | SelectedTowerNumber - (Dice_number[0] + Dice_number[0]) < 0 | DiceCheck[0])
                        {
                            DeliveryTower = SelectedTowerNumber - Dice_number[1];
                            click = "Tower " + (SelectedTowerNumber - Dice_number[1]);
                        }
                        else
                            click = "Locked";
                    }
                }

                if (even_dice)
                {
                    //calculates the exact delivery tower (destination tower)
                    if ((SelectedTowerNumber - Dice_number[0] > -1 && !BoardClass.boardArray[SelectedTowerNumber - Dice_number[0], 1].CompareTag("White_coin")
                        && SelectedTowerNumber - Dice_number[4] < 0) | (SelectedTowerNumber - Dice_number[4] > -1 && BoardClass.boardArray[SelectedTowerNumber - Dice_number[4], 1].CompareTag("White_coin")
                        && SelectedTowerNumber - Dice_number[0] > -1 && !BoardClass.boardArray[SelectedTowerNumber - Dice_number[0], 1].CompareTag("White_coin"))
                        | (DiceCheck[2] && SelectedTowerNumber - Dice_number[0] > -1 && !BoardClass.boardArray[SelectedTowerNumber - Dice_number[0], 1].CompareTag("White_coin")))
                    {
                        DeliveryTower = SelectedTowerNumber - Dice_number[0];
                        click = "Tower " + (SelectedTowerNumber - Dice_number[0]);
                    }
                    else
                        click = "Locked";
                }
            }

            if (click != "Locked")
            {
                Tower_obj = GameObject.Find("GreenPortal " + DeliveryTower);

                if (Tower_obj.CompareTag("Tower"))
                {
                    BoardClass.CoinDefaultColor = true;
                    DisconnectAutoMove = true;
                    UnTouchable_The_Coins();

                    for (int detwr = 0; detwr < 26; detwr++)
                        BoardClass.greenTowers[detwr].SetActive(false); // for prevent of selecting by player


                    yield return new WaitForSecondsRealtime(0.5f);
                    for (int twr = 0; twr < 24; twr++)
                        if (BoardClass.greenTowers[twr] == Tower_obj)
                        {
                            xcoin_point = SelectedCoin;
                            BoardClass.Coin_step_by_step = BoardClass.coin_auto_step = false;
                            DeliveryTower = twr;
                            Tower_Number = twr;
                            break;
                        }

                    Coin_index_label.text = "";

                    if (Tower_obj.CompareTag("Tower"))
                        StartCoroutine(Transfer_Coin_To_DeliveryTower());

                    Calculate_Dice_check();

                    MoveCounter_167();

                    BoardClass.CoinDefaultColor = true;

                    for (int i = 0; i < 15; i++)
                    {
                        if (PlayerClass.player_number == 1)
                            BoardClass.WhiteCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.WhiteCoinDefultColor;
                        else
                            BoardClass.BlackCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.BlackCoinDefultColor;
                    }

                    for (int i = 0; i < BoardClass.greenTowers.Length; i++) // need attention
                        BoardClass.greenTowers[i].SetActive(false);

                    Invoke(nameof(invoke_FindLegalTowers), 1f); // after 1 sec calculate next legal tower (added recently)

                    yield return new WaitForSecondsRealtime(0.5f);
                    DisconnectAutoMove = false;

                }
            }

        }

    }

}
