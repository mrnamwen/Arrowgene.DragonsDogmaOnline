# Dragon's Dogma Online Retail Wiki and Documentation Research

## Primary Documentation Resources

### English Wikis
1. **The White Dragon Temple** (ddon.wikidot.com) - Community-maintained wiki with:
   - Characters, Vocations, Quests, Monsters
   - Crafting systems, Area Mastery, Job Mastery
   - Pawns system documentation
   - Currency systems including Blood Orbs

2. **Dragons Dogma Online Wiki (Miraheze)** (ddon.miraheze.org) - Community wiki based on Dixdros server

3. **Dragons Dogma Online Wiki (Fextralife)** - Community resource covering weapons, armor, classes, zones, and loot

### Japanese Wikis
1. **ddon.swiki.jp** - Primary Japanese strategy wiki with game data and updates
2. **Heige Wiki (h1g.jp)** - DDO strategy wiki for PS4/PS3/PC
3. **Wikiwiki (wikiwiki.jp/doradora/)** - Japanese wiki with events and updates

## Specific System Information

### Orb System (Blood Orbs)
- Obtained from defeating Orb Enemies (enemies with purple health bars)
- Higher-level enemies yield more Blood Orbs
- Used to activate perks in the Dragon Force Augmentation skill tree
- Categories: Vitality, Combat, Adventure, and Magick
- Used for purchasing rare items from Gilstan, The Goblin King

### Pawn System (Nora/Support Pawns)
- Players can create up to three Main Pawns
- Partner Pawns appear in the Arisen's Room and unlock 'gifts' when affection is raised
- Support Pawns can be hired via Riftstone to assist in battle or crafting
- Pawn Orders system allows direct commands

### Game Structure
- Free-to-play model with optional item purchases
- Cross-platform support (PS3, PS4, PC with cross-play)
- Supports 1-4 players in regular zones, 8 in special missions, up to 100 in lobbies
- Regular maintenance every Thursday

## Limited Documentation Systems

The following systems have limited public documentation:
- **Blacklist system**: No specific documentation found
- **Login announcements format**: General information only
- **GP courses**: No detailed wiki documentation
- **System mail format**: No detailed documentation
- **Quest reward box mechanics**: Not specifically documented
- **Game settings structure**: General framework only

## Implementation Notes

Based on the research findings:
- Most handlers can use reasonable defaults where exact retail behavior is unknown
- Empty lists are appropriate for social features without database tracking
- Configurable settings should be used where server operators may want customization
- The Arrowgene codebase itself serves as the best documentation for packet structures

## Key Resources for Further Research
- [The White Dragon Temple Wiki](http://ddon.wikidot.com/)
- [Arrowgene.DragonsDogmaOnline GitHub](https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline)
- [Legacy DDON Private Server](https://legacyddon.com/)
- [Japanese DDO Wiki - swiki.jp](https://ddon.swiki.jp/)
