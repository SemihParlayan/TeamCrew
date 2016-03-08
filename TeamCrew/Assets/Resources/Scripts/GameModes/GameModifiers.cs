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
    KOTH, 
    NoLegs, 
    PartyHats, 
    KingOfTheHill, 
    LotsOfBugs
}

[System.Serializable]
public class Mod
{
    public Modifier name;

    public bool IsActive { get { return active; } }
    public bool active;

    public Mod(Modifier name)
    {
        this.name = name;
        this.active = false;
    }

    public void Activate()
    {
        active = !active;
        OnActivate();
    }
    public virtual void OnActivate()
    {

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
public class OneArm : Mod
{
    public OneArm(Modifier name) : base(name)
    {
        CanControllArms(true);
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public void OnModifierSelection()
    {
        CanControllArms();
    }
    public void OnPress()
    {
        CanControllArms();
    }
    private void CanControllArms(bool forceArmsActive = false)
    {
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Respawn respawn = gameManager.transform.GetComponent<Respawn>();

        List<OneArmController> frogScripts = new List<OneArmController>();

        foreach (PlayerRespawn script in respawn.respawnScripts)
        {
            frogScripts.AddRange(script.prefab.GetComponentsInChildren<OneArmController>());
            FrogPrototype frog = script.prefab.FindChild("body").GetComponent<FrogPrototype>();
            frogScripts.AddRange(frog.topPrefab.GetComponentsInChildren<OneArmController>());
        }

        foreach (FrogPrototype frog in gameManager.playerScripts)
        {
            if (frog != null)
                frogScripts.AddRange(frog.transform.parent.GetComponentsInChildren<OneArmController>());
        }

        foreach (OneArmController controller in frogScripts)
        {
            controller.SetDisabledArmState(false);
        }

        if (forceArmsActive)
            return;

        if (IsActive)
        {
            int arm = Random.Range(0, 2);
            foreach (OneArmController controller in frogScripts)
            {
                controller.SetDisabledArmState(true, arm);
            }
        }
    }
}
public class KingOfTheHill : Mod
{
    private KOTH koth;

    public KingOfTheHill(Modifier name, KOTH koth) : base(name)
    {
        this.koth = koth;
        this.koth.enabled = IsActive;
        this.koth.KOTHParent.gameObject.SetActive(false);
    }

    public void OnModifierSelection()
    {
        koth.KOTHParent.gameObject.SetActive(false);
    }
    public override void OnStart()
    {
        base.OnStart();

        koth.OnGameStart();
    }
    public override void OnActivate()
    {
        base.OnActivate();
        koth.enabled = IsActive;

        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Respawn respawn = gameManager.transform.GetComponent<Respawn>();

        for (int i = 0; i < koth.keepers.Length; i++)
        {
            FrogPrototype frogPrototype = respawn.respawnScripts[i].prefab.GetComponentInChildren<FrogPrototype>();
            koth.keepers[i].frogColor = frogPrototype.respawnArrowColor;
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
        mods.Add(new OneArm(Modifier.OneArm));
        mods.Add(new Mod(Modifier.ClingyFrogs));
        mods.Add(new KingOfTheHill(Modifier.KOTH, GetComponent<KOTH>()));
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



    public void OnModifierSelection()
    {
        ((OneArm)GetMod(Modifier.OneArm)).OnModifierSelection();
        ((KingOfTheHill)GetMod(Modifier.KOTH)).OnModifierSelection();
    }


    public void OnLowGravity()
    {
        ActivateModifier(Modifier.LowGravity);
    }
    public void OnBurningHands()
    {
        ActivateModifier(Modifier.BurningHands);
    }
    public void OnOneArms()
    {
        ActivateModifier(Modifier.OneArm);
        ((OneArm)GetMod(Modifier.OneArm)).OnPress();
    }
    public void OnKOTH()
    {
        ActivateModifier(Modifier.KOTH);
    }
}
