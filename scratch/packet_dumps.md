# DDON Packet Dumps and Protocol Documentation

This document summarizes the community resources for Dragon's Dogma Online (DDON) packet captures, protocol documentation, and analysis tools.

## Table of Contents

1. [Overview](#overview)
2. [Protocol Documentation in Repository](#protocol-documentation-in-repository)
3. [Community Tools for Packet Analysis](#community-tools-for-packet-analysis)
4. [GitHub Repositories](#github-repositories)
5. [Community Resources](#community-resources)
6. [Technical Details](#technical-details)

---

## Overview

Dragon's Dogma Online (DDON) was an MMORPG developed by Capcom that was officially shut down in December 2019. The community has since reverse-engineered the game's protocol to create private server implementations. The primary project is **Arrowgene.DragonsDogmaOnline**.

### Key Contributors to Reverse Engineering

- **Ando (Andoryuuta)** - Reverse Engineering & Tooling (Session Splitter, Camellia Key Cracker)
- **David** - Reverse Engineering (unpacking PC Executable, defeating Anti Debug and CRC checks)
- **Nothilvien (sebastian-heinz)** - Reverse Engineering & Server Code
- **The White Dragon Temple** - Community wiki and resources

---

## Protocol Documentation in Repository

The current repository contains extensive protocol documentation in the `research/` folder:

### Game Packets Documentation

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/research/GamePackets.md`

Contains comprehensive packet definitions for DDON v03.04.007, including:

- **Group 0 - CONNECTION**: Login, logout, server movement packets
- **Group 1 - SERVER**: Server list, game settings, weather, time
- **Group 2 - CHARACTER**: Character actions, revival, status
- **Group 3 - LOBBY**: Lobby join/leave, chat
- **Group 4 - CHAT**: Tell messages
- **Group 5 - USER**: User list
- **Group 6 - PARTY**: Party creation, invites, member management
- And many more groups for quests, items, instances, etc.

### Packet Handler Dump

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/research/ddon_game_packet_handler_dump_notated.json`

A JSON dump of packet handlers extracted from the game executable, containing:
- Handler addresses
- Packet names (e.g., `S2C_CONNECTION_PING_RES`)
- Group IDs, Handler IDs, Sub IDs
- Japanese comments explaining packet purposes

### Quest Packet Analysis

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/research/quests/quest30260_packet_analysis.md`

Detailed Mermaid sequence diagrams showing packet flow for the quest "Hopes Bitter End", demonstrating:
- Client-Server request/response patterns
- Quest progression packets
- Enemy spawn and kill events
- Reward distribution packets

### Context Packets

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/research/get_set_context.md`

Sequence diagrams for party context packets (`C2S_CONTEXT_GET_SET_CONTEXT_REQ`, `S2C_CONTEXT_SET_CONTEXT_BASE_NTC`, etc.)

### HTTP Protocol Captures

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/research/DDO_EXE_HTTP.txt`

HTTP requests to the original patch/download server (dl.dd-on.jp):
- Version check endpoint: `/patch/master/win/exe_version`
- Version list endpoint: `/patch/master/win/versionlist`
- Includes file hashes and version numbers

### DTI (Data Type Information) Dumps

**Directory:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/research/dti/`

- `dti_prop_dump.h` - Property definitions extracted from executable
- `dti_dump_log.txt` - DTI extraction log
- `dti_data.py` - Python script for DTI processing

### Test Packet Captures

**Directory:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Test/TestFiles/`

YAML files containing captured packet data:
- `stream51-marked.pcapng_tcp-stream-11.yaml`
- `stream85-marked.pcapng_tcp-stream-11_edge_case.yaml`
- `pcapng1-tcp-stream-33_reduced_test.yaml`

---

## Community Tools for Packet Analysis

### 1. ddon_pcap_split (Session Splitter)

**Repository:** https://github.com/Andoryuuta/ddon_pcap_split

A command-line tool for splitting DDON packet captures into structured JSON files.

**Features:**
- Parses PCAP files and extracts TCP stream data
- Organizes packets by connection and server type
- Outputs structured JSON for further analysis

**Usage:**
```bash
# Single file processing
ddon_pcap_split -i [filename.pcap]

# Batch processing
ddon_pcap_split -idir [input_folder] -odir [output_folder]
```

**Note:** This tool does NOT perform decryption. Each JSON file contains an encrypted Camellia cipher key that requires separate cryptographic analysis.

### 2. ddon_common_key_bruteforce (Camellia Key Cracker)

**Repository:** https://github.com/Andoryuuta/ddon_common_key_bruteforce

A tool for bruteforcing the Camellia encryption key used in DDON Login Server <-> Client exchange.

**How It Works:**
1. Seeds the PRNG by iterating over each millisecond
2. Generates crypto key characters for that PRNG state
3. Attempts to decrypt ciphertext and checks against a known crib value (L2C_CLIENT_CHALLENGE_RES packet header)

**Usage:**
1. Take the third packet from a Login Server <-> Client exchange
2. Remove the size prefix bytes (0060)
3. Take the next 16 bytes as hexadecimal input

**Performance Notes:**
- Approximately 90% of CPU time is spent in Camellia keygen and block decrypt
- May benefit from AES-NI & AVX optimized implementations (Linux kernel, libgcrypt)

### 3. Built-in PacketCommand (Arrowgene CLI)

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Cli/Command/PacketCommand.cs`

The Arrowgene server includes a built-in packet analysis command.

**Usage:**
```bash
packet "E:\dumps\58_9.yaml" --key=J2g4pE2_heyqIAengWy0N6D1SEklxz8I
```

**Features:**
- Reads YAML-formatted packet captures
- Decrypts packets using provided Camellia key
- Annotates packets with packet IDs and names
- Exports decrypted packets to files

### 4. DDOn-Tools

**Repository:** https://github.com/alborrajo/DDOn-Tools

Server management tools for Arrowgene.DragonsDogmaOnline, written in GDScript.

**Features:**
- Import, edit, and export custom enemy sets
- Manage gathering spots
- Server configuration utilities

**Note:** This is primarily a server management tool, not a packet analysis tool.

---

## GitHub Repositories

### Primary Server Emulator

| Repository | Description |
|------------|-------------|
| [sebastian-heinz/Arrowgene.DragonsDogmaOnline](https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline) | Main server emulator for DDON |

### Packet Analysis Tools

| Repository | Description |
|------------|-------------|
| [Andoryuuta/ddon_pcap_split](https://github.com/Andoryuuta/ddon_pcap_split) | PCAP file splitter for DDON |
| [Andoryuuta/ddon_common_key_bruteforce](https://github.com/Andoryuuta/ddon_common_key_bruteforce) | Camellia key bruteforce tool |

### Related Projects

| Repository | Description |
|------------|-------------|
| [alborrajo/DDOn-Tools](https://github.com/alborrajo/DDOn-Tools) | Server management tools |
| [riftcrystal/DDON-Translation](https://github.com/riftcrystal/DDON-Translation) | Translation patch project |
| [Atvaark/Dune.Emulator](https://github.com/Atvaark/Dune.Emulator) | Dragon's Dogma: Dark Arisen network protocol (related game) |
| [mhvuze/ArisenTools](https://github.com/mhvuze/ArisenTools) | Tools for DD:DA (ARC file unpacking) |

---

## Community Resources

### Discord Servers

- **DDON Discord** - The largest DDON private server community (20,000+ members)
  - Invite: https://discord.com/invite/9DfTm4Eb3H
- **The White Dragon Temple Discord** - Development and research community
- **Wyrm Hunt Discord** - Additional community server

### Wikis and Documentation

- **The White Dragon Temple Wiki**: http://ddon.wikidot.com/
  - Contains game mechanics, tools, and community resources
  - Tools and Apps section: http://ddon.wikidot.com/toolsapps:home
- **Dragon's Dogma Wiki (Fandom)**: https://dragonsdogma.fandom.com/wiki/Dragon's_Dogma_Online
- **Developer Wiki (Annuate)**: Community-maintained developer reference

### Private Servers

| Server | Description |
|--------|-------------|
| [LegacyDDON](https://legacyddon.com/) | First North American private server, custom content, weekly updates |
| DDON Server | Closest to original game experience |
| Dogma Rising | Smaller server with custom tweaks |

---

## Technical Details

### Encryption

DDON uses **Camellia** block cipher for packet encryption:
- Key exchange occurs during login
- Keys are derived from a PRNG seeded by timestamp
- The Arrowgene project includes Camellia implementation: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Crypto/Camellia.cs`

### Protocol Structure

Packets follow a consistent structure:
- **Size prefix**: 2 bytes (big-endian)
- **Packet ID**: Composed of GroupID, HandlerID, and SubID
- **Payload**: Variable length, encrypted with Camellia

### Packet Naming Convention

- `C2S_` - Client to Server
- `S2C_` - Server to Client
- `_REQ` - Request
- `_RES` - Response
- `_NTC` - Notice (notification)

Example: `C2S_QUEST_QUEST_PROGRESS_REQ` - Client requesting quest progress update

### Server Architecture

```
52099 - HTTP/Download (Web Server)
52000 - TCP (Game Server)
52100 - TCP (Login Server)
```

---

## How to Capture and Analyze Packets

### Step 1: Capture Packets

Use Wireshark or similar tool to capture traffic between client and server:
- Filter for TCP port 52000 (game) or 52100 (login)
- Export as PCAP file

### Step 2: Split Sessions

Use ddon_pcap_split to separate TCP streams:
```bash
ddon_pcap_split -i capture.pcap
```

### Step 3: Recover Encryption Key

For login server packets, use the Camellia key bruteforcer:
```bash
# Extract third packet, remove size prefix, get first 16 bytes
ddon_common_key_bruteforce -hex <hex_data>
```

### Step 4: Decrypt and Annotate

Use Arrowgene's PacketCommand:
```bash
dotnet run --project Arrowgene.Ddon.Cli -- packet "capture.yaml" --key=<recovered_key>
```

### Step 5: Analyze

Review the annotated output to understand packet structure and game mechanics.

---

## References

- [Arrowgene.DragonsDogmaOnline GitHub](https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline)
- [DDON Discord Community](https://discord.com/invite/9DfTm4Eb3H)
- [LegacyDDON Website](https://legacyddon.com/)
- [The White Dragon Temple Wiki](http://ddon.wikidot.com/)
- [Andoryuuta's DDON Tools](https://github.com/Andoryuuta)

---

*Last updated: 2026-01-25*
