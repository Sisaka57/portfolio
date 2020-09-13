using System.Collections;
using System.Linq;
using UnityEngine;
using static Board;
using static DataManager;
using static DiceRoll;
using static Player;

public class Robot : MonoBehaviour
{
    //2020 Arash khq(RVBinary)
    
    public bool think; // bot will decide to make a move
    public  int counter_thinking; // counts number of thinking
    public int countdp; // a variable that is a part of the even dice situation and stores a number that will return from a cycle
    public int break_the_rules; // if bot is not able to remove a checker from a regular tower, a tower that relative to dice number break_the_rules will return 1
    public int Switch_to_new_method; // changes the method for move when dice are even
    public int burn_regular; // normally bot could burn against checker
    public int fw0,fw1,big_fw; // makes main cycles of opposite dice to count possible moves for bot
    public int D0_Base_On_Position, D1_Base_On_Position; // parts of burn process
    public int D0_SameTwr, D1_SameTwr; // if bot is able to place two checker in a same tower 
    public int D0_Cnt_Twr,D1_Cnt_Twr; // consist of chosen legal tower and deliver tower in opposite dice situation
    public int D0_Switch_flag,D1_Switch_flag; //if returns 1, marks further methods unsuitable 
    public int D0_Twr_Index; // specifies the number possible moves of dice 0 
    public int D1_Twr_Index; // specifies the number possible moves of dice 1
    public int dice_pos_dbl, burn_flag_for_cover, double_dice_falses_index, way_is_safe; //dice_pos_dbl : bot is able to cover their checkers ? | burn_flag_for_cover: can burn against checkers | way_is_safe : could to make a move
    public int flag = 2,F_flag = 2;
    
    // check their usage in their functions
    public int sgl_d0_bo;
    public int sgl_d1_bo;
    public int deactive_all;
    public bool D0_Return;
    public bool D1_Return;
    public bool active_sgl_move;
    public bool SameTower;
    public bool active_D1_move;
    public bool active_D2_move;
    public bool Even_Burn_Activated;
    public bool rbt_passed_dble_req;
    
    public int[] EvenDeliverTowers = new int[15];
    public int[] EvenDiceLegalTowers = new int[15];
    public int[] EvenLegalTowers_Index = new int[15]; 
    public int[] Double_Dp = new int[15]; 
    public int[] Double_Dbs = new int[15]; 
    public int[] D0_OppositeLegalTower = new int[15];
    public int[] D1_OppositeLegalTower = new int[15];
    public int[] D0_OppositeDeliverTower = new int[15];
    public int[] D1_OppositeDeliverTower = new int[15];
    public int[] alone_d0 = new int[15];
    public int[] alone_d1 = new int[15];
    public int[] Opposite_Burn_deliverTower = new int[3];
    public int Even_Burn_deliverTower;
    public GameObject[] D0_RemovableCoins = new GameObject[15];
    public GameObject[] D1_RemovableCoins = new GameObject[15];
    GameObject Removabl_Coin; 
    public GameObject D0_Coin_volunteer_BO;
    public GameObject D1_Coin_volunteer_BO;
    public static Robot RobotClass;
    
    // if you want to modify the behavior of robot feel free to email us. we will answer and support you for sure
    // for determine difficulty of game you could disable some functions of robot
    void Start()
    {
        RobotClass = this;
    }


    #region Offer Double to Against
    public IEnumerator Offer_dbl_black () // black rbt(robot) makes destination that weather offer to double the score of match or not
    {
        int BCoin_Twr23_12, WCoin_Twr12_5, WBCoin_Twr12_15, Cnt_BCoin_Twr_5_0, Cnt_WCoin_Twr_5_0;

        BCoin_Twr23_12 = WCoin_Twr12_5 = WBCoin_Twr12_15 = Cnt_BCoin_Twr_5_0 = Cnt_WCoin_Twr_5_0 = 0;

        for (int i = 23; i >= 12; i--)
        for (int j = 0; j <= 14; j++)
            if (BoardClass.boardArray [i, j].CompareTag("Black_coin")) //finds out how many coin has in 23 till 12 towers
                BCoin_Twr23_12++;
		
        for (int i = 5; i <= 12; i++)
        {
            for (int j = 0; j <= 14; j++)
            {
                if (BoardClass.boardArray [i, j].CompareTag("White_coin")) //number white coins
                    WCoin_Twr12_5++;
				
                if (BoardClass.boardArray [i, j].CompareTag("Black_coin"))
                    WBCoin_Twr12_15++;
            }
        }
		
        for (int i = 5; i >= 0; i--)
        {
            for (int j = 0; j <= 14; j++)
            {

                if (BoardClass.boardArray [i, j].CompareTag("Black_coin"))
                    Cnt_BCoin_Twr_5_0++;
				
                if (BoardClass.boardArray [i, j].CompareTag("White_coin"))
                    Cnt_WCoin_Twr_5_0++;
            }
        }

        //if all data are suitable for doubling the score of match, the Ai will offer 
        if (!PlayerClass.Black_ofr_dbl && (BCoin_Twr23_12 <= 1 && PlayerClass.Container_Of_BC_burnt [1] == null) |
                (Cnt_WCoin_Twr_5_0 > 0 && BCoin_Twr23_12 <= 2 && PlayerClass.Container_Of_BC_burnt [0] == null) && Cnt_BCoin_Twr_5_0 >= 9 |
                WBCoin_Twr12_15 >= 12 && WCoin_Twr12_5 > 0)
            {
                yield return new WaitForSecondsRealtime (1.0f);
                PlayerClass.OfferDouble_btn ();
            }
            else
            {
                DiceRollClass.ResetAllData();
            }
		
    }
    #endregion
    
    #region Check Double Position
    public IEnumerator chk_dbl_black () // check to take the offer of double or not
    {
        int BCoin_Twr_23_18,WCoin_Twr_23_18,BCoin_Twr_23_12,WCoin_Twr_12_0,BCoin_Twr_12_0,BCoin_Alone_Twr_5_0,WCoin_Twr_5_0,Cnt_BCoin_Twr_5_0,Cnt_WCoin_Twr_5_0;
        BCoin_Twr_23_18 = WCoin_Twr_23_18 = BCoin_Twr_23_12 = WCoin_Twr_12_0 = BCoin_Twr_12_0 = BCoin_Alone_Twr_5_0 = WCoin_Twr_5_0 = Cnt_BCoin_Twr_5_0 = Cnt_WCoin_Twr_5_0 = 0;


        for (int i = 23; i >= 5; i--)
        for (int j = 0; j <= 14; j++)
            if (BoardClass.boardArray [i, j].CompareTag("White_coin"))
                Cnt_WCoin_Twr_5_0++;


        for(int i = 23; i >= 18; i--)
        {
            for(int j = 0; j <= 14; j++)
            {
                if(BoardClass.boardArray[i,j].CompareTag("Black_coin"))
                    BCoin_Twr_23_18++;

                if(BoardClass.boardArray[i,j].CompareTag("White_coin"))
                    WCoin_Twr_23_18++;
            }
        }

        for (int i = 23; i >= 12; i--)
        for(int j = 0; j <= 14; j++)
            if (BoardClass.boardArray [i, j].CompareTag("Black_coin"))
                BCoin_Twr_23_12++;
		

        for (int i = 12; i >= 0; i--)
        {
            for (int j = 0; j <= 14; j++)
            {
                if (BoardClass.boardArray [i, j].CompareTag("White_coin"))
                    WCoin_Twr_12_0++;
				
                if (BoardClass.boardArray [i, j].CompareTag("Black_coin"))
                    BCoin_Twr_12_0++;
            }
        }


        for(int i1 = 5; i1 >= 0; i1--)
        {
            for (int j = 0; j <= 14; j++)
                if(BoardClass.boardArray[i1,j].CompareTag("Black_coin"))
                    Cnt_BCoin_Twr_5_0++;

            if(BoardClass.boardArray[i1,0].CompareTag("Black_coin"))
                if(BoardClass.IndexOfCoinInSelectedTower(i1) == 0)
                    BCoin_Alone_Twr_5_0++;
			
            if(BoardClass.boardArray[i1,0].CompareTag("White_coin"))
                WCoin_Twr_5_0++;
        }

		

        yield return new WaitForSecondsRealtime (1f);
        if (PlayerClass.black_coin_move_counter >= 130 && PlayerClass.white_coin_move_counter >= 130 |
            PlayerClass.Black_Beardout_coins[0] != null | Cnt_BCoin_Twr_5_0 > 12)
        {
            rbt_passed_dble_req = true;
            PlayerClass.TakeDoubleOffer();
        }

        else

        {
            if (BCoin_Twr_23_18 <= 1 && WCoin_Twr_23_18 < 13 && Cnt_WCoin_Twr_5_0 > 0 &&
                Cnt_BCoin_Twr_5_0 >= 11 | BCoin_Twr_12_0 > 10 && PlayerClass.Container_Of_BC_burnt[1] == null)
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
           
            }

            else if (BCoin_Twr_23_12 <= 3 && WCoin_Twr_23_18 < 13 && Cnt_WCoin_Twr_5_0 > 0 && Cnt_BCoin_Twr_5_0 > 8 &&
                     PlayerClass.Container_Of_BC_burnt[0] == null)
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
             
            }

