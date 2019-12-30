
public bool HurtPlayer(PlayerStats.HitInfo info, GameObject go){ //probably
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
}

//requires 
using System.Reflection;

public void Roundrestart()
{
	//make userIDs start from 1 every round ( first round will start from 2 )
	FieldInfo field = typeof(QueryProcessor).GetField("_idIterator", BindingFlags.Static | BindingFlags.NonPublic);
	if (field != null)
	{
		field.SetValue(null, -1);
	}
	
	MirrorIgnorancePlayer[] array = UnityEngine.Object.FindObjectsOfType<MirrorIgnorancePlayer>();
	for (int i = 0; i < array.Length; i++)
	{
		array[i].OnDisable();
	}
	this.RpcRoundrestart((float)(PlayerPrefsSl.Get("LastRoundrestartTime", 5000) / 1000));
	base.Invoke("ChangeLevel", 2.5f);
}
