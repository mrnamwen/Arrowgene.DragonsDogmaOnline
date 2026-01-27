# Dragon's Dogma Online - External Resources

This document compiles external resources for Dragon's Dogma Online (DDON) packet dumps, protocol documentation, reverse engineering resources, and community documentation.

---

## GitHub Repositories

### Primary Server Emulator
- **Arrowgene.DragonsDogmaOnline** (Main Project)
  - URL: https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline
  - Description: Server emulator for Dragon's Dogma Online written in C# (.NET 9.0)
  - Contains packet structure definitions, serialization/deserialization, cryptography components
  - License: AGPL-3.0
  - Stars: 213+ | Forks: 74+

- **LegacyDDON Custom Fork**
  - URL: https://github.com/KytheLXI/Arrowgene.DragonsDogmaOnline.LegacyCustom
  - Description: Legacy Custom fork with additional features

### Reverse Engineering Tools (by Andoryuuta)
- **ddon_common_key_bruteforce**
  - URL: https://github.com/Andoryuuta/ddon_common_key_bruteforce
  - Description: Tool for bruteforcing the Camellia key used in DDON Login Server <-> Client exchange
  - Language: C
  - Technical Details: Seeds PRNG by iterating over each millisecond, generates crypto key characters, attempts to decrypt ciphertext against known crib value (L2C_CLIENT_CHALLENGE_RES packet header)

- **ddon_pcap_split**
  - URL: https://github.com/Andoryuuta/ddon_pcap_split
  - Description: Tool for splitting DDON packet captures, distinguishes client-to-server vs server-to-client traffic based on port numbers
  - Language: Go

### Server Management Tools
- **DDOn-Tools** (by alborrajo)
  - URL: https://github.com/alborrajo/DDOn-Tools
  - Description: Server management tools for importing, editing, and exporting custom enemy sets and gathering spots
  - Releases: https://github.com/alborrajo/DDOn-Tools/releases

- **DDON-Launcher**
  - URL: https://github.com/D00MK1D/DDON-Launcher
  - Description: Windows Forms launcher for Dragon's Dogma Online to communicate with private server API

### Translation Projects
- **DDON-Translation**
  - URL: https://github.com/riftcrystal/DDON-Translation
  - Description: Translation patch project for Dragon's Dogma Online

- **DDON-Translation (Alternative)**
  - URL: https://github.com/Sapphiratelaemara/DDON-translation
  - Description: Data repository for building translation patches

### Related Projects (Dragon's Dogma: Dark Arisen)
- **Dune.Emulator**
  - URL: https://github.com/Atvaark/Dune.Emulator
  - Description: Server, Client, and Proxy for Dragon's Dogma: Dark Arisen network protocol (NOT DDO)

- **ArisenTools**
  - URL: https://github.com/mhvuze/ArisenTools
  - Description: Collection of tools for Dragon's Dogma: Dark Arisen (PC version)

---

## Protocol & Encryption Details

