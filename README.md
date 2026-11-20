# Windows Activation Helper

This repository contains a Visual Basic .NET Windows Forms app, a safe batch companion, and a Hugo documentation website using the [Hugo Book theme](https://github.com/alex-shpak/hugo-book).

## Windows app

Requirements: Windows 11, .NET 8 SDK, and a valid Windows license for the installed edition.

From PowerShell or Command Prompt at the repository root:

```powershell
dotnet build .\WindowsActivationHelper\WindowsActivationHelper.vbproj -c Release
dotnet run --project .\WindowsActivationHelper\WindowsActivationHelper.vbproj
```

The app checks activation status, opens Windows Activation settings, and can install a product key supplied by the user. Installing a key requires running as administrator and confirming the operation.

To publish self-contained Windows x64 and x86 executables for the website downloads:

```powershell
dotnet publish .\WindowsActivationHelper\WindowsActivationHelper.vbproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o .\static\downloads
dotnet publish .\WindowsActivationHelper\WindowsActivationHelper.vbproj -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:AssemblyName=WindowsActivationHelper-x86 -o .\static\downloads
```

This creates `static/downloads/WindowsActivationHelper.exe` and `static/downloads/WindowsActivationHelper-x86.exe`. Hugo serves both files from `/downloads/`. Publish the site after generating the executables.

## Hugo documentation website

The Hugo site is configured at the repository root. Its source pages are in `content/`, and the Book theme is pinned as a Go module dependency.

Requirements: Hugo 0.158 or later, Go 1.25 or later, and Git.

```sh
hugo server
```

Hugo downloads the theme module on first run. Build the static site with:

```sh
hugo --minify
```

The generated website is written to `public/`.

## Batch companion and licensing

`activation-batch.txt` is a safe Command Prompt companion. Rename it to `activation-help.cmd` to run it. It opens Windows Activation settings and prints the Windows status command; it does not install product keys or change activation settings.

Use only a genuine product key or digital license assigned to you. This project does not supply keys, alter activation servers, or bypass Microsoft's licensing.
