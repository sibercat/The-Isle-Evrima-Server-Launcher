# 🦖 The Isle: Evrima Server Launcher

A modern, feature-rich dedicated server manager for **The Isle: Evrima**, built with **C# WPF** and **.NET 10**. This launcher simplifies the complex process of setting up, configuring, and maintaining an Evrima server by providing a unified graphical interface for all administrative tasks.

![Dashboard](https://raw.githubusercontent.com/sibercat/The-Isle-Evrima-Server-Launcher/refs/heads/main/docs/Dashboard.webp)

## ✨ Key Features
Executable can be found here [Releases](https://github.com/sibercat/IsleServerLauncher/releases)

### 🚀 Server Management
- **Automated Install/Update:** Built-in SteamCMD integration to install, update, and validate server files.
- **Crash Detection:** Automatically detects server crashes and restarts the process with configurable attempt limits.
- **Performance Tuning:** CPU core affinity/priority management and built-in network optimization (VirtIO/packet drop fixes).
- **Environment Fixes:** One-click fixes for common issues like SSL certificate errors (Epic Online Services) and firewall configuration.
- **Presets:** Save, load, import, export, and manage presets for quick server setups.

### ⚙️ Easy Configuration
- **GUI Settings Editor:** Modify gameplay rules without touching `Game.ini` or `Engine.ini`.
- **Gameplay Control:** Adjust day/night cycles, AI density, growth/decay rates, migration zones, and more.
- **Dino Whitelist:** Easy-to-use checklist to enable/disable specific playable dinosaurs.
- **Disallowed AI:** Checkbox list to disable specific AI species.
- **Extended Game.ini Parameters:** Region spawning, diets, patrol zones, mass migration, weather intervals, queue timeouts, and more.

### 🛠️ Administrator & RCON Tools
- **Player Management:** Live player list with Kick, Ban, and Direct Message functionality.
- **Broadcast System:** Send global announcements to all players.
- **Live Controls:** Toggle global chat, AI, humans, or whitelist on the fly.
- **Chat Monitor:** Real-time server chat log viewer with colored tagging (Global/Local/Admin).
- **RCON Tools:** Auto-save (world data), auto-wipe corpses, and live RCON output.

### 🤖 Automation & Integrations
- **Discord Webhooks:** Get notifications for server status (Start/Stop/Crash).
- **Chat Relay:** Forward in-game chat directly to a Discord channel.
- **Scheduled Restarts:** Automated restart intervals with in-game warning countdowns.
- **Post-Restart Script:** Run a configurable script after restarts, with delay.
- **Auto-Backups:** Schedule automated backups of your server's save data.

### 🧩 Mods
- **Mod Manager (Lightweight):** Manage mod loader/dll paths and config folder.
- **Injection Modes:** Built-in injector (auto-injects on every server start) or StartServerWithMod.bat (advanced).
- **Post-Restart Auto-Inject:** Bat method can be configured to automatically run after scheduled restarts with delay.

### 🖼️ UI/UX
- **Modern Interface:** Clean WPF design with Light and Dark mode support.
- **Toast Notifications:** Non-intrusive status updates for actions.
- **Update Check:** Simple GitHub Releases update checker.

## 🔧 Built With
- **Language:** C#
- **Framework:** WPF (.NET 10)
- **Styling:** XAML with custom control templates
- **Services:** SteamCMD, RCON

## 📦 Installation
1. Download the latest [Release](https://github.com/sibercat/IsleServerLauncher/releases).
2. Run `IsleServerLauncher.exe` as Administrator (required for network/firewall configuration).
3. Click **Install Server** on the dashboard to download the necessary files via SteamCMD.
4. Configure your settings in the **Server Settings** tab.
5. Click **Start Server**.

## ⚠️ Requirements
- Windows 10/11 or Windows Server 2019+
- Visual C++ Redistributable

---

This project is not affiliated with Afterthought LLC or The Isle development team.
