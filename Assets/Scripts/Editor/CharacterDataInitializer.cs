#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Initializes character data ScriptableObjects for all 10 characters.
/// Run via Tools > Initialize Character Data.
/// </summary>
public class CharacterDataInitializer
{
    [MenuItem("Tools/Initialize Character Data")]
    public static void InitializeAllCharacterData()
    {
        // Create the Characters folder under Resources if it doesn't exist
        string resourcesPath = "Assets/Resources";
        string charactersPath = resourcesPath + "/Characters";
        if (!AssetDatabase.IsValidFolder(charactersPath))
        {
            AssetDatabase.CreateFolder(resourcesPath, "Characters");
        }

        // Clear existing character data (optional)
        // string[] existingGuids = AssetDatabase.FindAssets("t:CharacterData", new[] { charactersPath });
        // foreach (string guid in existingGuids)
        // {
        //     string path = AssetDatabase.GUIDToAssetPath(guid);
        //     AssetDatabase.DeleteAsset(path);
        // }

        // Create data for each character
        CreateCharacterData("Kenshi", "The Wind Blade", "Dual-Wielding Speedster",
            new CharacterStats { maxHealth = 100, maxStamina = 120, movementSpeed = 7.5f, jumpHeight = 2.5f, weight = 0.8f, attackPower = 10f, staggerResistance = 0.4f,
                lightAttackDamage = 12, heavyAttackDamage = 22 },
            CreateKenshiAbilities(),
            CreateKenshiCombos(),
            "A wanderer from the misty islands, Kenshi seeks the legendary Blade of Winds to restore his clan's honor.",
            new List<string> { "Path of the Whispering Wind: Kenshi masters the Gale Technique, becoming untouchable in combat.",
                               "Path of the Storm's Fury: Kenshi embrarages the tempest, sacrificing speed for overwhelming offensive power.",
                               "Path of the Lone Blade: Kenshi rejects both extremes, forging a balanced style that adapts to any foe." });

        CreateCharacterData("Valeria", "The Iron Fist", "Heavy Elemental Brawler",
            new CharacterStats { maxHealth = 150, maxStamina = 100, movementSpeed = 3.5f, jumpHeight = 1.5f, weight = 2.0f, attackPower = 15f, staggerResistance = 0.7f,
                lightAttackDamage = 18, heavyAttackDamage = 32 },
            CreateValeriaAbilities(),
            CreateValeriaCombos(),
            "Born in the volcanic valleys, Valeria channels the earth's fury to protect her homeland from invading forces.",
            new List<string> { "Path of the Molten Core: Valeria unlocks the full potential of her fire affinity, leaving trails of lava in her wake.",
                               "Path of the Crystal Sentinel: Valeria shifts to defensive crystal formations, becoming an impregnable wall.",
                               "Path of the Echoing Tremor: Valeria learns to redirect seismic energy, turning enemy force against them." });

        // Character 3: Lyra, the Starfall Archer
        CreateCharacterData("Lyra", "The Starfall Archer", "Celestial Ranger",
            new CharacterStats { maxHealth = 110, maxStamina = 110, movementSpeed = 5f, jumpHeight = 2f, weight = 1.2f, attackPower = 8f, staggerResistance = 0.5f,
                lightAttackDamage = 10, heavyAttackDamage = 25 },
            CreateLyraAbilities(),
            CreateLyraCombos(),
            "Lyra was born under a meteor shower, marked by celestial sigils that grant her control over stellar energy.",
            new List<string> { "Path of the Shooting Star: Lyra focuses on rapid, penetrating shots that pierce multiple enemies.",
                               "Path of the Guardian Constellation: Lyra develops protective star barriers and healing light.",
                               "Path of the Supernova: Lyra learns to unleash devastating explosive starbursts at close range." });

        // Character 4: Baron, the Abyssal Tank
        CreateCharacterData("Baron", "The Abyssal Tank", "Maritime Fortress",
            new CharacterStats { maxHealth = 200, maxStamina = 90, movementSpeed = 2.5f, jumpHeight = 1f, weight = 2.5f, attackPower = 12f, staggerResistance = 0.8f,
                lightAttackDamage = 15, heavyAttackDamage = 35 },
            CreateBaronAbilities(),
            CreateBaronCombos(),
            "Baron is a former naval admiral who fused with a deep-sea leviathan, gaining immense strength and pressure resistance.",
            new List<string> { "Path of the Leviathan's Embrace: Baron's symbiosis deepens, granting him regenerative abilities in water.",
                               "Path of the Iron Hull: Baron focuses on pure physical dominance, becoming nearly immovable.",
                               "Path of the Siren's Call: Baron learns to manipulate water to trap and drown his foes." });

        // Character 5: Zane, the Void Assassin
        CreateCharacterData("Zane", "The Void Assassin", "Shadow Stalker",
            new CharacterStats { maxHealth = 90, maxStamina = 130, movementSpeed = 6f, jumpHeight = 2.2f, weight = 0.7f, attackPower = 11f, staggerResistance = 0.3f,
                lightAttackDamage = 14, heavyAttackDamage = 28 },
            CreateZaneAbilities(),
            CreateZaneCombos(),
            "Zane survived an experiment gone wrong that exposed him to void energy, allowing him to phase between dimensions.",
            new List<string> { "Path of the Silent Blade: Zane refines his assassination techniques, becoming undetectable until the strike lands.",
                               "Path of the Void Walker: Zane spends more time in the void dimension, gaining blink and phase-through-wall abilities.",
                               "Path of the Entropy Herald: Zane learns to release void explosions that erase matter from existence." });

        // Character 6: Juno, the Tempest Dancer
        CreateCharacterData("Juno", "The Tempest Dancer", "Stormweaver",
            new CharacterStats { maxHealth = 105, maxStamina = 115, movementSpeed = 6.5f, jumpHeight = 2.3f, weight = 0.9f, attackPower = 9f, staggerResistance = 0.45f,
                lightAttackDamage = 11, heavyAttackDamage = 24 },
            CreateJunoAbilities(),
            CreateJunoCombos(),
            "Juno was raised by wind monks on a floating monastery, where she learned to dance with the storms themselves.",
            new List<string> { "Path of the Wind Dancer: Juno's movements become so fast she creates vacuum blades in her wake.",
                               "Path of the Thunderclap: Juno focuses on powerful, staggered strikes that chain lightning between enemies.",
                               "Path of the Eye of the Storm: Juno learns to create a calm zone around her that buffets enemies away." });

        // Character 7: Astrid, the Crimson Flame
        CreateCharacterData("Astrid", "The Crimson Flame", "Pyromancer Berserker",
            new CharacterStats { maxHealth = 130, maxStamina = 100, movementSpeed = 4f, jumpHeight = 1.8f, weight = 1.6f, attackPower = 14f, staggerResistance = 0.6f,
                lightAttackDamage = 16, heavyAttackDamage = 30 },
            CreateAstridAbilities(),
            CreateAstridCombos(),
            "Astrid made a pact with a fire spirit to save her village, but now struggles to control the burning rage within her.",
            new List<string> { "Path of the Berserker's Rage: Astrid embraces the flame fully, gaining immense damage but losing defense over time.",
                               "Path of the Flame Warden: Astrid learns to control her fire, using it to protect allies and create barriers.",
                               "Path of the Ash Reborn: Astrid discovers she can rise from her own ashes, gaining resurrection mechanics." });

        // Character 8: Dante, the Gravity Well
        CreateCharacterData("Dante", "The Gravity Well", "Singularity Manipulator",
            new CharacterStats { maxHealth = 115, maxStamina = 105, movementSpeed = 4.5f, jumpHeight = 1.9f, weight = 1.4f, attackPower = 13f, staggerResistance = 0.55f,
                lightAttackDamage = 13, heavyAttackDamage = 27 },
            CreateDanteAbilities(),
            CreateDanteCombos(),
            "Dante studied ancient texts on gravitation and accidentally created a micro-singularity in his palm, which he now learns to control.",
            new List<string> { "Path of the Attractor: Dante focuses on pulling enemies together for devastating combo setups.",
                               "Path of the Repeller: Dante learns to push enemies away with explosive force, controlling space.",
                               "Path of the Orbital Dance: Dante masters hovering and rotating attacks that hit from all angles." });

        // Character 9: Mira, the Chrono Warden
        CreateCharacterData("Mira", "The Chrono Warden", "Time Bender",
            new CharacterStats { maxHealth = 100, maxStamina = 120, movementSpeed = 5.5f, jumpHeight = 2.1f, weight = 1f, attackPower = 10f, staggerResistance = 0.5f,
                lightAttackDamage = 12, heavyAttackDamage = 26 },
            CreateMiraAbilities(),
            CreateMiraCombos(),
            "Mira is a guardian of the timestream, tasked with preventing paradoxes from unraveling reality.",
            new List<string> { "Path of the Slow Warden: Mira specializes in slowing enemies to give her allies time to react.",
                               "Path of the Fast Warden: Mira learns to accelerate her own actions, striking multiple times before an enemy can blink.",
                               "Path of the Paradox Warden: Mira gains the ability to rewind small amounts of time to undo mistakes." });

        // Character 10: Orion, the Starfall Paladin
        CreateCharacterData("Orion", "The Starfall Paladin", "Divine Guardian",
            new CharacterStats { maxHealth = 140, maxStamina = 95, movementSpeed = 4f, jumpHeight = 1.7f, weight = 1.8f, attackPower = 16f, staggerResistance = 0.65f,
                lightAttackDamage = 17, heavyAttackDamage = 33 },
            CreateOrionAbilities(),
            CreateOrionCombos(),
            "Orion is a paladin of a fallen star temple, wielding a blade forged from meteorite iron and blessed by celestial light.",
            new List<string> { "Path of the Radiant Guardian: Orion focuses on healing allies and smiting undead with holy light.",
                               "Path of the Meteoric Crusader: Orion learns to call down meteor strikes and move at incredible bursts of speed.",
                               "Path of the Eclipse Knight: Orion learns to harness both light and darkness, gaining powerful shadow techniques." });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Initialized all character data ScriptableObjects.");
    }

