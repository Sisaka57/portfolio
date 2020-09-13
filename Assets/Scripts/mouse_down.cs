using UnityEngine;
using static Board;

public class mouse_down : MonoBehaviour
{
	void OnMouseDown()
	{
		Player.PlayerClass.Coin_index_label.text = ""; // cleans the label.text
	
		for (int deactiveTowers = 0; deactiveTowers < 26; deactiveTowers ++) // deactivates all towers 
			BoardClass.greenTowers[deactiveTowers].SetActive(false);
		
		BoardClass.CoinDefaultColor = true; // checkers no longer flash's
		
		for (int i = 0; i < 15; i++)
		{
			if (Player.PlayerClass.player_number == 1)
				BoardClass.WhiteCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.WhiteCoinDefultColor;
			else
				BoardClass.BlackCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.BlackCoinDefultColor;
		}
	}
}
