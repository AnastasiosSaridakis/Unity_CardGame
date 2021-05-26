using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour
{
   [SerializeField] private NetworkManager networkManager;
   [SerializeField] private string ServerAddress;

   public void Start()
   {
      if(!Application.isBatchMode)
      {
         Debug.Log("=-=-=-=Client Starting=-=-=-=");
         //networkManager.StartClient(); //THOUGHT: What if the player hasn't selected a deck yet.
      }
      else
      {
         Debug.Log("=-=-=-=Server Starting=-=-=-=");
      }
   }

   public void JoinLocal()
   {
      networkManager.networkAddress = ServerAddress;
      networkManager.StartClient();
   }
}
