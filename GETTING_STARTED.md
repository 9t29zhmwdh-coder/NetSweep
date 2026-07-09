# Getting Started with NetSweep

This guide is for people who have never used a terminal or built a .NET application before. It walks you through every step, from opening a terminal to running NetSweep on your own machine.

> 🇩🇪 Looking for German instructions? There isn't a translated version of this guide yet — but the commands below work the same regardless of language.

---

### Windows

NetSweep is a Windows-only desktop app (it uses WPF, which doesn't run on macOS or Linux), so this guide only covers Windows.

#### 1. Open a terminal

1. Right-click the **Start** button (bottom-left of your screen).
2. Choose **Terminal** (Windows 11) or **Windows PowerShell** (Windows 10).
3. A dark window with a blinking cursor opens — that's your terminal. You'll type commands into it and press **Enter** to run them.

#### 2. Check that .NET is installed

Type this and press Enter:

```powershell
dotnet --version
```

- **If you see a version number** (e.g. `8.0.100`) and it starts with `8.`, you're good — skip to step 3.
- **If you see an error** like `'dotnet' is not recognized as an internal or external command`, .NET isn't installed yet (or not the right version).

To install it:

1. Go to [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0).
2. Under **.NET 8.0**, download the **SDK** (not just the "Runtime" — the SDK includes the Runtime plus the tools needed to build the app). Pick the **x64 Installer** for standard Windows PCs.
3. Run the downloaded installer and click through it (default options are fine).
4. Close and reopen your terminal, then run `dotnet --version` again to confirm it now shows `8.x.x`.

#### 3. Get the NetSweep code

You don't need Git for this — a plain ZIP download works fine:

1. Go to the [NetSweep GitHub page](https://github.com/9t29zhmwdh-coder/NetSweep).
2. Click the green **Code** button → **Download ZIP**.
3. Once downloaded, right-click the ZIP file → **Extract All...** → choose a folder you'll remember (e.g. `Documents\NetSweep`).

**Alternative (if you have Git installed):**

```powershell
git clone https://github.com/9t29zhmwdh-coder/NetSweep
```

#### 4. Run NetSweep

In your terminal, navigate into the folder you extracted (adjust the path to where you extracted it):

```powershell
cd Documents\NetSweep
dotnet run --project NetSweep
```

The first run will take a little longer, since `dotnet` needs to download build dependencies and compile the app. Subsequent runs are much faster.

#### 5. What happens next

A window titled **NetSweep** should appear (a "Welcome" screen, followed by the main window). From there you can add a network connection (NAS share, UNC path, mapped SharePoint library, etc.) and start scanning.

> <!-- TODO: Screenshot of the Welcome screen -->

If you'd rather work in a full IDE with debugging, you can instead open `NetSweep.sln` in **Visual Studio 2022** (with the **.NET Desktop Development** workload installed) and press **F5**.

---

## Troubleshooting

| Problem | What's happening | Fix |
|---|---|---|
| `'dotnet' is not recognized as an internal or external command` | .NET SDK isn't installed, or your terminal was opened before installing it | Install the .NET 8 SDK from the link in step 2, then close and reopen your terminal |
| Windows SmartScreen: *"Windows protected your PC"* when running a self-built `.exe` | Windows doesn't recognize the app because it isn't signed by a known publisher — this is expected for apps you build yourself | Click **More info** → **Run anyway**. This is safe here because you built the app yourself from the source code |
| A Windows Firewall prompt appears asking to allow NetSweep network access | NetSweep needs to reach network shares (NAS, UNC paths) you connect to | Click **Allow access** for **Private networks** (your home/office network) |
| `dotnet run` fails with build errors mentioning missing workloads | You may have an incomplete .NET installation | Re-run the .NET 8 SDK installer, or open the solution in Visual Studio and let it restore the **.NET Desktop Development** workload |

If you're still stuck, open an [issue on GitHub](https://github.com/9t29zhmwdh-coder/NetSweep/issues) with the exact error message you see.
