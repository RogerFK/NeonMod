using System;
using System.Collections.Generic;
using RemoteAdmin;
using UnityEngine;
using GameCore;

// You can just copy and paste this. This is just a newer version of replace disconnecteds. I sure hope they use this function in Smod2 although it shouldn't matter
public partial class ReferenceHub : MonoBehaviour
{
	// Token: 0x06000B4A RID: 2890 RVA: 0x0004C780 File Offset: 0x0004A980
	private void OnDestroy()
	{
		ReferenceHub.Hubs.Remove(base.gameObject);
		RoleType curClass = base.gameObject.GetComponent<CharacterClassManager>().CurClass;
		if (curClass == RoleType.Spectator)
		{
			return;
		}
		if (!ConfigFile.ServerConfig.GetBool("neon_replacedisconnecteds", true))
		{
			return;
		}
		string @string = ConfigFile.ServerConfig.GetString("neon_replace_message", "You've replaced a player that disconnected.");
		Inventory component = base.GetComponent<Inventory>();
		Vector3 realModelPosition = base.GetComponent<PlyMovementSync>().RealModelPosition;
		foreach (ReferenceHub referenceHub in ReferenceHub.Hubs.Values)
		{
			if (!(PlayerManager.localPlayer == referenceHub.gameObject) && referenceHub.characterClassManager.CurClass == RoleType.Spectator)
			{
				referenceHub.characterClassManager.SetPlayersClass(curClass, referenceHub.gameObject, true, false);
				if (curClass == RoleType.Scp079)
				{
					this.Copy079(referenceHub.scp079PlayerScript, base.GetComponent<Scp079PlayerScript>());
				}
				Timing.RunCoroutine(this.ReallyFunCoroutine(realModelPosition, referenceHub.gameObject), 1);
				foreach (Inventory.SyncItemInfo syncItemInfo in component.items)
				{
					referenceHub.inventory.AddNewItem(syncItemInfo.id, syncItemInfo.durability, syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
				}
				PlayerManager.localPlayer.GetComponent<Broadcast>().TargetAddElement(referenceHub.characterClassManager.connectionToClient, @string, 7U, false);
				break;
			}
		}
	}
	
	private IEnumerator<float> ReallyFunCoroutine(Vector3 pos, GameObject go)
	{
		yield return Timing.WaitForSeconds(0.3f);
		go.GetComponent<PlyMovementSync>().OverridePosition(pos, 0f, false);
		yield break;
	}
	private void Copy079(Scp079PlayerScript dst, Scp079PlayerScript src) {
		dst.currentCamera = src.currentCamera;
		dst.currentRoom = src.currentRoom;
		dst.currentZone = src.currentZone;
		dst.lockedDoors = src.lockedDoors;
		dst.maxMana = src.maxMana;
		dst.nearbyInteractables = src.nearbyInteractables;
		dst.nearestCameras = src.nearestCameras;
		dst.NetworkcurExp = src.NetworkcurExp;
		dst.NetworkcurLvl = src.NetworkcurLvl;
		dst.NetworkcurMana = src.NetworkcurMana;
		dst.NetworkcurSpeaker = src.NetworkcurSpeaker;
		dst.NetworkmaxMana = src.NetworkmaxMana;
	}
}
