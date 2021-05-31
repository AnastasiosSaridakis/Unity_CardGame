using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayer : MonoBehaviour
{
   [SerializeField] private TMP_Text text;

   private PlayerManager player;

   public void SetPlayer(PlayerManager player)
   {
      this.player = player;
      text.text = "Player" + player.playerIndex.ToString();
   }
}
