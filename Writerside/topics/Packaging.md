# Publishing Nuget Packages

`dotnet pack -c Release -o ./nupkg`

The Nuget package publishing process includes creating and publishing the nuget packages
for AppLaunch.Admin, AppLaunch.Models, AppLaunch.Services, and AppLaunch.Themes

Go into each project and update the version manually. Once we build it will change the nuget package in the bin folder.
Sign in to nuget.org
Go to manage packages
Upload a new one for each.