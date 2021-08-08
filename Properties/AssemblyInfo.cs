using System.Reflection;
using System.Runtime.InteropServices;
using static NotOptifine.BuildInfo;
using MelonLoader;

[assembly: AssemblyTitle(Name)]
[assembly: AssemblyDescription(Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Company)]
[assembly: AssemblyProduct(Name)]
[assembly: AssemblyCopyright("Copyright © " + Author + " 2021")]
[assembly: AssemblyTrademark(Author)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("b22f4ccd-08f5-4aeb-8bb3-7a1973e702a1")]
[assembly: AssemblyVersion(Version)]
[assembly: AssemblyFileVersion(Version)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonInfo(typeof(NotOptifine.Main), Name, Version, Author, DownloadLink)]
[assembly: MelonColor(System.ConsoleColor.DarkCyan)]
//[assembly: MelonOptionalDependencies("")]