using System;
using System.Buffers;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;

using PowerNite.PowerShell.Extentions;

using ResoniteHotReloadLib;
using ResoniteModLoader;
namespace PowerMASM;
// More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class PowerMASMMod : ResoniteMod
{
    internal const string VERSION_CONSTANT = "1.0.0";
    public override string Name => "PowerMASM";
    public override string Author => "Finite";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://git.finite.ovh/PowerNite";

	public static string Domain = "ovh.finite.PowerMASM";
	public static PowerMASMMod INSTANCE { get; private set; }
	public static void Msg(string msg)
	{
		Console.WriteLine($"[{INSTANCE.Name}] {msg}");
	}
	public static void Debug(string msg) {
		Console.WriteLine($"[{INSTANCE.Name} Debug] {msg}");
	}

    public static void BeforeHotReload()
    {
		Harmony harm = new Harmony(Domain);
		harm.UnpatchAll(Domain);
		HotReloader.RemoveMenuOption("PowerMASM", "Create MASMEditor");

	}


public static void OnHotReload(ResoniteMod modInstance)
    {

	
		Console.WriteLine($"[{modInstance.Name}] Hot Reloaded");
		Setup();
		Console.WriteLine($"[{modInstance.Name}] Hot Reload Setup Complete");
		//Msg("PowerMASM Hot Reloaded");
	}

    public override void OnEngineInit()
    {
        // Assign the static instance field
        INSTANCE = this;

        HotReloader.RegisterForHotReload(this);
        Setup();
		Debug($"{Name} OnEngineInit Complete");
	}

	public static void Setup() {
		AddNewMenuOption("PowerMASM", "Create MASMEditor", () =>
		{
			UIXMLParser parser = new UIXMLParser();
			parser.Render("<canvas height=\"800\"></canvas>");
			parser.RootSlot.Name = "MASMEditor";
			parser.RootSlot.PositionInFrontOfUser(float3.Backward, new float3(0,0,-2f));
		});
	}

	public static void AddNewMenuOption(string path, string name, Action reloadAction)
    {
		//Debug("Begin AddReloadMenuOption");
		if (!Engine.Current.IsInitialized) {
			Engine.Current.RunPostInit(AddActionDelegate);
		} else {
			AddActionDelegate();
		}

		void AddActionDelegate()
        {
            DevCreateNewForm.AddAction(path, name, (x) =>
            {
				x.Destroy();
				reloadAction();
            });
            //Debug($"Added {INSTANCE.Name}'s option {name} for path {path}");
            
        }
    }
}
