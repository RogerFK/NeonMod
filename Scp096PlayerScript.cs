	// Change the functions one by one
	private void ProcessLooking()
	{
		if (this._processLookingQueue.IsEmpty<GameObject>())
		{
			foreach (GameObject item in PlayerManager.players)
			{
				this._processLookingQueue.Enqueue(item);
			}
			return;
		}
		GameObject gameObject = this._processLookingQueue.Dequeue();
		if (gameObject == null || !ReferenceHub.GetHub(gameObject).characterClassManager.IsHuman() || gameObject.GetComponent<FlashEffect>().blinded)
		{
			return;
		}
		// This is what pisses Reddking off
		Transform transform = gameObject.GetComponent<Scp096PlayerScript>().camera.transform;
		float num = this.lookingTolerance.Evaluate(Vector3.Distance(transform.position, this.camera.transform.position));
		RaycastHit raycastHit;
		if ((num < 0.75f || Vector3.Dot(transform.forward, (transform.position - this.camera.transform.position).normalized) < -num) && Physics.Raycast(transform.transform.position, (this.camera.transform.position - transform.position).normalized, out raycastHit, 20f, this.layerMask) && raycastHit.collider.gameObject.layer == 24 && raycastHit.collider.GetComponentInParent<Scp096PlayerScript>() == this)
		{
			if (Scp096PlayerScript.neonScp096Rework)
			{
				if (!this.visiblePlys.Contains(gameObject))
				{
					this.visiblePlys.Add(gameObject);
				}
				if (this.Networkenraged == Scp096PlayerScript.RageState.NotEnraged)
				{
					this.Networkenraged = Scp096PlayerScript.RageState.Panic;
					ModifiedJokerStuff();
					base.Invoke("StartRage", 5f);
					return;
				}
			}
			// Until here. This else should be kept as-is.
			else
			{
				this.IncreaseRage(Time.fixedDeltaTime);
			}
		}
	}
	public Dictionary<int, ReferenceHub> visiblePlys;
    private bool neonScp096Rework = true;
	
	internal void Init(RoleType classId, Role c)
	{
		this.iAm096 = (classId == RoleType.Scp096);
		if (this.iAm096 && NetworkServer.active)
		{
			if (this._processLookingQueue == null)
			{
				this._processLookingQueue = new Queue<GameObject>();
			}
			if (this.visiblePlys == null)
			{
				this.visiblePlys = new Dictionary<int, ReferenceHub>();
				if (Scp096PlayerScript.VisiblePlyLists == null) 
				{
					VisiblePlyLists = new List<Dictionary<int, ReferenceHub>>();
				}
				try { VisiblePlyLists.Add(this.visiblePlys); } catch { }
			}
			this.neonScp096Rework = ConfigFile.ServerConfig.GetBool("neon_scp096", true);
			return;
		}
		this._processLookingQueue = null;
		this.visiblePlys = null;
	}
	public static List<Dictionary<int, ReferenceHub>> VisiblePlyLists { private set; get; }
	
	private void ProcessLooking()
	{
		if (this._processLookingQueue.IsEmpty<GameObject>())
		{
			foreach (GameObject item in PlayerManager.players)
			{
				this._processLookingQueue.Enqueue(item);
			}
			return;
		}
		GameObject gameObject = this._processLookingQueue.Dequeue();
		if (gameObject == null || !ReferenceHub.GetHub(gameObject).characterClassManager.IsHuman() || gameObject.GetComponent<FlashEffect>().blinded)
		{
			return;
		}
		Transform transform = gameObject.GetComponent<Scp096PlayerScript>().camera.transform;
		float num = this.lookingTolerance.Evaluate(Vector3.Distance(transform.position, this.camera.transform.position));
		RaycastHit raycastHit;
		if ((num < 0.75f || Vector3.Dot(transform.forward, (transform.position - this.camera.transform.position).normalized) < -num) && Physics.Raycast(transform.transform.position, (this.camera.transform.position - transform.position).normalized, out raycastHit, 20f, this.layerMask) && raycastHit.collider.gameObject.layer == 24 && raycastHit.collider.GetComponentInParent<Scp096PlayerScript>() == this)
		{
			if (Scp096PlayerScript.neonScp096Rework)
			{
				ReferenceHub hub = ReferenceHub.GetHub(gameObject);
				if (!this.visiblePlys.ContainsKey(hub.queryProcessor.PlayerId))
				{
					this.visiblePlys.Add(hub.queryProcessor.PlayerId, hub);
				}
				if (this.Networkenraged == Scp096PlayerScript.RageState.NotEnraged)
				{
					this.Networkenraged = Scp096PlayerScript.RageState.Panic;
					base.Invoke("StartRage", 5f);
					this.ModifiedJokerStuff();
					return;
				}
			}
			else
			{
				this.IncreaseRage(Time.fixedDeltaTime);
			}
		}
	}
	void ModifiedJokerStuff() {
		MEC.Timing.RunCoroutine(GetClosestPlayer(), 1, "gcp");
		MEC.Timing.RunCoroutine(Punish(ReferenceHub.GetHub(this.gameObject)), 1, "punish");
	}

	public IEnumerator<float> GetClosestPlayer()
	{
		Broadcast bc = PlayerManager.localPlayer.GetComponent<Broadcast>();
		yield return Timing.WaitForSeconds(5.5f);
		this.gameObject.GetComponent<ServerRoles>().BypassMode = true;
		while (this.Networkenraged == Scp096PlayerScript.RageState.Enraged && this.visiblePlys.Count > 0 && base.gameObject.GetComponent<CharacterClassManager>().NetworkCurClass == RoleType.Scp096)
		{
			int i = 0;
			float num = 81f;
			while (i < this.visiblePlys.Count)
			{
				KeyValuePair<int, ReferenceHub> keyValuePair = this.visiblePlys.ElementAt(i);
				if (!keyValuePair.Value.characterClassManager.IsHuman())
				{
					this.visiblePlys.Remove(keyValuePair.Key);
				}
				else
				{
					float num2 = Vector3.Distance(base.gameObject.transform.position, keyValuePair.Value.gameObject.transform.position);
					if (num2 <= 80f)
					{
						if (num2 < num)
						{
							num = num2;
						}
						i++;
					}
					else
					{
						bc.TargetAddElement(keyValuePair.Value.characterClassManager.connectionToClient, "<i>You feel like SCP-096 forgot you...</i>", 4U, true);
						this.visiblePlys.Remove(keyValuePair.Key);
					}
				}
			}
			if (num > 80f)
			{
				break;
			}
			string text = Scp096PlayerScript.DrawBar((double)((80f - num) / 80f));
			bc.TargetClearElements(base.connectionToClient);
			bc.TargetAddElement(base.connectionToClient, string.Concat(new object[]
			{
				"<size=30><color=#c50000>Distance to nearest target: </color><color=#10F110>",
				text,
				"</color></size> \n<size=25>Targets Remaining: <color=#c50000>",
				this.visiblePlys.Count,
				"</color></size>"
			}), 1U, false);
			yield return Timing.WaitForSeconds(0.5f);
		}
		// This also pisses Reddking off. It should be Scp096PlayerScript.RageState.Cooldown, apparently. 
		this.Networkenraged = Scp096PlayerScript.RageState.NotEnraged;
		this.gameObject.GetComponent<ServerRoles>().BypassMode = false;
		Timing.KillCoroutines("punish");
		yield break;
	}
	// Same as Joker's but balanced towards rewarding good players over bad players. Yeah, it might have been better to do 1.20f and 90 * multi.
	private IEnumerator<float> Punish(ReferenceHub rh)
	{
		if(rh == null) yield break;
		yield return MEC.Timing.WaitForSeconds(5.5f);
		int counter = 0;
		while (this.Networkenraged == Scp096PlayerScript.RageState.Enraged && this.gameObject.GetComponent<CharacterClassManager>().NetworkCurClass == RoleType.Scp096)
		{
			counter++;
			float multi = Mathf.Pow(1.70f, counter);
			int dmg = Mathf.FloorToInt(20 * multi);
			rh.playerStats.HurtPlayer(
				new PlayerStats.HitInfo(dmg, rh.nicknameSync.MyNick, DamageTypes.Decont,
					rh.queryProcessor.PlayerId), rh.gameObject);
			yield return MEC.Timing.WaitForSeconds(5f);
		}
	}
	
	// Literally pasted from Joker. Thanks m8.
	private static string DrawBar(double percentage)
	{
		string bar = "<color=#ffffff>(</color>";
		const int barSize = 20;
		percentage *= 100;
		if (percentage == 0) return "(      )";
		for (double i = 0; i < 100; i += 100 / barSize)
			if (i < percentage)
				bar += "█";
			else
				bar += "<color=#c50000>█</color>";
		bar += "<color=#ffffff>)</color>";
		return bar;
	}
	
	// Another problematic function according to Reddking. If you're going to place a cooldown, use the this.DeductCooldown() function since that's probably fine
	private void FixedUpdate()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].playing = (i == (int)this.enraged && this.iAm096);
			this.tracks[i].Update(this.tracks.Length + 1);
		}
		if (!this.iAm096 || !NetworkServer.active)
		{
			return;
		}
		if (Scp096PlayerScript.neonScp096Rework)
		{
			Scp096PlayerScript.RageState rageState = this.enraged;
			if (rageState <= Scp096PlayerScript.RageState.Panic)
			{
				this.ProcessLooking();
			}
		}
		else
		{
			switch (this.enraged)
			{
			case Scp096PlayerScript.RageState.NotEnraged:
				this.ProcessLooking();
				break;
			case Scp096PlayerScript.RageState.Enraged:
				this.DeductRage();
				break;
			case Scp096PlayerScript.RageState.Cooldown:
				this.DeductCooldown();
				break;
			}
		}
		if (this.debugEnterRage)
		{
			this.debugEnterRage = false;
			this.IncreaseRage(this.rageCurve.Evaluate(20f));
		}
	}