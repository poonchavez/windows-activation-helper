---
title: Getting started
weight: 1
---

# Getting started

## Requirements

- Windows 11.
- .NET 8 SDK to build the desktop app.
- Hugo and Go to run or build this documentation site. The Book theme currently requires Hugo 0.158 or later.
- A valid product key or digital license for the installed Windows edition.

## Download the app

Download [WindowsActivationHelper.exe](/downloads/WindowsActivationHelper.exe) for 64-bit Windows. It is published as a self-contained app, so installing the .NET runtime is not required. The executable is currently unsigned, so Windows may show a SmartScreen warning. Only run it if you trust the source and have verified the release; do not bypass a warning for an unfamiliar or unverified download.

The downloadable executable is not included in source checkouts until a Windows release is published. To create it, use the publish command below.

## Build the Windows app

From PowerShell or Command Prompt at the repository root:

```powershell
dotnet build .\WindowsActivationHelper\WindowsActivationHelper.vbproj -c Release
dotnet run --project .\WindowsActivationHelper\WindowsActivationHelper.vbproj
```

To create the downloadable, self-contained Windows x64 executable from the repository root:

```powershell
dotnet publish .\WindowsActivationHelper\WindowsActivationHelper.vbproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o .\static\downloads
```

Checking activation status and opening Windows settings do not need elevation. Installing a product key requires launching the app with **Run as administrator**.

## Run the documentation site

The Hugo Book theme is managed as a Go module. From the repository root:

```sh
hugo server
```

Hugo downloads the declared theme module on first run. To generate static files:

```sh
hugo --minify
```

The generated site is written to `public/`.
