using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataManager;
using static DiceRoll;
using static Player;

public class Board : MonoBehaviour 

{ 
    //© 2017, Arash KhallaqDoust (Rvbinary)
    
    //Attention : if you have any questions feel free to ask. best way to contact is to email us, we will answer and support you for sure. the email address mentioned in the manual.pdf
    
    public GameObject[,] boardArray = new GameObject[24, 15]; // backgammon board
	public bool CoinDefaultColor ; //CoinDefaultColor = true: checker not flashing anymore
	public bool Coin_step_by_step, coin_auto_step ;
	public bool nextway; // usage on bear out 
	public bool root,Free_All_legal_Towers; // root relative to bear out process  Free_All_legal_Towers : allows to legal tower array get update
	public bool AgainstRules; // it means player could bear out those checkers that are not list in legal tower
	public Color32 White_flash_off,White_flash_on,Black_flash_off,Black_flash_on; //Color of Flashing for black/white coin
	public Color lerpedColor; // result of Color.Lerp
	public Material WhiteCoinDefultColor,BlackCoinDefultColor;
	public int NumOfPlayerMoves, NumOfBearOutMoves; // NumOfPlayerMoves: The number of moves the player has | NumOfBearOutMoves: Number of moves the player has on the board when leaving the coin
	public int Sum_Dice_1_num_Dice_2_num; // specifies the legal towers that player is able to select and this variable will store to legal tower array 
	public int SumTowers,tb_W,tb_B,min,max; //SumTowers represent dice number | tb_W relative to bear out and determines dice check when selected tower is not listed in legal tower array |
	//min : smallest dice_number max: biggest dice_number
	public int SwitchedTower,numberOfCheckers; // SwitchedTower : The result is the sum of the dice and the selected tower index | 
	// numberOfCheckers : relative to  Is_The_Delivery_Tower_Free functions that specifies number of Checkers in destination tower 
	public int Count_WC, even_count_TBO, count_TBO_d1, count_TBO_d2, TheEvenTower, monitor_tower_d1, monitor_tower_d2, Even_monitor_tbo, monitor_tower_d1_Bo, monitor_tower_d2_Bo;
	public List<int> LstevenDiceTowers = new List<int>(); // list of towers that player has permission to select even dice mode 
	public List<int> LstoppositeDiceTowers = new List<int>(); // list of towers that player has permission to select opposite dice mode 
	public List<int> LstoppositeDiceBearOut = new List<int>(); // list of towers that player has permission to select bear out mode 
	public int[] evenDiceCoinIndex = new int[15]; // in even mode "it is necessary to know number of checkers of selectable tower" 
	public int evenDiceIndexWin;
	public int evenDiceNumBearOut;
	public bool Switch_D_5_6; // when dice are same in value number of possible moves for player is more than 4 times so we have to decide last index
                           // of legaltower must fill with dicenumber [5] or dicenumber [6]
	public GameObject[] greenTowers = new GameObject[26]; // towers
	public GameObject[] WhiteCoinsContainer = new GameObject[15]; // all white checekrs
	public GameObject[] BlackCoinsContainer = new GameObject[15];

	public int Best_Tower_To_deliver_Coin; //Which tower should be chosen as the intermediate tower? when one of the legaltower is block by against player checkers 
	public static Board BoardClass;
	public GameObject blank,Last_Burnt_Coin; //blank: all empty towers has "Blank" Gameobject so by finding this Gameobject we figure out the which tower is empty
	
	
	
	
	private void Start ()
	{
		BoardClass = this;
		InitializeBoard ();
	}
	private void InitializeBoard() //initializes the board stuffs such as Checkers and towers + all possible points that Checkers could place
	{
	
		for (int i = 0; i < 24; i++)
		for (int j = 0; j < 15; j++)
			boardArray[i,j] = blank; // fills all the towers with blank object

		//*** installs the white & black Checkers in specific towers
		//rule of installing WhiteCoins: 2+5+3+5(15) + BlackCoins: 2+5+3+5(15)
		
		boardArray [0,0] = GameObject.Find("W0") ;
		boardArray [0,1] = GameObject.Find("W1") ;
		boardArray [11, 0] =  GameObject.Find("W2");
		boardArray [11, 1] = GameObject.Find("W3");
		boardArray [11, 2] = GameObject.Find("W4");
		boardArray [11, 3] = GameObject.Find("W5");
		boardArray [11, 4] = GameObject.Find("W6");
		boardArray [16, 0] = GameObject.Find("W7");
		boardArray [16, 1] = GameObject.Find("W8");
		boardArray [16, 2] = GameObject.Find("W9");
		boardArray [18, 0] = GameObject.Find("W10");
		boardArray [18, 1] = GameObject.Find("W11");
		boardArray [18, 2] = GameObject.Find("W12");
		boardArray [18, 3] = GameObject.Find("W13");
		boardArray [18, 4] = GameObject.Find("W14");
		boardArray [23,0] = GameObject.Find("B0");
		boardArray [23,1] = GameObject.Find("B1");
		boardArray [12, 0] = GameObject.Find("B2");
		boardArray [12, 1] = GameObject.Find("B3");
		boardArray [12, 2] = GameObject.Find("B4");
		boardArray [12, 3] = GameObject.Find("B5");
		boardArray [12, 4] = GameObject.Find("B6");
		boardArray [7, 0] = GameObject.Find("B7");
		boardArray [7, 1] = GameObject.Find("B8");
		boardArray [7, 2] = GameObject.Find("B9");
		boardArray [5, 0] = GameObject.Find("B10");
		boardArray [5, 1] = GameObject.Find("B11");
		boardArray [5, 2] = GameObject.Find("B12");
		boardArray [5, 3] = GameObject.Find("B13");
		boardArray [5, 4] = GameObject.Find("B14");


		// loads towers check them in inspector. type in search section : GreenPortal
		for (int greenTwr = 0; greenTwr < 24; greenTwr++) // finds and list all towers
		{
			greenTowers [greenTwr] = GameObject.Find("GreenPortal " + greenTwr);
			greenTowers [greenTwr].SetActive (false);

			if (PlayerPrefs.GetInt("legaltowers") == 0)
				greenTowers[greenTwr].GetComponent<Renderer>().enabled = false;
			
		}

		greenTowers[24] = GameObject.Find("GreenBearOutTower_White"); // reserved for bear out towers
		greenTowers[25] = GameObject.Find("GreenBearOutTower_Black");

		// legal towers are not visible anymore
		if (PlayerPrefs.GetInt("legaltowers") == 0) 
		{
			greenTowers[24].GetComponent<Renderer>().enabled = false;
			greenTowers[25].GetComponent<Renderer>().enabled = false;
		}

		greenTowers [24].SetActive (false);
		greenTowers [25].SetActive (false);

		for (int WhiteAry = 0; WhiteAry < 15; WhiteAry++) // finds and list all white checkers
		{
			WhiteCoinsContainer [WhiteAry] = GameObject.Find("W" + WhiteAry);
			WhiteCoinsContainer [WhiteAry].GetComponent<Collider> ().enabled = false;
		}
		
		for (int BlackAry = 0; BlackAry < 15; BlackAry++)
		{
			BlackCoinsContainer [BlackAry] = GameObject.Find("B" + BlackAry);
			BlackCoinsContainer [BlackAry].GetComponent<Collider> ().enabled = false;
		}

	}

	public int NumberOfBurntCoins (string typeOfCoin) // counts the burnt checkers
	{
		int numOfCoins = 0;
	
		if (typeOfCoin == "White")
			for (numOfCoins = 0; numOfCoins < 15; numOfCoins++)
				if (PlayerClass.Container_Of_WC_burnt [numOfCoins] == null)
					break;
		
		if (typeOfCoin == "Black")
			for (numOfCoins = 0; numOfCoins < 15; numOfCoins++)
				if (PlayerClass.Container_Of_BC_burnt [numOfCoins] == null)
					break;

		return numOfCoins; //if numOfCoins > 0 means one of the player has a brunet checker
		
	}

	public int IndexOfCoinInSelectedTower (int twr) //calculates how many black/white checkers is in the DeliveryTower
	{
		int Index = -1; // if a tower is empty functions returns -1.
		
		for (int i = 0; i < 15; i++)
			if (boardArray [twr, i].CompareTag("White_coin") | boardArray [twr, i].CompareTag("Black_coin") && !boardArray [twr, i].CompareTag("blank"))
				Index++;

		return Index  ; 
	}

