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

    public bool Activate()
    {
        if (active)
            return false;

        active = true;
        return true;
    }
}
public class GameModifiers : MonoBehaviour 
{
    public List<Mod> mods = new List<Mod>();

    void Awake()
    {
        mods.Add(new Mod(Modifier.LowGravity));
        mods.Add(new Mod(Modifier.BurningHands));
        mods.Add(new Mod(Modifier.OneArm));
        mods.Add(new Mod(Modifier.ClingyFrogs));
        mods.Add(new Mod(Modifier.NoBugs));
        mods.Add(new Mod(Modifier.NoLegs));
        mods.Add(new Mod(Modifier.PartyHats));
        mods.Add(new Mod(Modifier.KingOfTheHill));
        mods.Add(new Mod(Modifier.LotsOfBugs));
    }

    public void ActivateModifier(Modifier modName)
    {
        foreach(Mod m in mods)
        {
            if (m.name == modName)
            {
                if (m.Activate())
                {
                    //Success
                }
            }
        }
    }
}
