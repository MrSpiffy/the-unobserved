# Current status — 0.8.0

The physical-puzzle redesign is implemented and the standalone regression reports **233 passed, 0 failed**. There are still eight wings, 17 circuits and two instrument pickups. Puzzles now use real gaps, elevation, clear sightlines and shielding rather than named-form panel passwords. Decorative content comprises 24 curated slots, eight of which swap two-object arrangements.

See GAMEPLAY_REDESIGN.md for intended sequences and alternatives, VERIFICATION.md for evidence and limits, and the root README for the portfolio overview. Human first-time comprehension and broad hardware performance remain unverified.

---

# Current status — 0.7.0

Observation retention, coherent floor regions and twelve curated decorative slots are implemented. See [observation polish](OBSERVATION_POLISH.md) for design, architecture, testing and limitations. The notes below retain previous pass history.

# Current status — 0.6.0

The curated environment-art pass replaces the principal furniture, machinery, stairs and architectural elevations across all eight wings. See [art review](ART_PASS.md), [credits](THIRD_PARTY_ART.md) and [verification](VERIFICATION.md). The historical 0.5 implementation notes below describe the preceding systems pass.

# Polish status — 0.5.0

## Implemented in this pass

- Audited all production systems, authored routes, source assets, build setup and documentation before choosing edits; see POLISH_AUDIT.md.
- Split five functional collision shapes from three architectural interpretations in FormArchitecture. Held geometry remains unchanged; visual details cannot interfere with the proven collision contracts.
- Removed ubiquitous three-way ceiling cycling and the 16 decorative filler reserves. Room-specific ceilings restore tall atrium/observatory volumes and distinguish civic and service rooms.
- Added two authored replacements using the existing observation mechanic: garden to service core, shallow opening to recessed vestibule. Both are nonrepeating and guarded against remote occluded activation, visible popping and occupied-volume replacement.
- Restricted material revisions to structural surfaces, preserving furniture identity. Reduced luminous route outlines to painted markings.
- Moved the existing Rise objective onto its required stair's 2.4m landing, retaining the circuit count and overall route.
- Added occlusion-respecting logical targeting for Empty and contextual E prompts. Footstep tags follow a form's construction.
- Retained sparse ambient beds, restrained shifts, one/two-anchor progression, latched objectives, movement constants and the non-horror tone.

## Preserved scope

Eight wings, 17 circuits, two pickups, seven interlocks, five functions, three material realities. No enemies, inventory, additional objectives, stamina, waits or extended corridors.

## Remaining validation

A fresh first-time playthrough must establish completion time, confusion, puzzle repetition and perceived transformation strength. The 15–30 minute duration remains a target. Observatory's repeated pair-matching is the next candidate for refinement if playtest evidence confirms fatigue. Test audio on headphones and speakers and profile representative lower-end hardware. Automated camera/panel positioning does not substitute for that work.

## Known limits

Art remains deliberately stylized; version 0.6 combines imported models with the existing authored shell. This is not a photoreal asset replacement. Hero moments are one-way authored reveals, while functional forms continue their bounded observation-driven selection. Structural surface changes are conservative per-frustum patches and can temporarily leave mixed palettes. Observation uses seven occlusion samples rather than per-pixel visibility. There is no disk save, controller navigation or key-remapping UI. The editor setup command resets authored scene/data defaults.