            else if (Cnt_BCoin_Twr_5_0 >= 5 && WCoin_Twr_23_18 < 13 && Cnt_WCoin_Twr_5_0 > 0 && BCoin_Twr_12_0 >= 8 &&
                     PlayerClass.Container_Of_BC_burnt[1] == null)
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
            }

            else if (BCoin_Twr_23_18 <= 2 && WCoin_Twr_23_18 < 13 && Cnt_WCoin_Twr_5_0 > 0 &&
                     WCoin_Twr_5_0 <= 1 | WCoin_Twr_5_0 > 1 && BCoin_Twr_12_0 > 8)
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
                
            }

            else if (BCoin_Twr_12_0 >= 10 && WCoin_Twr_23_18 < 13 && Cnt_WCoin_Twr_5_0 > 0 && WCoin_Twr_12_0 >= 2)
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
            }

            else if (BCoin_Twr_23_18 == 0 | BCoin_Twr_12_0 <= 2 && WCoin_Twr_23_18 < 13 && Cnt_WCoin_Twr_5_0 > 0 && WCoin_Twr_12_0 >= 2)
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
             
            }

            else if ((PlayerClass.B_win > PlayerClass.W_win) | (PlayerClass.No_Bear_out_BC_Twr_5_0 > 0) | (Cnt_BCoin_Twr_5_0 >= 11 && PlayerClass.No_Bear_out_WC_Twr_18_23 <= 5 &&
                     DataManagerClass.OverAllScoreForBlack >= DataManagerClass.OverAllScoreForWhite))
            {
                rbt_passed_dble_req = true;
                PlayerClass.TakeDoubleOffer();
              
            }
            
        }
		
        if(!rbt_passed_dble_req)
            PlayerClass.PassDoubleOffer ();
        
    }
    #endregion
    
    #region Calculate Coins Position
    public void CalculateCoinsPosition ()
    {
        think = false;
        counter_thinking++;
        
        print (" Thinking for " + counter_thinking + " times ... D0: " + PlayerClass.Dice_number[0] + " D1: " + PlayerClass.Dice_number[1]);
        
        D0_Twr_Index = D1_Twr_Index  = way_is_safe = sgl_d0_bo = sgl_d1_bo = break_the_rules = 0;
        D0_Switch_flag = D1_Switch_flag = 0;
        flag = F_flag = 2;
        
        D0_Coin_volunteer_BO = D1_Coin_volunteer_BO = null;

        if (BoardClass.NumberOfBurntCoins(PlayerClass.Who) == 0)
            PlayerClass.playerHasBurntCoin = false;
        
        for (int i = 0; i < 15; i++)
        {
            D0_OppositeLegalTower [i] = D1_OppositeLegalTower [i] = 0;
            D0_OppositeDeliverTower [i] = D1_OppositeDeliverTower [i] = 0;
            EvenDeliverTowers [i] = EvenLegalTowers_Index [i] = EvenDiceLegalTowers [i] = 0;
            Double_Dp [i] = Double_Dbs [i] = 0;
            D0_RemovableCoins [i] = D1_RemovableCoins [i] = null;
            alone_d0 [i] = alone_d1 [i] = 0;
        }

        PlayerClass.xcoin_point = GameObject.Find ("XForm").transform;
        PlayerClass.xcoin_bear_out = GameObject.Find ("XForm_BearOut").transform;
        

        PlayerClass.BearOutPossible(PlayerClass.Who);
        
        if ((PlayerClass.player_number == 1 && !PlayerClass.WhiteBearOut) | (PlayerClass.player_number == 2 && !PlayerClass.BlackBearOut) && !PlayerClass.playerHasBurntCoin)
            CollectInformation (); //Collecting all necessary Data from the Board

        if (PlayerClass.playerHasBurntCoin)
        {
            if (PlayerClass.player_number == 2)
            {
                if (PlayerClass.opposite_dice) //dice are not same
                {
                    Opposite_Burn_deliverTower [0] = 24 - PlayerClass.Dice_number [0]; //Opposite_Burn_deliverTower : destination tower of rbt coins
                    Opposite_Burn_deliverTower [1] = 24 - PlayerClass.Dice_number [1];
                    Opposite_Burn_deliverTower [2] = 24 - PlayerClass.Dice_number [2];
                }
				
                if (PlayerClass.even_dice)
                    Even_Burn_deliverTower = 24 - PlayerClass.Dice_number [0]; //Even_Burn_deliverTower : destination tower of rbt coins
            }
            else // have to delete
            {
                if (PlayerClass.opposite_dice)
                {
                    Opposite_Burn_deliverTower [0] = PlayerClass.Dice_number [0]-1;
                    Opposite_Burn_deliverTower [1] = PlayerClass.Dice_number [1]-1;
                    Opposite_Burn_deliverTower [2] = PlayerClass.Dice_number [2]-1;
                }
                
                if (PlayerClass.even_dice)
                    Even_Burn_deliverTower = PlayerClass.Dice_number [0]-1;
            }

            StartCoroutine (BurnProcess());
        }

        if(PlayerClass.player_number == 2 && PlayerClass.BlackBearOut && !PlayerClass.playerHasBurntCoin)
        {
            active_D1_move = active_D2_move = active_sgl_move  = false;
            int WCoin_Twr_5_0 = 0;

            for (int i = 0; i <= 5; i++)
                if (BoardClass.boardArray[i,0].CompareTag("White_coin"))
                    WCoin_Twr_5_0++; // number of white coins *** home of white_player starts from zero to fifth tower of the board
                 

            if (PlayerClass.opposite_dice)
            {
                if (!PlayerClass.DiceCheck[0])
                {
                    for (int indx = 5; indx >= 0; indx--)
                    {
                        //finds out the Corresponding tower to dice_number[0] value
                        if (indx + 1 == PlayerClass.Dice_number[0] && BoardClass.IndexOfCoinInSelectedTower(indx) >= 0 && BoardClass.boardArray[indx, 0].CompareTag("Black_coin"))
                        {
                            sgl_d0_bo = indx; // index of that tower whom rbt will select

                            //white_in_5 == 0: if there were not any white coins
                            if (WCoin_Twr_5_0 == 0) // PlayerClass.Container_Of_WC_burnt[0] == null && 
                            {
                                //D0_Coin_volunteer_BO : the coin that selected
                                D0_Coin_volunteer_BO = BoardClass.boardArray[indx, BoardClass.IndexOfCoinInSelectedTower(indx)];
                                way_is_safe = 1;
                                flag = 1;
                                active_D1_move = true;
                                break;
                            }

                            if (PlayerClass.Container_Of_WC_burnt[0] != null | WCoin_Twr_5_0 > 0)
                            {
                                if (BoardClass.IndexOfCoinInSelectedTower(indx) > 1 | BoardClass.IndexOfCoinInSelectedTower(indx) == 0)
                                {
                                    D0_Coin_volunteer_BO = BoardClass.boardArray[indx, BoardClass.IndexOfCoinInSelectedTower(indx)];
                                    way_is_safe = 1;
                                    flag = 1;
                                    active_D1_move = true;
                                    break;
                                }

                                for (int i = 5; i >= PlayerClass.Dice_number[0] - 1; i--) // searches from fifth tower till Player.PlayerClass.Dice_number [0]-1 (Dice_number [0] = 6)
                                {
                                    //Board.BoardClass.Tower_coins (i) > 1: more than one Checker has
                                    if (BoardClass.boardArray[i, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower(i) > 1 | BoardClass.IndexOfCoinInSelectedTower(i) == 0 &&
                                        i - PlayerClass.Dice_number[0] >= 0)
                                    {
                                        way_is_safe = 0;
                                        flag = 2;
                                        break_the_rules = 1;
                                        break;
                                    }

                                    if (flag == 2 && break_the_rules == 0) // use 
                                    {
                                        D0_Coin_volunteer_BO = BoardClass.boardArray[indx, BoardClass.IndexOfCoinInSelectedTower(indx)];
                                        way_is_safe = 1;
                                        flag = 1;
                                        active_D1_move = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (flag == 2)
                    {
                        int tmp = 0;
                        for (int i = 5; i >= PlayerClass.Dice_number[0] - 1; i--)
                            if (BoardClass.boardArray[i, 0].CompareTag("Black_coin"))
                            {
                                tmp = 1;
                                way_is_safe = 0;
                                flag = 2;
                                break;
                            }
                        
                        if (tmp == 0)
                        {
                            for (int j = 5; j >= 0; j--)
                            {
                                if (BoardClass.boardArray[j, 0].CompareTag("Black_coin") && j < PlayerClass.Dice_number[0])
                                {
                                    sgl_d0_bo = j;
                                    D0_Coin_volunteer_BO = BoardClass.boardArray[j, BoardClass.IndexOfCoinInSelectedTower(j)];
                                    flag = 1;
                                    active_D1_move = true;
                                    way_is_safe = 1;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!PlayerClass.DiceCheck[1] && flag == 2 && way_is_safe == 0)
                {
                    for (int indx = 5; indx >= 0; indx--)
                    {
                        if (indx + 1 == PlayerClass.Dice_number[1] && BoardClass.IndexOfCoinInSelectedTower(indx) >= 0 && BoardClass.boardArray[indx, 0].CompareTag("Black_coin"))
                        {
                            sgl_d1_bo = indx;

                            if (PlayerClass.Container_Of_WC_burnt[0] == null && WCoin_Twr_5_0 == 0)
                            {
                                D1_Coin_volunteer_BO = BoardClass.boardArray[indx, BoardClass.IndexOfCoinInSelectedTower(indx)];
                                way_is_safe = 1;
                                flag = 1;
                                active_D2_move = true;
                                break;
                            }

                            if (PlayerClass.Container_Of_WC_burnt[0] != null | WCoin_Twr_5_0 > 0)
                            {
                                if (BoardClass.IndexOfCoinInSelectedTower(indx) > 1 | BoardClass.IndexOfCoinInSelectedTower(indx) == 0)
                                {
                                    D1_Coin_volunteer_BO = BoardClass.boardArray[indx,BoardClass.IndexOfCoinInSelectedTower(indx)];
                                    way_is_safe = 1;
                                    flag = 1;
                                    active_D2_move = true;
                                    break;
                                }

                                for (int i = 5; i >= PlayerClass.Dice_number[1] - 1; i--)
                                {
                                    if (BoardClass.boardArray[i, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower(i) > 1 | 
                                        BoardClass.IndexOfCoinInSelectedTower(i) == 0 && i - PlayerClass.Dice_number[1] >= 0)
                                    {
                                        way_is_safe = 0;
                                        flag = 2;
                                        break_the_rules = 2;
                                        break;
                                    }

                                    if (flag == 2 && break_the_rules == 0)
                                    {
                                        D1_Coin_volunteer_BO = BoardClass.boardArray[indx,BoardClass.IndexOfCoinInSelectedTower(indx)];
                                        way_is_safe = 1;
                                        flag = 1;
                                        active_D2_move = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    if (flag == 2)
                    {
                        int tmp = 0;

                        for (int i = 5; i >= PlayerClass.Dice_number[1] - 1; i--)
                            if (BoardClass.boardArray[i, 0].CompareTag("Black_coin"))
                            {
                                tmp = 1;
                                flag = 2;
                                way_is_safe = 0;
                                break;
                            }
                        
                        if (tmp == 0)
                        {
                            for (int j = 5; j >= 0; j--)
                            {
                                if (BoardClass.boardArray[j, 0].CompareTag("Black_coin") && j < PlayerClass.Dice_number[1])
                                {
                                    sgl_d1_bo = j;
                                    D1_Coin_volunteer_BO = BoardClass.boardArray[j,BoardClass.IndexOfCoinInSelectedTower(j)];
                                    flag = 1;
                                    active_D2_move = true;
                                    way_is_safe = 1;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (way_is_safe == 1 && flag == 1)
                    StartCoroutine(BearOut_Process());
                

                if (flag == 2 && way_is_safe == 0)
                {
                    CollectInformation();
                    StartCoroutine(Checking_Opposite_Dice_Values_then_makes_move());
                }
            }
            
            if (PlayerClass.even_dice)
            {
                if (!PlayerClass.DiceCheck[3])
                    for (int b0 = 5; b0 >= 0; b0--)
                    {
                        if (b0 + 1 == PlayerClass.Dice_number[0] && BoardClass.IndexOfCoinInSelectedTower(b0) >= 0 &&
                            BoardClass.boardArray[b0, 0].CompareTag("Black_coin"))
                        {
                            sgl_d0_bo = b0;
                            D0_Coin_volunteer_BO = BoardClass.boardArray[b0, BoardClass.IndexOfCoinInSelectedTower(b0)];
                            flag = 1;
                            way_is_safe = 1;
                            break;
                        }
                    }

                if (!PlayerClass.DiceCheck[3] && flag == 2)
                {
                    int tmp = 0;

                    for (int i = 5; i >= PlayerClass.Dice_number[0] - 1; i--)
                        if (BoardClass.boardArray[i, 0].CompareTag("Black_coin"))

                        {
                            tmp = 1;
                            way_is_safe = 0;
                            break;
                        }

                    if (tmp == 0)
                        for (int j = 5; j >= 0; j--)

                        {
                            if (BoardClass.boardArray[j, 0].CompareTag("Black_coin") &&
                                j < PlayerClass.Dice_number[0])

                            {
                                sgl_d0_bo = j;
                                D0_Coin_volunteer_BO = BoardClass.boardArray[j,BoardClass.IndexOfCoinInSelectedTower(j)];
                                flag = 1;
                                way_is_safe = 1;
                                break;
                            }
                        }
                }

                if (way_is_safe == 1 && flag == 1)
                    StartCoroutine(BearOut_Process());
                

                if (way_is_safe == 0 && flag == 2)
                {
                    CollectInformation();
                    StartCoroutine(Checking_Even_Dice_Values_then_makes_move_Black());
                }
            }
        }
        
        //even dice
        if (PlayerClass.even_dice && PlayerClass.player_number == 2 && !PlayerClass.BlackBearOut) // for updating number of alone coins.
        {
            int evenDicChecker;
            bool setEvenDicChecker = false;

            for (evenDicChecker = 3; evenDicChecker >= 0; evenDicChecker--)
            {
                if (PlayerClass.DiceCheck [evenDicChecker])
                {
                    setEvenDicChecker = true;
                    double_dice_falses_index = evenDicChecker + 1; // for deep covering alone checkers
                    break;
                }
            }

            if (setEvenDicChecker && !PlayerClass.DiceCheck [3])
            {
                for (int i = 0; i <= 3 - (evenDicChecker + 1); i++)
                    D0_OppositeLegalTower [i] = PlayerClass.Dice_number [3 + i]; // using  D0_OppositeLegalTower array for storing datas of even dice position
            }
            else
            if (!PlayerClass.DiceCheck [3])
                for (int i = 0; i < 4; i++)
                    D0_OppositeLegalTower [i] = PlayerClass.Dice_number [i+3];

				
            if(PlayerClass.player_number == 2 && deactive_all == 0 && !PlayerClass.playerHasBurntCoin)
                StartCoroutine(Checking_Even_Dice_Values_then_makes_move_Black());

        }

        //opposite dice
        if(deactive_all == 0 && PlayerClass.opposite_dice && !PlayerClass.playerHasBurntCoin && PlayerClass.player_number == 2 && !PlayerClass.BlackBearOut) 
            StartCoroutine(Checking_Opposite_Dice_Values_then_makes_move());
    }
    #endregion

    #region Collecting Data From the board
    private void CollectInformation ()
    {
        int Cnt_all_Coins = 0, monitor_Dice0 = 0,monitor_Dice1 = 0,monitor_EvenDice = 0;
        
        for (Cnt_all_Coins = 23; Cnt_all_Coins >= 0; Cnt_all_Coins--)
        {
            if (PlayerClass.even_dice && BoardClass.boardArray [Cnt_all_Coins, 0].CompareTag("Black_coin")) //dice are same
            {
                monitor_EvenDice = Cnt_all_Coins - PlayerClass.Dice_number [0];

                if (monitor_EvenDice > -1 && monitor_EvenDice < 24)
                {
                    if (!BoardClass.boardArray [monitor_EvenDice, 1].CompareTag("White_coin"))
                    {
                        EvenDiceLegalTowers [D0_Twr_Index] = Cnt_all_Coins; //list of legal towers that has coins for selecting
                        EvenDeliverTowers [D0_Twr_Index] = monitor_EvenDice; //list of legal target towers
                        EvenLegalTowers_Index [D0_Twr_Index] = BoardClass.IndexOfCoinInSelectedTower (Cnt_all_Coins); //number of coins that are in target tower
                        //specifies the newest coin in the source tower
                        Removabl_Coin = BoardClass.boardArray [EvenDiceLegalTowers [D0_Twr_Index], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Twr_Index])];
								
                        if (!Removabl_Coin.CompareTag("White_coin"))
                            D0_RemovableCoins [D0_Twr_Index] = Removabl_Coin; // coin that rbt will move

                        fw0 = D0_Twr_Index;
                        D0_Twr_Index++;
                    }
                }
            }

            //----------------------------------D1 != D2------------------------------------------

            if (PlayerClass.opposite_dice) // dice are not same
            {
                if (BoardClass.boardArray [Cnt_all_Coins, 0].CompareTag("Black_coin"))
                {
                    monitor_Dice0 = Cnt_all_Coins - PlayerClass.Dice_number [0];
                    monitor_Dice1 = Cnt_all_Coins - PlayerClass.Dice_number [1];

                    if (!PlayerClass.DiceCheck[0] && monitor_Dice0 > -1 && monitor_Dice0 < 24 && !BoardClass.boardArray[monitor_Dice0, 1].CompareTag("White_coin")) // dice_number [0]
                    {
                        D0_OppositeLegalTower [D0_Twr_Index] = Cnt_all_Coins; //list of legal source towers
                        D0_OppositeDeliverTower [D0_Twr_Index] = monitor_Dice0; //list of legal target towers
                            
                        Removabl_Coin = BoardClass.boardArray [D0_OppositeLegalTower [D0_Twr_Index], 
                            BoardClass.IndexOfCoinInSelectedTower (D0_OppositeLegalTower [D0_Twr_Index])]; //number of coins that are in target tower

                        D0_RemovableCoins [D0_Twr_Index] = Removabl_Coin; // coin that rbt will move
                            
                        fw0 = D0_Twr_Index;
                        D0_Twr_Index++;

                    }

                    if (!PlayerClass.DiceCheck[1] && monitor_Dice1 > -1 && monitor_Dice1 < 24 && !BoardClass.boardArray [monitor_Dice1, 1].CompareTag("White_coin")) // dice_number [1]
                    {
                        D1_OppositeLegalTower [D1_Twr_Index] = Cnt_all_Coins;
                        D1_OppositeDeliverTower [D1_Twr_Index] = monitor_Dice1;
                       
                        Removabl_Coin = BoardClass.boardArray [D1_OppositeLegalTower [D1_Twr_Index], 
                            BoardClass.IndexOfCoinInSelectedTower (D1_OppositeLegalTower [D1_Twr_Index])];

                        D1_RemovableCoins [D1_Twr_Index] = Removabl_Coin;
                            
                        fw1 = D1_Twr_Index;
                        D1_Twr_Index++;

                    }

                }

                big_fw = fw0 > fw1 ? fw0 : fw1;
            }

            //------------No Move----------------
           
            if (Cnt_all_Coins <= 0) // when no possible move is available
            {

                D0_Twr_Index = D1_Twr_Index = 0;

                if (PlayerClass.opposite_dice && D0_RemovableCoins[0] == null && D1_RemovableCoins[0] == null)
                {
                    deactive_all = 1;
                    SameTower = active_sgl_move = false;
                    StartCoroutine(RollDice());
                }

                if (PlayerClass.even_dice && D0_RemovableCoins[0] == null)
                {
                    deactive_all = 1;
                    StartCoroutine(RollDice());
                }
                
                break;
            }
        }
    }

    #endregion
    
    #region Burn Process
    private IEnumerator BurnProcess() //If RBT has some burnt coin *** this method will run before other methods
    {
        bool returnToHome = false;
        D0_Return = D1_Return = false;
        burn_regular = D0_Base_On_Position = D1_Base_On_Position = 0;

        int alterTwr = 0;
   
            if (PlayerClass.even_dice)
            {
                if (!BoardClass.boardArray[Even_Burn_deliverTower, 1].CompareTag("White_coin")) // checks the index 1 of deliver tower that has not against coin
                    Even_Burn_Activated = true;
                else
                {
                   // DataManagerClass.noMove.Play("Nomove");
                    Even_Burn_Activated = false;
                    StartCoroutine (RollDice());
                }
                 
                if (Even_Burn_Activated)
                {
                    
                    StartCoroutine(PlayerClass.BurnTheCoin(Even_Burn_deliverTower, "White"));
                    
                    yield return new WaitForSecondsRealtime(PlayerClass.pauseTime);
                    
                    PlayerClass.SelectedCoin = PlayerClass.Container_Of_BC_burnt[PlayerClass.Num_Of_BC_Burnt].transform;
                    PlayerClass.xcoin_point = PlayerClass.SelectedCoin;
                    
                    PlayerClass.black_coin_move_counter -= 24 - Even_Burn_deliverTower;
                    DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();
                    
                    PlayerClass.Update_TargetTower(Even_Burn_deliverTower);
                    
                    PlayerClass.Container_Of_BC_burnt[PlayerClass.Num_Of_BC_Burnt] = null;
                    
                    PlayerClass.Final_Position_OF_Coin = Even_Burn_deliverTower * 15 + BoardClass.IndexOfCoinInSelectedTower(Even_Burn_deliverTower);
                    DataManagerClass.coinImpactSnd.Play();
                    for (int diceChecker = 0; diceChecker <= 3; diceChecker++)
                    {
                        if (!PlayerClass.DiceCheck[diceChecker])
                        {
                            PlayerClass.DiceCheck[diceChecker] = true;
                            break;
                        }
                    }
                    
                    yield return new WaitForSecondsRealtime(0.65f);
                    PlayerClass.Num_Of_BC_Burnt--;
                    CalculateCoinsPosition();
                 
                }

            }

            //=============================================================================================================================

            if (PlayerClass.opposite_dice)
            {
                if (BoardClass.boardArray[Opposite_Burn_deliverTower[0], 1].CompareTag("White_coin") && BoardClass.boardArray[Opposite_Burn_deliverTower[1], 1].CompareTag("White_coin"))
                    StartCoroutine (RollDice ());
                else
                {
                    // when player has just one burned coin 
                  if (PlayerClass.Num_Of_BC_Burnt > -1)//burn
                  {
                      if (!PlayerClass.DiceCheck[0] && BoardClass.boardArray [Opposite_Burn_deliverTower [0], 0].CompareTag("White_coin") && BoardClass.boardArray [Opposite_Burn_deliverTower [0], 1].CompareTag("blank"))
                      {
                          StartCoroutine (PlayerClass.BurnTheCoin (Opposite_Burn_deliverTower [0], "White"));
                          burn_regular = 1;
                          alterTwr = Opposite_Burn_deliverTower[0];
                      }
                      else
                      if (!PlayerClass.DiceCheck[1] && BoardClass.boardArray [Opposite_Burn_deliverTower [1], 0].CompareTag("White_coin") && BoardClass.boardArray [Opposite_Burn_deliverTower [1], 1].CompareTag("blank"))
                      {
                          StartCoroutine (PlayerClass.BurnTheCoin (Opposite_Burn_deliverTower [1], "White"));
                          burn_regular = 1;
                          alterTwr = Opposite_Burn_deliverTower[1];
                      }
                    
                      if (burn_regular == 1) 
                          returnToHome = true;

                      if (!returnToHome && PlayerClass.Num_Of_BC_Burnt == 0 && !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1])
                      {
                          //rbt before returning his coins in the board from burn section, will checks out is it possible to burn its against coins 
                          for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                          {
                              if (BoardClass.boardArray [D0_OppositeDeliverTower [D0_Cnt_Twr], 0].CompareTag("White_coin") &&
                                  BoardClass.boardArray [D0_OppositeDeliverTower [D0_Cnt_Twr], 1].CompareTag("blank") && 
                                  !BoardClass.boardArray [Opposite_Burn_deliverTower[1],1].CompareTag("White_coin") &&
                                   BoardClass.IndexOfCoinInSelectedTower (D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0)
                              {
                                  D0_Base_On_Position = 1;
                                  alterTwr = Opposite_Burn_deliverTower[1];
                                  break;
                              }
                          }

                          if (D0_Base_On_Position == 0)
                              for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                              {
                                  if (BoardClass.boardArray [D1_OppositeDeliverTower [D1_Cnt_Twr], 0].CompareTag("White_coin") && 
                                      BoardClass.boardArray [D1_OppositeDeliverTower [D1_Cnt_Twr], 1].CompareTag("blank") && 
                                       !BoardClass.boardArray [Opposite_Burn_deliverTower[0],1].CompareTag("White_coin") && 
                                        BoardClass.IndexOfCoinInSelectedTower (D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0)
                                  {
                                      D1_Base_On_Position = 1;
                                      alterTwr = Opposite_Burn_deliverTower[0];
                                      break;
                                  }
                              }

                          if (D0_Base_On_Position == 1 | D1_Base_On_Position == 1 && alterTwr > 5)
                              returnToHome = true; // prevent of execute the other ways
                         
                      }
                    
                      // now rbt going to check...
                      if(!returnToHome && PlayerClass.Num_Of_BC_Burnt == 0)
                      {
                          for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                          {
                              if (BoardClass.IndexOfCoinInSelectedTower (D0_OppositeDeliverTower[D0_Cnt_Twr]) == 0 && 
                                  BoardClass.boardArray [D0_OppositeDeliverTower [D0_Cnt_Twr],0].CompareTag("Black_coin") && 
                                  !BoardClass.boardArray [Opposite_Burn_deliverTower[1],1].CompareTag("White_coin") &&
                                  BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) > 1 && 
                                  D0_OppositeDeliverTower[D0_Cnt_Twr] < 18 )
                              {
                                  D0_Base_On_Position = 2;
                                  alterTwr = Opposite_Burn_deliverTower[1];
                                  break;
                              }
                          }

                          if(D0_Base_On_Position == 0)
                              for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                              {
                                  if (BoardClass.IndexOfCoinInSelectedTower (D1_OppositeDeliverTower[D1_Cnt_Twr]) == 0 && 
                                      BoardClass.boardArray [D1_OppositeDeliverTower [D1_Cnt_Twr],0].CompareTag("Black_coin") && 
                                      !BoardClass.boardArray [Opposite_Burn_deliverTower[0],1].CompareTag("White_coin") &&
                                      BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) > 1 &&
                                      D0_OppositeDeliverTower[D1_Cnt_Twr] < 18)
                                  {
                                      D1_Base_On_Position = 2;
                                      alterTwr = Opposite_Burn_deliverTower[0];
                                      break;
                                  }
                              }

                          if(D0_Base_On_Position == 2 | D1_Base_On_Position == 2) 
                              returnToHome = true;
                      }
               
                    
                      if (!returnToHome)
                      {
                          if (!PlayerClass.DiceCheck[0] && !BoardClass.boardArray[Opposite_Burn_deliverTower[0], 1].CompareTag("White_coin"))
                          {
                              D0_Return = true;
                              alterTwr = Opposite_Burn_deliverTower[0];
                          }

                          if (!PlayerClass.DiceCheck[1] && !BoardClass.boardArray[Opposite_Burn_deliverTower[1], 1].CompareTag("White_coin"))
                          {
                              D1_Return = true;
                              alterTwr = Opposite_Burn_deliverTower[1];
                          }

                          if (D0_Return | D1_Return) returnToHome = true;
                      }
                  }
          
                  //if rbt is able just return one of his own coins in the board
                  if(!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck [1] && PlayerClass.Num_Of_BC_Burnt == 0 && !returnToHome)
                  {
                      if (!BoardClass.boardArray[Opposite_Burn_deliverTower[0], 1].CompareTag("White_coin"))
                      {
                          returnToHome = true;
                          StartCoroutine(PlayerClass.BurnTheCoin(Opposite_Burn_deliverTower[0], "White"));
                      }
                      else if (!BoardClass.boardArray[Opposite_Burn_deliverTower[1], 1].CompareTag("White_coin"))
                      {
                          returnToHome = true;
                          StartCoroutine(PlayerClass.BurnTheCoin(Opposite_Burn_deliverTower[1], "White"));
                      }
                  }
                
                  if(returnToHome)
                  {
                      PlayerClass.SelectedCoin = PlayerClass.Container_Of_BC_burnt [PlayerClass.Num_Of_BC_Burnt].transform;
                      PlayerClass.xcoin_point = PlayerClass.SelectedCoin.transform;

                      PlayerClass.black_coin_move_counter -= 24 - alterTwr;

                      DataManagerClass.blackMoveCounterTxt.text  = PlayerClass.black_coin_move_counter.ToString();
                            
                      PlayerClass.Update_TargetTower(alterTwr);
                      PlayerClass.Final_Position_OF_Coin = alterTwr * 15 + BoardClass.IndexOfCoinInSelectedTower (alterTwr);

                      if (24 - alterTwr == PlayerClass.Dice_number[0]) // if deliver tower equal to PlayerClass.Dice_number[0]
                          PlayerClass.DiceCheck[0] = true;
                      else
                          PlayerClass.DiceCheck[1] = true;

                      PlayerClass.Container_Of_BC_burnt [PlayerClass.Num_Of_BC_Burnt] = null;

                      PlayerClass.Num_Of_BC_Burnt--;
                    
                      yield return new WaitForSecondsRealtime (0.65f);
                      CalculateCoinsPosition();
                  }
                  else
                      StartCoroutine(RollDice());
                }
             
            }

    }
    #endregion

    #region Checking Opposite Dice Values_Black
    private IEnumerator Checking_Opposite_Dice_Values_then_makes_move()
    {
        if (deactive_all == 0 && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1] && !PlayerClass.playerHasBurntCoin && !PlayerClass.BlackBearOut | way_is_safe == 0)
        {
            int totalWCoinTwr23_18,Alo_totalBCoinTwr5_0,totalWCoinTwr5_0,TotalBCoinTwr5_0;

            totalWCoinTwr23_18 = Alo_totalBCoinTwr5_0 = totalWCoinTwr5_0 = TotalBCoinTwr5_0 = 0;
				
            int Dice0SameTwr = 0, Dice1SameTwr = 0;

            //Collecting data ***
            for (int i = 23; i >= 18; i--)
            for (int j = 0; j <= 14; j++)
                if (BoardClass.boardArray[i, j].CompareTag("White_coin"))
                    totalWCoinTwr23_18++; // number of white coins

            for (int i1 = 5; i1 >= 0; i1--)
            {
                for (int j = 0; j <= 14; j++)
                    if (BoardClass.boardArray[i1, j].CompareTag("Black_coin"))
                        TotalBCoinTwr5_0++; // number of black coins

                if (BoardClass.boardArray[i1, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower(i1) == 0)
                    Alo_totalBCoinTwr5_0++; //number of those towers which has just one black coins
					
                if (BoardClass.boardArray[i1, 0].CompareTag("White_coin") && BoardClass.IndexOfCoinInSelectedTower(i1) == 0)
                    totalWCoinTwr5_0++; //number of those towers which has just one white coins
            }
            //***

            if (D0_Switch_flag == 0 && D1_Switch_flag == 0 && !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && PlayerClass.White_beardout_Coin[0] == null && 
                !PlayerClass.BlackBearOut | PlayerClass.No_Bear_out_BC_Twr_5_0 == 0 && totalWCoinTwr23_18 < 14)
                SameTower = true; // when rbt is able to place two coins in a same tower

            else if (!PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
                active_sgl_move = true;
	
            #region SameTower Process

            if (SameTower) // !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && 
            {
                int deactivateSameTowerMethod,temp_value_of_alone_coin,DeliverSameTwr;

                deactivateSameTowerMethod = temp_value_of_alone_coin = DeliverSameTwr = -1;
					
                D0_SameTwr = D1_SameTwr = 0;

                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 0].CompareTag("White_coin") && 
                        BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 1].CompareTag("blank") && D0_OppositeDeliverTower[D0_Cnt_Twr] <= 6 &&
                        BoardClass.IndexOfCoinInSelectedTower(D0_OppositeDeliverTower[D0_Cnt_Twr]) > 1)
                    {
                        deactivateSameTowerMethod = 1;
                        Dice0SameTwr = 1;
                        DeliverSameTwr = D0_OppositeDeliverTower[D0_Cnt_Twr];
                        break;
                    }
                }

                for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                {
                    if (BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], 0].CompareTag("White_coin") && 
                        BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], 1].CompareTag("blank") && D1_OppositeDeliverTower[D1_Cnt_Twr] <= 6
                        && BoardClass.IndexOfCoinInSelectedTower(D1_OppositeDeliverTower[D1_Cnt_Twr]) > 1)
                    {
                        deactivateSameTowerMethod = 1;
                        Dice1SameTwr = 1;
                        DeliverSameTwr = D1_OppositeDeliverTower[D0_Cnt_Twr]; //be fore was place_D1 !
                        break;
                    }
                }

                if (Dice0SameTwr == 1 && Dice1SameTwr == 1 && DeliverSameTwr > 17 ) // && TargetSameTwr < 6
                    deactivateSameTowerMethod = 0;
               

                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (D0_OppositeLegalTower[D0_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]) >= 6
                        && BoardClass.boardArray[D0_OppositeLegalTower[D0_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]),1].CompareTag("blank")
                        && BoardClass.boardArray[D0_OppositeLegalTower[D0_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]),0].CompareTag("White_coin"))
                        deactivateSameTowerMethod = 1;
                    
                }

                for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                {
                    if (D1_OppositeLegalTower[D1_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]) >= 6
                        && BoardClass.boardArray[D1_OppositeLegalTower[D1_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]),1].CompareTag("blank")
                        && BoardClass.boardArray[D1_OppositeLegalTower[D1_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]),0].CompareTag("White_coin"))
                        deactivateSameTowerMethod = 1;
                  
                }

                for (int i = 5; i >= 0; i--)
                    if (BoardClass.boardArray[i, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower(i) == 0)
                    {
                        temp_value_of_alone_coin = i;
                        break;
                    }
                
        
                for (int D0_aln = 0; D0_aln <= fw0; D0_aln++)
                {
                    if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_aln]) >= 1 && D0_OppositeDeliverTower[D0_aln] == temp_value_of_alone_coin)
                    {
                        deactivateSameTowerMethod = 1;
                        break;
                    }

                }
                    
                for (int D1_aln = 0; D1_aln <= fw1; D1_aln++)
                {
                    if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_aln]) >= 1 && D1_OppositeDeliverTower[D1_aln] == temp_value_of_alone_coin)
                    {
                        deactivateSameTowerMethod = 1;
                        break;
                    }
                }

                if (totalWCoinTwr5_0 == 0 && PlayerClass.Container_Of_WC_burnt[0] == null && PlayerClass.White_beardout_Coin[0] != null)
                    deactivateSameTowerMethod = 1;
               
                if (deactivateSameTowerMethod == -1)
                    for (D0_SameTwr = big_fw; D0_SameTwr >= 0; D0_SameTwr--)
                    {
                        for (D1_SameTwr = big_fw; D1_SameTwr >= 0; D1_SameTwr--)
                        {
                            if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && D0_OppositeDeliverTower[D0_SameTwr] == D1_OppositeDeliverTower[D1_SameTwr] &&
                                !BoardClass.boardArray[D0_OppositeDeliverTower[D0_SameTwr], 0].CompareTag("Black_coin") && 
                                D0_OppositeLegalTower[D0_SameTwr] >= 1 && D1_OppositeLegalTower[D1_SameTwr] >= 1 && 
                                D0_OppositeLegalTower[D0_SameTwr] != D1_OppositeLegalTower[D1_SameTwr] && 
                                BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_SameTwr]) > 1 |
                                BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_SameTwr]) == 0 && 
                                BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_SameTwr]) > 1 |
                                BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_SameTwr]) == 0)
                            {
                                F_flag = 1;
                                active_D1_move = true;
                                    
                                if (BoardClass.boardArray[D0_OppositeDeliverTower[D0_SameTwr], 0].CompareTag("White_coin") &&
                                    !BoardClass.boardArray[D0_OppositeDeliverTower[D0_SameTwr], 1].CompareTag("White_coin"))
                                {
                                    StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_SameTwr], "White"));
                                    if (PlayerClass.pauseTime == 0f)
                                        yield return new WaitForSecondsRealtime(0.65f);
                                    else
                                        yield return new WaitForSecondsRealtime(PlayerClass.pauseTime);

                                }

                                if (active_D1_move)
                                {
                                    PlayerClass.SelectedCoin = D0_RemovableCoins[D0_SameTwr].transform;
                                    PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_SameTwr]);
                                    PlayerClass.SelectedTowerNumber = D0_OppositeLegalTower[D0_SameTwr];
                                    PlayerClass.DeliveryTower = D0_OppositeDeliverTower[D0_SameTwr];
                                    PlayerClass.Tower_Number = PlayerClass.DeliveryTower;

                                    PlayerClass.xcoin_point = PlayerClass.SelectedCoin.transform;

                                    PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;

                                    DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();
											
                                    PlayerClass.Update_TargetTower(PlayerClass.Tower_Number);

                                    PlayerClass.Final_Position_OF_Coin = PlayerClass.DeliveryTower * 15 + BoardClass.IndexOfCoinInSelectedTower(PlayerClass.DeliveryTower);

                                    PlayerClass.Update_board(true,PlayerClass.SelectedCoin.gameObject);

                                    DataManagerClass.coinImpactSnd.Play();
                                        
                                    PlayerClass.DiceCheck[0] = true;
								
                                    active_D1_move = false;

                                    yield return new WaitForSecondsRealtime(0.65f);
                                    active_D2_move = true;
                                }

                                if (active_D2_move)
                                {
                               
                                    PlayerClass.SelectedCoin = D1_RemovableCoins[D1_SameTwr].transform;
                                    PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_SameTwr]);
                                    PlayerClass.SelectedTowerNumber = D1_OppositeLegalTower[D1_SameTwr];
                                    PlayerClass.DeliveryTower = D1_OppositeDeliverTower[D1_SameTwr];
                                    PlayerClass.Tower_Number = PlayerClass.DeliveryTower;

                                    PlayerClass.xcoin_point = PlayerClass.SelectedCoin;

                                    PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;

                                    DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();

                                    PlayerClass.Update_TargetTower(PlayerClass.Tower_Number);

                                    PlayerClass.Final_Position_OF_Coin = PlayerClass.DeliveryTower * 15 + BoardClass.IndexOfCoinInSelectedTower(PlayerClass.DeliveryTower);

                                    PlayerClass.Update_board(true,PlayerClass.SelectedCoin.gameObject);
                                    DataManagerClass.coinImpactSnd.Play();

                                    PlayerClass.DiceCheck[1] = true;

                                    active_D2_move = false;
                                
                                }

                            }
                        }
                    }
                
                if (PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1])
                {
                    if (PlayerClass.SelectedCoin == null)
                        PlayerClass.SelectedCoin = GameObject.Find("XForm").transform;
               
                    F_flag = 0;
                    SameTower = false;
                    StartCoroutine(RollDice());
				 
                }

                if (F_flag == 2 | !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1] && !PlayerClass.BlackBearOut | way_is_safe == 0)
                {
                    SameTower = false;
                    active_sgl_move = true;
                }
            }

            #endregion
                
            //--------------------Single_Mode---------------------------------------------------------------->

            #region Single_Mode_black

            if (!PlayerClass.BlackBearOut | way_is_safe == 0 && active_sgl_move)
            {
                    
                int fromAToB = 0,A_to_B_sng_dx = 0,X_Twr_Brn_1 = 0,X_Twr_Brn_2 = 0,Tmp_of_sng_dx = 0,
                    Tmp_Selected_Twr_dx = 0,RiskPercent = 0,can_move_from_home = 0,cover_option = 0,flag_cover_option = 0,
                    WCoin_Chance_To_Win = 0,Cnt_Bcoin_Twr_23_18 = 0, Coverd_BCoin_Twr_5_0 = 0 , WCoin_5_0 = 0;

                D0_Switch_flag = D1_Switch_flag = 0;

                //Collecting data
                for (int i = 0; i <= 5; i++)
                    if (BoardClass.boardArray[i, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower(i) > 0)
                        Coverd_BCoin_Twr_5_0++;
                     
                for (int j = 23; j >= 19; j--)
                    if (BoardClass.boardArray[j, 0].CompareTag("Black_coin"))
                        Cnt_Bcoin_Twr_23_18++;

                for (int i = 0; i < 6; i++)
                    if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                        WCoin_5_0++;
             
                if (PlayerClass.W_win > 0 && Cnt_Bcoin_Twr_23_18 > 0 && PlayerClass.Container_Of_WC_burnt[0] == null)
                    WCoin_Chance_To_Win++;
					
                //burn b4 all
                if (!PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    if (!PlayerClass.DiceCheck[0]) //  && D0_OppositeLegalTower[0] > 0 // CompareTag("Black_coin")
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 0].CompareTag("White_coin") && 
                                BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 1].CompareTag("blank") && 
                                BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) == 0 |
                                (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0 && D0_OppositeDeliverTower[D0_Cnt_Twr] >= 6) |
                                (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 2 && D0_OppositeLegalTower[D0_Cnt_Twr] >= 8 && Cnt_Bcoin_Twr_23_18 > 0))
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                X_Twr_Brn_1 = D0_Cnt_Twr;
                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                break;
                            }
                        }
                    }

                    if (!PlayerClass.DiceCheck[1] ) //  && D1_OppositeLegalTower[0] > 0 // CompareTag("Black_coin")
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], 0].CompareTag("White_coin") && 
                                BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr],1].CompareTag("blank") &&
                                BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) == 0 | 
                                (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0 && D1_OppositeDeliverTower[D1_Cnt_Twr] >= 6) |
                                (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 2 && D1_OppositeLegalTower[D1_Cnt_Twr] >= 8 && Cnt_Bcoin_Twr_23_18 > 0))
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                X_Twr_Brn_2 = D1_Cnt_Twr;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                break;
                            }
                        }

                    if (D0_Switch_flag == 1) // it will burn with dice0 and covers by dice1 value
                        cover_option = Tmp_of_sng_dx - PlayerClass.Dice_number[1];

                    if (D1_Switch_flag == 1)
                        cover_option = Tmp_of_sng_dx - PlayerClass.Dice_number[0];

                    if (Tmp_of_sng_dx > 0 && Tmp_of_sng_dx <= 5)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                        {
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                RiskPercent++;
                           
                            if (cover_option > -1 && BoardClass.boardArray[cover_option, 0].CompareTag("Black_coin"))
                            {
                                RiskPercent = 0;
                                flag_cover_option = 1;
                            }
                        }

                    if (Tmp_of_sng_dx > 0 && Tmp_of_sng_dx <= 5 && flag_cover_option == 0) // move from black home.
                    {
                        for (int i = 23; i >= 17; i--)
                        {
                            if (BoardClass.boardArray[i, 0].CompareTag("Black_coin")) 
                                fromAToB = i - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);
                                
                            if (BoardClass.boardArray[fromAToB, 0].CompareTag("White_coin") && BoardClass.boardArray[fromAToB, 1].CompareTag("blank"))
                            {
                                can_move_from_home++;
                                break;
                            }
                        }
                    }
                        
                    if ((PlayerClass.Container_Of_WC_burnt[0] != null && Tmp_of_sng_dx <= 6) | 
                        RiskPercent >= 1 | can_move_from_home >= 1 | (Alo_totalBCoinTwr5_0 >= 1 && Tmp_of_sng_dx < 6))
                    {
                        flag = 2;
                        D0_Switch_flag = D1_Switch_flag = 0;
                    }

                    if (D0_Switch_flag == 1 && D1_Switch_flag == 1)
                    {
                        RiskPercent = 0;
                        if (D0_OppositeDeliverTower[X_Twr_Brn_1] != D1_OppositeDeliverTower[X_Twr_Brn_2])
                        {
                            if (D0_OppositeDeliverTower[X_Twr_Brn_1] > D1_OppositeDeliverTower[X_Twr_Brn_2])
                            {
                                if (D0_OppositeDeliverTower[X_Twr_Brn_1] < 8)
                                    for (int i = 0; i < D0_OppositeDeliverTower[X_Twr_Brn_1]; i++)
                                    {
                                        if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                        {
                                            RiskPercent++;
                                            D0_Switch_flag = 0;
                                            flag = 2;
                                        }
                                    }

                                if (RiskPercent == 0)
                                {
                                    StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[X_Twr_Brn_1], "White"));
                                    flag = D1_Switch_flag = 0;
                                }
                            }

                            if ((D1_OppositeDeliverTower[X_Twr_Brn_2] > D0_OppositeDeliverTower[X_Twr_Brn_1]) | flag == 2)
                            {
                                RiskPercent = 0;

                                if (D1_OppositeDeliverTower[X_Twr_Brn_2] < 8)
                                    for (int i = 0; i < D1_OppositeDeliverTower[X_Twr_Brn_2]; i++)
                                    {
                                        if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                        {
                                            RiskPercent++;
                                            D1_Switch_flag = D0_Switch_flag = 0;
                                            flag = 2;
                                        }
                                    }

                                if (RiskPercent == 0)
                                {
                                    StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[X_Twr_Brn_2], "White"));
                                    flag = D0_Switch_flag = 0;
                                }
                            }
                        }

                        if (D0_OppositeDeliverTower[X_Twr_Brn_1] == D1_OppositeDeliverTower[X_Twr_Brn_2])
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[X_Twr_Brn_1]) == 0)
                            {
                                D1_Switch_flag = 0;
                                StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[X_Twr_Brn_1], "White"));
                            }

                            else if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[X_Twr_Brn_2]) == 0)

                            {
                                D0_Switch_flag = 0;
                                StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[X_Twr_Brn_2], "White"));
                            }

                            if (D0_Switch_flag == 1 && D1_Switch_flag == 1)

                            {
                                if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[X_Twr_Brn_1]) > 1)
                                {
                                    D1_Switch_flag = 0;
                                    StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[X_Twr_Brn_1], "White"));
                                }

                                else if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[X_Twr_Brn_2]) > 1)
                                {
                                    D0_Switch_flag = 0;
                                    StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[X_Twr_Brn_2], "White"));
                                }

                                if (D0_Switch_flag == 1 && D1_Switch_flag == 1)// if X_Twr_Brn_1 & X_Twr_Brn_2 have the same index tower it choose on of them for porcess of burn
                                {
                                    D0_Switch_flag = 1;
                                    D1_Switch_flag = 0;
                                    StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[X_Twr_Brn_1], "White"));
                                }
                            }
                        }

                        if (flag == 2)
                            D0_Switch_flag = D1_Switch_flag = 0;
                    }
                    else
                    {
                        if (D0_Switch_flag == 1)
                            StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[X_Twr_Brn_1], "White"));
                     
                        if (D1_Switch_flag == 1)
                            StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[X_Twr_Brn_2], "White"));
                       
                    }
                }
                    
                // if there is a black checker out of home its home area (tower 0 till 6) of white player rbt will move that black checker
                // cause makes able rbt to use second move for bearing out its checkers (win_floor)
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0 && !PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && TotalBCoinTwr5_0 >= 14)
                {
                    if (BoardClass.boardArray[PlayerClass.Dice_number[0],0].CompareTag("Black_coin"))
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                            if (D0_OppositeLegalTower[D0_Cnt_Twr] - PlayerClass.Dice_number[0] < 0)
                                Tmp_of_sng_dx = 1;
                           
                    }

                    if (Tmp_of_sng_dx == 0)
                        if (BoardClass.boardArray[PlayerClass.Dice_number[1], 0].CompareTag("Black_coin"))
                        {
                            for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                                if (D1_OppositeLegalTower[D1_Cnt_Twr] - PlayerClass.Dice_number[1] < 0)
                                    Tmp_of_sng_dx = 2;
                        }
                    
                    if (Tmp_of_sng_dx == 2)
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0)
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));
                                break;
                            }
                        }
                    }

                    if (Tmp_of_sng_dx == 1 && D0_Switch_flag == 0)
                    {
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0)
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                                break;
                            }
                        }
                    }

                    if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                        Tmp_of_sng_dx = 0;
                }


                if (D0_Switch_flag == 0 && D1_Switch_flag == 0 && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1] && WCoin_Chance_To_Win > 0)
                {
                    RiskPercent = 0;

                    if (!PlayerClass.DiceCheck[0] && D0_RemovableCoins[0] != null) 
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0 && PlayerClass.W_win >= 2 && WCoin_5_0 == 0)
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                break;
                            }
                        }
                    }

                    if (D1_OppositeLegalTower[0] > 0 && !PlayerClass.DiceCheck[1] && D0_Switch_flag == 0)
                    {
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0 && PlayerClass.W_win >= 2 && WCoin_5_0 == 0)
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                break;
                            }
                        }
                    }

                    // !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin"): if in deliver tower was not any against checker
                    // and BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("White_coin"): checks if against player could not burn its checker
                    
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") | BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("White_coin"))
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                RiskPercent++;
                          
                        
                    //if against has burnt checker and it will bring it in its home
                    if ((Coverd_BCoin_Twr_5_0 < 5 && PlayerClass.Container_Of_WC_burnt[0] != null && 
                         !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin")) | RiskPercent >= 1)
                    {
                        flag = 2;
                        D0_Switch_flag = D1_Switch_flag = 0;
                    }

                    if (D0_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));
                    
                    if (D1_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                }

                //covering alone coin: when there were an alone coin in the specific tower
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    if (!PlayerClass.DiceCheck[0] && D0_RemovableCoins[0] != null)
                        for (D0_Cnt_Twr = fw0; D0_Cnt_Twr >= 0; D0_Cnt_Twr--)
                        {
                            if (WCoin_5_0 > 0 | PlayerClass.Container_Of_WC_burnt[0] != null && (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 1 && Alo_totalBCoinTwr5_0 >= 1 && D0_OppositeDeliverTower[D0_Cnt_Twr] <= 5)
                                | (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) == 0 && Alo_totalBCoinTwr5_0 >= 1 && D0_OppositeDeliverTower[D0_Cnt_Twr] <= 5) &&
                                BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 0].CompareTag("Black_coin") &&
                                BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 1].CompareTag("blank"))
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                Tmp_Selected_Twr_dx = D0_OppositeLegalTower[D0_Cnt_Twr];
                                break;
                            }
                        }

                    if (D0_Switch_flag == 0 && !PlayerClass.DiceCheck[1] && D1_RemovableCoins[0] != null)
                        for (D1_Cnt_Twr = fw1; D1_Cnt_Twr >= 0; D1_Cnt_Twr--)
                        {
                            if (WCoin_5_0 > 0  | PlayerClass.Container_Of_WC_burnt[0] != null  && (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 1 && Alo_totalBCoinTwr5_0 >= 1 && D1_OppositeDeliverTower[D1_Cnt_Twr] <= 5)  |
                                (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) == 0 && Alo_totalBCoinTwr5_0 >= 1 && D1_OppositeDeliverTower[D1_Cnt_Twr] <= 5) &&
                                BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], 0].CompareTag("Black_coin") &&
                                BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], 1].CompareTag("blank"))
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                Tmp_Selected_Twr_dx = D1_OppositeLegalTower[D1_Cnt_Twr];
                                break;
                            }
                        }

                }
                
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    if (!PlayerClass.DiceCheck[0] && D0_RemovableCoins.ElementAt(0) != null )
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (Cnt_Bcoin_Twr_23_18 >= 1 &&  (PlayerClass.W_win >= 12 && PlayerClass.B_win == 0 && D0_OppositeLegalTower[D0_Cnt_Twr] > 17) | 
                                (BoardClass.boardArray[17, 1].CompareTag("White_coin") && BoardClass.boardArray[16, 1].CompareTag("White_coin")) && D0_OppositeDeliverTower[D0_Cnt_Twr] > 12)
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                X_Twr_Brn_1 = D0_OppositeLegalTower[D0_Cnt_Twr];
                                StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));
                                break;
                            }
                        }

                    if (D0_Switch_flag == 0 && !PlayerClass.DiceCheck[1] && D1_RemovableCoins.ElementAt(0) != null ) 
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (Cnt_Bcoin_Twr_23_18 >= 1 && (PlayerClass.W_win >= 12 && PlayerClass.B_win == 0 && D1_OppositeLegalTower[D1_Cnt_Twr] > 17) | 
                                (BoardClass.boardArray[17, 1].CompareTag("White_coin") && BoardClass.boardArray[16, 1].CompareTag("White_coin")) && D1_OppositeDeliverTower[D1_Cnt_Twr] > 12)
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                X_Twr_Brn_1 = D1_OppositeLegalTower[D1_Cnt_Twr];
                                StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                                break;
                            }
                        }

                    for (int i = 23; i >= 17; i--)
                    {
                        if (BoardClass.boardArray[i, 0].CompareTag("Black_coin"))
                            fromAToB = i - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);

                        if (BoardClass.boardArray[fromAToB, 0].CompareTag("White_coin") && BoardClass.boardArray[fromAToB, 1].CompareTag("blank"))
                        {
                            D0_Switch_flag = D1_Switch_flag = 0; //switch
                            break;
                        }
                    }

                    if (D0_Switch_flag != 0 | D1_Switch_flag != 0 && Tmp_of_sng_dx <= 15 && !BoardClass.boardArray[X_Twr_Brn_1, 0].CompareTag("Black_coin"))
                        D0_Switch_flag = D1_Switch_flag = 0;
                   
                }

                if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    var Num_B_coin_indx_1 = 0;
                    
                    for (int i = 0; i < fw0; i++)
                        if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[i]) == 1)
                            Num_B_coin_indx_1++;
                    
                    for (int i = 0; i < fw1; i++)
                        if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[i]) == 1)
                            Num_B_coin_indx_1++;
                
                    RiskPercent = 0;
                        
                    for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                    {
                        fromAToB = D0_OppositeLegalTower[D0_Cnt_Twr] -(PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);

                        // D1_RemovableCoins[0] == null / D0_RemovableCoins[0] == null  : checks if there only move for player and after that the other move will specify or maybe not
                        if (fromAToB > -1 && BoardClass.Is_The_Delivery_Tower_Free(fromAToB) && D1_RemovableCoins[0] == null | Num_B_coin_indx_1 > 3 |
                            (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) == 0 && BoardClass.boardArray[fromAToB, 0].CompareTag("Black_coin")) |
                            (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) > 0 && BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) <= 3
                            && BoardClass.boardArray[fromAToB, 0].CompareTag("Black_coin"))  |
                            (BoardClass.boardArray[fromAToB, 0].CompareTag("White_coin") && BoardClass.boardArray[fromAToB, 1].CompareTag("blank")) )
                        {
                            flag = 0;
                            D0_Switch_flag = A_to_B_sng_dx = 1;
                            Tmp_of_sng_dx = D0_OppositeLegalTower[D0_Cnt_Twr];
                            break;
                        }
                    }
             
                    if (D1_RemovableCoins[0] != null && D0_Switch_flag == 1 && !BoardClass.boardArray[fromAToB, 0].CompareTag("Black_coin")) // not coverage
                    {
                        if (fromAToB <= 6 && BoardClass.IndexOfCoinInSelectedTower(Tmp_of_sng_dx) > 0)
                            for (int i = 0; i < fromAToB; i++) // checks to find wc checker
                                if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                    RiskPercent++;
                             

                        if ((PlayerClass.Container_Of_WC_burnt[0] != null && fromAToB <= 6) | RiskPercent > 0) // | can_move_from_home > 0
                        {
                            flag = 2;
                            D0_Switch_flag = 0;
                            fromAToB = 0;
                            A_to_B_sng_dx = 0;
                            RiskPercent = 0;
                        }
                    }
                        
                    if (D0_Switch_flag == 0 && flag == 2 && D1_RemovableCoins[0] != null)
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            fromAToB = D1_OppositeLegalTower[D1_Cnt_Twr] - (PlayerClass.Dice_number[0] + PlayerClass.Dice_number[1]);

                            if (fromAToB > -1 && BoardClass.Is_The_Delivery_Tower_Free(fromAToB) && D0_RemovableCoins[0] == null | Num_B_coin_indx_1 > 3 |
                                (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) == 0 && BoardClass.boardArray[fromAToB, 0].CompareTag("Black_coin")) |
                                (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) > 0 && BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) <= 3
                                 && BoardClass.boardArray[fromAToB, 0].CompareTag("Black_coin")) |
                                (BoardClass.boardArray[fromAToB, 0].CompareTag("White_coin") && BoardClass.boardArray[fromAToB, 1].CompareTag("blank")))
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                A_to_B_sng_dx = 1;
                                Tmp_of_sng_dx = D1_OppositeLegalTower[D1_Cnt_Twr];
                                break;
                            }
                        }

                    if (D0_RemovableCoins[0] != null && fromAToB > -1 && D1_Switch_flag == 1 && !BoardClass.boardArray[fromAToB, 0].CompareTag("Black_coin"))
                    {
                        if (fromAToB <= 6 && BoardClass.IndexOfCoinInSelectedTower(Tmp_of_sng_dx) > 0)
                            for (int i = 0; i < fromAToB; i++)
                                if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                    RiskPercent++;
                       
                        if ((PlayerClass.Container_Of_WC_burnt[0] != null && fromAToB <= 6) | RiskPercent > 0 ) // | can_move_from_home > 0
                        {
                            flag = 2;
                            D1_Switch_flag = 0;
                            fromAToB = 0;
                            A_to_B_sng_dx = 0;
                            RiskPercent = 0;
                        }
               
                    }

                    //if black were able to burns white coins in his way
                    if (D0_Switch_flag == 1)
                    {
                        if (D0_OppositeLegalTower[D0_Cnt_Twr] - PlayerClass.Dice_number[1] > -1 
                            && BoardClass.boardArray[D0_OppositeLegalTower[D0_Cnt_Twr] - PlayerClass.Dice_number[1],0].CompareTag("White_coin") 
                            && BoardClass.boardArray[D0_OppositeLegalTower[D0_Cnt_Twr] - PlayerClass.Dice_number[1], 1].CompareTag("blank"))
								
                            StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeLegalTower[D0_Cnt_Twr] - PlayerClass.Dice_number[1], "White"));
                        StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));
                    }

                    if (D1_Switch_flag == 1)
                    {
                        //if the way of checker were any white checker rbt going to burn the against checker
                        if (D1_OppositeLegalTower[D1_Cnt_Twr] - PlayerClass.Dice_number[0] > -1 
                            && BoardClass.boardArray[D1_OppositeLegalTower[D1_Cnt_Twr] -PlayerClass.Dice_number[0],0].CompareTag("White_coin") 
                            && BoardClass.boardArray[D1_OppositeLegalTower[D1_Cnt_Twr] -PlayerClass.Dice_number[0], 1].CompareTag("blank"))
								
                            StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeLegalTower[D1_Cnt_Twr] - PlayerClass.Dice_number[0],"White"));
                        StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                    }
                }   

                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    RiskPercent = 0;

                    if (!PlayerClass.DiceCheck[0])
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 2 && D0_OppositeLegalTower[D0_Cnt_Twr] > 0 |
                                (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0 && D0_OppositeLegalTower[D0_Cnt_Twr] > 17))
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                break;
                            }
                        }
                    }
                    
                    
                    // Tmp_of_sng_dx > 0 : means deliver tower should not be the zero tower
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                D0_Switch_flag = 0;
                   
                    if (!PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_RemovableCoins[0] != null)
                    {
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 2 && D1_OppositeLegalTower[D1_Cnt_Twr] > 0 |
                                (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0 && D1_OppositeLegalTower[D1_Cnt_Twr] > 17))
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                RiskPercent = 0;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                break;
                            }
                        }
                    }
                
                    
                    // Tmp_of_sng_dx > 0 : means deliver tower should not be the the zero tower
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                RiskPercent++;
             
                    if (PlayerClass.Container_Of_WC_burnt[0] != null | RiskPercent > 0 ) 
                    {
                        flag = 2;
                        D0_Switch_flag = 0;
                        D1_Switch_flag = 0;
                        RiskPercent = 0;
                    }
                    
                    if (D0_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));

                    if (D1_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                }

                //Chance	
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    RiskPercent = 0;

                    if (!PlayerClass.DiceCheck[0])
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 1 && D0_OppositeLegalTower[D0_Cnt_Twr] > 0)
                            {
                                flag = 0;
                                D0_Switch_flag = 1;

                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                Tmp_Selected_Twr_dx = D0_OppositeLegalTower[D0_Cnt_Twr];
                                break;
                            }
                        }
                    }
                    
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                D0_Switch_flag = 0;
                  
                    if (!PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_RemovableCoins[0] != null)
                    {
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 1)
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                Tmp_Selected_Twr_dx = D1_OppositeLegalTower[D1_Cnt_Twr];
                                break;
                            }
                        }
                    }
                    
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                RiskPercent++;

                    if ((PlayerClass.Container_Of_WC_burnt[0] != null && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin")) |
                        (PlayerClass.Container_Of_WC_burnt[0] != null && Tmp_Selected_Twr_dx < 6) | RiskPercent > 0)
                    {
                        flag = 2;
                        D0_Switch_flag = D1_Switch_flag = 0;
                    }


                    if (D0_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));

                    if (D1_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                }

                //Chance_2 == B
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    if (!PlayerClass.DiceCheck[0])
                    {
                        for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0 &&
                                BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], 0].CompareTag("Black_coin"))
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                Tmp_Selected_Twr_dx = D0_OppositeLegalTower[D0_Cnt_Twr];
                                break;
                            }
                        }
                    }
                    
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                D0_Switch_flag = 0;
                    
                    if (!PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_RemovableCoins[0] != null)
                    {
                        for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0 &&
                                BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], 0].CompareTag("Black_coin"))
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                Tmp_Selected_Twr_dx = D1_OppositeLegalTower[D1_Cnt_Twr];
                                break;
                            }
                        }
                    }


                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                RiskPercent++;
                
                    if (PlayerClass.Container_Of_WC_burnt[0] != null && Tmp_Selected_Twr_dx < 6 | RiskPercent > 0)
                    {
                        flag = 2;
                        D0_Switch_flag = D1_Switch_flag = RiskPercent = 0;
                    }
                }

                //Switch in > 3  situation.
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    if (!PlayerClass.DiceCheck[0])
                    {
                        for (D0_Cnt_Twr = fw0; D0_Cnt_Twr >= 0; D0_Cnt_Twr--)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 3) // if one of towers that contains more than 2 black coins
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                Tmp_of_sng_dx = D0_OppositeDeliverTower[D0_Cnt_Twr];
                                break;
                            }
                        }
                    }
                    
                    
                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                D0_Switch_flag = 0;

                    if (!PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_RemovableCoins[0] != null)
                    {
                        for (D1_Cnt_Twr = fw1; D1_Cnt_Twr >= 0; D1_Cnt_Twr--)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 3)
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                Tmp_of_sng_dx = D1_OppositeDeliverTower[D1_Cnt_Twr];
                                break;
                            }
                        }
                    }


                    if (Tmp_of_sng_dx > 0 && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin") && Tmp_of_sng_dx < 18)
                        for (int i = 0; i < Tmp_of_sng_dx; i++)
                            if (BoardClass.boardArray[i, 0].CompareTag("White_coin"))
                                RiskPercent++;
                    
                    if (PlayerClass.Container_Of_WC_burnt[0] != null && !BoardClass.boardArray[Tmp_of_sng_dx, 0].CompareTag("Black_coin")
                        | RiskPercent > 0 | (Alo_totalBCoinTwr5_0 >= 1 && Tmp_of_sng_dx < 6 && !PlayerClass.BlackBearOut))
                    {
                        flag = 2;
                        D0_Switch_flag = D1_Switch_flag = 0;
                    }


                    if (D0_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));

                    if (D1_Switch_flag == 1)
                        StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                }
                
                //last remain method
                if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                {
                    if (!PlayerClass.DiceCheck[0] && D0_RemovableCoins[0] != null)
                    {
                        for (D0_Cnt_Twr = fw0; D0_Cnt_Twr >= 0; D0_Cnt_Twr--)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0 && WCoin_5_0 == 0 && PlayerClass.Container_Of_WC_burnt[0] == null)
                            {
                                flag = 0;
                                D0_Switch_flag = 1;
                                break;
                            }
                        }

                        if (D0_Switch_flag == 0)
                            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                            {
                                if (BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]) >= 0)
                                {
                                    flag = 0;
                                    D0_Switch_flag = 1;
                                    break;
                                }
                            }


                        if (D0_Switch_flag == 1)
                            StartCoroutine(PlayerClass.BurnTheCoin(D0_OppositeDeliverTower[D0_Cnt_Twr], "White"));
                    }

                    if (!PlayerClass.DiceCheck[1] && D0_Switch_flag == 0 && D1_RemovableCoins[0] != null)
                    {
                        for (D1_Cnt_Twr = fw1; D1_Cnt_Twr >= 0; D1_Cnt_Twr--)
                        {
                            if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0 && WCoin_5_0 == 0 && PlayerClass.Container_Of_WC_burnt[0] == null)
                            {
                                flag = 0;
                                D1_Switch_flag = 1;
                                break;
                            }
                        }

                        if (D1_Switch_flag == 0)
                            for (D1_Cnt_Twr = 0; D1_Cnt_Twr <= fw1; D1_Cnt_Twr++)
                            {
                                if (BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]) >= 0 )
                                {
                                    flag = 0;
                                    D1_Switch_flag = 1;
                                    break;
                                }
                            }


                        if (D1_Switch_flag == 1)
                            StartCoroutine(PlayerClass.BurnTheCoin(D1_OppositeDeliverTower[D1_Cnt_Twr], "White"));
                    }


                    if (D0_Switch_flag == 0 && D1_Switch_flag == 0)
                        StartCoroutine(RollDice());
                    
                }

                yield return new WaitForSecondsRealtime(0.65f);
                if (flag == 0 && PlayerClass.B_win < 15)
                {
                    if (D0_Switch_flag == 1)
                    {
                        PlayerClass.SelectedCoin = D0_RemovableCoins[D0_Cnt_Twr].transform;
                        PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower(D0_OppositeLegalTower[D0_Cnt_Twr]);
                        PlayerClass.SelectedTowerNumber = D0_OppositeLegalTower[D0_Cnt_Twr];
                        PlayerClass.DeliveryTower = D0_OppositeDeliverTower[D0_Cnt_Twr];
                        PlayerClass.Tower_Number = PlayerClass.DeliveryTower;

                        PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;
                        DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();
                    }

                    if (D1_Switch_flag == 1)
                    {
                        PlayerClass.SelectedCoin = D1_RemovableCoins[D1_Cnt_Twr].transform;
                        PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower(D1_OppositeLegalTower[D1_Cnt_Twr]);
                        PlayerClass.SelectedTowerNumber = D1_OppositeLegalTower[D1_Cnt_Twr];
                        PlayerClass.DeliveryTower = D1_OppositeDeliverTower[D1_Cnt_Twr];
                        PlayerClass.Tower_Number = PlayerClass.DeliveryTower;

                        PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;
                        DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();

                    }


                    PlayerClass.xcoin_point = PlayerClass.SelectedCoin;
                    PlayerClass.Update_TargetTower(PlayerClass.Tower_Number);
                    PlayerClass.Final_Position_OF_Coin = PlayerClass.DeliveryTower * 15 + BoardClass.IndexOfCoinInSelectedTower(PlayerClass.DeliveryTower);
                    PlayerClass.Update_board(true, PlayerClass.SelectedCoin.gameObject);
                     DataManagerClass.coinImpactSnd.Play();


                    if (!PlayerClass.DiceCheck[0] && !PlayerClass.DiceCheck[1])
                    {
                        if (D0_Switch_flag == 1)
                            PlayerClass.DiceCheck[0] = true;

                        if (D1_Switch_flag == 1)
                            PlayerClass.DiceCheck[1] = true;

                    }
                    else
                        for (int diceFT = 0; diceFT <= 1; diceFT++)
                        {
                            if (!PlayerClass.DiceCheck[diceFT])
                            {
                                PlayerClass.DiceCheck[diceFT] = true;
                                break;
                            }
                        }

                    // A_To_B Method
                    if (A_to_B_sng_dx == 1) // using one of tmp_dx_single_flag for run the order.
                    {
                        StartCoroutine(PlayerClass.BurnTheCoin(fromAToB, "White"));

                        if (PlayerClass.pauseTime == 0f)
                            yield return new WaitForSecondsRealtime(0.65f);
                        else
                            yield return new WaitForSecondsRealtime(PlayerClass.pauseTime);

                        if (D0_Switch_flag == 1) // continually using previous checker for next move
                        {
                            PlayerClass.SelectedCoin = BoardClass.boardArray[D0_OppositeDeliverTower[D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower(D0_OppositeDeliverTower[D0_Cnt_Twr])].transform; 
                            PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower(D0_OppositeDeliverTower[D0_Cnt_Twr]);
                            PlayerClass.SelectedTowerNumber = D0_OppositeDeliverTower[D0_Cnt_Twr];
                            PlayerClass.DeliveryTower = fromAToB;
                            PlayerClass.Tower_Number = PlayerClass.DeliveryTower;
                                
                            PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;
                            DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();
                        }

                        if (D1_Switch_flag == 1)
                        {
                            PlayerClass.SelectedCoin = BoardClass.boardArray[D1_OppositeDeliverTower[D1_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower(D1_OppositeDeliverTower[D1_Cnt_Twr])].transform;
                            PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower(D1_OppositeDeliverTower[D1_Cnt_Twr]);
                            PlayerClass.SelectedTowerNumber = D1_OppositeDeliverTower[D1_Cnt_Twr];
                            PlayerClass.DeliveryTower = fromAToB;
                            PlayerClass.Tower_Number = PlayerClass.DeliveryTower;
                                
                            PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;
                            DataManagerClass.blackMoveCounterTxt.text = PlayerClass.black_coin_move_counter.ToString();
                        }


                        PlayerClass.xcoin_point = PlayerClass.SelectedCoin;
                        PlayerClass.Update_TargetTower(PlayerClass.Tower_Number);
                        PlayerClass.Final_Position_OF_Coin = PlayerClass.DeliveryTower * 15 + BoardClass.IndexOfCoinInSelectedTower(PlayerClass.DeliveryTower);
                        PlayerClass.Update_board(true, PlayerClass.SelectedCoin.gameObject);

                         DataManagerClass.coinImpactSnd.Play();
                            
                        if (D0_Switch_flag == 0)
                            PlayerClass.DiceCheck[0] = true;

                        if (D1_Switch_flag == 0)
                            PlayerClass.DiceCheck[1] = true;
                    }


                    yield return new WaitForSecondsRealtime(0.65f);
                    if (A_to_B_sng_dx == 0 && !PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
                    {
                        D0_Switch_flag = D1_Switch_flag = 0;

                        totalWCoinTwr23_18 = Alo_totalBCoinTwr5_0 = 0;

                        for (int i = 23; i >= 18; i--)
                        for (int j = 0; j <= 14; j++)
                            if (BoardClass.boardArray[i, j].CompareTag("White_coin"))
                                totalWCoinTwr23_18++;
                             

                        for (int i1 = 5; i1 >= 0; i1--)
                            if (BoardClass.boardArray[i1, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower(i1) == 0)
                                Alo_totalBCoinTwr5_0++;
 
                        CalculateCoinsPosition();
                    }
                }


                if (PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1])
                    StartCoroutine(RollDice());
                active_sgl_move = false;
            }

            #endregion
        }
        
    }

    #endregion 
    
    #region Checking Even Dice Values_Black
    private IEnumerator Checking_Even_Dice_Values_then_makes_move_Black () // dice is same in value
    {
        int Cnt_BCoin_Twr_5_0 = 0;
        int Cnt_WBCoin_Twr_5_0 = 0;
        int Cnt_Alone_BCoin_Twr_5_0 = 0;
        int Cnt_Coverd_BCoin_Twr_5_0 = 0;

        countdp = Switch_to_new_method = 0;
        
        #region Collect data from the board

        for (int i = 5; i >= 0; i--)
        {
            for (int j = 14; j >= 0; j--)
            {
                if (BoardClass.boardArray [i, j].CompareTag("Black_coin"))
                    Cnt_Alone_BCoin_Twr_5_0++;

                if ( BoardClass.boardArray [i, j].CompareTag("White_coin"))
                    Cnt_WBCoin_Twr_5_0++;
            }

            if (BoardClass.boardArray [i, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower (i) == 0)
                Cnt_Alone_BCoin_Twr_5_0++;

            if (BoardClass.boardArray [i, 0].CompareTag("Black_coin") && BoardClass.IndexOfCoinInSelectedTower (i) >= 1)
                Cnt_Coverd_BCoin_Twr_5_0++;
        }

        #endregion

        // burn complex.
        if (Switch_to_new_method == 0)
        { 
            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
            {
                // the values of Place_Double_D already Specified in count_all_forward_moves functions 
                if ((BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 0].CompareTag("White_coin") && 
                    BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 1].CompareTag("blank") && 
                    EvenLegalTowers_Index [D0_Cnt_Twr] > 1 && EvenDeliverTowers [D0_Cnt_Twr] >= 6)) 
                {
                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
                    countdp++;
                    break;
                }
            }

            if (countdp == 0 )
            {
                // runs when it could cover its single checker for making it safe from burning cause in previous move the checker became alone
                //when rbt could able burn the against checker and cover another black checker
                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 0].CompareTag("White_coin") && 
                        BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 1].CompareTag("blank") &&
                        EvenLegalTowers_Index[D0_Cnt_Twr] >= 0 && EvenDiceLegalTowers[D0_Cnt_Twr] >= PlayerClass.Dice_number[4])
                    {
                        Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                        D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                        D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
                        dice_pos_dbl = D0_OppositeDeliverTower [0];
                        break;
                    }
                }

                //checks if it can burn white coins and cover an alone black coin
                if (D1_RemovableCoins [0] != null && burn_flag_for_cover == 0 && D0_OppositeDeliverTower[0] > PlayerClass.Dice_number[5]) 
                    for (int i = D0_OppositeDeliverTower [0]; i >= 0 ; i--)
                    { 
                        if (i-PlayerClass.Dice_number[0] > -1 && BoardClass.boardArray [i-PlayerClass.Dice_number[0], 0].CompareTag("Black_coin"))
                        {
                            countdp = i;
                            burn_flag_for_cover = 1;
                            break;
                        }
                    }

                if (countdp == 0)
                    burn_flag_for_cover = 0;
            }

            //makes a position for covering alone coin 
            if (burn_flag_for_cover == 1 && countdp == 0 && !PlayerClass.DiceCheck [2])
            {
                if (EvenLegalTowers_Index [D0_Cnt_Twr] == 0 && BoardClass.boardArray [dice_pos_dbl, 0].CompareTag("Black_coin"))
                {
                    Double_Dp [0] = dice_pos_dbl;
                    D1_RemovableCoins [0] = BoardClass.boardArray [dice_pos_dbl, BoardClass.IndexOfCoinInSelectedTower (dice_pos_dbl)];
                    D0_OppositeDeliverTower [0] = dice_pos_dbl - PlayerClass.Dice_number [0];
                    countdp++;
                }

            }

            if (countdp == 0)
                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 0].CompareTag("White_coin") &&  
                        BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 1].CompareTag("blank") && EvenLegalTowers_Index[D0_Cnt_Twr] == 1)
                    {
                        Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                        D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                        D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
                        countdp++;
                        break;
                    }
                }

        }


        if(countdp == 0) // situation of bear out.
            Switch_to_new_method = 1;

        if (Switch_to_new_method == 1)
            for (D0_Cnt_Twr = fw0; D0_Cnt_Twr >= 0 ; D0_Cnt_Twr--)
            {
                if (EvenLegalTowers_Index [D0_Cnt_Twr] >= 0 && EvenDeliverTowers[D0_Cnt_Twr] == PlayerClass.Dice_number[0] - 1 && PlayerClass.BlackBearOut)
                {
                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
                    countdp++;
                    break;
                }

            }


        if(countdp == 0) //Near to win mode 
            Switch_to_new_method = 2;

        if (Switch_to_new_method == 2 && BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [0]) >= 0 && 
            !PlayerClass.BlackBearOut | PlayerClass.Black_Beardout_coins[0]  == null && 
            Cnt_WBCoin_Twr_5_0 == 0 | PlayerClass.White_beardout_Coin [2] != null && PlayerClass.Container_Of_WC_burnt[0] == null && 
            Cnt_Alone_BCoin_Twr_5_0 >= 12 | PlayerClass.White_beardout_Coin [0] != null) //if most of his coins were near to white_home:  transfers his coins to home of white 
        {
            Double_Dp [0] = EvenDiceLegalTowers [0];
            D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [0], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [0])];
            D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
			
            countdp++;

        }

        if(countdp == 0)// Surround in first action.
            Switch_to_new_method = 3;

        if (Switch_to_new_method == 3) 
            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0 ; D0_Cnt_Twr++)
            {
                if (EvenLegalTowers_Index [D0_Cnt_Twr] >= 1 && EvenDiceLegalTowers[D0_Cnt_Twr] >= 6 && Cnt_Coverd_BCoin_Twr_5_0 > 2  && PlayerClass.BlackBearOut)
                {
                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
				
                    countdp++;
                    break;
                }

            }


        if(countdp == 0)// covering (1) coins. very important method.
            Switch_to_new_method = 4;

        if (Switch_to_new_method == 4) // single bCoin covers another single bcoins!
        { 
            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
            {
                if (EvenDiceLegalTowers [D0_Cnt_Twr] <= 18 && EvenLegalTowers_Index [D0_Cnt_Twr] == 0 && 
                    BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], 0].CompareTag("Black_coin")
                    && BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 0].CompareTag("Black_coin"))
                {
                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [countdp] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
				
                    countdp++;

                    break;
                }

            }

            if (countdp == 0 && PlayerClass.Dice_number[0] != 1) // Cause tower_0 is safe for black
            {
                int aloneCoinForCover = 0;
            
                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (EvenDiceLegalTowers [D0_Cnt_Twr] > PlayerClass.Dice_number [0] && EvenLegalTowers_Index [D0_Cnt_Twr] == 0 && 
                        BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], 0].CompareTag("Black_coin") &&
                        EvenDiceLegalTowers [D0_Cnt_Twr] - PlayerClass.Dice_number [4] > 0 )
                    {
                        aloneCoinForCover = EvenDiceLegalTowers [D0_Cnt_Twr];
                        break;
                    }

                }

                if (aloneCoinForCover > 0)
                {
                    for (int i = double_dice_falses_index; i <= 3; i++)
                    {
                        //step by step rbt will checks to find out is that possible to cover the alone coin
                        if (aloneCoinForCover - PlayerClass.Dice_number [3 + i] < 23 && aloneCoinForCover - PlayerClass.Dice_number [3 + i] > -1 && 
                            BoardClass.boardArray [aloneCoinForCover - PlayerClass.Dice_number [3 + i], 0].CompareTag("Black_coin"))
                        {
                            countdp = aloneCoinForCover - PlayerClass.Dice_number [3 + i];
                            break;
                        }

                        if(countdp > 0 && aloneCoinForCover - PlayerClass.Dice_number [3 + i] < 23 && aloneCoinForCover - PlayerClass.Dice_number [3 + i] > 0 && 
                           BoardClass.boardArray [aloneCoinForCover - PlayerClass.Dice_number [3 + i], 1].CompareTag("White_coin"))
                        {
                            countdp = 0;
                            break;
                        }
                    }
                    
                    if (countdp > 0)
                    {
                        Double_Dp [0] = aloneCoinForCover;
                        D1_RemovableCoins [0] = BoardClass.boardArray [Double_Dp [0], BoardClass.IndexOfCoinInSelectedTower (Double_Dp [0])];
                        D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
                    }
                }
            }

        }

        if(countdp == 0)// covering(2) coins for target place in process very (important method).
            Switch_to_new_method = 5;

        if (Switch_to_new_method == 5) 
            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
            {
                if (BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 0].CompareTag("Black_coin") && 
                    BoardClass.boardArray [EvenDeliverTowers [D0_Cnt_Twr], 1].CompareTag("blank") && 
                    EvenLegalTowers_Index [D0_Cnt_Twr] >= 2 | (Cnt_Alone_BCoin_Twr_5_0 >= 1 | PlayerClass.Container_Of_WC_burnt[0] != null && EvenDeliverTowers[D0_Cnt_Twr] <= 5))
                {

                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
				
                    countdp++;
                    break;
                }

            }


        if(countdp == 0) // Surround home of white.
            Switch_to_new_method = 6;

        if (Switch_to_new_method == 6)
            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
            {
                if(!PlayerClass.DiceCheck[2] && PlayerClass.Container_Of_WC_burnt[0] != null &&
                   EvenDeliverTowers[D0_Cnt_Twr] <= 5 && EvenLegalTowers_Index [D0_Cnt_Twr] >= 2) // if rbt could to one more time cover its checkers from burning
                {
                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
				
                    countdp++;
                    break;
                }
            }

        if(countdp == 0) // Normal positions
            Switch_to_new_method = 7;

        if (Switch_to_new_method == 7)
        {
            int twr_indx_3 = 0;

            for (int i = 23; i >= 0; i--) 
                if (BoardClass.IndexOfCoinInSelectedTower (i) > 2)
                    twr_indx_3++;


            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
            {
                if (twr_indx_3 > 1 && EvenLegalTowers_Index [D0_Cnt_Twr] > 3)
                {
                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
			
                    countdp++;
                    break;
                }



            }

            if(countdp == 0)
                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (EvenLegalTowers_Index [D0_Cnt_Twr] > 2)
                    {
                        Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                        D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                        D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
				
                        countdp++;
                        break;

                    }


                }


            if(countdp == 0)
                for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
                {
                    if (EvenLegalTowers_Index [D0_Cnt_Twr] == 1 && !PlayerClass.DiceCheck [2] && EvenDeliverTowers [D0_Cnt_Twr] > 17 | EvenDeliverTowers [D0_Cnt_Twr] < 6)
                    {
                        Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                        D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                        D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
				
                        countdp++;
                        break;

                    }

                }

            if (PlayerClass.DiceCheck [2] && D0_OppositeDeliverTower[0] > 17 | EvenDeliverTowers [D0_Cnt_Twr] < 6 && D0_OppositeDeliverTower[0] > 0 && 
                !BoardClass.boardArray [D0_OppositeDeliverTower[0],0].CompareTag("Black_coin") )
            {
                for (int i = 0; i < D0_OppositeDeliverTower [0]; i++)
                {
                    if (BoardClass.boardArray [i, 0].CompareTag("White_coin"))

                    {
                        countdp = 0;
                        break;
                    }

                }

                if (PlayerClass.Container_Of_WC_burnt [0] != null)
                    countdp = 0;

            }

        }

        if(countdp == 0) //Last Choice positions
            Switch_to_new_method = 8;

        if (Switch_to_new_method == 8) 
            for (D0_Cnt_Twr = 0; D0_Cnt_Twr <= fw0; D0_Cnt_Twr++)
            {
                if (EvenLegalTowers_Index [D0_Cnt_Twr] >= 0 )
                {

                    Double_Dp [0] = EvenDiceLegalTowers [D0_Cnt_Twr];
                    D1_RemovableCoins [0] = BoardClass.boardArray [EvenDiceLegalTowers [D0_Cnt_Twr], BoardClass.IndexOfCoinInSelectedTower (EvenDiceLegalTowers [D0_Cnt_Twr])];
                    D0_OppositeDeliverTower [0] = Double_Dp [0] - PlayerClass.Dice_number [0];
			
                    countdp++;
                    break;
                }


            }

        if (countdp == 0)
            Switch_to_new_method = -1;
 
        if (Switch_to_new_method > -1) // process of moving of black coins
        {
            StartCoroutine (PlayerClass.BurnTheCoin (D0_OppositeDeliverTower [0], "White"));

            if(PlayerClass.pauseTime == 0f)
                yield return new WaitForSecondsRealtime (0.65f);
            else
                yield return new WaitForSecondsRealtime (PlayerClass.pauseTime);

            if(!PlayerClass.DiceCheck[3])
            {
                
                PlayerClass.SelectedCoin = D1_RemovableCoins [0].transform;
                PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower (Double_Dp [0]);
                PlayerClass.SelectedTowerNumber = Double_Dp [0];
                PlayerClass.DeliveryTower = D0_OppositeDeliverTower [0];
                PlayerClass.Tower_Number = PlayerClass.DeliveryTower;

                PlayerClass.xcoin_point = PlayerClass.SelectedCoin;

                PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber - PlayerClass.DeliveryTower;

                DataManagerClass.blackMoveCounterTxt.text  = PlayerClass.black_coin_move_counter.ToString();


                PlayerClass.Update_board (true, PlayerClass.SelectedCoin.gameObject);
               
                PlayerClass.Update_TargetTower(PlayerClass.Tower_Number);
                
                DataManagerClass.coinImpactSnd.Play();
                
                PlayerClass.Final_Position_OF_Coin = PlayerClass.DeliveryTower * 15 + BoardClass.IndexOfCoinInSelectedTower (PlayerClass.DeliveryTower);

                for (int dice_f_t = 0; dice_f_t <= 3; dice_f_t++)
                {
                    if(!PlayerClass.DiceCheck [dice_f_t])
                    {
                        PlayerClass.DiceCheck[dice_f_t] = true;
                        break;
                    }
                }

                yield return new WaitForSecondsRealtime(0.65f);
                if (!PlayerClass.DiceCheck [3])
                    CalculateCoinsPosition ();
                else
                    StartCoroutine (RollDice ());
              
            }
        }
    }

    #endregion

    #region Bear Out_Process
    private IEnumerator BearOut_Process() 
    {
      //  PlayerClass.xcoin_point = PlayerClass.xcoin_point;

        #region evenDice
           
            if(PlayerClass.even_dice)
            {
                yield return new WaitForSecondsRealtime (0.65f);
                PlayerClass.SelectedCoin = D0_Coin_volunteer_BO.transform;
                PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower (sgl_d0_bo);
                PlayerClass.SelectedTowerNumber = sgl_d0_bo;
                PlayerClass.xcoin_bear_out = PlayerClass.SelectedCoin;
                PlayerClass.Black_Beardout_coins [PlayerClass.B_win] = PlayerClass.SelectedCoin.gameObject;
                PlayerClass.Black_Beardout_coins [PlayerClass.B_win].GetComponent<Collider> ().enabled = false;
                StartCoroutine (PlayerClass.ToQueue());
                
                PlayerClass.Update_board(true,PlayerClass.SelectedCoin.gameObject);
                DataManagerClass.coinImpactSnd.Play();
                if (PlayerClass.player_number == 2)
                {
                    PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber + 1;
                    DataManagerClass.blackMoveCounterTxt.text  = PlayerClass.black_coin_move_counter.ToString();
  
                }
                else
                {
                    PlayerClass.white_coin_move_counter -= PlayerClass.SelectedTowerNumber + 1;
                    DataManagerClass.whiteMoveCounterTxt.text  = PlayerClass.white_coin_move_counter.ToString();

                }
                yield return new WaitForSecondsRealtime (0.65f);
                for (int i = 0; i < 15; i++)
                {
                    if (BoardClass.boardArray [PlayerClass.SelectedTowerNumber, i] == PlayerClass.SelectedCoin.gameObject)
                    {
                        BoardClass.boardArray [PlayerClass.SelectedTowerNumber, i] = null;
                        break;
                    }
                }

                flag = 2;

                for (int diceFT = 0; diceFT <= 3; diceFT++)
                {
                    if(!PlayerClass.DiceCheck [diceFT])
                    {
                        PlayerClass.DiceCheck[diceFT] = true;
                        break;
                    }
                }

                if(!PlayerClass.DiceCheck[3] && PlayerClass.Black_Beardout_coins[14] == null)
                    CalculateCoinsPosition ();
                else
                    StartCoroutine (RollDice ());
                
            }

            #endregion

        #region oppositeDice

            if(PlayerClass.opposite_dice)
            {
                if(PlayerClass.DiceCheck[1])
                    yield return new WaitForSecondsRealtime (0.65f);

                if (active_D1_move && D0_Coin_volunteer_BO != null)
                {

                    PlayerClass.SelectedCoin = D0_Coin_volunteer_BO.transform;
                    PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower (sgl_d0_bo);
                    PlayerClass.SelectedTowerNumber = sgl_d0_bo;
                    
                    PlayerClass.xcoin_bear_out = PlayerClass.SelectedCoin;
                    PlayerClass.Black_Beardout_coins [PlayerClass.B_win] = PlayerClass.SelectedCoin.gameObject;
                    PlayerClass.Black_Beardout_coins [PlayerClass.B_win].GetComponent<Collider> ().enabled = false;
                    PlayerClass.Update_board(true,PlayerClass.SelectedCoin.gameObject);
                    StartCoroutine (PlayerClass.ToQueue ());
                    
                    DataManagerClass.coinImpactSnd.Play();
                    PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber+1;
                    DataManagerClass.blackMoveCounterTxt.text  = PlayerClass.black_coin_move_counter.ToString();

                    yield return new WaitForSecondsRealtime (0.65f);
                    for (int i = 0; i < 15; i++)
                    {
                        if (BoardClass.boardArray [PlayerClass.SelectedTowerNumber, i] == PlayerClass.SelectedCoin.gameObject)
                        {
                            BoardClass.boardArray [PlayerClass.SelectedTowerNumber, i] = null;
                            break;
                        }
                    }
                    
                    flag = 2;
                    PlayerClass.DiceCheck [0] = true;
                }

                if (active_D2_move && D1_Coin_volunteer_BO != null)
                {
                    if(PlayerClass.DiceCheck[0])
                      yield return new WaitForSecondsRealtime (0.65f);

                    PlayerClass.SelectedCoin = D1_Coin_volunteer_BO.transform;
                    PlayerClass.Coin_Index = BoardClass.IndexOfCoinInSelectedTower (sgl_d1_bo);
                    PlayerClass.SelectedTowerNumber = sgl_d1_bo;
                    PlayerClass.xcoin_bear_out = PlayerClass.SelectedCoin;
                    PlayerClass.Black_Beardout_coins [PlayerClass.B_win] = PlayerClass.SelectedCoin.gameObject;
                    PlayerClass.Black_Beardout_coins [PlayerClass.B_win].GetComponent<Collider> ().enabled = false;

                    PlayerClass.Update_board(true,PlayerClass.SelectedCoin.gameObject);
                    StartCoroutine (PlayerClass.ToQueue ());
                    
                    DataManagerClass.coinImpactSnd.Play();

                    PlayerClass.black_coin_move_counter -= PlayerClass.SelectedTowerNumber+1;
                    DataManagerClass.blackMoveCounterTxt.text  = PlayerClass.black_coin_move_counter.ToString();

                    yield return new WaitForSecondsRealtime (0.65f);
                    for (int i = 0; i < 15; i++)
                    {
                        if (BoardClass.boardArray [PlayerClass.SelectedTowerNumber, i] == PlayerClass.SelectedCoin.gameObject)
                        {
                            BoardClass.boardArray [PlayerClass.SelectedTowerNumber, i] = null;
                            break;
                        }
                    }

                    PlayerClass.DiceCheck [1] = true;
                    flag = 2;

                    active_D2_move = false;
             
                }

                if(!PlayerClass.DiceCheck[0] | !PlayerClass.DiceCheck[1])
                    CalculateCoinsPosition();

                if(PlayerClass.DiceCheck[0] && PlayerClass.DiceCheck[1])
                    StartCoroutine (RollDice ());
              

            }

            #endregion
    }

    #endregion

    #region Roll Dice
    private IEnumerator RollDice ()
    {
 
        rbt_passed_dble_req = false;

        yield return new WaitForSecondsRealtime (0.5f);
        PlayerClass.removeDice = true;
	
        if (PlayerPrefs.GetInt("mode") == 1)
        {
            if (PlayerClass.player_number == 2 && PlayerClass.Black_Bot && PlayerClass.White_dbl_chk == 0 && PlayerPrefs.GetInt("dblcube") == 1 && PlayerClass.num_double != 64)
                PlayerClass.doubleCube.layer = 9;
        }

        if ((PlayerClass.opposite_dice && !PlayerClass.DiceCheck[0] |!PlayerClass.DiceCheck[1]) | PlayerClass.even_dice && !PlayerClass.DiceCheck[3])
            DataManagerClass.noMove.Play("Nomove");
        
        DiceRollClass.dice1RemoveDiceAlarm.SetActive(true);
        DiceRollClass.dice2RemoveDiceAlarm.SetActive(true);
        DiceRollClass.particleRemoveDice1Alarm.Play();
        DiceRollClass.particleRemoveDice2Alarm.Play();

    }

    #endregion

}