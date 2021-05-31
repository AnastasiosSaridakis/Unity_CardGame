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

   public void JoinServer()
   {
      if(networkManager.networkAddress.Equals("")){
         Debug.Log("Connecting to localhost");
         networkManager.networkAddress = "localhost";
         networkManager.StartClient();
      }
      else
      {
         Debug.Log($"Connecting to {networkManager.networkAddress}");
         networkManager.StartClient();
      }
         
   }
}
