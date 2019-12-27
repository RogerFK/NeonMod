	// This is my own rework, do whatever you want with it, for I do not care.
	// Token: 0x0600121F RID: 4639
	private void FixedUpdate()
	{
		this.DoBlinkingSequence();
		if (!this.iAm173 || (!base.isLocalPlayer && !NetworkServer.active))
		{
			return;
		}
		if (this._allowMove)
		{
			global::Scp173PlayerScript.reworkPlusTime -= Time.fixedDeltaTime * 1.25f;
			if (global::Scp173PlayerScript.reworkPlusTime < 0f)
			{
				global::Scp173PlayerScript.reworkPlusTime = 0f;
			}
		}
		this._allowMove = true;
		foreach (GameObject gameObject in global::PlayerManager.players)
		{
			global::Scp173PlayerScript component = gameObject.GetComponent<global::Scp173PlayerScript>();
			if (!component.SameClass && component.LookFor173(base.gameObject, true) && this.LookFor173(component.gameObject, false))
			{
				this._allowMove = false;
				break;
			}
		}
	}
	// Token: 0x06001222 RID: 4642 RVA: 0x0006A180 File Offset: 0x00068380
	private void DoBlinkingSequence()
	{
		if (!base.isServer || !base.isLocalPlayer)
		{
			return;
		}
		global::Scp173PlayerScript._remainingTime -= Time.fixedDeltaTime;
		global::Scp173PlayerScript._blinkTimeRemaining -= Time.fixedDeltaTime;
		if (global::Scp173PlayerScript._remainingTime >= 0f)
		{
			return;
		}
		global::Scp173PlayerScript._blinkTimeRemaining = this.blinkDuration_see + 0.5f + reworkPlusTime / 3f;
		global::Scp173PlayerScript._remainingTime = UnityEngine.Random.Range(this.minBlinkTime, this.maxBlinkTime) - reworkPlusTime;
		if (!this._allowMove)
		{
			global::Scp173PlayerScript.reworkPlusTime += UnityEngine.Random.Range(0.25f, 0.45f);
			if(global::Scp173PlayerScript.reworkPlusTime > this.minBlinkTime - 1f)
			{
				global::Scp173PlayerScript.reworkPlusTime = this.minBlinkTime - 1f;
			}
		}
		global::Scp173PlayerScript[] array = UnityEngine.Object.FindObjectsOfType<global::Scp173PlayerScript>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RpcBlinkTime();
		}
	}
	private static float reworkPlusTime = 0f;