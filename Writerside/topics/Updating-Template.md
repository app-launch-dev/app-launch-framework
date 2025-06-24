# Updating Template

Update the AppLaunch.Core Project in the template with any changes from the AppLaunch.Core in the Framework.

In the AppLaunch.Template folder, open the AppLaunchProject.sln file, and to do that, right-click 
on AppLaunch.Template and click "Show Package Contents".

Then update the nuget packages for AppLaunch.Core to the version that we just uploaded (e.g. 1.0.2)
Remove all older versions and Upgrade to lastest on all the packages.

Build then Rebuild Solution so it will regenerate all the stuff that it needs to gerenerate. Maybe on release.

Close the AppLaunchProject.sln

Then edit AppLaunch.Packaging.csproj file and update the PackageVersion.

Commit and push changes to Github.