    // Helper struct for stats
    private struct CharacterStats
    {
        public float maxHealth, maxStamina, movementSpeed, jumpHeight, weight, attackPower, staggerResistance;
        public float lightAttackDamage, heavyAttackDamage;
    }

    // ----- Character Data Creation Methods -----

    private void CreateCharacterData(string name, string title, string archetype, CharacterStats stats,
                                     CharacterAbility[] abilities, ComboData[] combos,
                                     string backstory, List<string> narrativePaths)
    {
        // Create CharacterData asset
        CharacterData data = ScriptableObject.CreateInstance<CharacterData>();
        data.characterName = name;
        data.title = title;
        data.archetype = archetype;
        data.maxHealth = stats.maxHealth;
        data.maxStamina = stats.maxStamina;
        data.movementSpeed = stats.movementSpeed;
        data.jumpHeight = stats.jumpHeight;
        data.weight = stats.weight;
        data.attackPower = stats.attackPower;
        data.staggerResistance = stats.staggerResistance;
        data.lightAttackDamage = stats.lightAttackDamage;
        data.heavyAttackDamage = stats.heavyAttackDamage;
        data.abilities = abilities;
        // Assume ultimate is the last ability in the array for simplicity; we'll set it separately if needed
        if (abilities.Length > 0)
        {
            data.ultimateAbility = abilities[abilities.Length - 1]; // Last ability is ultimate
            // Create a copy without the ultimate for the regular abilities array
            CharacterAbility[] regularAbilities = new CharacterAbility[abilities.Length - 1];
            System.Array.Copy(abilities, regularAbilities, abilities.Length - 1);
            data.abilities = regularAbilities;
        }
        else
        {
            data.ultimateAbility = null;
        }
        data.combos = combos;
        // Note: We don't store backstory or narrative paths in CharacterData; they would be in a separate NarrativeData ScriptableObject.
        // For simplicity, we'll just log them here. In a full game, you'd have a separate system.
        Debug.Log($"Creating {name}: {backstory}");
        foreach (string path in narrativePaths)
        {
            Debug.Log($"  - {path}");
        }

        // Save asset
        string assetPath = $"Assets/Resources/Characters/{name}Data.asset";
        AssetDatabase.CreateAsset(data, assetPath);
    }

