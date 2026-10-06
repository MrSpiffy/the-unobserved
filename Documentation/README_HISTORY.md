# THE UNOBSERVED — 0.7.0

A first-person escape game about an institution whose contents change when you stop observing them. This revision replaces the original repeated hallway with eight individually authored wings connected by turning service galleries. It follows feedback that the first version took only about **3½ minutes** to complete.

## Observation and environmental memory

Version 0.7 adds conservative screen-edge retention, full-bound occlusion checks, atomic floor-finish regions and 12 curated non-puzzle decorative slots across the eight wings. See [implementation and limitations](Documentation/OBSERVATION_POLISH.md). The existing route, puzzles and anchor progression remain unchanged.

## Environment art pass

All eight wings now use a curated vocabulary of 73 imported models: civic joinery and furniture, service machinery, modular elevations, proper stair meshes, platform framing and architectural fixtures. The atrium and gallery hero transformations retain their observation/occupancy guards. Functional forms use imported constructions with separate gameplay collision; selected late-game states deliberately lose detail. This remains stylized art, not photorealism.

See [third-party credits](Documentation/THIRD_PARTY_ART.md) and [current art review](Documentation/ART_PASS.md).

## Focused polish after implementation audit

The [implementation audit](Documentation/POLISH_AUDIT.md) identifies the problems in version 0.4 before this pass: abstract functional geometry, competing cosmetic cycles, repetitive form-matching, hard-to-target absences and unverified pacing.

The five existing functions now have three architectural interpretations: institutional stone/joinery, exposed concrete, and service metalwork. Gameplay collision stays separate from the visual construction. A solid blocker can be an archive cabinet, structural pier or louvered plant housing; stairs and crossings receive corresponding construction and footstep surfaces. Held forms preserve their interpretation.

The ubiquitous cycling ceiling bays and 16 filler reserves are removed. Fixed room-specific ceilings and quieter markings support the layout. Two authored hero replacements provide contrast: the atrium garden becomes a sealed service core, and the gallery's shallow opening becomes a recessed vestibule. Each happens once after actual observation and leaving view, with a union-envelope visibility and player-occupancy guard. Furniture retains its material identity while unseen structural surfaces change.

The existing Rise test now occupies the 2.4m landing reached by its required Steps form. Empty passages can be targeted through their aperture without aiming at a tiny marker, while nearer solids still block targeting. The eight wings, 17 circuits, two pickups, movement speed and jump are retained. Original sparse audio is retained; no additional gameplay systems were introduced.

## Play

Launch `Builds/Windows/The Unobserved.exe`. Keep its data folder, DLLs, and MonoBleedingEdge folder beside it. Open this project with **Unity 6000.3.12f1**, load `Assets/Scenes/TheUnobserved.unity`, and press Play to run from source.

| Control | Action |
|---|---|
| WASD / arrows | Walk |
| Mouse | Look |
| Space | Jump |
| F | Use a control panel / take an instrument |
| E | Hold or release an aimed unstable form, after acquiring the instrument |
| R | Release every manual hold |
| Escape | Pause / resume |
| Backspace | Recover at this wing's entrance |

Settings separately control master sound, score, room ambience, effects, sensitivity, and FOV. Settings persist between runs. Falls retain connected circuits and holds. Restart begins a new session.

## What changed

