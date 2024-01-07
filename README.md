# BibitesModsLoader

BibitesModsLoader is a lite loder use to load simple or more complexe mods for the [The Bibites Project](https://thebibites.itch.io/the-bibites).

## Installation

- Download and Unzip the [last verion](https://github.com/warquys/BibitesModsLoader/releases/latest).
- Place `Mod` fodler inside the root folder of The Bibites Project (where the exe is located).
- Replace the `BibitesAssembly.dll` located in The `Bibites_Data\Managed`.

To install a mod just drop the dll mode inside the mod folder, if the mod need som depdancy drop them inside of depdancy.
(only valid if the mode was designed for this loader mode)

## Usage

You can start the program with "-nomods" to disable all mods and use the projet like in vanila.
A log file will be generated at the root folder, the log file is reset at each restart of the projet.

## Dev

To create a mod you need to create a dll with the following structure:

```cs

[Mod("MyMod", "1.0.0", "MyName", Description = "MyMod is a mod for The Bibites Project")]
public class MyMod : IMod
{
	void Enable()
	{
		//... mod code...
	}
}
```

ModAttribute also has a "Nicess" property, the lower it is, the higher the priority of the mod will be.
To change default code of the game you need to use Harmony, you can find more information in this [articles](https://harmony.pardeike.net/articles/patching.html).
  
A version of the assembly release is available in releases. This dll is only there to be used as a dependency during compilation.
If use change the value `AllowUnsafeBlocks` of your csproj to true.
To do this go to property and cherche for "unsafe" change it to true, 
or add `<AllowUnsafeBlocks>True</AllowUnsafeBlocks>` inisde a `PropertyGroup`, 
if it is already present and set to "false" just change the value to "true".

### Info and tips for harmony: 
```cs

[Mod("MyMod", "1.0.0", "MyName", Description = "MyMod is a mod for The Bibites Project")]
public class MyMod : IMod
{
	public Harmony harmony;

	void Enable()
	{
		harmony = new Harmony("mymod");
		harmony.PatchAll();
	}
}

using BibitesModsLoader;

[HarmonyPatch(typeof(ClassToPatch), nameof(MethodToPatch))]
public class PatchMyPatch
{
	[HarmonyPrefix, HarmonyPriority(Priority.First)]
	public bool Prefix1(/** if the method is not static **/ClassToPatch __instance, ... /** parms of the default method **/)
	{
		
		//... patch code...
		reutrn false; // if return false the original code will not be executed
	}

	[HarmonyPrefix]
	public bool Prefix2(...)
	{
		//... patch code...
		reutrn false; // if return false the original code will not be executed
	}

	[HarmonyPostfix]
	public void Postfix1((...)
	{
		//... patch code...
	}
}
```

You can nam your method "Prefix" or "Postfix" to indicate Harmony is a [prefix](https://harmony.pardeike.net/articles/patching-prefix.html) or [postfix](https://harmony.pardeike.net/articles/patching-postfix.html), 
or you can add `HarmonyPrefixAttribute` or `HarmonyPostfixAttribute` to indicate it.
  
A prefix is call beffor the orignal method, a postfix is call after the origanl method.
A prefix can return void or bool, if returing a bool, the value of the bool will indicate if the 
original method will be executed.
  
[Transpiler](https://harmony.pardeike.net/articles/patching-transpiler.html) can be use to change the original code
by edditing the IL of base method. This method is harder but allows you to replace any part of the method and keep a
compatiblity with other patch.
If you whant to do transpiler use a `CodeMatcher`. This will make it easier to read and maintain.
  
To debug the code genrated by harmony you can use `Harmony.DEBUG = true`

### Why use this?

  
This will make it easier to add modes or remove modes for users.
For dev mods will not always need to be recompiled, between each version, 
only the modify dll will have to be replaced.
This project can grow and become a larger library if necessary. 
In the case of frequently patching method or possible conflict between modes,
Code can be added directly to the projet to overall reduce the code and conflict.
  
You can also if you need create APIs shared between several modes, 
the API must either be added to this project or loaded as a mod. 
If the project is loaded as a mod then it must have a lower level of nicess
than the modes who is using it.
  
In the case of an API and patch performed by the API. 
It strongly advised to take the entire method code with modification and place 
it in a prefix that returns false every time.
why? this will allow better readability of the code, although less stable, 
it will be much easier and faster to maintain or edit (copy/past if breaking change from an update and redit the code).

Or use a Transpiler if you are determined.
