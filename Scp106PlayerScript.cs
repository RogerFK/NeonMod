using System;
using System.Collections.Generic;
using Mirror;
using Security;
using UnityEngine;

// Modify the functions in the order they're here with your decompiler of choice (I use dnSpy). Add the functions that are missing until no errors pop up, then recompile.

// Token: 0x020002E4 RID: 740
public partial class Scp106PlayerScript : NetworkBehaviour
{
    private static float stalky106LastTick;
    private static float disableFor;
    private static float stalkyCd;
    private static string[] parser;

	// Token: 0x060011D2 RID: 4562 RVA: 0x000688B8 File Offset: 0x00066AB8
	static Scp106PlayerScript()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(global::Scp106PlayerScript), "CmdMakePortal", new NetworkBehaviour.CmdDelegate(global::Scp106PlayerScript.InvokeCmdCmdMakePortal));
		NetworkBehaviour.RegisterCommandDelegate(typeof(global::Scp106PlayerScript), "CmdUsePortal", new NetworkBehaviour.CmdDelegate(global::Scp106PlayerScript.InvokeCmdCmdUsePortal));
		NetworkBehaviour.RegisterCommandDelegate(typeof(global::Scp106PlayerScript), "CmdMovePlayer", new NetworkBehaviour.CmdDelegate(global::Scp106PlayerScript.InvokeCmdCmdMovePlayer));
		NetworkBehaviour.RegisterRpcDelegate(typeof(global::Scp106PlayerScript), "RpcContainAnimation", new NetworkBehaviour.CmdDelegate(global::Scp106PlayerScript.InvokeRpcRpcContainAnimation));
		NetworkBehaviour.RegisterRpcDelegate(typeof(global::Scp106PlayerScript), "RpcTeleportAnimation", new NetworkBehaviour.CmdDelegate(global::Scp106PlayerScript.InvokeRpcRpcTeleportAnimation));
		Scp106PlayerScript.parser = new string[]
		{
			"<color=#F00>SCP-173</color>",
			"<color=#FF8E00>Class D</color>",
			"spec",
			"<color=#F00>SCP-106</color>",
			"<color=#0096FF>NTF Scientist</color>",
			"<color=#F00>SCP-049</color>",
			"<color=#FFFF7CFF>Scientist</color>",
			"pc",
			"<color=#008f1e>Chaos Insurgent</color>",
			"<color=#f00>SCP-096</color>",
			"<color=#f00>Zombie</color>",
			"<color=#0096FF>NTF Lieutenant</color>",
			"<color=#0096FF>NTF Commander</color>",
			"<color=#0096FF>NTF Cadet</color>",
			"Tutorial",
			"<color=#59636f>Facility Guard</color>",
			"<color=#f00>SCP-939-53</color>",
			"<color=#f00>SCP-939-89</color>"
		};
		global::Scp106PlayerScript.stalky106LastTick = 0f;
		global::Scp106PlayerScript.stalkyCd = Time.time + 120f;
		global::Scp106PlayerScript.disableFor = 0f;
	}

	// Token: 0x060011AC RID: 4524
	public void Init(RoleType classID, Role c)
	{
		if (!this.iAm106 && classID == RoleType.Scp106)
		{
			NetworkConnection connectionToClient = base.connectionToClient;
			Broadcast component = PlayerManager.localPlayer.GetComponent<Broadcast>();
			component.TargetClearElements(connectionToClient);
			component.TargetAddElement(connectionToClient, "<size=80><color=#0020ed><b>Stalk</b></color></size>" + Environment.NewLine + "In this server, you can <color=#0020ed><b>stalk</b></color> humans by double-clicking the portal creation button in the <b>[TAB]</b> menu.", 12U, false);
		}
		this.iAm106 = (classID == RoleType.Scp106);
		this.sameClass = (c.team == Team.SCP);
		if (Scp106PlayerScript.stalkyCd >= Time.time + 120f)
		{
			Scp106PlayerScript.stalkyCd = Time.time + 120f;
		}
	}
	// Token: 0x06001225 RID: 4645 RVA: 0x0006A720 File Offset: 0x00068920
	public void CallCmdMakePortal()
	{
		if (!this._interactRateLimit.CanExecute(true))
		{
			return;
		}
		if (!base.GetComponent<FallDamage>().isGrounded)
		{
			return;
		}
		Debug.DrawRay(base.transform.position, -base.transform.up, Color.red, 10f);
		RaycastHit raycastHit;
		if (this.iAm106 && !this.goingViaThePortal && Physics.Raycast(new Ray(base.transform.position, -base.transform.up), out raycastHit, 10f, this.teleportPlacementMask))
		{
			if (!Scp106PlayerScript.neonStalky106)
			{
				this.SetPortalPosition(raycastHit.point - Vector3.up);
				return;
			}
			if (Scp106PlayerScript.disableFor > Time.time)
			{
				return;
			}
			float num = Time.time - Scp106PlayerScript.stalky106LastTick;
			float num2 = Scp106PlayerScript.stalkyCd - Time.time;
			Broadcast component = PlayerManager.localPlayer.GetComponent<Broadcast>();
			if (num > 6f)
			{
				Scp106PlayerScript.stalky106LastTick = Time.time;
				if (num2 < 0f)
				{
					component.TargetClearElements(base.connectionToClient);
					component.TargetAddElement(base.connectionToClient, Environment.NewLine + "Press the portal creation button again to <color=#ff0955><b>Stalk</b></color> a random player.", 6U, false);
				}
				this.SetPortalPosition(raycastHit.point - Vector3.up);
				return;
			}
			component.TargetClearElements(base.connectionToClient);
			if (num2 > 0f)
			{
				Scp106PlayerScript.stalky106LastTick = Time.time;
				int num3 = 0;
				while (num3 < 5 && num2 > (float)num3)
				{
					component.TargetAddElement(base.connectionToClient, Environment.NewLine + "You have to wait $time seconds to use <color=#0020ed><b>Stalk</b></color>.".Replace("$time", (num2 - (float)num3).ToString("00")), 1U, false);
					num3++;
				}
				Scp106PlayerScript.disableFor = Time.time + (float)num3 + 1f;
				return;
			}
			Scp106PlayerScript.disableFor = Time.time + 4f;
			Timing.RunCoroutine(this.StalkCoroutine(component), 0);
		}
	}
	// Token: 0x0600122F RID: 4655
	private IEnumerator<float> StalkCoroutine(Broadcast bc)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in PlayerManager.players)
		{
			if (gameObject.GetComponent<CharacterClassManager>().CurClass != RoleType.ChaosInsurgency && gameObject.GetComponent<CharacterClassManager>().CurClass != RoleType.Spectator && gameObject.GetComponent<CharacterClassManager>().CurClass != RoleType.Tutorial && gameObject.GetComponent<CharacterClassManager>().IsHuman())
			{
				list.Add(gameObject);
			}
		}
		if (list.Count < 1)
		{
			bc.TargetAddElement(base.connectionToClient, "No valid player found.", 4U, false);
			yield break;
		}
		Scp106PlayerScript.stalky106LastTick = Time.time;
		GameObject gameObject2;
		RaycastHit raycastHit;
		do
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			gameObject2 = list[index];
			Physics.Raycast(new Ray(gameObject2.transform.position, -Vector3.up), out raycastHit, 10f, this.teleportPlacementMask);
			if (Vector3.Distance(gameObject2.transform.position, new Vector3(0f, -1998f, 0f)) < 40f)
			{
				gameObject2 = null;
				raycastHit.point = Vector3.zero;
			}
			list.RemoveAt(index);
		}
		while (raycastHit.point.Equals(Vector3.zero) && list.Count > 0);
		if (gameObject2 == null)
		{
			bc.TargetAddElement(base.connectionToClient, "No valid player found.", 4U, false);
			yield break;
		}
		if (raycastHit.point.Equals(Vector3.zero))
		{
			bc.TargetAddElement(base.connectionToClient, "An error has ocurred. Try it again in a few seconds.", 4U, false);
			yield break;
		}
		this.MovePortal(raycastHit.point - Vector3.up);
		Scp106PlayerScript.stalkyCd = Time.time + 60f;
		Timing.RunCoroutine(Scp106PlayerScript.StalkyCooldownAnnounce(60f), 1);
		Scp106PlayerScript.stalky106LastTick = Time.time;
		Scp106PlayerScript.disableFor = Time.time + 10f;
		string data = string.Concat(new string[]
		{
			Environment.NewLine,
			"<i>You will <color=#0020ed><b>stalk</b></color><b>",
			gameObject2.GetComponent<NicknameSync>().MyNick,
			"</b>, who is a ",
			Scp106PlayerScript.parser[(int)gameObject2.GetComponent<CharacterClassManager>().CurClass],
			"</i>\n<size=30><color=#FFFFFF66>Cooldown: 60</color></size>"
		});
		bc.TargetAddElement(base.connectionToClient, data, 5U, false);
		yield break;
	}

	private void MovePortal(Vector3 pos)
	{
		Timing.RunCoroutine(this.PortalProcedure(pos), Segment.FixedUpdate);
	}

	private IEnumerator<float> PortalProcedure(Vector3 pos)
	{
		yield return 0f;
		global::Scp106PlayerScript component = global::PlayerManager.localPlayer.GetComponent<global::Scp106PlayerScript>();
		component.NetworkportalPosition = pos;
		Animator anim = component.portalPrefab.GetComponent<Animator>();
		anim.SetBool("activated", false);
		component.portalPrefab.transform.position = pos;
		Timing.RunCoroutine(this.ForceTeleportLarry(), 1);
		yield return Timing.WaitForSeconds(1f);
		anim.SetBool("activated", true);
		yield break;
	}
	private IEnumerator<float> ForceTeleportLarry()
	{
		yield return Timing.WaitForSeconds(0.1f);
		do
		{
			this.CallCmdUsePortal();
			yield return 0f;
		}
		while (!this.goingViaThePortal);
		yield break;
	}
	public static IEnumerator<float> StalkyCooldownAnnounce(float duration)
	{
		yield return Timing.WaitForSeconds(duration);
		Broadcast component = PlayerManager.localPlayer.GetComponent<Broadcast>();
		using (List<GameObject>.Enumerator enumerator = PlayerManager.players.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameObject gameObject = enumerator.Current;
				if (gameObject.GetComponent<CharacterClassManager>().CurClass == RoleType.Scp106)
				{
					component.TargetAddElement(gameObject.GetComponent<NetworkIdentity>().connectionToClient, Environment.NewLine + "<b><color=#0020ed><b>Stalk</b></color> <color=#00e861>ready</color></b>." + Environment.NewLine + "<size=30>Press the portal creation button twice to use it.</size>", 5U, false);
				}
			}
			yield break;
		}
		yield break;
	}
	// this must be called on round start btw
	public static void InitializeStalky106()
	{
		global::Scp106PlayerScript.stalky106LastTick = 0f;
		global::Scp106PlayerScript.stalkyCd = Time.time + 80f;
		global::Scp106PlayerScript.disableFor = Time.time + 12f;
		MEC.Timing.RunCoroutine(Scp106PlayerScript.StalkyCooldownAnnounce(80f), 1);	
	}
}
