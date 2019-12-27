	// This is just RemoteKeycard.
	public void CallCmdOpenDoor(GameObject doorId)
	{
		if (!this._playerInteractRateLimit.CanExecute(true) || (this._hc.CufferId > 0 && !this.CanDisarmedInteract))
		{
			return;
		}
		if (doorId == null)
		{
			return;
		}
		if (this._ccm.CurClass == RoleType.None || this._ccm.CurClass == RoleType.Spectator)
		{
			return;
		}
		Door component = doorId.GetComponent<Door>();
		if (component == null)
		{
			return;
		}
		if (!((component.buttons.Count == 0) ? this.ChckDis(doorId.transform.position) : component.buttons.Any((GameObject item) => this.ChckDis(item.transform.position))))
		{
			return;
		}
		Scp096PlayerScript component2 = base.GetComponent<Scp096PlayerScript>();
		if (component.destroyedPrefab != null && (!component.isOpen || component.curCooldown > 0f) && component2.iAm096 && component2.enraged == Scp096PlayerScript.RageState.Enraged)
		{
			if (this._096DestroyLockedDoors || !component.locked || this._sr.BypassMode)
			{
				component.DestroyDoor(true);
			}
			return;
		}
		this.OnInteract();
		if (this._sr.BypassMode)
		{
			component.ChangeState(true);
			return;
		}
		// This is the Remote Keycard bool
		if (this.neonRemoteKeycard)
		{
			try
			{
				Inventory.SyncListItemInfo availableItems = this._inv.items;
				int i = 0;
				while (i < availableItems.Count)
				{
					if (this._inv.GetItemByID(availableItems[i].id).permissions.Contains(component.permissionLevel))
					{
						if (!component.locked)
						{
							component.ChangeState(false);
							return;
						}
						this.RpcDenied(doorId);
						return;
					}
					else
					{
						i++;
					}
				}
			}
			catch
			{
				this.RpcDenied(doorId);
			}
		}
		// Everything below here should be reviewed after every update.
		if (string.Equals(component.permissionLevel, "CHCKPOINT_ACC", StringComparison.OrdinalIgnoreCase) && base.GetComponent<CharacterClassManager>().Classes.SafeGet(base.GetComponent<CharacterClassManager>().CurClass).team == Team.SCP)
		{
			component.ChangeState(false);
			return;
		}
		try
		{
			if (string.IsNullOrEmpty(component.permissionLevel))
			{
				if (!component.locked)
				{
					component.ChangeState(false);
				}
			}
			else if (this._inv.GetItemByID(this._inv.curItem).permissions.Contains(component.permissionLevel))
			{
				if (!component.locked)
				{
					component.ChangeState(false);
				}
				else
				{
					this.RpcDenied(doorId);
				}
			}
			else
			{
				this.RpcDenied(doorId);
			}
		}
		catch
		{
			this.RpcDenied(doorId);
		}
	}

	private bool neonRemoteKeycard;
	// Place this line in the Start() function.
	neonRemoteKeycard = ConfigFile.ServerConfig.GetBool("neon_remotekeycard", true);