	public void ShowLegalTowers() // if player touches one of its own coins this function shows the legal towers for choosing based on what values dice is shown
	{
		var legalTowerForBurnedCoin = 0;
		SumTowers = PlayerClass.Dice_number[0];
		var balanceTowers = 4;

		coin_auto_step = Coin_step_by_step = false; 
		root = AgainstRules = Free_All_legal_Towers = nextway = false; 
		
		if (PlayerClass.playerHasBurntCoin )
		{
			if (PlayerClass.player_number == 1)
			{
				if (PlayerClass.opposite_dice && PlayerClass.Container_Of_WC_burnt[PlayerClass.Num_Of_WC_Burnt] == PlayerClass.SelectedCoin.gameObject)
				{
					int[] burnedCoinLegalTower = new int[3]; //target place(0,1,2,3,4,5) of burned coins in the board will save in dts[] array

					burnedCoinLegalTower[0] = PlayerClass.Dice_number[0] - 1; //towers index begins from zero 
					burnedCoinLegalTower[1] = PlayerClass.Dice_number[1] - 1;
					burnedCoinLegalTower[2] = PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1] - 1;

					PlayerClass.Coin_Legal_Tower[0] = Mathf.Min(PlayerClass.Dice_number[0], PlayerClass.Dice_number[1]) - 1; // exact towers will save in Coin_Legal_Place array the towers will arrange from small index to big
					PlayerClass.Coin_Legal_Tower[1] = Mathf.Max(PlayerClass.Dice_number[0], PlayerClass.Dice_number[1]) - 1;
					PlayerClass.Coin_Legal_Tower[2] = PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1] - 1;
					PlayerClass.Coin_Legal_Tower[3] = 24; // reserved place

					Best_Tower_To_deliver_Coin = Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[0]) ? PlayerClass.Coin_Legal_Tower[0] : PlayerClass.Coin_Legal_Tower[1];

					// so Best_Tower_To_deliver_Coin  variable will manage that how coins should move in the board for more info please check the player scripts

						//Dice_Do [] Array defines how many move is remains for player
						
						for (int i = 0; i < 2; i++)
							if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && Is_The_Delivery_Tower_Free(burnedCoinLegalTower[i]) && PlayerClass.Num_Of_WC_Burnt < 1)
								legalTowerForBurnedCoin = 3;
							else
							if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && Is_The_Delivery_Tower_Free(burnedCoinLegalTower[i]) && PlayerClass.Num_Of_WC_Burnt >= 1)
								legalTowerForBurnedCoin = 2;
							
						if (PlayerClass.DiceCheck[0] | PlayerClass.DiceCheck[1])
								legalTowerForBurnedCoin = 2;

					for (int j = 0; j < legalTowerForBurnedCoin; j++)
					{
						if (Is_The_Delivery_Tower_Free(burnedCoinLegalTower[j]) && !PlayerClass.DiceCheck[j])
							greenTowers[burnedCoinLegalTower[j]].SetActive(true);
						
						if (PlayerClass.DiceCheck[PlayerClass.diceChecker] && !Is_The_Delivery_Tower_Free(burnedCoinLegalTower[j]))
						{
							if (!PlayerClass.Black_Bot)
								PlayerClass.removeDice = true;
							
							PlayerClass.DoubleCube();
							DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
							DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
							DiceRollClass.particleRemoveDice1Alarm.Play();
							DiceRollClass.particleRemoveDice2Alarm.Play();

							DataManagerClass.noMove.Play("Nomove");
							DataManagerClass.noMoveSnd.Play();
							
						}

						else if (!Is_The_Delivery_Tower_Free(burnedCoinLegalTower[0]) && !Is_The_Delivery_Tower_Free(burnedCoinLegalTower[1]))
						{
							if (!PlayerClass.Black_Bot)
								PlayerClass.removeDice = true;
							
							PlayerClass.DoubleCube();
							greenTowers[burnedCoinLegalTower[j]].SetActive(false);

							DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
							DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

							DiceRollClass.particleRemoveDice1Alarm.Play();
							DiceRollClass.particleRemoveDice2Alarm.Play();
							DataManagerClass.noMoveSnd.Play();
							DataManagerClass.noMove.Play("Nomove");
						}
					}
				}

				if (PlayerClass.even_dice && PlayerClass.Container_Of_WC_burnt[PlayerClass.Num_Of_WC_Burnt] == PlayerClass.SelectedCoin.gameObject)
				{
					if (PlayerClass.Num_Of_WC_Burnt == 0)
						Free_All_legal_Towers = true;
					else if (!PlayerClass.DiceCheck[0])
						Free_All_legal_Towers = false;
				
					if (!Free_All_legal_Towers)
						PlayerClass.Coin_Legal_Tower[0] = PlayerClass.Coin_Legal_Tower[1] = PlayerClass.Coin_Legal_Tower[2] = 
							PlayerClass.Coin_Legal_Tower[3] = PlayerClass.Dice_number[0] - 1;
					else
					{
						for (int i = PlayerClass.DiceCheck.Length - 1; i >= 0; i--) // based on dice check next legal tower will calculate
						{
							//!PlayerClass.DiceCheck[3-i]  : from last index of dice_check array moves will check
							if (!PlayerClass.DiceCheck[3-i]) // i = 2 : so 2 + dice[3](value 1) ==> 24- 3 = 21 ==> determines last legal and selectable tower
								PlayerClass.Coin_Legal_Tower[3-i] = PlayerClass.Dice_number[i+3] - 1; // determines legal tower
							
							if (PlayerClass.DiceCheck[i])
								PlayerClass.Coin_Legal_Tower[i] = -1;
						}
					}

					for (int i = 0; i < PlayerClass.Coin_Legal_Tower.Length / 2; i++)//  Length / 2 prevent of overlapping the values of legal tower array
					{
						int tmp = PlayerClass.Coin_Legal_Tower[i]; // tmp will exchange the value of legal tower from index zero to 1 
						PlayerClass.Coin_Legal_Tower[i] = PlayerClass.Coin_Legal_Tower[PlayerClass.Coin_Legal_Tower.Length - i - 1]; // PlayerClass.Coin_Legal_Tower[i] = [4 - i(0/1) - 1] Coin_Legal_Tower
						PlayerClass.Coin_Legal_Tower[PlayerClass.Coin_Legal_Tower.Length - i - 1] = tmp;
					}
					
					for (int i = 0; i < 4; i++)
					{
						if (!Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[i]))
						{
							balanceTowers = i; // finds the blocked tower and not showing it to player
							break;
						}
					}

					for (int j = 0; j < balanceTowers; j++) // shows legal tower to player
					{
						if (Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[j]) && PlayerClass.Coin_Legal_Tower[j] >= 0)
							greenTowers[PlayerClass.Coin_Legal_Tower[j]].SetActive(true);

						if (!Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[0]))
						{
							if (!PlayerClass.Black_Bot)
								PlayerClass.removeDice = true;
							
							PlayerClass.DoubleCube();
							DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
							DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

							DiceRollClass.particleRemoveDice1Alarm.Play();
							DiceRollClass.particleRemoveDice2Alarm.Play();
							DataManagerClass.noMove.Play("Nomove");
							DataManagerClass.noMoveSnd.Play();
						}
					}
				}
			}
		
			if (PlayerClass.player_number == 2)
			{
				if (PlayerClass.opposite_dice && PlayerClass.Container_Of_BC_burnt[PlayerClass.Num_Of_BC_Burnt] == PlayerClass.SelectedCoin.gameObject)
				{
					int[] burnedCoinLegalTower = new int[3];

					//specifies legal tower for bringing back checker from burn section to board
					burnedCoinLegalTower[0] = 24 - PlayerClass.Dice_number[0];
					burnedCoinLegalTower[1] = 24 - PlayerClass.Dice_number[1];
					burnedCoinLegalTower[2] = 24 - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);

					// the movement of checker must be in order, so towers will be in order from small to big
					PlayerClass.Coin_Legal_Tower[0] = 24 - Mathf.Min(PlayerClass.Dice_number[0], PlayerClass.Dice_number[1]);
					PlayerClass.Coin_Legal_Tower[1] = 24 - Mathf.Max(PlayerClass.Dice_number[0], PlayerClass.Dice_number[1]);
					PlayerClass.Coin_Legal_Tower[2] = 24 - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);
					PlayerClass.Coin_Legal_Tower[3] = 24;

					Best_Tower_To_deliver_Coin = Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[0]) ? PlayerClass.Coin_Legal_Tower[0] : PlayerClass.Coin_Legal_Tower[1];

					for (int i = 0; i < 2; i++)
						if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && Is_The_Delivery_Tower_Free(burnedCoinLegalTower[i]) && PlayerClass.Num_Of_BC_Burnt < 1)
							legalTowerForBurnedCoin = 3;
						else
						if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && Is_The_Delivery_Tower_Free(burnedCoinLegalTower[i]) && PlayerClass.Num_Of_BC_Burnt >= 1)
							legalTowerForBurnedCoin = 2;

					if (PlayerClass.DiceCheck[0] | PlayerClass.DiceCheck[1])
						legalTowerForBurnedCoin = 2;
				
					for (int j = 0; j < legalTowerForBurnedCoin; j++)
					{
						if (Is_The_Delivery_Tower_Free(burnedCoinLegalTower[j]) && !PlayerClass.DiceCheck[j])
							greenTowers[burnedCoinLegalTower[j]].SetActive(true);

						if (PlayerClass.DiceCheck[PlayerClass.diceChecker] && !Is_The_Delivery_Tower_Free(burnedCoinLegalTower[j]))
						{
							PlayerClass.removeDice = true;
							PlayerClass.DoubleCube();
							DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
							DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

							DiceRollClass.particleRemoveDice1Alarm.Play();
							DiceRollClass.particleRemoveDice2Alarm.Play();

							DataManagerClass.noMove.Play("Nomove");
							DataManagerClass.noMoveSnd.Play();
						}

						else if (!Is_The_Delivery_Tower_Free(burnedCoinLegalTower[0]) && !Is_The_Delivery_Tower_Free(burnedCoinLegalTower[1]))
						{
							PlayerClass.removeDice = true;
							PlayerClass.DoubleCube();
							greenTowers[burnedCoinLegalTower[j]].SetActive(false);

							DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
							DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

							DiceRollClass.particleRemoveDice1Alarm.Play();
							DiceRollClass.particleRemoveDice2Alarm.Play();
							DataManagerClass.noMove.Play("Nomove");
							DataManagerClass.noMoveSnd.Play();
						}

					}

				}

				if (PlayerClass.even_dice && PlayerClass.Container_Of_BC_burnt[PlayerClass.Num_Of_BC_Burnt] == PlayerClass.SelectedCoin.gameObject)
				{
					if (PlayerClass.Num_Of_BC_Burnt == 0)
						Free_All_legal_Towers = true;
					else if (!PlayerClass.DiceCheck[0])
						Free_All_legal_Towers = false;
					
					if (!Free_All_legal_Towers)
						PlayerClass.Coin_Legal_Tower[0] = PlayerClass.Coin_Legal_Tower[1] = PlayerClass.Coin_Legal_Tower[2] = 
							PlayerClass.Coin_Legal_Tower[3] = 24 - PlayerClass.Dice_number[0];
					else
					{
						for (int i = PlayerClass.DiceCheck.Length - 1; i >= 0; i--)
						{
							if (!PlayerClass.DiceCheck[3 - i]) 
								PlayerClass.Coin_Legal_Tower[3 - i] = 24 - PlayerClass.Dice_number[i + 3];
							
							if (PlayerClass.DiceCheck[i])
								PlayerClass.Coin_Legal_Tower[i] = -1;
						}
						
					}

					for (int i = 0; i < PlayerClass.Coin_Legal_Tower.Length / 2; i++)
					{
						int tmp = PlayerClass.Coin_Legal_Tower[i]; 
						PlayerClass.Coin_Legal_Tower[i] = PlayerClass.Coin_Legal_Tower[PlayerClass.Coin_Legal_Tower.Length - i - 1]; 
						PlayerClass.Coin_Legal_Tower[PlayerClass.Coin_Legal_Tower.Length - i - 1] = tmp;
					}

					for (int i = 0; i < 4; i++)
					{
						if (!Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[i]))
						{
							balanceTowers = i;
							break;
						}
					}

					for (int j = 0; j < balanceTowers; j++)
					{
						if (Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[j]) && PlayerClass.Coin_Legal_Tower[j] >= 0)
							greenTowers[PlayerClass.Coin_Legal_Tower[j]].SetActive(true);

						if (!Is_The_Delivery_Tower_Free(PlayerClass.Coin_Legal_Tower[0]))
						{
							PlayerClass.removeDice = true;
							PlayerClass.DoubleCube();
							DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
							DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

							DiceRollClass.particleRemoveDice1Alarm.Play();
							DiceRollClass.particleRemoveDice2Alarm.Play();
							DataManagerClass.noMove.Play("Nomove");
							DataManagerClass.noMoveSnd.Play();
						}
					}
				}
			}
		}
		else
		{
			if (PlayerClass.even_dice && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1] | !PlayerClass.DiceCheck[2] | !PlayerClass.DiceCheck[3])
			{
				for (int i = 0; i < 15; i++)
				{
					if (PlayerClass.player_number == 1)
						WhiteCoinsContainer[i].GetComponent<Collider>().enabled = true;
					else
						BlackCoinsContainer[i].GetComponent<Collider>().enabled = true;
				}
				
				//dice1/dice2/dice3/dice4	
				for (int i = 0; i < 4; i++)
				{
					if (PlayerClass.player_number == 1)
					{
						SwitchedTower = PlayerClass.SelectedTowerNumber + SumTowers; 
						
						PlayerClass.Coin_Legal_Tower[0] = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0];
						PlayerClass.Coin_Legal_Tower[1] = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[4];
						PlayerClass.Coin_Legal_Tower[2] = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[5];

						if (!Switch_D_5_6)
							PlayerClass.Coin_Legal_Tower[3] = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[5];
						else
							PlayerClass.Coin_Legal_Tower[3] = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[6];

						Best_Tower_To_deliver_Coin = PlayerClass.Coin_Legal_Tower[0];
						
					}
					else
					{
						SwitchedTower = PlayerClass.SelectedTowerNumber - SumTowers;

						PlayerClass.Coin_Legal_Tower[0] = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0];
						PlayerClass.Coin_Legal_Tower[1] = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[4];
						PlayerClass.Coin_Legal_Tower[2] = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[5];

						if (!Switch_D_5_6)
							PlayerClass.Coin_Legal_Tower[3] = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[5];
						else
							PlayerClass.Coin_Legal_Tower[3] = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[6];

						Best_Tower_To_deliver_Coin = PlayerClass.Coin_Legal_Tower[0];
				
					}

					if (!PlayerClass.DiceCheck[i])
						if (Is_The_Delivery_Tower_Free(SwitchedTower) && SwitchedTower < 24 && SwitchedTower > -1)
							greenTowers[SwitchedTower].SetActive(true);

					if (!PlayerClass.DiceCheck[i])
					{
						if (PlayerClass.player_number == 1 && Is_The_Delivery_Tower_Free(PlayerClass.SelectedTowerNumber + SumTowers))
						{
							SumTowers += PlayerClass.Dice_number[i]; // next legal tower will save in SumTowers
							Switch_D_5_6 = true;
						}

						if (PlayerClass.player_number == 2 && Is_The_Delivery_Tower_Free(PlayerClass.SelectedTowerNumber - SumTowers))
						{
							SumTowers += PlayerClass.Dice_number[i];
							Switch_D_5_6 = true;
						}
					}
				}

				//---------------------------bear out the checkers of the board----------------------------------------------------------------
				
				if (PlayerClass.WhiteBearOut && PlayerClass.player_number == 1)
				{
					for (int i = 0; i < 4; i++)
						if (PlayerClass.DiceCheck[i])
							Free_All_legal_Towers = true;

					if (24 - PlayerClass.SelectedTowerNumber == PlayerClass.Dice_number[0])
					{
						for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
							PlayerClass.Bearout_Delivery_Tower[i] = 24 - PlayerClass.Dice_number[0];
						
						root = true;
					}
					else
					{
						root = false;

						if (!PlayerClass.DiceCheck[0])
							for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
								PlayerClass.Bearout_Delivery_Tower[i] = 24 - PlayerClass.Dice_number[3+i];
						

						if (Free_All_legal_Towers)
						{
							if (PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1])
							{
								PlayerClass.Bearout_Delivery_Tower[0] = PlayerClass.Bearout_Delivery_Tower[1] = 24 - PlayerClass.Dice_number[3];
								PlayerClass.Bearout_Delivery_Tower[2] = 24 - PlayerClass.Dice_number[4];
								PlayerClass.Bearout_Delivery_Tower[3] = 24 - PlayerClass.Dice_number[5];
							}

							if (PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1])
							{
								PlayerClass.Bearout_Delivery_Tower[0] = PlayerClass.Bearout_Delivery_Tower[1] = PlayerClass.Bearout_Delivery_Tower[2] = 24 - PlayerClass.Dice_number[3];
								PlayerClass.Bearout_Delivery_Tower[3] = 24 - PlayerClass.Dice_number[4];
							}

							if (PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1] && PlayerClass.DiceCheck[2])
							{
								for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
									PlayerClass.Bearout_Delivery_Tower[i] = 24 - PlayerClass.Dice_number[3];
							}
						}
						
						for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
						{
							if (!Is_The_Delivery_Tower_Free(24 - PlayerClass.Dice_number[3 + i]))
							{
								for (int j = 0; j < 4; j++)
								{
									PlayerClass.Bearout_Delivery_Tower[j] = 24 - PlayerClass.Dice_number[3];
								}
							}
						}
					}

					for (int d_w = 0; d_w < 4; d_w++)
					{
						if (!PlayerClass.DiceCheck[d_w] && PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[d_w] && Is_The_Delivery_Tower_Free(PlayerClass.Bearout_Delivery_Tower[d_w]) && root)
						{
							greenTowers[24].SetActive(true);
							coin_auto_step = false;
							Coin_step_by_step = true;
							tb_W = d_w;
							nextway = false;
							break;
						}

						
						if (!PlayerClass.DiceCheck[d_w] && PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[d_w] && Is_The_Delivery_Tower_Free(PlayerClass.Bearout_Delivery_Tower[d_w]) && !root)
						{
							greenTowers[24].SetActive(true);
							coin_auto_step = true;
							Coin_step_by_step = false;
							nextway = false;
							tb_W = d_w;
							break;
						}

					}
					if (!coin_auto_step)
					{
						for (var index = 0; index < PlayerClass.Bearout_Delivery_Tower.Length; index++)
						{
							var t = PlayerClass.Bearout_Delivery_Tower[index];
							nextway = PlayerClass.SelectedTowerNumber != t;
						}

						tb_W = 0;
					}


					if (nextway)
						for (int f = 18; f < PlayerClass.SelectedTowerNumber; f++) //According to the rules of backgammon, if the relative tower of dice numbers is empty, the player is able to use the closest and smaller tower 
						{
							if (boardArray[f, 0].CompareTag("White_coin"))
							{
								greenTowers[24].SetActive(false);
								nextway = false;
							}

							if (boardArray[f, 0].CompareTag("blank") && nextway && PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0] > 23)
							{
								greenTowers[24].SetActive(true);
								Coin_step_by_step = true;
							}
						}
				}
				
				//Black---------------------------------------------------------------------------------------------------------
				
				if (PlayerClass.BlackBearOut && PlayerClass.player_number == 2)
				{
					for (int i = 0; i < 4; i++)
						if (PlayerClass.DiceCheck[i])
							Free_All_legal_Towers = true;

					if (PlayerClass.SelectedTowerNumber == PlayerClass.Dice_number[0] - 1)
					{
						for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
						{
							PlayerClass.Bearout_Delivery_Tower[i] = PlayerClass.Dice_number[0] - 1;
						}
						root = true;
					}
					else
					{
						root = false;

						if (!PlayerClass.DiceCheck[0])
							for (int j = 0; j < PlayerClass.Bearout_Delivery_Tower.Length; j++)
								PlayerClass.Bearout_Delivery_Tower[j] = PlayerClass.Dice_number[3 + j] - 1 ;
						
						
						if (Free_All_legal_Towers)
						{
							if (PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1])
							{
								PlayerClass.Bearout_Delivery_Tower[0] = PlayerClass.Bearout_Delivery_Tower[1] = PlayerClass.Dice_number[3] - 1;
								PlayerClass.Bearout_Delivery_Tower[2] = PlayerClass.Dice_number[4] - 1;
								PlayerClass.Bearout_Delivery_Tower[3] = PlayerClass.Dice_number[5] - 1;
							}

							if (PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1])

							{
								PlayerClass.Bearout_Delivery_Tower[0] = PlayerClass.Bearout_Delivery_Tower[1] = PlayerClass.Bearout_Delivery_Tower[2] = PlayerClass.Dice_number[3] - 1;
								PlayerClass.Bearout_Delivery_Tower[3] = PlayerClass.Dice_number[4] - 1;
							}

							if (PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1] && PlayerClass.DiceCheck[2])

							{
								for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++) PlayerClass.Bearout_Delivery_Tower[i] = PlayerClass.Dice_number[3] - 1;
							}
						}

						for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
						{
							if (!Is_The_Delivery_Tower_Free(PlayerClass.Dice_number[3 + i] - 1))
							{
								for (int j = 0; j < 4; j++)
								{
									PlayerClass.Bearout_Delivery_Tower[j] = PlayerClass.Dice_number[3] - 1;
								}
							}
						}
					}
					
					for (int d_b = 0; d_b < 4; d_b++)
					{
						if (!PlayerClass.DiceCheck[d_b] && PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[d_b] && Is_The_Delivery_Tower_Free(PlayerClass.Bearout_Delivery_Tower[d_b]) && root)
						{
							greenTowers[25].SetActive(true);
							coin_auto_step = false;
							Coin_step_by_step = true;
							tb_B = d_b;
							nextway = false;
							break;
						}

						if (!PlayerClass.DiceCheck[d_b] && PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[d_b] && Is_The_Delivery_Tower_Free(PlayerClass.Bearout_Delivery_Tower[d_b]) && !root)
						{
							greenTowers[25].SetActive(true);
							coin_auto_step = true;
							Coin_step_by_step = false;
							tb_B = d_b;
							nextway = false;
							break;
						}

					}

					if (!coin_auto_step)
					{
						for (var index = 0; index < PlayerClass.Bearout_Delivery_Tower.Length; index++)
						{
							var t = PlayerClass.Bearout_Delivery_Tower[index];
							nextway = PlayerClass.SelectedTowerNumber != t;
						}

						tb_B = 0;
					}

					if (nextway)
						for (int f = 5; f > PlayerClass.SelectedTowerNumber; f--)
						{
							if (boardArray[f, 0].CompareTag("Black_coin"))
							{
								greenTowers[25].SetActive(false);
								nextway = false;
							}

							if (boardArray[f, 0].CompareTag("blank") && nextway && PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0] < 0)
							{
								greenTowers[25].SetActive(true);
								Coin_step_by_step = true;
							}
						}
				}
				
			}
			
			if (PlayerClass.opposite_dice && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
			{
				for (int i = 0; i < 15; i++)
				{
					if (PlayerClass.player_number == 1)
						WhiteCoinsContainer[i].GetComponent<Collider>().enabled = true;
					else
						BlackCoinsContainer[i].GetComponent<Collider>().enabled = true;
				}

				for (int i = 0; i < 2; i++)
				{
					if (!PlayerClass.DiceCheck[i])
						SumTowers = PlayerClass.Dice_number[i];

					if (PlayerClass.player_number == 1)
					{
						SwitchedTower = PlayerClass.SelectedTowerNumber + SumTowers;

						min = Mathf.Min(PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0], PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[1]);
						max = Mathf.Max(PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0], PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[1]);

						if (min < 24)
							PlayerClass.Coin_Legal_Tower[0] = min;
						else
							PlayerClass.Coin_Legal_Tower[0] = PlayerClass.SelectedTowerNumber;

						if (max < 24)
							PlayerClass.Coin_Legal_Tower[1] = max;
						else
							PlayerClass.Coin_Legal_Tower[1] = PlayerClass.SelectedTowerNumber;

						if (Is_The_Delivery_Tower_Free(PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0]))
							Best_Tower_To_deliver_Coin = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0];
						else
							Best_Tower_To_deliver_Coin = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[1];
						

						Sum_Dice_1_num_Dice_2_num = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1];

						if (Sum_Dice_1_num_Dice_2_num < 24)
							PlayerClass.Coin_Legal_Tower[2] = Sum_Dice_1_num_Dice_2_num;
						else
							PlayerClass.Coin_Legal_Tower[2] = PlayerClass.SelectedTowerNumber;
					}

					else

					{
						SwitchedTower = PlayerClass.SelectedTowerNumber - SumTowers;

						min = Mathf.Min(PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0], PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[1]);
						max = Mathf.Max(PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0], PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[1]);

						if (max > -1)
							PlayerClass.Coin_Legal_Tower[0] = max;
						else
							PlayerClass.Coin_Legal_Tower[0] = PlayerClass.SelectedTowerNumber;

						if (min > -1)
							PlayerClass.Coin_Legal_Tower[1] = min;
						else
							PlayerClass.Coin_Legal_Tower[1] = PlayerClass.SelectedTowerNumber;


						if (Is_The_Delivery_Tower_Free(PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0]))
							Best_Tower_To_deliver_Coin = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0];
						else
							Best_Tower_To_deliver_Coin = PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[1];
						

						Sum_Dice_1_num_Dice_2_num = PlayerClass.SelectedTowerNumber - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);

						if (Sum_Dice_1_num_Dice_2_num > -1)
							PlayerClass.Coin_Legal_Tower[2] = Sum_Dice_1_num_Dice_2_num;
						else
							PlayerClass.Coin_Legal_Tower[2] = PlayerClass.SelectedTowerNumber;
					}


					if (!PlayerClass.DiceCheck[i] && Is_The_Delivery_Tower_Free(SwitchedTower) && SwitchedTower < 24 && SwitchedTower > -1)
						greenTowers[SwitchedTower].SetActive(true);
					else
						nextway = true;


					int sum_Of_D1_And_D2;

					if (PlayerClass.player_number == 1)
						sum_Of_D1_And_D2 = PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1];
					else
						sum_Of_D1_And_D2 = PlayerClass.SelectedTowerNumber - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);


					if (Is_The_Delivery_Tower_Free(SwitchedTower) && !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && 
					    Is_The_Delivery_Tower_Free(sum_Of_D1_And_D2) && sum_Of_D1_And_D2 < 24 && sum_Of_D1_And_D2 > -1)
						greenTowers[sum_Of_D1_And_D2].SetActive(true);
				}

				//-------------------------------------------------------------------------------------------------------------

				if (PlayerClass.WhiteBearOut && PlayerClass.player_number == 1)
				{
					PlayerClass.Bearout_Delivery_Tower[0] = 24 - PlayerClass.Dice_number[0];
					PlayerClass.Bearout_Delivery_Tower[1] = 24 - PlayerClass.Dice_number[1];
					PlayerClass.Bearout_Delivery_Tower[2] = 24 - (PlayerClass.Dice_number[1] + PlayerClass.Dice_number[0]);

					for (int dt = 0; dt < 2; dt++)
						if (PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[dt] && !PlayerClass.DiceCheck[dt])
						{
							greenTowers[24].SetActive(true);
							Coin_step_by_step = true;
							coin_auto_step = false;
							nextway = false;
							tb_W = dt;
							break;
						}

					if (PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[2] &&
					    !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1])
					{
						greenTowers[24].SetActive(true);
						Coin_step_by_step = false;
						coin_auto_step = true;
						tb_W = 2;
						nextway = false;
					}

					for (int i = 0; i <= 2; i++) 
						if (PlayerClass.SelectedTowerNumber != PlayerClass.Bearout_Delivery_Tower[i] && nextway) // for test
						{
							for (int f = 18; f < PlayerClass.SelectedTowerNumber; f++)
							{
								if (boardArray[f, 0].CompareTag("White_coin"))
								{
									greenTowers[24].SetActive(false);
									nextway = false;
								}

								if (boardArray[PlayerClass.Bearout_Delivery_Tower[0], 0].CompareTag("White_coin") && nextway)
								{
									tb_W = PlayerClass.Dice_number[0] > PlayerClass.Dice_number[1] ? 0 : 1; // which one is bigger : PlayerClass.Dice_number [0] or PlayerClass.Dice_number [1]
									//put the result in tb_w

									if (!boardArray[f, 0].CompareTag("White_coin") && !PlayerClass.DiceCheck[tb_W])
									{
										if (PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[0] > 23 |
										    PlayerClass.SelectedTowerNumber + PlayerClass.Dice_number[1] > 23)
											greenTowers[24].SetActive(true);

										Coin_step_by_step = true;

									}
								}

								else

								{
									if (!PlayerClass.DiceCheck[0] && 24 - PlayerClass.SelectedTowerNumber < PlayerClass.Dice_number[0] && nextway)
									{
										tb_W = 0;
										greenTowers[24].SetActive(true);
										Coin_step_by_step = true;
									}

									if (!PlayerClass.DiceCheck[1] && 24 - PlayerClass.SelectedTowerNumber < PlayerClass.Dice_number[1] && nextway)
									{
										tb_W = 1;
										greenTowers[24].SetActive(true);
										Coin_step_by_step = true;
									}

									if (!boardArray[f, 0].CompareTag("White_coin") && !PlayerClass.DiceCheck[tb_W] &&
									    PlayerClass.SelectedTowerNumber < PlayerClass.Dice_number[tb_W] && nextway)
									{
										greenTowers[24].SetActive(true);
										Coin_step_by_step = true;
									}
								}
							}
						}
				}
				
				//Black--------------------------------------------------------------------------------------------
				
				if (PlayerClass.BlackBearOut && PlayerClass.player_number == 2)
				{
					PlayerClass.Bearout_Delivery_Tower[0] = PlayerClass.Dice_number[0] - 1;
					PlayerClass.Bearout_Delivery_Tower[1] = PlayerClass.Dice_number[1] - 1;
					PlayerClass.Bearout_Delivery_Tower[2] = PlayerClass.Dice_number[1] + PlayerClass.Dice_number[0] - 1;

					for (int st = 0; st < 2; st++)
						if (PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[st] && !PlayerClass.DiceCheck[st])
						{
							greenTowers[25].SetActive(true);
							Coin_step_by_step = true;
							coin_auto_step = false;
							tb_B = st;
							nextway = false;
							break;
						}

					if (PlayerClass.SelectedTowerNumber == PlayerClass.Bearout_Delivery_Tower[2] && !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1])
					{
						greenTowers[25].SetActive(true);
						Coin_step_by_step = false;
						coin_auto_step = true;
						tb_B = 2;
						nextway = false;
					}

					for (int i = 0; i <= 2; i++)
						if (PlayerClass.SelectedTowerNumber != PlayerClass.Bearout_Delivery_Tower[i] && nextway) //for test
						{
							for (int f = 5; f > PlayerClass.SelectedTowerNumber; f--)
							{
								if (boardArray[f, 0].CompareTag("Black_coin"))
								{
									nextway = false;
									greenTowers[25].SetActive(false);
								}

								if (nextway && boardArray[PlayerClass.Bearout_Delivery_Tower[0], 0].CompareTag("Black_coin")) //| Board_Array [PlayerClass.Bearout_Delivery_Tower [1], 0].CompareTag("Black_coin")
								{
									tb_B = PlayerClass.Dice_number[0] > PlayerClass.Dice_number[1] ? 0 : 1; // which one is bigger ?
									if (!PlayerClass.DiceCheck[tb_B])
									{
										if (PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[0] < 0 |
										    PlayerClass.SelectedTowerNumber - PlayerClass.Dice_number[1] < 0)
											greenTowers[25].SetActive(true);
										Coin_step_by_step = true;
									}
								}
								else
								{
									if (!PlayerClass.DiceCheck[0] && PlayerClass.SelectedTowerNumber + 1 < PlayerClass.Dice_number[0] && nextway)
									{
										tb_B = 0;
										greenTowers[25].SetActive(true);
										Coin_step_by_step = true;
									}
									else if (!PlayerClass.DiceCheck[1] && PlayerClass.SelectedTowerNumber + 1 < PlayerClass.Dice_number[1] && nextway)
									{
										tb_B = 1;
										greenTowers[25].SetActive(true);
										Coin_step_by_step = true;
									}

									if (!boardArray[f, 0].CompareTag("Black_coin") && nextway && !PlayerClass.DiceCheck[tb_B] && PlayerClass.SelectedTowerNumber < PlayerClass.Dice_number[tb_B])
									{
										greenTowers[25].SetActive(true);
										Coin_step_by_step = true;
									}
								}
							}
						}
				}
			}
		}
	}

	#region Is_Free_Tower

	public bool Is_The_Delivery_Tower_Free (int trgTwr) //if the Chosen towers have more than one checker of its competitor will return false
	{
		numberOfCheckers = 0;
		if (trgTwr < 24 && trgTwr > -1)
			while (!boardArray [trgTwr, numberOfCheckers].CompareTag(PlayerClass.SelectedCoin.gameObject.tag) && !boardArray [trgTwr, numberOfCheckers].CompareTag("blank"))
				numberOfCheckers++;
		return numberOfCheckers < 2;
	}

	#endregion


	public void FindLegalTowers () //Calculate and find out which towers are legal to select and how many move the player has
	{
		//!: false	
		
		//****** At first all resets necessary variables
	
		int cn0 , cnBearOut,cn1BearOut , evencoinCacher, evencoinCacherBearOut,mirror_of_legal_twr;
		
		cn0  = cnBearOut = cn1BearOut = evencoinCacher  = mirror_of_legal_twr = 0;
		NumOfPlayerMoves = NumOfBearOutMoves = -1;
		
		bool d0Source = false, d1Source = false;
		
		CoinDefaultColor = false;
		AgainstRules = false;
		
		for (int i = 0; i < 15; i++)
			evenDiceCoinIndex[i] = 0;
		
		LstoppositeDiceTowers.Clear();
		LstoppositeDiceBearOut.Clear();
		LstevenDiceTowers.Clear();

		//*******

		if (PlayerClass.player_number == 1)
		{
			if(!PlayerClass.playerHasBurntCoin )
				for (Count_WC = 0; Count_WC < 24; Count_WC++) //will find all white_coins in the board
				{
					if (boardArray[Count_WC, 0].CompareTag("White_coin")) // gets index of tower
					{
						if (PlayerClass.even_dice)
						{
							TheEvenTower = Count_WC + PlayerClass.Dice_number[0];
							if (Is_The_Delivery_Tower_Free(TheEvenTower) && TheEvenTower <= 23 && !PlayerClass.DiceCheck[3])
							{
								LstevenDiceTowers.Add(Count_WC);
								
								evenDiceCoinIndex[cn0] = IndexOfCoinInSelectedTower(Count_WC); // b4  (Count_WC) +1
								if (!PlayerClass.DiceCheck[3])
								{
									evencoinCacher = evencoinCacher + evenDiceCoinIndex[cn0];
									NumOfPlayerMoves = evencoinCacher > 3 ? 4 : evencoinCacher;
								}

								cn0++;
							}
						}
						if (PlayerClass.opposite_dice)
						{
							monitor_tower_d1 = Count_WC + PlayerClass.Dice_number[0];
							monitor_tower_d2 = Count_WC + PlayerClass.Dice_number[1];
							
							if (!PlayerClass.DiceCheck[0] && Is_The_Delivery_Tower_Free(monitor_tower_d1) &&
							    monitor_tower_d1 > -1 && monitor_tower_d1 <= 23)

							{
								LstoppositeDiceTowers.Add(Count_WC);
								cn0++;
							}
							if (!PlayerClass.DiceCheck[1] && Is_The_Delivery_Tower_Free(monitor_tower_d2) &&
							    monitor_tower_d2 > -1 && monitor_tower_d2 <= 23)
								LstoppositeDiceTowers.Add(Count_WC);

							if (!PlayerClass.DiceCheck[1] | !PlayerClass.DiceCheck[0])
								if (LstoppositeDiceTowers.Count > 1)
									NumOfPlayerMoves = 2;
								else if (LstoppositeDiceTowers.Count == 1)
									NumOfPlayerMoves = 1;
								else
									NumOfPlayerMoves = -1;
						}
					}
				}

			//---------------------------------------------
			
			if(PlayerClass.playerHasBurntCoin)
			{
				if (PlayerClass.opposite_dice)
				{
					if (!Is_The_Delivery_Tower_Free(PlayerClass.Dice_number[0] - 1) && !Is_The_Delivery_Tower_Free(PlayerClass.Dice_number[1] - 1))
						NumOfPlayerMoves = -1;
					else
						NumOfPlayerMoves = PlayerClass.Num_Of_WC_Burnt == 0 ? 2 : 1;

					if (PlayerClass.Num_Of_WC_Burnt >= 0 && (PlayerClass.DiceCheck[0] && !Is_The_Delivery_Tower_Free(PlayerClass.Dice_number[1] - 1)) | 
					    (PlayerClass.DiceCheck[1] && !Is_The_Delivery_Tower_Free(PlayerClass.Dice_number[0] - 1)))
					{
						NumOfPlayerMoves = -1;
						CoinDefaultColor = true;
					}
				}
		
				if(PlayerClass.even_dice)
					if (!Is_The_Delivery_Tower_Free (PlayerClass.Dice_number [0]-1))
						NumOfPlayerMoves = -1;
					else
						NumOfPlayerMoves = PlayerClass.Num_Of_WC_Burnt == 0 ? 2 : 1;
			}
			
			//--------------------------------------------

			if (PlayerClass.WhiteBearOut)
			{
				if (PlayerClass.even_dice)
				{
					for (even_count_TBO = 18; even_count_TBO <= 23; even_count_TBO++)
					{
						Even_monitor_tbo = PlayerClass.Dice_number[0] + even_count_TBO;

						if (Even_monitor_tbo == 24 && !PlayerClass.DiceCheck[3] && IndexOfCoinInSelectedTower(even_count_TBO) > -1 && even_count_TBO == 24 - PlayerClass.Dice_number[0] && boardArray[even_count_TBO, 0].CompareTag("White_coin"))
						{
							evenDiceNumBearOut = even_count_TBO;
							evenDiceIndexWin = IndexOfCoinInSelectedTower(even_count_TBO);
							cn1BearOut = 1;
							break;
						}
					}

					if (cn1BearOut == 0)
					{
						for (int i = 18; i <= 23; i++)
						{
							if (boardArray[i, 0].CompareTag("White_coin"))
							{
								for (int j = 18; j < i; j++)
								{
									if (i + PlayerClass.Dice_number[0] > 23 && !boardArray[j, 0].CompareTag("White_coin"))
									{
										mirror_of_legal_twr = i;
										evenDiceIndexWin = IndexOfCoinInSelectedTower(mirror_of_legal_twr) + 1;
										AgainstRules = true;
									}
								}

								break;
							}
						}
						
						if (AgainstRules)
							for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
								if (mirror_of_legal_twr != PlayerClass.Bearout_Delivery_Tower[i] && !PlayerClass.DiceCheck[3])
									evenDiceNumBearOut = mirror_of_legal_twr;
					}

					evencoinCacherBearOut = cn1BearOut + evenDiceIndexWin;

					NumOfBearOutMoves = evencoinCacherBearOut > 3 ? 4 : evencoinCacherBearOut;
				}
				
				if (PlayerClass.opposite_dice)
				{
					for (count_TBO_d1 = 18; count_TBO_d1 <= 23; count_TBO_d1++)
					{
						monitor_tower_d1_Bo = PlayerClass.Dice_number[0] + count_TBO_d1;
						
						if (monitor_tower_d1_Bo == 24 && count_TBO_d1 == 24 - PlayerClass.Dice_number[0] &&
						    !PlayerClass.DiceCheck[0] && boardArray[count_TBO_d1, 0].CompareTag("White_coin"))
						{
							LstoppositeDiceBearOut.Add(count_TBO_d1);
							cnBearOut = 1;
							break;
						}
					}

					for (count_TBO_d2 = 18; count_TBO_d2 <= 23; count_TBO_d2++)
					{
						monitor_tower_d2_Bo = PlayerClass.Dice_number[1] + count_TBO_d2;
						
						if (monitor_tower_d2_Bo == 24 && count_TBO_d2 == 24 - PlayerClass.Dice_number[1] &&
						    !PlayerClass.DiceCheck[1] && boardArray[count_TBO_d2, 0].CompareTag("White_coin"))
						{
							LstoppositeDiceBearOut.Add(count_TBO_d2);
							cn1BearOut = 1;
							break;
						}
					}

					if (cnBearOut == 0 && cn1BearOut == 0 && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
					{
						for (int i = 18; i <= 23; i++)
						{
							if (boardArray[i, 0].CompareTag("White_coin"))
							{
								if (!PlayerClass.DiceCheck[0])
									for (int j = 18; j <= i; j++)
										if (boardArray[j, 0].CompareTag("White_coin")) // if towers before of selected tower consist of white checker
											d0Source = true;
								
								//----------------------------------------------------------

								if (!PlayerClass.DiceCheck[1])
									for (int j1 = 18; j1 <= i; j1++)
										if (boardArray[j1, 0].CompareTag("White_coin"))
											d1Source = true;
								
								if (d0Source | d1Source)
								{
									mirror_of_legal_twr = i;
									AgainstRules = true;
								}

								break;
							}
						}
					}

					if (AgainstRules)
					{
						if (cnBearOut == 0 && !PlayerClass.DiceCheck[0] && mirror_of_legal_twr + PlayerClass.Dice_number[0] > 23)
						{
							LstoppositeDiceBearOut.Clear();
							LstoppositeDiceBearOut.Add(mirror_of_legal_twr);
							cnBearOut = 1;
						}

						if (cn1BearOut == 0 && !PlayerClass.DiceCheck[1] && mirror_of_legal_twr + PlayerClass.Dice_number[1] > 23)
						{
							LstoppositeDiceBearOut.Clear();
							LstoppositeDiceBearOut.Add(mirror_of_legal_twr);
							cn1BearOut = 1;
						}
					}

					if (!PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
						NumOfBearOutMoves = cn1BearOut + cnBearOut;

				}
			}
					
	    }

//----------------------------------Black-----------------------------------------------------------

		if (PlayerClass.player_number == 2)
		{
			if(!PlayerClass.playerHasBurntCoin)
				for (Count_WC = 23; Count_WC >= 0; Count_WC--)
				{
					if (boardArray[Count_WC, 0].CompareTag("Black_coin"))
					{
						if (PlayerClass.even_dice)
						{
							TheEvenTower = Count_WC - PlayerClass.Dice_number[0];

							if (Is_The_Delivery_Tower_Free(TheEvenTower) && TheEvenTower >= 0 && TheEvenTower < 24 && !PlayerClass.DiceCheck[3]) // ThePairTower>-1
							{
								LstevenDiceTowers.Add(Count_WC);
								
								evenDiceCoinIndex[cn0] = IndexOfCoinInSelectedTower(Count_WC) ; // + 1

								if (!PlayerClass.DiceCheck[3])

								{
									evencoinCacher = evencoinCacher + evenDiceCoinIndex[cn0];

									NumOfPlayerMoves = evencoinCacher > 3 ? 4 : evencoinCacher;
								}

								cn0++;
							}
						}

						if (PlayerClass.opposite_dice)
						{
							monitor_tower_d1 = Count_WC - PlayerClass.Dice_number[0];
							monitor_tower_d2 = Count_WC - PlayerClass.Dice_number[1];

							if (!PlayerClass.DiceCheck[0] && Is_The_Delivery_Tower_Free(monitor_tower_d1) && monitor_tower_d1 >= 0 && monitor_tower_d1 < 23) //monitor_tower_d1 > 23
								LstoppositeDiceTowers.Add(Count_WC);
						
							if (!PlayerClass.DiceCheck[1] && Is_The_Delivery_Tower_Free(monitor_tower_d2) &&
							    monitor_tower_d2 >= 0 && monitor_tower_d2 <= 23)
								LstoppositeDiceTowers.Add(Count_WC);

							if (!PlayerClass.DiceCheck[1] | !PlayerClass.DiceCheck[0])
								if (LstoppositeDiceTowers.Count > 1)
									NumOfPlayerMoves = 2;
								else if (LstoppositeDiceTowers.Count == 1)
									NumOfPlayerMoves = 1;
								else
									NumOfPlayerMoves = -1;
						}
					}
				}
			
			//----------------------------------------------

			if(PlayerClass.playerHasBurntCoin)
			{
				if (PlayerClass.opposite_dice)
				{
					if (!Is_The_Delivery_Tower_Free (24 - PlayerClass.Dice_number[0]) && !Is_The_Delivery_Tower_Free (24 - PlayerClass.Dice_number[1]))
						NumOfPlayerMoves = -1;
					else
						NumOfPlayerMoves = PlayerClass.Num_Of_BC_Burnt == 0 ? 2 : 1;

					if (PlayerClass.Num_Of_BC_Burnt == 0 && (PlayerClass.DiceCheck[0] && !Is_The_Delivery_Tower_Free(24 - PlayerClass.Dice_number[1])) | 
					    (PlayerClass.DiceCheck[1] && !Is_The_Delivery_Tower_Free(24 - PlayerClass.Dice_number[0])))
					{
						NumOfPlayerMoves = -1;
						CoinDefaultColor = true;
					}
				}
				
				if(PlayerClass.even_dice)
					if (!Is_The_Delivery_Tower_Free (24-PlayerClass.Dice_number [0])) 
						NumOfPlayerMoves = -1;
					else
						NumOfPlayerMoves = PlayerClass.Num_Of_BC_Burnt > 0 ? 2 : 1;

			}

			//--------------------------------------------------------------------------------------------
			
			if (PlayerClass.BlackBearOut)
			{
				if (PlayerClass.even_dice)
				{
					for (even_count_TBO = 5; even_count_TBO >= 0; even_count_TBO--)
					{
						Even_monitor_tbo = even_count_TBO - PlayerClass.Dice_number [0];

						if (Even_monitor_tbo == -1 && even_count_TBO + 1 == PlayerClass.Dice_number[0] &&
						    !PlayerClass.DiceCheck [3] && IndexOfCoinInSelectedTower(even_count_TBO) > -1 && boardArray[even_count_TBO,0].CompareTag("Black_coin"))
						{
							evenDiceNumBearOut  = even_count_TBO;
							evenDiceIndexWin = IndexOfCoinInSelectedTower (even_count_TBO);
							cn1BearOut = 1;
							break;
						}

					}

					if(cn1BearOut == 0)
					{
						for (int i = 5; i >= 0; i--)
						{
							if (boardArray[i, 0].CompareTag("Black_coin"))
							{
								for (int j = 5; j > i; j--)
								{
									if (i - PlayerClass.Dice_number[0] < 0 && !boardArray[j, 0].CompareTag("Black_coin"))
									{
										mirror_of_legal_twr = i;
										evenDiceIndexWin = IndexOfCoinInSelectedTower(mirror_of_legal_twr) + 1;
										AgainstRules = true;
									}
								}

								break;
							}
						}
						
						if(AgainstRules)
							for (int i = 0; i < PlayerClass.Bearout_Delivery_Tower.Length; i++)
								if (mirror_of_legal_twr != PlayerClass.Bearout_Delivery_Tower[i] && !PlayerClass.DiceCheck[3])
									evenDiceNumBearOut = mirror_of_legal_twr;

					}

					evencoinCacherBearOut = cn1BearOut + evenDiceIndexWin;

					if (evencoinCacherBearOut > 3)
						NumOfBearOutMoves = 4;
					else
						NumOfBearOutMoves = evencoinCacherBearOut;
				}
				if (PlayerClass.opposite_dice)
				{
					for (count_TBO_d1 = 5; count_TBO_d1 >= 0; count_TBO_d1--)
					{
						monitor_tower_d1_Bo = count_TBO_d1 - PlayerClass.Dice_number [0];
																			
						if (monitor_tower_d1_Bo == -1 && count_TBO_d1 + 1 == PlayerClass.Dice_number[0] 
						    && !PlayerClass.DiceCheck[0] && boardArray[count_TBO_d1,0].CompareTag("Black_coin") )
						{
							LstoppositeDiceBearOut.Add(count_TBO_d1);
							cnBearOut = 1;
							break;
						}
					}

					for (count_TBO_d2 = 5; count_TBO_d2 >= 0; count_TBO_d2--)
					{
						monitor_tower_d2_Bo = count_TBO_d2 - PlayerClass.Dice_number [1];

						if (monitor_tower_d2_Bo == -1 && count_TBO_d2 + 1 == PlayerClass.Dice_number[1] 
						                              && !PlayerClass.DiceCheck[1] && boardArray[count_TBO_d2,0].CompareTag("Black_coin"))
						{
							LstoppositeDiceBearOut.Add(count_TBO_d2);
							cn1BearOut = 1;
							break;
						}

					}
					
					if(cnBearOut == 0 && cn1BearOut == 0 && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
					{
						for(int i = 5; i >= 0; i--)
						{
							if(boardArray[i,0].CompareTag("Black_coin"))
							{

								if(!PlayerClass.DiceCheck[0])
									for(int j = 5; j >= i ; j--)
										if(!boardArray[j,0].CompareTag("Black_coin"))
											d0Source = true;

								if(!PlayerClass.DiceCheck[1])
									for(int j1 = 5; j1 >= i; j1--)
										if(!boardArray[j1,0].CompareTag("Black_coin"))
											d1Source = true;
								
								if(d0Source | d1Source )

								{
									mirror_of_legal_twr = i;
									AgainstRules = true;
								}

								break;
							}
						}
					}

					if(AgainstRules )
					{
						if(cnBearOut == 0 && !PlayerClass.DiceCheck[0] && mirror_of_legal_twr - PlayerClass.Dice_number[0] < 0)
						{
							LstoppositeDiceBearOut.Clear();
							LstoppositeDiceBearOut.Add(mirror_of_legal_twr);
							cnBearOut = 1;
						}

						if(cn1BearOut == 0 && !PlayerClass.DiceCheck[1] && mirror_of_legal_twr- PlayerClass.Dice_number[1]  < 0)
						{
							LstoppositeDiceBearOut.Clear();
							LstoppositeDiceBearOut.Add(mirror_of_legal_twr);
							cn1BearOut = 1;
						}

					}
					
					if (!PlayerClass.DiceCheck [0] | !PlayerClass.DiceCheck [1])
						NumOfBearOutMoves = cn1BearOut + cnBearOut;
					
				}

			}

		}
		
	//	PlayerClass.BearOutPossible(PlayerClass.Who); // task of this function is to determine that player is able to bear out it's checkers.

		// if there is no other move for player

		//if (!PlayerClass.DisconnectAutoMove)
			if ((NumOfPlayerMoves == -1 && (PlayerClass.player_number == 1 && !PlayerClass.WhiteBearOut) | (PlayerClass.player_number == 2 && !PlayerClass.BlackBearOut)) |
		    ((PlayerClass.player_number == 1 && PlayerClass.WhiteBearOut) | (PlayerClass.player_number == 2 && PlayerClass.BlackBearOut) && NumOfBearOutMoves == -1) )
		{
			if ((PlayerClass.opposite_dice && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1]) |
			    (PlayerClass.even_dice && !PlayerClass.DiceCheck[3]))
			{
				if (PlayerClass.player_number == 1 | (PlayerClass.player_number == 2 && !PlayerClass.Black_Bot))
				{
					DataManagerClass.noMove.Play("Nomove");
					DataManagerClass.noMoveSnd.Play();
				}

			}

			// in situation that player just has only one possible move and after making its move the dice should pass to against player
			if ((!PlayerClass.Black_Bot && PlayerClass.player_number == 1) | PlayerClass.player_number == 2)
			{
				PlayerClass.removeDice = true;
				
				PlayerClass.DoubleCube();
				DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
				DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);

				DiceRollClass.particleRemoveDice1Alarm.Play();
				DiceRollClass.particleRemoveDice2Alarm.Play();
			
			}
			else if (PlayerClass.Black_Bot && PlayerClass.player_number == 1 && (PlayerClass.opposite_dice && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1]) | 
			        (PlayerClass.even_dice && !PlayerClass.DiceCheck[3]))
				StartCoroutine(IEResetAllData());
		
		}

	}

	IEnumerator IEResetAllData() // after 1 sec rolls the dice
	{
		yield return new WaitForSecondsRealtime(1f);
		DiceRollClass.ResetAllData();
	}
	

	#region FTC
	public void FlashTouchableCoins () // the legal coins will flash and gives a quick guide to player
	{
		if (!CoinDefaultColor)
		    lerpedColor = PlayerClass.player_number == 1 ? Color.Lerp(White_flash_off, White_flash_on, Mathf.PingPong(Time.time, 1f)) :
			    Color.Lerp(Black_flash_off, Black_flash_on, Mathf.PingPong(Time.time, 1f));
		
	    #region regular 

	    if (!PlayerClass.playerHasBurntCoin)
	    {
		    if (PlayerClass.player_number == 1 && !PlayerClass.WhiteBearOut | PlayerClass.player_number == 2 && !PlayerClass.BlackBearOut && 
		        (PlayerClass.even_dice && LstevenDiceTowers.Count == 0) | (PlayerClass.opposite_dice && LstoppositeDiceTowers.Count == 0))
			    CoinDefaultColor = true;

		    if (PlayerClass.player_number == 1)
		    {
			    if (PlayerClass.even_dice && LstevenDiceTowers.Count > 0)
				    for (var index = 0; index < LstevenDiceTowers.Count; index++)
				    {
					    if (CoinDefaultColor && !boardArray[LstevenDiceTowers.ElementAt(index),IndexOfCoinInSelectedTower(LstevenDiceTowers.ElementAt(index))].CompareTag("Black_coin"))
						    boardArray[LstevenDiceTowers.ElementAt(index), IndexOfCoinInSelectedTower(LstevenDiceTowers.ElementAt(index))].GetComponent<Renderer>().material =
							    WhiteCoinDefultColor;
					    
					    if (!CoinDefaultColor && !boardArray[LstevenDiceTowers.ElementAt(index),IndexOfCoinInSelectedTower(LstevenDiceTowers.ElementAt(index))].CompareTag("Black_coin"))
						    boardArray[LstevenDiceTowers.ElementAt(index),IndexOfCoinInSelectedTower(LstevenDiceTowers.ElementAt(index))].GetComponent<Renderer>().material.color =
							    lerpedColor;

				    }

			    if (PlayerClass.opposite_dice && LstoppositeDiceTowers.Count > 0)
				    for (var index = 0; index < LstoppositeDiceTowers.Count; index++)
				    {
					
					    if (CoinDefaultColor && !boardArray[LstoppositeDiceTowers.ElementAt(index),IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))].CompareTag("Black_coin"))
						    boardArray[LstoppositeDiceTowers.ElementAt(index), IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))].GetComponent<Renderer>().material = WhiteCoinDefultColor;
					    
					    if (!CoinDefaultColor && !boardArray[LstoppositeDiceTowers.ElementAt(index),IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))].CompareTag("Black_coin"))
						    boardArray[LstoppositeDiceTowers.ElementAt(index), IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))].GetComponent<Renderer>().material.color = lerpedColor;
					}
			  
		    }

		    if (PlayerClass.player_number == 2)
		    {
			    if (PlayerClass.even_dice && LstevenDiceTowers.Count > 0)
				    for (var index = 0; index < LstevenDiceTowers.Count; index++)
				    {
					
					    if (CoinDefaultColor)
						    boardArray[LstevenDiceTowers.ElementAt(index), 
							    IndexOfCoinInSelectedTower(LstevenDiceTowers.ElementAt(index))].GetComponent<Renderer>().material = BlackCoinDefultColor;
					    
					    if (!CoinDefaultColor)
						    boardArray[LstevenDiceTowers.ElementAt(index),
								    IndexOfCoinInSelectedTower(LstevenDiceTowers.ElementAt(index))].GetComponent<Renderer>().material.color = lerpedColor;

				    }
			    
			    if (PlayerClass.opposite_dice &&  LstoppositeDiceTowers.Count > 0)
				    for (var index = 0; index < LstoppositeDiceTowers.Count; index++)
				    {
						
					    if (CoinDefaultColor)
						    boardArray[LstoppositeDiceTowers.ElementAt(index), IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))]
							    .GetComponent<Renderer>().material = BlackCoinDefultColor;
					    
					    if (!CoinDefaultColor && !boardArray[LstoppositeDiceTowers.ElementAt(index), IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))].CompareTag("White_coin"))
						    boardArray[LstoppositeDiceTowers.ElementAt(index), IndexOfCoinInSelectedTower(LstoppositeDiceTowers.ElementAt(index))]
							    .GetComponent<Renderer>().material.color = lerpedColor;

					  
				    }
		    }
		    
	    }
	    else
	    {
		    //flashes burn coins 
		    if(PlayerClass.player_number == 1 )
		    {
			    for (int wcBurnt = 0; wcBurnt < 15; wcBurnt++)
			    {
				    if (PlayerClass.Container_Of_WC_burnt [wcBurnt] == null && wcBurnt > 0 )
				    {
					    Last_Burnt_Coin = PlayerClass.Container_Of_WC_burnt[wcBurnt-1];
					    break;
				    }
			    }

			    if (Last_Burnt_Coin != null)
				    Last_Burnt_Coin.GetComponent<Renderer> ().material.color = lerpedColor;
			 
		    }

		    if(PlayerClass.player_number == 2 )
		    {
			    for (int bcBurnt = 0; bcBurnt < 15; bcBurnt++)
			    {
				    if (PlayerClass.Container_Of_BC_burnt [bcBurnt] == null && bcBurnt > 0 )
				    {
					    Last_Burnt_Coin = PlayerClass.Container_Of_BC_burnt[bcBurnt-1];
					    break;
				    }
			    }

			    if (Last_Burnt_Coin != null)
			    {
				    Last_Burnt_Coin.GetComponent<Renderer> ().material.color = lerpedColor;
			    }
	
		    }
		  
	    }
		
	    #endregion

	    #region hit_coin_WB_win
		
	    if (PlayerClass.player_number == 1 && PlayerClass.WhiteBearOut)
	    {
		    if ((NumOfPlayerMoves == 0 && NumOfBearOutMoves == 0) | PlayerClass.DiceCheck [3])
			    CoinDefaultColor = true;
				
		    if (PlayerClass.even_dice)
		    {
			    if (!CoinDefaultColor && IndexOfCoinInSelectedTower(evenDiceNumBearOut) > -1 && 
			        !boardArray [evenDiceNumBearOut, IndexOfCoinInSelectedTower (evenDiceNumBearOut)].CompareTag("Black_coin")) // error index out of range
				    boardArray [evenDiceNumBearOut, IndexOfCoinInSelectedTower (evenDiceNumBearOut)].GetComponent<Renderer> ().material.color = lerpedColor;
			    
			    if (CoinDefaultColor && IndexOfCoinInSelectedTower(evenDiceNumBearOut) > -1 && 
			        !boardArray [evenDiceNumBearOut, IndexOfCoinInSelectedTower (evenDiceNumBearOut)].CompareTag("Black_coin"))
				    boardArray [evenDiceNumBearOut, IndexOfCoinInSelectedTower (evenDiceNumBearOut)].GetComponent<Renderer> ().material = WhiteCoinDefultColor;
		    }

		    //------------------------------------------------------------------------------------------------------------
		    
		    if (PlayerClass.opposite_dice)
		    {
			    if (NumOfPlayerMoves == 0 && LstoppositeDiceBearOut.Count == 0)
				    CoinDefaultColor = true;
					
			    if (!CoinDefaultColor && LstoppositeDiceBearOut.Count > 0)
				    for (int index = 0; index < LstoppositeDiceBearOut.Count; index++)
					    boardArray [LstoppositeDiceBearOut.ElementAt(index), 
							 IndexOfCoinInSelectedTower (LstoppositeDiceBearOut.ElementAt(index))].GetComponent<Renderer> ().material.color = lerpedColor;

			    if (CoinDefaultColor && LstoppositeDiceBearOut.Count > 0)
				    for (int index = 0; index < LstoppositeDiceBearOut.Count; index++)
					    boardArray [LstoppositeDiceBearOut.ElementAt(index), 
						    IndexOfCoinInSelectedTower (LstoppositeDiceBearOut.ElementAt(index))].GetComponent<Renderer> ().material = WhiteCoinDefultColor;
		    }

	    }


	    if (PlayerClass.player_number == 2  && PlayerClass.BlackBearOut)
	    {
		    
		    if (NumOfPlayerMoves == 0 && NumOfBearOutMoves == 0  | PlayerClass.DiceCheck [3])
			    CoinDefaultColor = true;
			
		    if (PlayerClass.even_dice)
		    {
			    if (!CoinDefaultColor && IndexOfCoinInSelectedTower(evenDiceNumBearOut) > -1)
				    boardArray [evenDiceNumBearOut, IndexOfCoinInSelectedTower (evenDiceNumBearOut)].GetComponent<Renderer> ().material.color = lerpedColor;
				
			    if (CoinDefaultColor && IndexOfCoinInSelectedTower(evenDiceNumBearOut) > -1)
				    boardArray [evenDiceNumBearOut, IndexOfCoinInSelectedTower (evenDiceNumBearOut)].GetComponent<Renderer> ().material = BlackCoinDefultColor;
		    }


		    //------------------------------------------------------------------------------------------------------------

		    if (PlayerClass.opposite_dice)
		    {

			    if (NumOfPlayerMoves == 0 && LstoppositeDiceBearOut.Count == 0)
				    CoinDefaultColor = true;
					
			    if (!CoinDefaultColor && LstoppositeDiceBearOut.Count > 0 ) //ERROR
				    for (int index = 0; index < LstoppositeDiceBearOut.Count; index++)
					    boardArray [LstoppositeDiceBearOut.ElementAt(index), 
						    IndexOfCoinInSelectedTower (LstoppositeDiceBearOut.ElementAt(index))].GetComponent<Renderer> ().material.color = lerpedColor;

			    if (CoinDefaultColor && LstoppositeDiceBearOut.Count > 0)
				    for (int index = 0; index < LstoppositeDiceBearOut.Count; index++)
					    boardArray [LstoppositeDiceBearOut.ElementAt(index), 
						    IndexOfCoinInSelectedTower (LstoppositeDiceBearOut.ElementAt(index))].GetComponent<Renderer> ().material = BlackCoinDefultColor;
		    }
				
	    }
	    #endregion

	}
	#endregion

}


