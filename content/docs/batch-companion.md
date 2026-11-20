---
title: Batch companion
weight: 3
---

# Batch companion

`activation-batch.txt` is a safe Command Prompt companion. Rename it to `activation-help.cmd` if you want to run it. It opens Windows Activation settings and prints this status command:

```bat
cscript //nologo %windir%\system32\slmgr.vbs /xpr
```

The batch file does not install product keys or change activation settings.
