using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CustomPlayerEffects;
using Mirror;
using Security;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public partial class PlyMovementSync : NetworkBehaviour
{
	private Camera079 _lastCamera;
	private Vector2 _lastCameraRot;
    // Fix for the horribly badly designed AFK kicker :shrug:
	private void FixedUpdate()
	{
		if (this.AFKTime > 0f && this._isAFK)
		{
			if (this._hub.characterClassManager.CurClass == RoleType.Scp079)
			{
				if (this._hub.characterClassManager.AliveTime > 13f)
				{
					Vector2 cameraNow = new Vector2(this._hub.scp079PlayerScript.currentCamera.curPitch, this._hub.scp079PlayerScript.currentCamera.curRot);
					if (cameraNow != _lastCameraRot && _lastCamera != this._hub.scp079PlayerScript.currentCamera)
					{
						this.IsAFK = false;
						_lastCameraRot = cameraNow;
						_lastCamera = this._hub.scp079PlayerScript.currentCamera;
						return;
					}
					this._timeAFK += Time.deltaTime;
					if (this._timeAFK >= 90f)
					{
                        // Sorry, this is in spanish.
						ServerConsole.Disconnect(base.connectionToClient, "Has sido echado por inactividad (estar en la misma cámara de 079, sin girar ni cambiar la cámara).");
						return;
					}
				}
			}
			else if (this._hub.characterClassManager.CurClass != RoleType.Spectator)
			{
				Vector3 realPosition2 = this.GetRealPosition();
				if (Mathf.Abs(realPosition2.x - this._AFKLastPosition.x) < 0.11f && Mathf.Abs(realPosition2.y - this._AFKLastPosition.y) < 0.11f && Mathf.Abs(realPosition2.z - this._AFKLastPosition.z) < 0.11f)
				{
					this._timeAFK += Time.deltaTime;
					if (this._timeAFK >= this.AFKTime)
					{
						ServerConsole.Disconnect(base.connectionToClient, "Has sido echado por inactividad.");
						return;
					}
				}
				else
				{
					this.IsAFK = false;
				}
			}
		}
	}
}