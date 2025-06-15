# Mega Subtitles Reborn

Mega Subtitles Reborn is a WPF application for advanced subtitle management, editing, and integration with REAPER. It provides tools for parsing, editing, exporting, and synchronizing subtitle files, with support for multiple languages and actor-based workflows.

## Features

- **Subtitle Parsing & Editing**: Import, parse, and edit subtitles in various formats (ASS, SRT).
- **Actor Management**: Assign colors, rename, and manage actors for subtitle lines.
- **Comment System**: Add, import, export, and manage comments for each subtitle line.
- **Export Options**: Export subtitles and comments in multiple formats, including per-actor and full-project exports.
- **Integration with REAPER**: Lua scripts enable synchronization and automation with REAPER projects.
- **Multi-language UI**: Supports English and Russian, with easy language switching.
- **Update System**: Built-in version checking and update dialog.
- **Customizable Hotkeys**: Common actions are accessible via keyboard shortcuts.

## Requirements

- **.NET 9.0 (Windows)**
- **Windows OS**
- **REAPER** (for full integration and Lua script usage)
- **Lua libraries** (Copy all files contains from 'Required lua librarys' to **Reaper** application directory)

## Getting Started

1. **Clone or Download the Repository**
2. **Open the Solution in Visual Studio 2022**
3. **Restore NuGet Packages** (Newtonsoft.Json)
4. **Build and Run the Project**

### Lua Integration

- Lua scripts are located in the `Lua Wrapper Files` directory.
- For REAPER integration, add 'New Action' -> 'New ReaScript' and choose `Mega Subtitles Wrapper.lua`.

### Fonts

Custom fonts are included in the `Fonts` directory and are embedded as resources.

## Usage

- **Import Subtitles**: Use the UI to select and import subtitle files.
- **Edit & Manage**: Assign actors, colors, and comments. Use context menus and hotkeys for quick actions.
- **Export**: Save subtitles or comments using the export buttons. Choose between full or per-actor exports.
- **Sync with REAPER**: Use the provided Lua scripts to synchronize subtitle data with your REAPER project.

## Hotkeys

| Action                        | Shortcut         |
|-------------------------------|------------------|
| Save Subtitles                | Ctrl + S         |
| Import Comments               | Ctrl + I         |
| Export Full Comments          | Ctrl + E         |
| Export Separated Comments     | Shift + E        |
| Open Cache Folder             | Ctrl + O         |
| Clear Project                 | Ctrl + D         |
| Check for Missing             | Ctrl + M         |
| Check for Repeats             | Ctrl + R         |
| Select All Actors             | Ctrl + A         |
| Parse Subtitles               | Ctrl + P         |
| Find                          | Ctrl + F         |
| Undo Deleted Lines            | Ctrl + Z         |

## Project Structure

- `Mega Subtitles Reborn/` - Main application source code
- `Lua Wrapper Files/` - Lua scripts for REAPER integration
- `Required lua librarys/` - Lua libraries for Reaper
- `Fonts/` - Custom font resources
- `Utilities/` - Helper classes for file IO, parsing, and more

## Troubleshooting

- **Lua Errors**: If you see errors like `attempt to index a nil value (global 'json' or 'colorise' or 'utf8_filenames')` , ensure a Lua librarys are available and required at the top of your Lua scripts:
Place `json.lua` and `colorise.lua` and `utf8_filenames.lua` in the same directory as your Reaper application(by default is `'C:\Program Files\REAPER (x64)')`.

- **.NET Version Issues**: Make sure you have .NET 9.0 SDK installed.

## License

This project is provided under the MIT License.

## Author

Artur Tonoyan  
VK: https://vk.com/aatonoyan  
Discord: arturtonoyan  
Email: artur.tonoyan2012@yandex.ru  
Telegram: @BioWareZ
