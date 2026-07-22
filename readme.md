# Create a empty project

1. ```dotnet new addin-project -P TiaAddin -N TiaAddin ```

2. ```dotnet new addin-project-tree-menu -P TiaAddin -N TiaAddin ```

3. Check correct TIA version in csproj-file, 

One example from the generated project files
```
<HintPath>@(TiaPortalLocation)\PublicAPI\V19.AddIn\Siemens.Engineering.AddIn.dll</HintPath>
```
New
```
<HintPath>@(TiaPortalLocation)\PublicAPI\V20.AddIn\Siemens.Engineering.AddIn.dll</HintPath>
```

Also in in the `Config.xml`

```
<PackageConfiguration xmlns="http://www.siemens.com/automation/Openness/AddIn/Publisher/V19">
```

```
<PackageConfiguration xmlns="http://www.siemens.com/automation/Openness/AddIn/Publisher/V20">
```

4.Modify namespace of C# files

5. Try to build, run `Ctrl+Shift+P` in VS Code and select `Run build task`

If it worked, you should end up with a addin in the `bin/debug`folder that you can drag into C:/Programs/<TIA INSTALL FOLDER>/Addins

