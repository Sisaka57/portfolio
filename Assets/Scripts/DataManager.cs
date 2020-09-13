using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
	//2020 Arash KhallaqDoust (RVBinary)
	
	//this class will manage menu / settings and all process of saving and loading data of game
	public static DataManager DataManagerClass;
	public int White_Overall_double_Offer,Black_Overall_double_Offer;
	public int OverAllScoreForWhite,OverAllScoreForBlack;
	public int Overall_DiceRoll_White,Overall_DiceRoll_Black;
	public GameObject doubleCubeObj;
	public Animator scoreTables,winnerMsg, offerDouble, noMove,settings_menu_anim;
	public Text whiteMoveCounterTxt,blackMoveCounterTxt;
	public Text[] scoreTableTexts = new Text[8];
	public ParticleSystem[] claps = new ParticleSystem[2];
	
	public AudioSource removeDiceSnd, coinImpactSnd, UndoSnd, noMoveSnd, btnSnd,evendice_Snd;
	public Button Undo;
	[Header("Main Menu")]
	[Space]
	public Text notification;
	string[] note_txt = new string[5];
	public GameObject settings_panel,main_mene_btns,snd_btn;
	public Toggle[] OnToggles = new Toggle[8];
	public Image[] skins = new Image[2];
	public Image[] checkers = new Image[3];
	public int play_to_number;
	public Text play_to_txt;
	private int skin_id,Checker_id;
	public GameObject[] Boards = new GameObject[2];
	public Texture[] P1_Checkers = new Texture[3];
	public Texture[] P2_Checkers = new Texture[3];
	public Texture[] P1_Checkers_bo = new Texture[3];
	public Texture[] P2_Checkers_bo = new Texture[3];
	public Material Player1_checker,Player2_checker,Player1_checker_bo,Player2_checker_bo;
	private string[] lvlname = {"MainMenu","PlayTable"};
	private int double_cube_layer;
	public float AlternativeOfTime,AlternativeOfTime_burn;
	public bool menuIsClose;

	void Start ()
	{
		DataManagerClass = this;
		if (PlayerPrefs.GetInt("DefaultData") == 0)
		{
			PlayerPrefs.SetInt("sound",1);
			PlayerPrefs.SetInt("legalmoves",1);
			PlayerPrefs.SetInt("legaltowers",1);
			PlayerPrefs.SetInt("automove",1);
			PlayerPrefs.SetInt("dblcube",1);
			PlayerPrefs.SetInt("undobtn",1);
			PlayerPrefs.SetInt("fastanim",0);
			PlayerPrefs.SetInt("crawford",1);
			PlayerPrefs.SetInt("playto",7);
			PlayerPrefs.SetInt("skins",1);
			PlayerPrefs.SetInt("playerchecker",1);
			PlayerPrefs.SetInt("MoveMode", 1);

			removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = false;

		}
		if (SceneManager.GetActiveScene().buildIndex == 0) // MainMenu
		{
			note_txt[0] = "Backgammon involves a combination of strategy and luck (from rolling dice).";
			note_txt[1] = "Backgammon using real world physics equations.";
			note_txt[2] = "To Start the game, each player rolls a single dice but in this case game will do it.";
			note_txt[3] = "for Deactivating Auto move options you can change it in the settings.";
			note_txt[4] = "Welcome to the Backgammon this game using Strong AI that never cheats";

			notification.text = note_txt[Random.Range(0, 5)];
			OnToggles[0].isOn = PlayerPrefs.GetInt("sound") == 1; // sound On
			OnToggles[1].isOn = PlayerPrefs.GetInt("legalmoves") == 1; // Show Legal Checkers for making a move
			OnToggles[2].isOn = PlayerPrefs.GetInt("legaltowers") == 1; // Show Legal Tower for choose
			OnToggles[3].isOn = PlayerPrefs.GetInt("automove") == 1; // last remain move will done automatically
			OnToggles[4].isOn = PlayerPrefs.GetInt("dblcube") == 1; // double cube is available 
			OnToggles[5].isOn = PlayerPrefs.GetInt("undobtn") == 1; // undo button is available
			OnToggles[6].isOn = PlayerPrefs.GetInt("fastanim") == 1; // checkers removes and moves fast
			OnToggles[7].isOn = PlayerPrefs.GetInt("MoveMode") == 1; // checkers moves in wave motion
			
			Checker_id = PlayerPrefs.GetInt("playerchecker"); // selected checker by player
			skin_id = PlayerPrefs.GetInt("skins");  // selected board by player
			play_to_number = PlayerPrefs.GetInt("playto"); // score of match
			play_to_txt.text = play_to_number.ToString();

			switch (skin_id)
			{
				case 1:
					skins[0].enabled = true;
					skins[1].enabled = false;
					skins[2].enabled = false;
					break;
				
				case 2:
					skins[0].enabled = false;
					skins[1].enabled = true;
					skins[2].enabled = false;
					break;
				
				case 3:
					skins[0].enabled = false;
					skins[1].enabled = false;
					skins[2].enabled = true;
					break;
			}
			
			switch (Checker_id)
			{
				case 1:
					checkers[0].enabled = true;
					checkers[1].enabled = false;
					checkers[2].enabled = false;
					break;
				
				case 2:
					checkers[0].enabled = false;
					checkers[1].enabled = true;
					checkers[2].enabled = false;
					break;
				
				case 3:
					checkers[0].enabled = false;
					checkers[1].enabled = false;
					checkers[2].enabled = true;
					break;
			}
			
		}

		if (SceneManager.GetActiveScene().buildIndex == 1) // Play Scene 
		{
			if (PlayerPrefs.GetInt("undobtn") == 0)
				Undo.gameObject.SetActive(false);

			if (PlayerPrefs.GetInt("dblcube") == 0)
				doubleCubeObj.SetActive(false);
			
			if (PlayerPrefs.GetInt("sound") == 1)
			{
				removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = false;
				GameObject.Find("Dice_1").GetComponent<AudioSource>().mute = false;
				GameObject.Find("Dice_2").GetComponent<AudioSource>().mute = false;
				
				snd_btn.transform.GetChild(1).GetComponent<Image>().enabled = true;
				snd_btn.transform.GetChild(2).GetComponent<Image>().enabled = false;

			}
			else
			{
				removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = true;
				GameObject.Find("Dice_1").GetComponent<AudioSource>().mute = true;
				GameObject.Find("Dice_2").GetComponent<AudioSource>().mute = true;
				
				snd_btn.transform.GetChild(1).GetComponent<Image>().enabled = true;
				snd_btn.transform.GetChild(2).GetComponent<Image>().enabled = false;
				
			}
		
			switch (PlayerPrefs.GetInt("skins"))
			{
				case 1:
					Boards[0].SetActive(true);
					Boards[1].SetActive(false);
					Boards[2].SetActive(false);
					break;
				
				case 2:
					Boards[0].SetActive(false);
					Boards[1].SetActive(true);
					Boards[2].SetActive(false);
					break;
				
				case 3:
					Boards[0].SetActive(false);
					Boards[1].SetActive(false);
					Boards[2].SetActive(true);
					break;
			}

			switch (PlayerPrefs.GetInt("playerchecker"))
			{
				case 1: 
					Player1_checker.SetTexture("_MainTex", P1_Checkers[0]);
					Player2_checker.SetTexture("_MainTex", P2_Checkers[0]);
					Player1_checker_bo.SetTexture("_MainTex", P1_Checkers_bo[0]);
					Player2_checker_bo.SetTexture("_MainTex", P2_Checkers_bo[0]);
					break;
				
				case 2: 
					Player1_checker.SetTexture("_MainTex", P1_Checkers[1]);
					Player2_checker.SetTexture("_MainTex", P2_Checkers[1]);
					Player1_checker_bo.SetTexture("_MainTex", P1_Checkers_bo[1]);
					Player2_checker_bo.SetTexture("_MainTex", P2_Checkers_bo[1]);
					break;
				
				case 3: 
					Player1_checker.SetTexture("_MainTex", P1_Checkers[2]);
					Player2_checker.SetTexture("_MainTex", P2_Checkers[2]);
					Player1_checker_bo.SetTexture("_MainTex", P1_Checkers_bo[2]);
					Player2_checker_bo.SetTexture("_MainTex", P2_Checkers_bo[2]);
					break;
		
			}

			play_to_number = PlayerPrefs.GetInt("playto");
			
			if (PlayerPrefs.GetInt("fastanim") == 1) // speed of checkers movement
				AlternativeOfTime = AlternativeOfTime_burn = 15;
			else
				AlternativeOfTime = AlternativeOfTime_burn = 10;

		}

		menuIsClose = true;
	}

	public void increase_btn()
	{
		if (play_to_number < 15)
		{
			play_to_number = PlayerPrefs.GetInt("playto") + 4;
			PlayerPrefs.SetInt("playto",play_to_number);
			play_to_txt.text = play_to_number.ToString();
		}
		
		btnSnd.Play();
	}
	
	public void decrease_btn()
	{
		if (play_to_number > 7)
		{
			play_to_number = PlayerPrefs.GetInt("playto") - 4;
			PlayerPrefs.SetInt("playto",play_to_number);
			play_to_txt.text = play_to_number.ToString();
		}
		
		btnSnd.Play();
	}
	

	public void select_skin()
	{
		if (skin_id < 3)
			skin_id++;
		else
			skin_id = 1;

		PlayerPrefs.SetInt("skins", skin_id);
		switch (skin_id)
		{
			case 1:
				
				skins[0].enabled = true;
				skins[1].enabled = false;
				skins[2].enabled = false;

				checkers[0].enabled = true;
				checkers[1].enabled = false;
				checkers[2].enabled = false;
	
				PlayerPrefs.SetInt("playerchecker", 1);
				
				break;
			case 2:
				
				skins[0].enabled = false;
				skins[1].enabled = true;
				skins[2].enabled = false;

				checkers[0].enabled = false;
				checkers[1].enabled = true;
				checkers[2].enabled = false;
			
				PlayerPrefs.SetInt("playerchecker", 2);
				
				break;
			
			case 3:
				
				skins[0].enabled = false;
				skins[1].enabled = false;
				skins[2].enabled = true;

				checkers[0].enabled = false;
				checkers[1].enabled = false;
				checkers[2].enabled = true;

				PlayerPrefs.SetInt("playerchecker", 3);
				
				break;
		}

		btnSnd.Play();
	}
	
	public void select_Checker()
	{
		if (Checker_id < 3)
			Checker_id++;
		else
			Checker_id = 1;

		PlayerPrefs.SetInt("playerchecker",Checker_id);
	
		switch (Checker_id)
		{
			case 1:
				checkers[0].enabled = true;
				checkers[1].enabled = false;
				checkers[2].enabled = false;
				break;
			case 2:
				checkers[0].enabled = false;
				checkers[1].enabled = true;
				checkers[2].enabled = false;
				break;
			
			case 3:
				checkers[0].enabled = false;
				checkers[1].enabled = false;
				checkers[2].enabled = true;
				break;

		}
	
		btnSnd.Play();
	}

	public void single_Player()
	{
		PlayerPrefs.SetInt ("DefaultData", 1);
		PlayerPrefs.SetInt ("mode", 1);
	
		StartCoroutine(loadLevel());
		
		play_to_number = PlayerPrefs.GetInt("playto");

		if (OverAllScoreForBlack >= play_to_number | OverAllScoreForWhite >= play_to_number)
		{
			PlayerPrefs.SetInt ("w_overall_dbl", 0);
			PlayerPrefs.SetInt ("b_overall_dbl", 0);
			PlayerPrefs.SetInt ("w_overall_score", 0);
			PlayerPrefs.SetInt ("b_overall_score", 0);
			PlayerPrefs.SetInt ("w_overall_dice_roll", 0);
			PlayerPrefs.SetInt ("b_overall_dice_roll", 0);
		}
		
		btnSnd.Play();
	}
	public	void two_player ()
	{
		PlayerPrefs.SetInt ("DefaultData", 1);
		PlayerPrefs.SetInt ("mode", 2);

		StartCoroutine(loadLevel());
		
		play_to_number = PlayerPrefs.GetInt("playto");
		
		if (OverAllScoreForBlack >= play_to_number | OverAllScoreForWhite >= play_to_number)
		{
			PlayerPrefs.SetInt ("w_overall_dbl", 0);
			PlayerPrefs.SetInt ("b_overall_dbl", 0);
			PlayerPrefs.SetInt ("w_overall_score", 0);
			PlayerPrefs.SetInt ("b_overall_score", 0);
			PlayerPrefs.SetInt ("w_overall_dice_roll", 0);
			PlayerPrefs.SetInt ("b_overall_dice_roll", 0);
		}
		
		btnSnd.Play();
	}

	public void connentToMaster()
    {
		PhotonConnection.instance.TwoPlayersClicked();
    }

	public void two_playerOnline()
    {
		PlayerPrefs.SetInt("DefaultData", 1);
		PlayerPrefs.SetInt("mode", 3);

		StartCoroutine(loadLevel());

		play_to_number = PlayerPrefs.GetInt("playto");

		if (OverAllScoreForBlack >= play_to_number | OverAllScoreForWhite >= play_to_number)
		{
			PlayerPrefs.SetInt("w_overall_dbl", 0);
			PlayerPrefs.SetInt("b_overall_dbl", 0);
			PlayerPrefs.SetInt("w_overall_score", 0);
			PlayerPrefs.SetInt("b_overall_score", 0);
			PlayerPrefs.SetInt("w_overall_dice_roll", 0);
			PlayerPrefs.SetInt("b_overall_dice_roll", 0);
		}

		btnSnd.Play();
	}

	IEnumerator loadLevel()
	{
		AsyncOperation loadscene = SceneManager.LoadSceneAsync("PlayTable");

		while (!loadscene.isDone)
		{
			yield return null;
		}
	}

	public void Settings ()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0) // opens settings panel of MainMenu scene
		{
			PlayerPrefs.SetInt("DefaultData",1);
			settings_panel.SetActive(true);
			main_mene_btns.SetActive(false);
		}
		else
		{
			settings_menu_anim.Play("settingsmenu"); // opens Menu of PlayTable scene

			double_cube_layer = Player.PlayerClass.doubleCube.layer;
			Player.PlayerClass.doubleCube.layer = 2;

			menuIsClose = false;

		}
		
		btnSnd.Play();
	}
	public void close_settings()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0) // settings panel of MainMenu scene
		{
			settings_panel.SetActive(false);
			main_mene_btns.SetActive(true);
			btnSnd.Play();
		}
		else
		{
			if (double_cube_layer == 9)
				Player.PlayerClass.doubleCube.layer = 9;
			
			settings_menu_anim.Play("settingsmenu_close"); // close Menu of PlayTable scene

			menuIsClose = true;
		}

	}

	public void Sound_check()
	{
		if (OnToggles[0].isOn)
		{
			PlayerPrefs.SetInt("sound",1);
			removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = false;
		}
		else
		{
			PlayerPrefs.SetInt("sound",0);
			removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = true;
		}
	}
	
	public void MoveMode()
	{
		if (OnToggles[7].isOn)
			PlayerPrefs.SetInt("MoveMode",1);
		else
			PlayerPrefs.SetInt("MoveMode",0);
	}
	
	public void FlashLegalMoves_ckeck ()
	{
		PlayerPrefs.SetInt("legalmoves", OnToggles[1].isOn ? 1 : 0);
	}

	public void ShowLegalTowers_check()
	{
		PlayerPrefs.SetInt("legaltowers", OnToggles[2].isOn ? 1 : 0);
	}
	
	public void AutoMoveCoin_check()
	{
		PlayerPrefs.SetInt("automove", OnToggles[3].isOn ? 1 : 0);
	}
	
	public void DoubleCube_check()
	{
		PlayerPrefs.SetInt("dblcube", OnToggles[4].isOn ? 1 : 0);
	}

	public void UndoButton_check()
	{
		PlayerPrefs.SetInt("undobtn", OnToggles[5].isOn ? 1 : 0);
	}
	public void FastAnimation_check()
	{
		PlayerPrefs.SetInt("fastanim", OnToggles[6].isOn ? 1 : 0);
	}

	public void CrawFordLaw_check()
	{
		PlayerPrefs.SetInt("crawford", OnToggles[7].isOn ? 1 : 0);
	}

	public void Sound()
	{
		if (PlayerPrefs.GetInt("sound") == 0)
		{
			PlayerPrefs.SetInt("sound",1);
			removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = false;
			snd_btn.transform.GetChild(1).GetComponent<Image>().enabled = true;
			snd_btn.transform.GetChild(2).GetComponent<Image>().enabled = false;
		}
		else
		{
			PlayerPrefs.SetInt("sound",0);
			removeDiceSnd.mute = UndoSnd.mute = btnSnd.mute = coinImpactSnd.mute = noMoveSnd.mute = true;
			snd_btn.transform.GetChild(1).GetComponent<Image>().enabled = false;
			snd_btn.transform.GetChild(2).GetComponent<Image>().enabled = true;
		}
		
	}


	public void Resign_Round()
	{
		Player.PlayerClass.Calculate_Score();
	}

	public void NewGame()
	{
		// white or black score reached to play_to_number value it means game is over
		if (PlayerPrefs.GetInt("w_overall_score") >= play_to_number | PlayerPrefs.GetInt("b_overall_score") >= play_to_number) 
		{
			PlayerPrefs.SetInt("w_overall_dbl", 0);
			PlayerPrefs.SetInt("b_overall_dbl", 0);
			PlayerPrefs.SetInt("w_overall_score", 0);
			PlayerPrefs.SetInt("b_overall_score", 0);
			PlayerPrefs.SetInt("w_overall_dice_roll", 0);
			PlayerPrefs.SetInt("b_overall_dice_roll", 0);
			
			StartCoroutine(loadMainMenu(lvlname[1]));
		}
		else // game doesn't reached to play_to_number value and game is not over yet
			StartCoroutine(loadMainMenu(lvlname[1]));
	}
	public void MainMenu()
	{
		StartCoroutine(loadMainMenu(lvlname[0]));
	}

	public void ScoreTable_MainMenu ()
	{
		if (PlayerPrefs.GetInt("w_overall_score") >= play_to_number | PlayerPrefs.GetInt("b_overall_score") >= play_to_number)
		{
			PlayerPrefs.SetInt("w_overall_dbl", 0);
			PlayerPrefs.SetInt("b_overall_dbl", 0);
			PlayerPrefs.SetInt("w_overall_score", 0);
			PlayerPrefs.SetInt("b_overall_score", 0);
			PlayerPrefs.SetInt("w_overall_dice_roll", 0);
			PlayerPrefs.SetInt("b_overall_dice_roll", 0);
			
			StartCoroutine(loadMainMenu(lvlname[0]));
		}
		else
			StartCoroutine(loadMainMenu(lvlname[0]));
	}

	IEnumerator loadMainMenu(string lvl)
	{
		Time.timeScale = 1f;
		Physics.gravity = new Vector3(0, -9.81f, 0);
		yield return new WaitForSecondsRealtime(1f);
		AsyncOperation laodmainlvl = SceneManager.LoadSceneAsync(lvl);

		while (!laodmainlvl.isDone)
		{
			yield return null;
		}
	}


}