    // ----- Ability and Combo Creation for Each Character -----

    // Kenshi
    private CharacterAbility[] CreateKenshiAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Flash Step", "Q", "A quick dash that leaves afterimages.", 5, 2, 10, 20f, 0f, 1.2f, 0f, 0f),
            CreateAbility("Wind Blade", "E", "A slicing wind projectile.", 10, 3, 15, 1.5f, 0f, 0f, 5f, 0f),
            CreateAbility("Cyclone Spin", "R", "Spin attack that hits all around.", 15, 5, 20, 2f, 0f, 2f, 8f, true) // Ultimate
        };
    }

    private ComboData[] CreateKenshiCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Basic Slash", new string[] { "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1.1f, 1.2f }),
            CreateCombo("Wind Cutter", new string[] { "Light", "Heavy", "Light" }, new float[] { 0.1f, 0.2f, 0.1f }, new float[] { 1f, 1.3f, 1f }),
            CreateCombo("Gale Thrust", new string[] { "Heavy", "Light", "Heavy" }, new float[] { 0.2f, 0.1f, 0.2f }, new float[] { 1.5f, 1f, 1.5f })
        };
    }

    // Valeria
    private CharacterAbility[] CreateValeriaAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Earthshaker Slam", "Q", "Ground slam that creates a shockwave.", 10, 4, 20, 2f, 0f, 1.5f, 3f, 0.5f),
            CreateAbility("Molten Fist", "E", "Punch infused with lava, leaves burning ground.", 8, 3, 12, 1.2f, 0f, 1f, 4f, 0.3f),
            CreateAbility("Cataclysmic Uppercut", "R", "Devastating uppercut that launches enemies high.", 20, 6, 25, 3f, 0f, 2.5f, 8f, true) // Ultimate
        };
    }

    private ComboData[] CreateValeriaCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Pummel", new string[] { "Light", "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1f, 1f, 1f }),
            CreateCombo("Geo Crush", new string[] { "Heavy", "Heavy" }, new float[] { 0.2f, 0.2f }, new float[] { 1.8f, 1.8f }),
            CreateCombo("Lava Combo", new string[] { "Light", "Heavy", "Light" }, new float[] { 0.1f, 0.2f, 0.1f }, new float[] { 1f, 1.5f, 1f })
        };
    }

    // Lyra
    private CharacterAbility[] CreateLyraAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Star Shot", "Q", "Fires a fast energy arrow.", 6, 2, 10, 1f, 0f, 0f, 3f, 0f),
            CreateAbility("Constellation Burst", "E", "Releases a burst of star energy in all directions.", 12, 4, 18, 1.8f, 0f, 1.2f, 5f, 0.4f),
            CreateAbility("Supernova", "R", "Gathers stellar energy for a massive explosion.", 25, 8, 30, 4f, 0f, 3f, 10f, true) // Ultimate
        };
    }

    private ComboData[] CreateLyraCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Quick Shot", new string[] { "Light", "Light" }, new float[] { 0.1f, 0.1f }, new float[] { 1f, 1f }),
            CreateCombo("Charged Shot", new string[] { "Heavy" }, new float[] { 0.3f }, new float[] { 2f }),
            CreateCombo("Starfall Volley", new string[] { "Light", "Light", "Heavy" }, new float[] { 0.1f, 0.1f, 0.3f }, new float[] { 1f, 1f, 1.8f })
        };
    }

    // Baron
    private CharacterAbility[] CreateBaronAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Tidal Wave", "Q", "Summons a wave of water that pushes enemies back.", 12, 5, 20, 2f, 0f, 1.5f, 4f, 0.5f),
            CreateAbility("Abyssal Grab", "E", "Latches onto an enemy and pulls them close.", 8, 3, 15, 1f, 0f, 2f, 0f, 1f), // Stun/Grab
            CreateAbility("Leviathan's Roar", "R", "Releases a deafening shockwave that stuns all around.", 20, 6, 25, 3f, 0f, 2.5f, 0f, 1.5f, true) // Ultimate
        };
    }

    private ComboData[] CreateBaronCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Anchor Drop", new string[] { "Heavy", "Heavy" }, new float[] { 0.2f, 0.2f }, new float[] { 2f, 2f }),
            CreateCombo("Wave Dash", new string[] { "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1f, 1f }),
            CreateCombo("Abyssal Combo", new string[] { "Light", "Heavy", "Light" }, new float[] { 0.1f, 0.2f, 0.1f }, new float[] { 1f, 1.5f, 1f })
        };
    }

    // Zane
    private CharacterAbility[] CreateZaneAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Void Step", "Q", "Teleports a short distance through the void.", 4, 1, 8, 0f, 0f, 0f, 0f, 0f), // Mobility, no damage
            CreateAbility("Null Touch", "E", "Melee attack that ignores armor.", 6, 2, 12, 1.5f, 0f, 0f, 0f, 0.2f),
            CreateAbility("Event Horizon", "R", "Creates a small void sphere that pulls and damages enemies.", 18, 6, 22, 2.5f, 0f, 2f, 6f, true) // Ultimate
        };
    }

    private ComboData[] CreateZaneCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Phantom Strike", new string[] { "Light", "Light" }, new float[] { 0.1f, 0.1f }, new float[] { 1f, 1.1f }),
            CreateCombo("Void Combo", new string[] { "Light", "Heavy" }, new float[] { 0.1f, 0.2f }, new float[] { 1f, 1.4f }),
            CreateCombo("Assassin's Gambit", new string[] { "Light", "Light", "Heavy", "Light" }, new float[] { 0.1f, 0.1f, 0.2f, 0.1f }, new float[] { 1f, 1f, 1.3f, 1f })
        };
    }

    // Juno
    private CharacterAbility[] CreateJunoAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Gust Dash", "Q", "Dash forward on a gust of wind.", 5, 2, 10, 0f, 0f, 0f, 0f, 0f), // Mobility
            CreateAbility("Storm Call", "E", "Summons a lightning strike at target location.", 15, 4, 20, 2f, 0f, 1f, 6f, 0.5f),
            CreateAbility("Tempest Waltz", "R", "Dance with the storm, hitting multiple times with wind blades.", 20, 6, 25, 2.2f, 0f, 1.8f, 8f, true) // Ultimate
        };
    }

    private ComboData[] CreateJunoCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Wind Dance", new string[] { "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1f, 1f }),
            CreateCombo("Tempest Strike", new string[] { "Heavy", "Light" }, new float[] { 0.2f, 0.1f }, new float[] { 1.5f, 1f }),
            CreateCombo("Stormbreaker", new string[] { "Light", "Heavy", "Heavy" }, new float[] { 0.1f, 0.2f, 0.2f }, new float[] { 1f, 1.6f, 1.6f })
        };
    }

    // Astrid
    private CharacterAbility[] CreateAstridAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Flame Jab", "Q", "Quick fiery punch.", 6, 2, 12, 1f, 0f, 0f, 3f, 0.1f),
            CreateAbility("Molten Shield", "E", "Creates a temporary shield of lava that burns attackers.", 10, 3, 18, 0f, 50f, 0f, 0f, 0f), // Shield
            CreateAbility("Crimson Inferno", "R", "Unleashes a massive wave of fire in a cone.", 25, 8, 30, 3.5f, 0f, 3f, 12f, true) // Ultimate
        };
    }

    private ComboData[] CreateAstridCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Burning Fists", new string[] { "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1f, 1f }),
            CreateCombo("Flame Spit", new string[] { "Light", "Heavy" }, new float[] { 0.1f, 0.2f }, new float[] { 1f, 1.3f }),
            CreateCombo("Fire Dance", new string[] { "Heavy", "Light", "Heavy" }, new float[] { 0.2f, 0.1f, 0.2f }, new float[] { 1.5f, 1f, 1.5f })
        };
    }

    // Dante
    private CharacterAbility[] CreateDanteAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Gravity Pull", "Q", "Creates a small gravity well that draws enemies inward.", 8, 3, 15, 0f, 0f, 1f, 4f, 0.3f),
            CreateAbility("Singularity Punch", "E", "Punch that briefly increases local gravity.", 10, 4, 18, 1.8f, 0f, 0f, 5f, 0.2f),
            CreateAbility("Event Horizon Collapse", "R", "Expands the singularity into a devastating burst.", 22, 7, 28, 3f, 0f, 2.5f, 10f, true) // Ultimate
        };
    }

    private ComboData[] CreateDanteCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Orbitals", new string[] { "Light", "Light" }, new float[] { 0.1f, 0.1f }, new float[] { 1f, 1f }),
            CreateCombo("Gravity Combo", new string[] { "Light", "Heavy" }, new float[] { 0.1f, 0.2f }, new float[] { 1f, 1.4f }),
            CreateCombo("Singularity Spin", new string[] { "Heavy", "Light", "Heavy" }, new float[] { 0.2f, 0.1f, 0.2f }, new float[] { 1.6f, 1f, 1.6f })
        };
    }

    // Mira
    private CharacterAbility[] CreateMiraAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Chrono Slow", "Q", "Creates a field that slows enemy actions.", 10, 4, 20, 0f, 0f, 2f, 0f, 0f), // Debuff
            CreateAbility("Time Shift", "E", "Teleports a short distance by phasing through time.", 6, 2, 12, 0f, 0f, 0f, 0f, 0f), // Mobility
            CreateAbility("Temporal Acceleration", "R", "Speeds up Mira's own time for rapid strikes.", 18, 6, 25, 2.5f, 0f, 1.5f, 8f, true) // Ultimate
        };
    }

    private ComboData[] CreateMiraCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Slow Trap", new string[] { "Light", "Heavy" }, new float[] { 0.1f, 0.2f }, new float[] { 1f, 1.3f }),
            CreateCombo("Time Dash", new string[] { "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1f, 1f }),
            CreateCombo("Acceleration Combo", new string[] { "Heavy", "Light", "Heavy" }, new float[] { 0.2f, 0.1f, 0.2f }, new float[] { 1.6f, 1f, 1.6f })
        };
    }

    // Orion
    private CharacterAbility[] CreateOrionAbilities()
    {
        return new CharacterAbility[]
        {
            CreateAbility("Holy Strike", "Q", "Blade strike that deals bonus damage to undead.", 8, 3, 15, 1.6f, 0f, 0f, 5f, 0.2f),
            CreateAbility("Meteor Call", "E", "Calls down a small meteorite on target location.", 20, 6, 25, 0f, 0f, 3f, 10f, 0.5f),
            CreateAbility("Eclipse Blade", "R", "Blade shrouded in eclipse energy, dealing massive damage.", 22, 7, 28, 3.5f, 0f, 3f, 15f, true) // Ultimate
        };
    }

    private ComboData[] CreateOrionCombos()
    {
        return new ComboData[]
        {
            CreateCombo("Paladin's Grace", new string[] { "Light", "Light", "Light" }, new float[] { 0.1f, 0.1f, 0.1f }, new float[] { 1f, 1f, 1f }),
            CreateCombo("Meteor Smash", new string[] { "Heavy", "Light" }, new float[] { 0.2f, 0.1f }, new float[] { 1.8f, 1f }),
            CreateCombo("Eclipse Combo", new string[] { "Light", "Heavy", "Light" }, new float[] { 0.1f, 0.2f, 0.1f }, new float[] { 1f, 1.5f, 1f })
        };
    }

    // ----- Helper Methods -----

    private CharacterAbility CreateAbility(string name, string input, string description,
                                          int startup, int active, int recovery,
                                          float damageMultiplier, float knockback, float stun,
                                          float staminaCost, float manaCost = 0f)
    {
        CharacterAbility ability = ScriptableObject.CreateInstance<CharacterAbility>();
        ability.abilityName = name;
        ability.inputCommand = input;
        ability.description = description;
        ability.startupFrames = startup;
        ability.activeFrames = active;
        ability.recoveryFrames = recovery;
        ability.damageMultiplier = damageMultiplier;
        ability.knockbackForce = knockback;
        ability.stunDuration = stun;
        ability.staminaCost = staminaCost;
        ability.manaCost = manaCost;
        // In a full implementation, you'd assign VFX and SFX here
        return ability;
    }

    private ComboData CreateCombo(string name, string[] inputs, float[] hitStops, float[] damageMults)
    {
        ComboData combo = ScriptableObject.CreateInstance<ComboData>();
        combo.comboName = name;
        combo.inputSequence = inputs;
        combo.hitStopTimes = hitStops;
        combo.damageMultipliers = damageMults;
        // Validate lengths
        if (combo.hitStopTimes.Length != combo.inputSequence.Length)
        {
            combo.hitStopTimes = new float[combo.inputSequence.Length];
        }
        if (combo.damageMultipliers.Length != combo.inputSequence.Length)
        {
            combo.damageMultipliers = new float[combo.inputSequence.Length];
            for (int i = 0; i < combo.damageMultipliers.Length; i++)
                combo.damageMultipliers[i] = 1f;
        }
        return combo;
    }
}
#endif