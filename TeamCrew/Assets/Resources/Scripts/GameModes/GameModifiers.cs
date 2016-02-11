using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Modifier 
{ 
    LowGravity,
    BurningHands, 
    OneArm, 
    ClingyFrogs, 
    NoBugs, 
    NoLegs, 
    PartyHats, 
    KingOfTheHill, 
    LotsOfBugs
}

public class Mod
{
    public Modifier name;

    public bool IsActive { get { return active; } }
    private bool active;

    public Mod(Modifier name)
    {
        this.name = name;
        this.active = false;
    }

    public void Activate()
    {
        active = !active;
    }

    public virtual void OnStart()
    {

    }
}
public class LowGravityMod : Mod
{
    private float defaultGravity = -17;
    private float gravity = -10f;

    public LowGravityMod(Modifier name) : base(name)
    {

    }

    public override void OnStart()
    {
        base.OnStart();

        if (IsActive)
        {
            Physics2D.gravity = new Vector2(0, gravity);
        }
        else
        {
            Physics2D.gravity = new Vector2(0, defaultGravity);
        }
    }
}
public class BurningHandsMod : Mod
{
    private float gripLimit = 3.0f;

    public BurningHandsMod(Modifier name) : base(name)
    {

    }

    public override void OnStart()
    {
        base.OnStart();

        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Respawn respawn = gameManager.transform.GetComponent<Respawn>();

        List<BurningHands> burningHandScripts = new List<BurningHands>();

        foreach(PlayerRespawn script in respawn.respawnScripts)
        {
            burningHandScripts.AddRange(script.prefab.GetComponentsInChildren<BurningHands>());
        }

        foreach (FrogPrototype frog in gameManager.playerScripts)
        {
            if (frog != null)
                burningHandScripts.AddRange(frog.transform.parent.GetComponentsInChildren<BurningHands>());
        }

        if (IsActive)
        {
            if (burningHandScripts.Count <= 0)
                return;

            foreach(BurningHands hand in burningHandScripts)
            {
                hand.enabled = true;
                hand.gripLimit = gripLimit;
            }
        }
        else
        {
            if (burningHandScripts.Count <= 0)
                return;

            foreach (BurningHands hand in burningHandScripts)
            {
                hand.enabled = false;
            }
        }
    }
}


public class GameModifiers : MonoBehaviour 
{
    public List<Mod> mods = new List<Mod>();

    void Awake()
    {
        mods.Clear();
        mods.Add(new LowGravityMod(Modifier.LowGravity));
        mods.Add(new BurningHandsMod(Modifier.BurningHands));
        mods.Add(new Mod(Modifier.OneArm));
        mods.Add(new Mod(Modifier.ClingyFrogs));
        mods.Add(new Mod(Modifier.NoBugs));
        mods.Add(new Mod(Modifier.NoLegs));
        mods.Add(new Mod(Modifier.PartyHats));
        mods.Add(new Mod(Modifier.KingOfTheHill));
        mods.Add(new Mod(Modifier.LotsOfBugs));
    }

    public void OnGameStart()
    {
        foreach(Mod m in mods)
        {
            m.OnStart();
        }
    }

    private Mod GetMod(Modifier modName)
    {
        foreach (Mod m in mods)
        {
            if (m.name == modName)
            {
                return m;
            }
        }
        return null;
    }
    private void ActivateModifier(Modifier modName)
    {
        foreach(Mod m in mods)
        {
            if (m.name == modName)
            {
                m.Activate();
            }
        }
    }




    public void OnLowGravity()
    {
        ActivateModifier(Modifier.LowGravity);
    }
    public void OnBurningHands()
    {
        ActivateModifier(Modifier.BurningHands);
    }
}