- **Immediate causality:** an observed object changes on the first safely unobserved frame, with a conservative viewport margin. No dwell timer, minimum look-away time, or background rerolling. Occlusion still counts. A structure supporting the player stays safe.
- **The room changes too:** functional shifts request a coordinated visual revision. Modular wall surfaces and whole floor regions adopt the revision only when safely unseen. Local lights stay stable. Most furniture keeps its identity; twelve curated decorative slots change objects, while two authored replacements change major silhouettes and spatial framing. Held objects, controls and navigation markings remain dependable.
- **New route:** reception, archive maze, elevated winter atrium, calibration workshop, reservoir, parallax observatory, return gallery, and continuity engine. The route bends through the institution rather than repeating a single corridor.
- **More decisions:** 17 persistent circuit panels and two physical instrument pickups. Calibrate multiple forms of one specimen, reuse one hold across sequential pump circuits, coordinate two lenses, preserve an empty doorway, and reclaim holds for the final return spans.
- **Art:** 21 original 1024px seamless albedo, normal and surface-detail maps for plaster, concrete, carpet, tile, wood, metal and water. Architectural trims, shelving, workbenches, planters, an atrium tree, pipes, pressure vessels and a suspended observatory; soft bloom, local shadows and animated water normals.
- **Sound:** 25 original audio assets, including a 96-second evolving score, three 48-second interior beds, outdoor air/bird-like tones, four footstep variations each for carpet/stone/metal, four restrained form-specific shifts and distinct anchor, circuit-success, release and door effects. Ambient beds crossfade; spatial effects have reverb/wall muffling and quiet local room sources.
- **Motivation:** you are the last visitor still recorded inside. The exterior doors have lost their departure signal. Restore 17 circuits to reach daylight; the HUD and environmental signs track that objective, and the ending checks it.

This build adds puzzle work and spatial variety without artificial waits. **The revised first-play completion time has not yet been established.** The original 15–30 minute aspiration remains a playtesting target, not a verified claim.

## Architecture

| System | Main files |
|---|---|
| Observation | `ObservationManager`, `ObservationTarget`: LateUpdate expanded-frustum checks, convex occlusion proof, cached visual bounds, visible→hidden edge |
| Reality | `UnstableEntity`, `FormArchitecture`, `RealityState`, `StateDeck`: five collision functions, three visual interpretations, bounded selection and support safety |
| Anchoring | `AnchorManager`: explicit one/two channel limits and release |
| Puzzles | `PuzzleRelay`, `FormRequirement`, `CircuitDoor`, `PlayerInteraction`: reusable exact-form conditions, prerequisites, latched circuits, F interaction |
| Environment | `WorldBuilder`, `RoomArchitecture`, `SpatialRevision`, `EnvironmentThemeController`: authored layout, fixed architectural families, two guarded hero replacements, structural palettes |
| Audio | `Soundscape`, `SurfaceTag`, `AudioImport`: pooled effects, surface footsteps, loop crossfades, streaming/compression |
| Player and presentation | `FirstPersonController`, `GameFlow`, `GameUI`, `AtmosphereEffect`, `WorldSign` |

Individual entities change when their full bounds leave the retained viewport or are provably hidden by a solid box. Logical bounds are conservative, and a conservative convex-occluder check protects partially visible bounds. A functional shift also requests a room palette revision. Eligible structural renderers adopt that revision only outside a retained frustum; floors commit as complete authored landing regions over stable collision. A 0.65-second grouping window gives multiple changes during one turn a coherent palette, without delaying any functional change.

Geometry is constructed in Play mode from fixed authored coordinates. It is not randomized level generation. State/theme assets live under `Assets/Data`. The **Unobserved → Create or refresh authored scene** editor command resets the scene and data assets to these defaults; preserve custom variants before using it.

## Build and verify

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.12f1\Editor\Unity.exe' `
  -batchmode -nographics -quit -projectPath "$PWD" `
  -executeMethod ProjectSetup.Build -logFile "$PWD/build.log"
```

Launch the player with `-batchmode -smokeTest -logFile runtime-v2.log` to run integration checks and direct-camera captures. Do not use `-nographics` for captures. Results go to `Builds/VerificationV2`. These tests use automated positioning and do not establish blind-play pacing or subjective polish.

`Tools/generate_audio.py` regenerates the original PCM audio using Python and NumPy. `Tools/generate_surfaces.py` uses NumPy and Pillow to regenerate the seamless maps. Unity imports normal maps correctly, surface masks as linear data, and texture mipmaps with anisotropic filtering. Audio loops stream as Vorbis. The selected third-party environment art is included in Assets; see [asset credits and import workflow](Documentation/THIRD_PARTY_ART.md). No third-party sound or music is used.

See [before/after screenshots](Documentation/VISUAL_PREVIEW.md), [level guide](Documentation/LEVEL_DESIGN.md), [development status](Documentation/STATUS.md), and [verification](Documentation/VERIFICATION.md).
