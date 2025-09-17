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
	public override string Name => "PowerMASM";
	public override string Author => "ExampleAuthor";
	public override string Version => "1.0.0";
	public override string Link => "https://github.com/resonite-modding-group/ExampleMod/";

	public override void OnEngineInit() {
		Harmony harmony = new("com.example.ExampleMod");
		harmony.PatchAll();
	}
	public static void BeforeHotReload()
    {

	}


	public static void OnHotReload(ResoniteMod modInstance)
    {
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
