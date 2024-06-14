
## BibitesModsLoader PluginInfo generator

Generates `MyPluginInfo.cs` based on csproj tags.

## Basic usage

Define the following properties in your `csproj`:

```xml
<AssemblyName>Example.Plugin</AssemblyName>
<Product>My first plugin</Product>
<Authors>Me</Authors>
<Version>1.0.0</Version>
<Description>My first plugin ! Use to ...</Description>
<ModNicesse>10</ModNicesse> 
```

this will generate the following class:

```cs
namespace MyRootNamespace
{
	public static class ModInfo
	{
		public const string MOD_GUID = "Example.Plugin";
		public const string MOD_NAME = "My first plugin";
		public const string MOD_AUTHORS = "Me";
		public const string MOD_VERSION = "1.0.0";
		public const string MOD_DESCRIPTION = "My first plugin ! Use to ...";
		public const byte MOD_NICESSE = 10;
	}
}		
```

### Value more in deep

The properties `Assemblyname`, `Product`, `Authors`, `Version` and `Description` are used to define `ModGuid`, `ModName`, `ModAuthors`, `ModVersion`, and `ModDescription`.
If you prefer not use the `.NET` property values to define the modinfo constant, you can instead utilize the last described property. For example, you can follow a similar approach as demonstrated with the `ModNicesse` property in the provided example.

```xml
<AssemblyName>Example.Plugin</AssemblyName>
<Product>My first plugin</Product>
<Authors>Me</Authors>
<Version>1.0.0</Version>
<Description>My first plugin ! Use to ...</Description> <--! This one is use to define the out put discription of the dll. -->
<ModDescription>An other desciption</Description> <--! This one is use for the const  value. -->
<ModNicesse>10</ModNicesse> 
```

### Credit

This project is an edit of : [Original](https://github.com/BepInEx/BepInEx.Templates/tree/master/BepInEx.PluginInfoProps).