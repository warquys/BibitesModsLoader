# BibitesModsLoader

A Library for easily implement mods for [The Bibites Project](https://thebibites.itch.io/the-bibites).

To test your mods you first need to install the modloader, localy: [Installation](https://github.com/warquys/BibitesModsLoader?tab=readme-ov-file#installation). 



## Dev

To create a mod you need to create a dll with the following structure:

```cs
using BibitesModsLoader;

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

[Some tips for Harmony.](https://github.com/warquys/BibitesModsLoader?tab=readme-ov-file#info-and-tips-for-harmony)
[Harmony Documentation.](https://harmony.pardeike.net/)