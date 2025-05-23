# Scaffolding

In AppLaunch Framework, the scaffolded Identity pages need to be generated and then moved to a separate Razor Class library. 
This can be done by running the following scaffolding commands.

```
Commands go here.
```


The scaffolded pages will be generated in AppLaunch.Core, but we now need to move them to AppLaunch.Admin. Once moved, the _Imports.razor file will need to be updated 
and the following changes made to the signatures so that both AppLaunch.Core and AppLaunch.Admin can access the resources.

```
Class changes go here.
```