# Repository Template

This repository is optimized for .NET projects.

```
+
|
|--- .editorconfig
|--- .gitattributes
|--- .gitignore
|--- build.ps1
|--- build.sh
|--- artifacts
|--- docs
|--- lib
|--- LICENSE
|--- NuGet.Config
|--- packages
|--- README.md
|--- samples
|--- src
|--- tests
```

Sample NuGet.Config:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<config>
  		<add key="repositoryPath" value=".\packages" />
	</config>
	<packageSources>
		<add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
	</packageSources>
</configuration>
```