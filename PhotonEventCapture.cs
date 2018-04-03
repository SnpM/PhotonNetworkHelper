using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep;
using Photon;

namespace Lockstep.NetworkHelpers {
	public class PhotonEventCapture : Photon.PunBehaviour {
		public PhotonNetworkHelper Target;
		void Start () {
			if (Target == null)
			Target = GetComponent <PhotonNetworkHelper> ();
		}
		public override void OnConnectedToMaster ()
		{
			Target.OnJoinedLobby ();
		}
		public override void OnJoinedRoom ()
		{
			Target.OnJoinedRoom ();
		}
		public override void OnPhotonJoinRoomFailed (object[] codeAndMsg)
		{
			Target.OnPhotonJoinedRoomFailed ();
		}
	}
}