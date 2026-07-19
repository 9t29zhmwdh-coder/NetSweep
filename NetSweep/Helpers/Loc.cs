using System.ComponentModel;

namespace NetSweep.Helpers;

/// <summary>
/// Minimal runtime EN/DE localization. English is the default language; German is the
/// opt-in toggle (see MEMORY: UI language default). Bind XAML to the named properties
/// via {Binding Source={x:Static Helpers:Loc.Instance}, Path=Xxx}; call
/// <see cref="Get"/> directly from C# for messages/status text.
/// </summary>
public sealed class Loc : INotifyPropertyChanged
{
    public static Loc Instance { get; } = new();

    private static readonly Dictionary<string, (string En, string De)> Strings = new()
    {
        // Main window
        ["MainTitle"] = ("NetSweep - Connections", "NetSweep - Verbindungen"),
        ["ConnectionsHeading"] = ("Network Drive Connections", "Verbindungen zu Netzlaufwerken"),
        ["Add"] = ("Add", "Hinzufügen"),
        ["Edit"] = ("Edit", "Bearbeiten"),
        ["Remove"] = ("Remove", "Entfernen"),
        ["Connect"] = ("Connect", "Verbinden"),
        ["OpenAnalyze"] = ("Open / Analyze", "Öffnen / Analysieren"),
        ["ColumnName"] = ("Name", "Name"),
        ["ColumnPath"] = ("Path", "Pfad"),
        ["ColumnUser"] = ("User", "Benutzer"),
        ["ColumnStatus"] = ("Status", "Status"),
        ["Connected"] = ("Connected", "Verbunden"),
        ["Disconnected"] = ("Disconnected", "Getrennt"),

        // MainViewModel status / dialogs
        ["StatusReady"] = ("Ready.", "Bereit."),
        ["StatusConnecting"] = ("Connecting...", "Verbinde..."),
        ["StatusConnectionAdded"] = ("Connection added.", "Verbindung hinzugefügt."),
        ["StatusConnectionSaved"] = ("Connection saved.", "Verbindung gespeichert."),
        ["StatusConnectionRemoved"] = ("Connection removed.", "Verbindung entfernt."),
        ["NewConnectionName"] = ("New Connection", "Neue Verbindung"),
        ["RemoveConfirmTitle"] = ("Remove Connection", "Verbindung entfernen"),
        ["RemoveConfirmMessage"] = ("Really remove connection „{0}“?\n\n(No files will be deleted, only this entry.)", "Verbindung „{0}“ wirklich entfernen?\n\n(Es werden keine Dateien gelöscht, nur dieser Eintrag.)"),
        ["ConnectionDialogTitle"] = ("Connection", "Verbindung"),

        // NetworkConnectionService messages
        ["NoPathGiven"] = ("No path given.", "Kein Pfad angegeben."),
        ["ConnectedLocalPath"] = ("Connected (local path).", "Verbunden (lokaler Pfad)."),
        ["PathNotFound"] = ("Path not found.", "Pfad nicht gefunden."),
        ["ConnectionEstablished"] = ("Connection established.", "Verbindung hergestellt."),
        ["AuthenticatedPathUnreachable"] = ("Authenticated, but path unreachable (code {0}).", "Authentifiziert, aber Pfad nicht erreichbar (Code {0})."),
        ["AccessDenied"] = ("Access denied. Check username or password.", "Zugriff verweigert. Benutzername oder Passwort prüfen."),
        ["NetworkPathNotFound"] = ("Network path not found. Is the NAS reachable?", "Netzwerkpfad nicht gefunden. NAS erreichbar?"),
        ["NetworkNameNotFound"] = ("Network name not found.", "Netzwerkname nicht gefunden."),
        ["WrongPassword"] = ("Wrong password.", "Falsches Passwort."),
        ["LoginFailed"] = ("Login failed: wrong username or password.", "Anmeldung fehlgeschlagen: falscher Benutzername oder Passwort."),
        ["ConnectionFailedCode"] = ("Connection failed (Windows error code {0}).", "Verbindung fehlgeschlagen (Windows-Fehlercode {0})."),

        // WelcomeWindow
        ["WelcomeTitle"] = ("NetSweep - Welcome", "NetSweep - Willkommen"),
        ["WelcomeSubtitle"] = ("Network Storage Cleanup", "Network Storage Cleanup"),
        ["WelcomeSafetyHeading"] = ("Quick & important safety notes", "Kurz & wichtig zur Sicherheit"),
        ["WelcomeRecommendedHeading"] = ("Recommended approach", "Empfohlenes Vorgehen"),
        ["WelcomeCapabilitiesHeading"] = ("What you can do", "Was du tun kannst"),
        ["WelcomeDontShowAgain"] = ("I understand this notice", "Diesen Hinweis kenne ich"),
        ["WelcomeClose"] = ("Close", "Schliessen"),
        ["WelcomeStart"] = ("Start", "Starten"),

        // ConnectionEditDialog
        ["EditConnectionTitle"] = ("Edit Connection", "Verbindung bearbeiten"),
        ["FieldName"] = ("Name", "Name"),
        ["FieldPath"] = ("Path (UNC / folder)", "Pfad (UNC / Ordner)"),
        ["FieldUsername"] = ("Username", "Benutzername"),
        ["FieldPassword"] = ("Password", "Passwort"),
        ["FieldQuarantineFolder"] = ("Quarantine folder", "Quarantäne-Ordner"),
        ["PathHint"] = ("Example path: \\\\nas01\\Cleanup or a local folder. The password is encrypted and stored only for your Windows account. Quarantine is optional (target for \"move instead of delete\").", "Beispiel-Pfad: \\\\nas01\\Cleanup oder ein lokaler Ordner. Das Passwort wird verschlüsselt und nur für dein Windows-Konto gespeichert. Quarantäne ist optional (Ziel für „verschieben statt löschen“)."),
        ["Cancel"] = ("Cancel", "Abbrechen"),
        ["Save"] = ("Save", "Speichern"),
        ["ChooseFolderTitle"] = ("Choose folder / network drive", "Ordner / Netzlaufwerk wählen"),
        ["ChooseQuarantineFolderTitle"] = ("Choose quarantine folder", "Quarantäne-Ordner wählen"),
        ["PathRequiredTitle"] = ("Required Field", "Pflichtfeld"),
        ["PathRequiredMessage"] = ("Please provide a path.", "Bitte einen Pfad angeben."),

        // AnalysisWindow toolbar / tabs
        ["AnalysisTitleFormat"] = ("Analysis: {0}  ({1})", "Analyse: {0}  ({1})"),
        ["Scan"] = ("Scan", "Scannen"),
        ["CancelAction"] = ("Cancel", "Abbrechen"),
        ["FindDuplicates"] = ("Find Duplicates", "Duplikate suchen"),
        ["ShowEmptyFolders"] = ("Show Empty Folders", "Leere Ordner zeigen"),
        ["DeleteEmptyFolders"] = ("Delete Empty Folders", "Leere Ordner löschen"),
        ["TabFilesCleanup"] = ("Files & Cleanup", "Dateien & Bereinigung"),
        ["TabFolderSize"] = ("Folder Size (TreeSize)", "Ordnergrösse (TreeSize)"),
        ["TabDuplicates"] = ("Duplicates", "Duplikate"),
        ["FilterOlderThanDays"] = ("Older than (days):", "Älter als (Tage):"),
        ["FilterLargerThan"] = ("Larger than:", "Grösser als:"),
        ["FilterLargerThanTooltip"] = ("e.g. 500 MB, 2 GB, 1048576", "z.B. 500 MB, 2 GB, 1048576"),
        ["FilterExtension"] = ("Extension:", "Endung:"),
        ["FilterExtensionTooltip"] = ("e.g. .tmp, .log, .iso (comma-separated)", "z.B. .tmp, .log, .iso (mehrere mit Komma)"),
        ["FilterNameContains"] = ("Name contains:", "Name enthält:"),
        ["FilterApply"] = ("Filter", "Filtern"),
        ["FilterReset"] = ("Reset", "Zurücksetzen"),
        ["QuickOld"] = ("Quick: old (1 yr.)", "Schnell: alt (1 J.)"),
        ["QuickLarge"] = ("Quick: large (>500MB)", "Schnell: gross (>500MB)"),
        ["DeletePermanently"] = ("Delete Permanently", "Endgültig löschen"),
        ["MoveToQuarantine"] = ("Move to Quarantine", "In Quarantäne verschieben"),
        ["CopyBackup"] = ("Copy / Backup...", "Kopieren / Backup..."),
        ["ExportCsv"] = ("Export as CSV", "Als CSV exportieren"),
        ["MultiSelectHint"] = ("Tip: select multiple rows with Ctrl/Shift.", "Tipp: mehrere Zeilen mit Strg/Shift markieren."),
        ["ColumnSize"] = ("Size", "Grösse"),
        ["ColumnModified"] = ("Modified", "Geändert"),
        ["ColumnAgeDays"] = ("Age (days)", "Alter (Tage)"),
        ["ColumnType"] = ("Type", "Typ"),
        ["ColumnSizePerFile"] = ("Size per file", "Grösse je Datei"),
        ["ColumnCount"] = ("Count", "Anzahl"),
        ["ColumnReclaimable"] = ("Reclaimable", "Freigebbar"),
        ["FileCountSuffix"] = (" ({0} files)", " ({0} Dateien)"),

        // AnalysisViewModel status / dialogs
        ["ClickScanToStart"] = ("Click “Scan” to start.", "Klicke „Scannen“, um zu starten."),
        ["Scanning"] = ("Scanning...", "Scanne..."),
        ["ScanSummaryFormat"] = ("{0:N0} files, {1:N0} folders, {2} total, {3} empty folders.", "{0:N0} Dateien, {1:N0} Ordner, {2} gesamt, {3} leere Ordner."),
        ["ScanDoneWithWarnings"] = ("Scan finished with {0} warning(s) (e.g. missing permissions).", "Scan fertig mit {0} Warnung(en) (z.B. fehlende Rechte)."),
        ["ScanComplete"] = ("Scan complete.", "Scan abgeschlossen."),
        ["ScanCancelled"] = ("Scan cancelled.", "Scan abgebrochen."),
        ["ScanError"] = ("Error during scan.", "Fehler beim Scannen."),
        ["ScanErrorTitle"] = ("Scan Error", "Scan-Fehler"),
        ["FilesFilteredFormat"] = ("{0:N0} files filtered ({1}).", "{0:N0} Dateien gefiltert ({1})."),
        ["SearchingDuplicates"] = ("Searching for duplicates (hashing files)...", "Suche Duplikate (Dateien werden gehasht)..."),
        ["DuplicateGroupsFormat"] = ("{0} duplicate groups, up to {1} reclaimable.", "{0} Duplikatgruppen, bis zu {1} freigebbar."),
        ["DuplicateSearchCancelled"] = ("Duplicate search cancelled.", "Duplikatsuche abgebrochen."),
        ["NoEmptyFoldersFound"] = ("No empty folders found.", "Keine leeren Ordner gefunden."),
        ["EmptyFoldersTitle"] = ("Empty Folders", "Leere Ordner"),
        ["EmptyFoldersCountTitleFormat"] = ("{0} Empty Folders", "{0} leere Ordner"),
        ["NoEmptyFoldersToRemove"] = ("No empty folders to remove.", "Keine leeren Ordner zum Entfernen."),
        ["RemoveEmptyFoldersConfirmFormat"] = ("Remove {0} empty folders?", "{0} leere Ordner entfernen?"),
        ["RemoveEmptyFoldersTitle"] = ("Remove Empty Folders", "Leere Ordner entfernen"),
        ["EmptyFoldersResultFormat"] = ("Empty folders: {0}", "Leere Ordner: {0}"),
        ["NoSelectionMessage"] = ("Please select one or more files in the list first.", "Bitte zuerst eine oder mehrere Dateien in der Liste markieren."),
        ["NoSelectionTitle"] = ("No Selection", "Keine Auswahl"),
        ["DeleteConfirmFormat"] = ("Permanently delete {0} file(s) ({1})?", "{0} Datei(en) ({1}) endgültig löschen?"),
        ["DeleteConfirmTitle"] = ("Really Delete?", "Wirklich löschen?"),
        ["DeleteFinalWarning"] = ("WARNING: These files will NOT be moved to the Recycle Bin.\nThey CANNOT be recovered afterwards.\n\nDelete permanently now?", "ACHTUNG: Diese Dateien werden NICHT in den Papierkorb verschoben.\nSie können danach NICHT wiederhergestellt werden.\n\nJetzt endgültig löschen?"),
        ["FinalWarningTitle"] = ("Final Warning", "Letzte Warnung"),
        ["DeleteResultPrefix"] = ("Delete: ", "Löschen: "),
        ["ChooseQuarantineFolderPrompt"] = ("Choose quarantine folder", "Quarantäne-Ordner wählen"),
        ["MoveToQuarantineConfirmFormat"] = ("Move {0} file(s) to\n{1}\n(quarantine)?", "{0} Datei(en) nach\n{1}\nverschieben (Quarantäne)?"),
        ["MoveToQuarantineTitle"] = ("Move to Quarantine", "In Quarantäne verschieben"),
        ["QuarantineResultPrefix"] = ("Quarantine: ", "Quarantäne: "),
        ["ChooseCopyTargetTitle"] = ("Choose target folder for copy / backup", "Zielordner für Kopie / Backup wählen"),
        ["CopyResultPrefix"] = ("Copy: ", "Kopieren: "),
        ["ExportFilesDialogTitle"] = ("Export File List", "Dateiliste exportieren"),
        ["ExportDuplicatesDialogTitle"] = ("Export Duplicates", "Duplikate exportieren"),
        ["CsvFilterLabel"] = ("CSV File (*.csv)|*.csv", "CSV-Datei (*.csv)|*.csv"),
        ["ExportedPrefix"] = ("Exported: ", "Exportiert: "),
        ["ErrorsCountTitleFormat"] = ("{0} Errors", "{0} Fehler"),
        ["MoreSuffixFormat"] = ("\n... (+{0} more)", "\n... (+{0} weitere)"),

        // Language toggle
        ["LanguageToggleLabel"] = ("DE", "EN"),
        ["LanguageToggleTooltip"] = ("Switch to German", "Auf Englisch umschalten"),
    };

