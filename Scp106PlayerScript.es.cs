using System;
using System.Collections.Generic;
using Mirror;
using Security;
using UnityEngine;

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
		global::Scp106PlayerScript.parser = new string[]
		{
			"<color=#F00>SCP-173</color>",
			"<color=#FF8E00>Clase D</color>",
			"spec",
			"<color=#F00>SCP-106</color>",
			"<color=#0096FF>Científico NTF</color>",
			"<color=#F00>SCP-049</color>",
			"<color=#FFFF7CFF>Científico</color>",
			"pc",
			"<color=#008f1e>Insurgente del Caos</color>",
			"<color=#f00>SCP-096</color>",
			"<color=#f00>Zombie</color>",
			"<color=#0096FF>Teniente NTF</color>",
			"<color=#0096FF>Comandante NTF</color>",
			"<color=#0096FF>Cadete NTF</color>",
			"Tutorial",
			"<color=#59636f>Guardia MTF</color>",
			"<color=#f00>SCP-939-53</color>",
			"<color=#f00>SCP-939-89</color>"
		};
		global::Scp106PlayerScript.stalky106LastTick = 0f;
		global::Scp106PlayerScript.stalkyCd = Time.time + 120f;
		global::Scp106PlayerScript.disableFor = 0f;
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x0006A740 File Offset: 0x00068940
	public void Init(RoleType classID, Role c)
	{
		if (!this.iAm106 && classID == RoleType.Scp106)
		{
			MEC.Timing.RunCoroutine(FuckStupidClassPicker(), 1);
		}
		this.iAm106 = (classID == RoleType.Scp106);
		this.sameClass = (c.team == Team.SCP);
		if (Scp106PlayerScript.stalkyCd >= Time.time + 120f)
		{
			Scp106PlayerScript.stalkyCd = Time.time + 120f;
		}
	}
	private IEnumerator<float> FuckStupidClassPicker() {
		yield return MEC.Timing.WaitForSeconds(0.3f);
		if (!this.iAm106) yield break;
		
			NetworkConnection connectionToClient = base.connectionToClient;
			Broadcast component = PlayerManager.localPlayer.GetComponent<Broadcast>();
			component.TargetClearElements(connectionToClient);
			component.TargetAddElement(connectionToClient, "<size=80><color=#0020ed><b>Stalk</b></color></size>" + Environment.NewLine + "En este server, puedes <color=#0020ed><b>stalkear</b></color> humanos haciendo doble click al botón de crear portal en el menú de la tecla <b>[TAB]</b>.", 12U, false);
	}
    // Token: 0x060011CB RID: 4555 RVA: 0x00068210 File Offset: 0x00066410
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
		//only thing that's modified, below here
            if (Scp106PlayerScript.disableFor > Time.time) return;

            float timeDifference = Time.time - Scp106PlayerScript.stalky106LastTick;
            float cdAux = Scp106PlayerScript.stalkyCd - Time.time;
            Broadcast bc = PlayerManager.localPlayer.GetComponent<Broadcast>();
            if (timeDifference > 6f)
            {
                Scp106PlayerScript.stalky106LastTick = Time.time;
                if (cdAux < 0)
                {
                    bc.TargetClearElements(base.connectionToClient);
                    bc.TargetAddElement(base.connectionToClient, Environment.NewLine + "Vuelve a pulsar el botón de crear un portal para <color=#ff0955><b>Stalkear</b></color> a un humano.", 6u, false);
                }
                this.SetPortalPosition(raycastHit.point - Vector3.up);
                return;
            }
            else
            {
                bc.TargetClearElements(base.connectionToClient);
                if (cdAux > 0)
                {
                    Scp106PlayerScript.stalky106LastTick = Time.time;
                    int i = 0;
                    for (; i < 5 && cdAux > i; i++) bc.TargetAddElement(base.connectionToClient, Environment.NewLine + "Tienes que esperar $time segundos para usar <color=#0020ed><b>Stalk</b></color>.".Replace("$time", (cdAux - i).ToString("00")), 1u, false);
                    Scp106PlayerScript.disableFor = Time.time + i + 1;
                    return;
                }
                Scp106PlayerScript.disableFor = Time.time + 4;
                MEC.Timing.RunCoroutine(StalkCoroutine(bc), 0);
            }
        }
    }
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
			bc.TargetAddElement(base.connectionToClient, "Ningún jugador válido encontrado.", 4u, false);
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
			bc.TargetAddElement(base.connectionToClient, "Ningún jugador válido encontrado.", 4u, false);
			yield break;
		}
		if (raycastHit.point.Equals(Vector3.zero))
		{
			bc.TargetAddElement(base.connectionToClient, "Ha ocurrido un error. Prueba a lanzar el comando de nuevo.", 4u, false);
			yield break;
		}
		this.MovePortal(raycastHit.point - Vector3.up);
		Scp106PlayerScript.stalkyCd = Time.time + 60f;
		MEC.Timing.RunCoroutine(Scp106PlayerScript.StalkyCooldownAnnounce(60f), 1);
		Scp106PlayerScript.stalky106LastTick = Time.time;
		Scp106PlayerScript.disableFor = Time.time + 10f;
		string data = string.Concat(new string[]
		{
			Environment.NewLine,
			"<i>Vas a <color=#0020ed><b>stalkear</b></color> a <b>",
			gameObject2.GetComponent<NicknameSync>().MyNick,
			"</b>, que es ",
			parser[(int)gameObject2.GetComponent<CharacterClassManager>().CurClass],
			"</i>\n<size=30><color=#FFFFFF66>Cooldown: 6</color></size>"
		});
		bc.TargetAddElement(base.connectionToClient, data, 5u, false);
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
		global::Broadcast component = global::PlayerManager.localPlayer.GetComponent<global::Broadcast>();
		using (List<GameObject>.Enumerator enumerator = global::PlayerManager.players.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameObject gameObject = enumerator.Current;
				if (gameObject.GetComponent<global::CharacterClassManager>().CurClass == global::RoleType.Scp106)
				{
					component.TargetAddElement(gameObject.GetComponent<NetworkIdentity>().connectionToClient, Environment.NewLine + "<b><color=#0020ed><b>Stalk</b></color> <color=#00e861>preparado</color></b>." + Environment.NewLine + "<size=30>Haz doble click al botón de crear un portal para usarlo.</size>", 5U, false);
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
