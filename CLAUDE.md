# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **Arrowgene.DragonsDogmaOnline**, a server emulator for Dragon's Dogma Online written in C# targeting .NET 10.0. The server consists of four main components: Web Server (port 52099), Login Server (port 52100), Game Server (port 52000), and a database layer (SQLite or PostgreSQL).

## Build Commands

```bash
# Build the entire solution
dotnet build Arrowgene.DragonsDogmaOnline.sln

# Build in Release mode
dotnet build Arrowgene.DragonsDogmaOnline.sln -c Release

# Publish for distribution (creates self-contained executables)
./publish.sh                    # Linux/macOS
./publish.cmd                   # Windows

# Run the server
dotnet run --project Arrowgene.Ddon.Cli server start

# Run with custom config
dotnet run --project Arrowgene.Ddon.Cli server start --config=path/to/config.json
```

## Testing

```bash
# Run all tests (uses xUnit)
dotnet test Arrowgene.Ddon.Test/Arrowgene.Ddon.Test.csproj

# Run a specific test
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"
```

## Database Migration

```bash
# Run database migrations
dotnet run --project Arrowgene.Ddon.Cli dbmigration
```

## Architecture Overview

### Project Structure

- **Arrowgene.Ddon.Cli** - Entry point CLI application; handles command parsing and server startup
- **Arrowgene.Ddon.GameServer** - Core game server logic with ~490 packet handlers in `Handler/` directory
- **Arrowgene.Ddon.LoginServer** - Authentication and character selection
- **Arrowgene.Ddon.WebServer** - HTTP server for client downloads
- **Arrowgene.Ddon.Server** - Base server abstractions and network layer
- **Arrowgene.Ddon.Shared** - Shared models, entities, packet structures, and serialization
- **Arrowgene.Ddon.Database** - Database abstraction supporting SQLite and PostgreSQL
- **Arrowgene.Ddon.Scripts** - C# scripting system for quests, settings, and game configuration
- **Arrowgene.Ddon.Rpc/Rpc.Web** - Inter-server communication for multi-channel setups
- **Arrowgene.Ddon.Client** - Client data file parsing utilities

### Key Architectural Patterns

**Packet Handler Pattern**: Each client request type has a dedicated handler class in `GameServer/Handler/`. Handlers follow the naming convention `{Category}{Action}Handler.cs` (e.g., `BazaarExhibitHandler.cs`).

**Manager Pattern**: The `DdonGameServer` class instantiates ~30 manager classes for different game systems (ItemManager, QuestManager, PartyManager, etc.). These are found in `GameServer/Characters/`.

**Entity Serialization**: Packet structures are defined in `Shared/Entity/PacketStructure/` with naming convention:
- `C2S` = Client to Server request
- `S2C` = Server to Client response
- `C2L` / `L2C` = Client/Login server communication

**Scripting System**: Game logic is extensible via C# scripts (`.csx` files) in the `scripts/` directory of the assets folder. Scripts support hot-reloading and can define quests, chat commands, drop tables, and server settings.

### Configuration

The server uses JSON configuration files. Default location: `Files/Arrowgene.Ddon.config.json`. Sample configs are provided:
- `Arrowgene.Ddon.config.local_dev.json` - Development with SQLite
- `Arrowgene.Ddon.config.psql.local_dev.json` - Development with PostgreSQL

### Scripting Modules

Scripts are organized under `<assets>/scripts/`:
- `settings/` - Server configuration (XP rates, drop rates, etc.)
- `quests/msq/` - Main story quests organized by season
- `quests/world/` - World quests organized by area
- `quests/seasonal_events/` - Event quests
- `chat_commands/` - In-game chat commands
- `enemies/drop_generators/` - Custom enemy loot tables

## Code Style

- Use PascalCase for classes, methods, properties, constants
- Use camelCase for method arguments and local variables
- Use _camelCase (underscore prefix) for private fields
- Use the built-in logger (`LogProvider.Logger`) instead of `Console.WriteLine`

## Git Workflow

- Work on feature branches: `feature/feature-name` or `fix/bug-fix-name`
- Create pull requests to merge into `develop` (main development branch)
