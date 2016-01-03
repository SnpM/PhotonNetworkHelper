﻿using UnityEngine;
using System.Collections;
using Photon;
using System.Linq;

namespace Lockstep.NetworkHelpers
{
    //TODO: Finish PhotonNetworkHelper
    public class PhotonNetworkHelper : NetworkHelper
    {
        public override bool IsConnected
        {
            get
            {
                return PhotonNetwork.connected && PhotonNetwork.masterClient != null;
            }
        }

        public override int ID
        {
            get
            {
                return PhotonNetwork.player.ID;
            }
        }

        private bool _isServer;

        public override bool IsServer
        {
            get
            {
                return _isServer;
            }
        }

        public override int PlayerCount
        {
            get
            {
                return PhotonNetwork.playerList.Length;
            }
        }

        void Awake()
        {
            PhotonNetwork.OnEventCall += HandleData;
        }

        void HandleData(byte eventCode, object content, int id)
        {
            base.Receive((MessageType)eventCode, (byte[])content);
        }

        const string Version = "1";

        public override void Host(int roomSize)
        {
            PhotonNetwork.ConnectUsingSettings(Version);
            _isServer = true;
        }

        public override void Connect(string ip)
        {
            PhotonNetwork.ConnectUsingSettings(Version);
            _isServer = false;
        }

        void OnJoinedLobby()
        {

            RoomOptions roomOptions = new RoomOptions();
            TypedLobby typedLobby = new TypedLobby();
            PhotonNetwork.JoinOrCreateRoom("TestRoom", roomOptions, typedLobby);
        }

        void OnJoinedRoom()
        {
            Debug.Log("On Joined Room");
            if (this._isServer)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.player);
            }
        }

        void OnPhotonJoinedRoomFailed()
        {
            Debug.Log("Failed Joined Room");
        }

        public override void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void SendMessageToAll(MessageType messageType, byte[] data)
        {
            
            RaiseEventOptions options = new RaiseEventOptions();
            PhotonPlayer[] players = PhotonNetwork.playerList;
            options.TargetActors = new int[players.Length - 1];
            int i = 0;
            foreach (PhotonPlayer player in players)
            {
                if (player.isMasterClient == false)
                {
                    options.TargetActors [i++] = player.ID;
                }
            }
            PhotonNetwork.RaiseEvent((byte)messageType, data, true, options);
            this.Receive(messageType, data);

        }

        public override void SendMessageToServer(MessageType messageType, byte[] data)
        {
            if (this.IsServer)
            {
                this.Receive(messageType,data);
            } else
            {
                RaiseEventOptions options = new RaiseEventOptions();
                options.TargetActors = new int[]{ PhotonNetwork.masterClient.ID };
                PhotonNetwork.RaiseEvent((byte)messageType, data, true, options);
            }
        }

        void OnGUI()
        {
            GUILayout.Space(200f);
            GUILayout.Label(PhotonNetwork.connectionState.ToString());
            GUILayout.Label(PhotonNetwork.GetPing().ToString());
        }
    }
}
