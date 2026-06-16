<div align="center">
  <img src="RayStudio.png" alt="RayStudio Logo" width="120"/>

  <h1>NetSweep – Netzwerkspeicher bereinigen</h1>
</div>

> 🇬🇧 [English Version](README.md)

# NetSweep – Netzwerkspeicher bereinigen

Eine Windows-Desktop-App (WPF, .NET 8) zum Prüfen und Bereinigen von Netzlaufwerken (NAS / UNC-Pfade). Verbindungen verwalten, Speicherbelegung pro Ordner visualisieren, alte/grosse/doppelte Dateien finden, leere Ordner entfernen sowie Dateien kopieren, in Quarantäne verschieben oder endgültig löschen.

## Funktionen

- **Verbindungsverwaltung** — Mehrere NAS/UNC-Verbindungen anlegen, bearbeiten und verbinden
- **Speicheransicht** — Aggregierte Speicherbelegung je Ordner mit Prozentanteil
- **Dateifilter** — Nach Alter (Tage), Grösse, Endung oder Dateiname filtern
- **Duplikaterkennung** — Identische Dateien per SHA-256-Hash finden, freigebbaren Platz sehen
- **Leere Ordner** — Leere Verzeichnisse auflisten und entfernen
- **Aktionen** — Endgültig löschen (zweifache Bestätigung), Quarantäne, Kopieren/Backup, CSV-Export

## Voraussetzungen

- Windows 10 / 11
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (oder self-contained veröffentlichen)
- Visual Studio 2022 (17.8+) mit Workload **".NET-Desktopentwicklung"** *(nur zum Bauen)*

## Erste Schritte

```bash
# Projektmappe öffnen
NetSweep.sln   # → Visual Studio → F5

# Oder per CLI
dotnet build
dotnet run --project NetSweep
```

**Eigenständige .exe veröffentlichen:**
```bash
dotnet publish NetSweep/NetSweep.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Projektstruktur

```
NetSweep.sln
└─ NetSweep/
   ├─ App.xaml(.cs)       App-Einstieg, Welcome → Hauptfenster
   ├─ Models/             Datenmodelle (Verbindung, FileEntry, FolderNode, ScanResult)
   ├─ Services/           Logik (Scan, Duplikate, Dateioperationen, Verschlüsselung, CSV)
   ├─ ViewModels/         MVVM (MainViewModel, AnalysisViewModel, RelayCommand)
   ├─ Views/              XAML-Fenster (Welcome, Main, Verbindung bearbeiten, Analyse)
   └─ Helpers/            Hilfsfunktionen (Grössen-Formatierung)
```

## Sicherheit

- Passwörter werden mit **Windows DPAPI** (CurrentUser-Scope) verschlüsselt — niemals im Klartext
- Gespeichert unter `%AppData%\NetSweep\connections.json` — von Versionskontrolle ausgeschlossen
- **Löschen ist endgültig** (kein Papierkorb) — zweifache Bestätigung vor der Ausführung
- Empfehlung: eingeschränktes NAS-Konto mit Schreibrechten nur auf relevante Ordner verwenden

## Roadmap

- [ ] Geplante/automatische Scans
- [ ] Inkrementelles Backup mit Versionierung
- [ ] In Windows-Papierkorb verschieben als Option
- [ ] Dateityp-Statistik (Diagramm)
- [ ] Mehrere Pfade gleichzeitig scannen
- [ ] Audit-Protokoll für alle Lösch-/Verschiebeaktionen

---

**Author:** [Rafael Yilmaz](https://github.com/9t29zhmwdh-coder) &nbsp;·&nbsp; **Status:** Early Release &nbsp;·&nbsp; **Last Updated:** June 2026