    private string _currentLanguage = "en";
    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage == value) return;
            _currentLanguage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }

    public bool IsGerman => CurrentLanguage == "de";
    public bool IsEnglish => CurrentLanguage == "en";

    public void Toggle() => CurrentLanguage = CurrentLanguage == "en" ? "de" : "en";

    public string Get(string key) => Strings.TryGetValue(key, out var pair)
        ? (CurrentLanguage == "de" ? pair.De : pair.En)
        : key;

    public string Get(string key, params object[] args) => string.Format(Get(key), args);

    /// <summary>Enables XAML indexer bindings: Path=[SomeKey].</summary>
    public string this[string key] => Get(key);

    // Named convenience properties for direct XAML bindings.
    public string MainTitle => Get("MainTitle");
    public string ConnectionsHeading => Get("ConnectionsHeading");
    public string Add => Get("Add");
    public string Edit => Get("Edit");
    public string Remove => Get("Remove");
    public string Connect => Get("Connect");
    public string OpenAnalyze => Get("OpenAnalyze");
    public string ColumnName => Get("ColumnName");
    public string ColumnPath => Get("ColumnPath");
    public string ColumnUser => Get("ColumnUser");
    public string ColumnStatus => Get("ColumnStatus");
    public string LanguageToggleLabel => Get("LanguageToggleLabel");
    public string LanguageToggleTooltip => Get("LanguageToggleTooltip");

    public event PropertyChangedEventHandler? PropertyChanged;
}
