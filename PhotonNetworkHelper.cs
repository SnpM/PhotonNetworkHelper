using UnityEngine;
using System.Collections;
using Photon;
using System.Linq;

namespace Lockstep.NetworkHelpers
{
    //TODO: Finish PhotonNetworkHelper
    public class PhotonNetworkHelper : NetworkHelper
    {
        [SerializeField]
        private bool _directMessages = true;
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

        const string Version = "1.1";

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
            PhotonNetwork.JoinOrCreateRoom("Test", roomOptions, typedLobby);
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

        bool reliable = false;
        protected override void OnSendMessageToAll(MessageType messageType, byte[] data)
        {
            
            RaiseEventOptions options = RaiseEventOptions.Default;
            if (_directMessages)
            {
                options.Receivers = ExitGames.Client.Photon.ReceiverGroup.Others;
                this.Receive(messageType, data);
            }
            else {
                options.Receivers = ExitGames.Client.Photon.ReceiverGroup.All;
            }
            PhotonNetwork.RaiseEvent((byte)messageType, data, reliable, options);

        }

        protected override void OnSendMessageToServer(MessageType messageType, byte[] data)
        {
            if (this.IsServer && this._directMessages)
            {
                this.Receive(messageType,data);
            } else
            {
                RaiseEventOptions options = RaiseEventOptions.Default;
                options.Receivers = ExitGames.Client.Photon.ReceiverGroup.MasterClient;
                PhotonNetwork.RaiseEvent((byte)messageType, data, reliable, options);
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
