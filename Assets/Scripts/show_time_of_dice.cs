using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataManager;
using static Player;

public class show_time_of_dice : MonoBehaviour

{
    private Vector3 initial_pos_Dice1; // initial position of dice 1
    private Vector3 initial_pos_Dice2; // // initial position of dice 2

    public Quaternion[] Dice1_numbers_angles = new Quaternion[6]; // all sides of dice 1
    private Quaternion[] Dice2_numbers_angles = new Quaternion[6]; // all sides of dice 2
    private Quaternion Dice1_angle, Dice2_angle; // based on dice number rotates the dice
    public Transform Dice_1, Dice_2; // dice1 dice2 transforms
    public Transform target_d1, target_d2; // destination of dice
    public Animator start_game_btn, msg_popdown; // shows up the buttons
    public bool stopDicesFromRolling; // makes dice stop from rolling
    public bool PlayerDetectedAtFrstRoll; // shows the dice to player

    public GameObject Msg_Of_P1, Msg_oF_P2, Msg_Of_Even_Dice_situation; // message to show
    public Text p1, p2;
    public GameObject menu_btn, help_btn;
    public static show_time_of_dice ShowTimeOfDice;
    public CanvasGroup D1, D2, D3, D4; // dice icons
    private void Start()
    {
        PlayerDetectedAtFrstRoll = false;
        ShowTimeOfDice = this;

        // Quaternion of Dice1
        Dice1_numbers_angles[0] = Quaternion.Euler(90f, 0f, 0f);
        Dice1_numbers_angles[1] = Quaternion.Euler(0, 0f, 90f);
        Dice1_numbers_angles[2] = Quaternion.Euler(180, 0f, 0f);
        Dice1_numbers_angles[3] = Quaternion.Euler(0, 0f, 0f);
        Dice1_numbers_angles[4] = Quaternion.Euler(0, 0f, -90f);
        Dice1_numbers_angles[5] = Quaternion.Euler(-90, 0f, 0f);

        // Quaternion of Dice2
        Dice2_numbers_angles[0] = Quaternion.Euler(90f, 0f, 0f);
        Dice2_numbers_angles[1] = Quaternion.Euler(0, 0f, 90f);
        Dice2_numbers_angles[2] = Quaternion.Euler(180, 0f, 0f);
        Dice2_numbers_angles[3] = Quaternion.Euler(0, 0f, 0f);
        Dice2_numbers_angles[4] = Quaternion.Euler(0, 0f, -90f);
        Dice2_numbers_angles[5] = Quaternion.Euler(-90, 0f, 0f);

        //show time dices initial positions
        initial_pos_Dice1 = Dice_1.position;
        initial_pos_Dice2 = Dice_2.position;

        stopDicesFromRolling = false;

        if (PlayerPrefs.GetInt("skins") == 1 | PlayerPrefs.GetInt("skins") == 3) // changes the color of dice base on skins that player selected
        {
            Dice_1.GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 0.7f);
            Dice_1.GetComponent<Renderer>().materials[1].color = Color.black;

            Dice_2.GetComponent<Renderer>().materials[0].color = new Color(1f, 1f, 0.7f);
            Dice_2.GetComponent<Renderer>().materials[1].color = Color.black;

            Dice_1.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", new Color(0.46f, 0.46f, 0.34f));
            Dice_1.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", Color.black);

            Dice_2.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", new Color(0.46f, 0.46f, 0.34f));
            Dice_2.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", Color.black);
        }

    }

    void Update()
    {
        if (DiceRoll.DiceRollClass.active_show_time && !PlayerDetectedAtFrstRoll)
        {
            BringTheDice(); // show the dice icons and send msg to IEnumerator SwitchPlayer () functions that decide who should starts the game
        }
    }

    void BringTheDice()
    {
        if (!stopDicesFromRolling)
        {
            switch (Dice_Tracker.DiceTrackerClass.diceNumber1)
            {
                case 1:
                    Dice1_angle = Dice1_numbers_angles[0];
                    break;
                case 2:
                    Dice1_angle = Dice1_numbers_angles[1];
                    break;
                case 3:
                    Dice1_angle = Dice1_numbers_angles[2];
                    break;
                case 4:
                    Dice1_angle = Dice1_numbers_angles[3];
                    break;
                case 5:
                    Dice1_angle = Dice1_numbers_angles[4];
                    break;
                case 6:
                    Dice1_angle = Dice1_numbers_angles[5];
                    break;

            }

            switch (Dice_Tracker1.DiceTracker1Class.diceNumber2)
            {
                case 1:
                    Dice2_angle = Dice2_numbers_angles[0];
                    break;
                case 2:
                    Dice2_angle = Dice2_numbers_angles[1];
                    break;
                case 3:
                    Dice2_angle = Dice2_numbers_angles[2];
                    break;
                case 4:
                    Dice2_angle = Dice2_numbers_angles[3];
                    break;
                case 5:
                    Dice2_angle = Dice2_numbers_angles[4];
                    break;
                case 6:
                    Dice2_angle = Dice2_numbers_angles[5];
                    break;

            }

            if (Dice_Tracker.DiceTrackerClass.diceNumber1 == Dice_Tracker1.DiceTracker1Class.diceNumber2)
            {

                Msg_Of_Even_Dice_situation.SetActive(true);
                Msg_Of_P1.gameObject.SetActive(false);
                Msg_oF_P2.gameObject.SetActive(false);

                StartCoroutine(explain_even_dice_Situation());
            }

            else
            //if (PlayerClass.myPlayerNumber == 1)
                if (Dice_Tracker.DiceTrackerClass.diceNumber1 > Dice_Tracker1.DiceTracker1Class.diceNumber2)
                {
                    PlayerClass.player_number = 1;

                    Msg_Of_P1.SetActive(true);
                    msg_popdown.Play("PopUp_note");

                }

                else
                {
                    PlayerClass.player_number = 2;

                    Msg_oF_P2.SetActive(true);
                    msg_popdown.Play("PopUp_note");
                }
            //else
            //{
            //    if (Dice_Tracker.DiceTrackerClass.diceNumber1 > Dice_Tracker1.DiceTracker1Class.diceNumber2)
            //    {
            //        PlayerClass.player_number = 2;

            //        Msg_oF_P2.SetActive(true);
            //        msg_popdown.Play("PopUp_note");

            //    }

            //    else
            //    {
            //        PlayerClass.player_number = 1;

            //        Msg_Of_P1.SetActive(true);
            //        msg_popdown.Play("PopUp_note");
            //    }
            //}
                
            D1.alpha = D2.alpha = D3.alpha = D4.alpha = 1f;

            Vector3 magnetute = target_d1.position - Dice_1.position;

            Dice_1.position = Vector3.Lerp(Dice_1.position, target_d1.position, 4f * Time.deltaTime); // moves dice to destination
            Dice_2.position = Vector3.Lerp(Dice_2.position, target_d2.position, 4f * Time.deltaTime);

            if (magnetute.sqrMagnitude > 0.25f) // if dice gets close to destination rotate not longer happens to dice
            {
                Dice_1.Rotate(-Dice_1.right * Time.timeSinceLevelLoad * 2f); // speed of rotate
                Dice_2.Rotate(Dice_2.right * Time.timeSinceLevelLoad * 2f);

            }
            else
            {
                Dice_1.rotation = Quaternion.identity;
                Dice_2.rotation = Quaternion.identity;

                Dice_1.rotation = Quaternion.Lerp(Dice_1.rotation, Dice1_angle, Time.timeSinceLevelLoad * 2f);
                Dice_2.rotation = Quaternion.Lerp(Dice_2.rotation, Dice2_angle, Time.timeSinceLevelLoad * 2f);

                if (Dice_Tracker.DiceTrackerClass.diceNumber1 != Dice_Tracker1.DiceTracker1Class.diceNumber2) // shows up start_game_btn
                    start_game_btn.Play("start_game_btn");

                p1.enabled = true;
                p2.enabled = true;


            }
        }

        if (stopDicesFromRolling)
        {
            Time.timeScale = 1f;
            Dice_1.position = Vector3.Lerp(Dice_1.position, initial_pos_Dice1, Time.deltaTime * 1f);
            Dice_2.position = Vector3.Lerp(Dice_2.position, initial_pos_Dice2, Time.deltaTime * 1f);

            Dice_1.Rotate(2f * Time.timeSinceLevelLoad * Dice_1.right);
            Dice_2.Rotate(2f * Time.timeSinceLevelLoad * -Dice_2.right);

        }

        Time.timeScale = 1f; // when dice rolls the timescale will gets higher and after finishing process of specifying dice number timesclae will be 1 again 

    }

    public void show_off_dices()
    {
        stopDicesFromRolling = true;
        start_game_btn.Play("start_game_btn_rev");
        msg_popdown.Play("PopUp_note_close");

        StartCoroutine(deactivate_all_stuffs());

        p1.enabled = p2.enabled = false;
        DataManagerClass.removeDiceSnd.Play();
        StartCoroutine(PlayerClass.SwitchPlayer()); // starts the game by calling SwitchPlayer()'s function

    }

    IEnumerator deactivate_all_stuffs() // deactivates unnecessary things
    {
        yield return new WaitForSecondsRealtime(1f);
        Dice_1.gameObject.SetActive(false);
        Dice_2.gameObject.SetActive(false);
        DiceRoll.DiceRollClass.active_show_time = false;
        menu_btn.SetActive(true);
        help_btn.SetActive(true);
        PlayerDetectedAtFrstRoll = true;
        enabled = false;
    }

    IEnumerator explain_even_dice_Situation()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        msg_popdown.Play("PopUp_note");
        yield return new WaitForSecondsRealtime(0.5f);
        stopDicesFromRolling = true;
        p1.enabled = p2.enabled = false;
        SceneManager.LoadScene("PlayTable");

    }


}
