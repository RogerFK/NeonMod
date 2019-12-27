	private bool[] tutorials;
	private bool invisTutorial;
	// Token: 0x0600105D RID: 4189
	[ServerCallback]
	private void TransmitData()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		List<GameObject> players = PlayerManager.players;
		this.usedData = players.Count;
		if (this.receivedData == null || this.receivedData.Length < this.usedData)
		{
			this.receivedData = new PlayerPositionData[this.usedData * 2];
			tutorials = new bool[this.usedData * 2];
		}
		for (int i = 0; i < this.usedData; i++)
		{
			this.receivedData[i] = new PlayerPositionData(players[i]);
			// Initialize this variable down below on the Start() function using invisTutorial = GameCore.ConfigFile.ServerConfig.GetBool("neon_invistutorial", false);
			if(invisTutorial) {
				tutorials[i] = players[i].GetComponent<CharacterClassManager>().CurClass == RoleType.Tutorial;
			}
			else
			{
				this.tutorials[i] = false;
			}	
		}
		if (this.transmitBuffer == null || this.transmitBuffer.Length < this.usedData)
		{
			this.transmitBuffer = new PlayerPositionData[this.usedData * 2];
		}
		foreach (GameObject gameObject in players)
		{
			CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
			Array.Copy(this.receivedData, this.transmitBuffer, this.usedData);
			if (component.CurClass.Is939())
			{
				for (int j = 0; j < this.usedData; j++)
				{
					if (this.transmitBuffer[j].position.y < 800f)
					{
						CharacterClassManager component2 = players[j].GetComponent<CharacterClassManager>();
						if (component2.Classes.SafeGet(component2.CurClass).team != Team.SCP && component2.Classes.SafeGet(component2.CurClass).team != Team.RIP && !players[j].GetComponent<Scp939_VisionController>().CanSee(component.GetComponent<Scp939PlayerScript>()))
						{
							this.transmitBuffer[j] = new PlayerPositionData(Vector3.up * 6000f, 0f, this.transmitBuffer[j].playerID, false);
						}
					}
				}
			}
			// OhNeinSix uses this, you shouldn't probably change it ngl
			else if (component.CurClass == RoleType.Scp096)
			{
				Scp096PlayerScript component3 = gameObject.GetComponent<Scp096PlayerScript>();
				for (int k = 0; k < this.usedData; k++)
				{
					if (component3.Neon096Rework && component3.Networkenraged == Scp096PlayerScript.RageState.Enraged && !component3.visiblePlys.ContainsKey(this.transmitBuffer[k].playerID))
					{
						this.transmitBuffer[k] = new PlayerPositionData(Vector3.up * 6000f, 0f, this.transmitBuffer[k].playerID, false);
					}
					else if (this.transmitBuffer[k].uses268)
					{
						this.transmitBuffer[k] = new PlayerPositionData(Vector3.up * 6000f, 0f, this.transmitBuffer[k].playerID, false);
					}
				}
			}
			// This uses ñ for a reason. The reason is: Viva España.
			else if (component.CurClass == RoleType.Spectator) {
				for (int ñ = 0; ñ < this.usedData; ñ++)
				{
					if (tutorials[ñ])
					{
						this.transmitBuffer[ñ] = new PlayerPositionData(Vector3.up * 6000f, 0f, this.transmitBuffer[ñ].playerID, false);
					}
				}
			}
			else if (component.CurClass != RoleType.Scp079)
			{
				for (int l = 0; l < this.usedData; l++)
				{
					if (this.transmitBuffer[l].uses268)
					{
						this.transmitBuffer[l] = new PlayerPositionData(Vector3.up * 6000f, 0f, this.transmitBuffer[l].playerID, false);
					}
				}
			}
			NetworkConnection networkConnection = component.netIdentity.isLocalPlayer ? NetworkServer.localConnection : component.netIdentity.connectionToClient;
			if (this.usedData <= 20)
			{
				networkConnection.Send<PlayerPositionManager.PositionMessage>(new PlayerPositionManager.PositionMessage(this.transmitBuffer, (byte)this.usedData, 0), 1);
			}
			else
			{
				byte b = 0;
				while ((int)b < this.usedData / 20)
				{
					networkConnection.Send<PlayerPositionManager.PositionMessage>(new PlayerPositionManager.PositionMessage(this.transmitBuffer, 20, b), 1);
					b += 1;
				}
				byte b2 = (byte)(this.usedData % (int)(b * 20));
				if (b2 > 0)
				{
					networkConnection.Send<PlayerPositionManager.PositionMessage>(new PlayerPositionManager.PositionMessage(this.transmitBuffer, b2, b), 1);
				}
			}
		}
	}