### Camellia Encryption
- DDON uses **Camellia block cipher** for network protocol encryption
- Developed by Mitsubishi and NTT
- Login server <-> client exchange uses encrypted challenge-response
- Key cracking tool available (see Andoryuuta's ddon_common_key_bruteforce)

### Packet Structure Naming Convention
From the Arrowgene project:
- `C2S` = Client to Server request
- `S2C` = Server to Client response
- `C2L` / `L2C` = Client/Login server communication

### Network Ports
- TCP 52000: Game Server
- TCP 52100: Login Server
- HTTP 52099: Web/Download Server

### Client Launch Parameters
```
DDO.exe "addr=localhost port=52100 token=00000000000000000000 DL=http://127.0.0.1/win/ LVer=03.04.003.20181115.0 RVer=3040008"
```

---

## Community Wikis

### The White Dragon Temple (Wikidot)
- URL: http://ddon.wikidot.com/
- Description: Original English wiki for Dragon's Dogma Online
- Content: Game guides, quest walkthroughs, item databases, lore

### DDON Miraheze Wiki
- URL: https://ddon.miraheze.org/wiki/Main_Page
- Description: Community wiki with game information
- Discord: https://discord.gg/yeeb2BtP9F
- Note: Based around server created by Dixdros

### Dragon's Dogma Fandom Wiki
- URL: https://dragonsdogma.fandom.com/wiki/Dragon's_Dogma_Online
- Description: General Dragon's Dogma franchise wiki with DDO section

### PCGamingWiki
- URL: https://www.pcgamingwiki.com/wiki/Dragon's_Dogma_Online
- Description: Technical information about the PC version

---

## Discord Communities

### Main DDON Discord
- Invite: https://discord.com/invite/9DfTm4Eb3H
- Alternative: https://discord.com/invite/bTz52trTC6
- Description: Largest DDON community (20,000+ members)

### LegacyDDON Discord
- Invite: https://discord.gg/ezxtdqfeQ6
- Description: North American private server community

### Miraheze Wiki Discord
- Invite: https://discord.gg/yeeb2BtP9F
- Description: Wiki community contact

### White Dragon Temple & Wyrm Hunt
- These are referenced as development/community Discord servers
- Invite links may be found through dd-on.com or other community hubs

---

## Private Servers

### LegacyDDON
- Website: https://legacyddon.com/
- Download Page: https://legacyddon.com/download.html
- Server: legacyddon.com:52100
- Download: legacyddon.com:52099
- Description: First North American private server, custom content, weekly updates

### DDON Server (Original Experience)
- Description: Closest to original game experience
- Community-run using Arrowgene emulator

### Dogma Rising
- Description: Smaller server with custom tweaks, reduced MMO grindiness, faster leveling

---

## Modding Tools

### ARCtool (FluffyQuack)
- Download: https://www.fluffyquack.com/tools/ARCtool.rar
- Batch Files: https://www.fluffyquack.com/tools/ARCtool-PC-batch.rar
- Alternative: https://fluffyquack.com/tools/ARCtool.zip
- Steam Guide: https://steamcommunity.com/sharedfiles/filedetails/?id=601818667
- Description: Unpack and repack ARC files (MT Framework container format)
- Supported formats: TEX (textures), XFS (scripts/variables), LOT (enemy positions), GMD (text)
- DDO Note: Use `-encrypt` flag for encrypted ARCs (though unencrypted may work)

### FluffyQuack's Website
- URL: https://www.fluffyquack.com/
- Description: Home of ARCtool and other modding resources

---

## Key Contributors (Reverse Engineering)

Based on the Arrowgene project README:

| Name | Contribution |
|------|-------------|
| **Ando (Andoryuuta)** | Session Splitter, Camellia Key Cracker tools |
| **David** | PC executable unpacking, anti-debug/CRC defeat |
| **Nothilvien (sebastian-heinz)** | Protocol reverse engineering, server architecture |
| **The White Dragon Temple** | Community organization and support |

---

## Technical Documentation

### Arrowgene Project Documentation
- FAQ: `/docs/faq.md` in repository
- Wiki: Project wiki tracks "What Works?" features

### Development Requirements
- .NET 9.0 SDK or later
- Visual Studio 2022+ or IntelliJ Rider 2021.3+
- SQLite (development) or PostgreSQL (production)

### Database Schema
- SQLite schema files included in project
- Migration scripts for database updates

---

## Additional Resources

### Community Hub
- URL: http://dd-on.com/info.html
- Description: Central information hub with links to developer wiki, modding tools, and community resources
- Note: Site may have connectivity issues

### MT Framework Engine
- PCGamingWiki: https://www.pcgamingwiki.com/wiki/Engine:MT_Framework
- Description: Game engine used by Dragon's Dogma series

---

## Historical Notes

- **Official Servers Shutdown**: December 2019
- **Official Launch**: August 31, 2015
- **Revival Announcement**: Early 2023
- **Main Questlines Restored**: November 2024
- **Engine**: MT Framework (Capcom)
- **Platform**: PC (Windows)
- **Region**: Originally Japan-only, now accessible via private servers

---

## How to Contribute

The Arrowgene project is open source and welcomes contributions:
1. Fork the repository
2. Create feature branches
3. Submit pull requests
4. Join Discord communities for discussion

Technical help needed in areas:
- Packet handler implementation
- Quest system scripting
- Database optimization
- Client data parsing

---

*Last Updated: January 2026*
