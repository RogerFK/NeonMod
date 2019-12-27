/* This is a modification I have in my own assembly. Try to figure out what it does, bucko.*/
if (go.GetComponent<global::Handcuffs>().NetworkCufferId > 0 && this.ccm.Classes.SafeGet(this.ccm.CurClass).team != global::Team.SCP
			&& info.Tool != global::DamageTypes.ToIndex(DamageTypes.Falldown) && info.Tool != global::DamageTypes.ToIndex(DamageTypes.Wall)
			&& info.Tool != global::DamageTypes.ToIndex(DamageTypes.Decont) && info.Tool != global::DamageTypes.ToIndex(DamageTypes.Flying)
			&& info.Tool != global::DamageTypes.ToIndex(DamageTypes.Lure) && info.Tool != global::DamageTypes.ToIndex(DamageTypes.Tesla)
			&& info.Tool != global::DamageTypes.ToIndex(DamageTypes.Nuke) && info.Tool != global::DamageTypes.ToIndex(DamageTypes.RagdollLess)
			&& info.Tool != global::DamageTypes.ToIndex(DamageTypes.None) && info.Tool != global::DamageTypes.ToIndex(DamageTypes.Pocket))
		{
			return false;
		}