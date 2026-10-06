# Observation and environmental memory — 0.7

## Audit and fixes

The previous seven interior rays could all miss the visible sliver at a screen edge. Logical bounds also omitted the bottom of some platform supports. Independent floor-tile updates created patchwork finishes, and offscreen light-source changes could recolor visible geometry. Fixed prop dressing did little to reinforce non-puzzle instability.

ObservationManager now uses a frustum expanded by 4% of viewport width/height on each side. The expanded planes are cached until the camera matrix changes and shared by functional targets, decorative slots, surface zones and hero guards. There is no visibility-distance cutoff shorter than the camera's far plane. Functional bounds include the current renderer geometry and an enlarged authored envelope covering lower supports. Hero envelopes union the actual meshes of every version.

Within the retained frustum, invisibility needs positive evidence: a solid BoxCollider must intercept rays to all eight target-bound corners before they reach the target. A convex box's shadow then covers the target bounds. A center ray chooses the candidate; open edges, door apertures, thin blockers, non-box blockers and uncertain composite occlusion retain observation. This deliberately favors false observation over visible popping. Fully hidden targets behind one sufficiently large wall still change. No dwell timer or background reroll is added.

## Architectural regions

Each existing Floor call becomes one SurfaceRevisionZone: reception/archive/workshop/observatory/gallery continuous decks, the atrium's separate perimeter/raised landings, reservoir decks and maintenance islands, and the engine's separated platforms. Boundaries follow the authored landing edges and changes in elevation. Corridor flooring remains stable.

Visible top surfaces and landing thickness commit a single shared finish in the same frame, only when the complete zone is outside the expanded frustum. Painted edges and structural beams remain stable. The three finishes use world-space institutional tile, raw concrete and industrial metal maps, with matching footstep classification. Imported UV layouts no longer make the finish depend on individual tile size. Modular wall panels can still change at their modeled seams. Local lighting stays stable because an unseen fixture can still illuminate a visible floor; gradual room fog remains atmospheric.

A large floor may wait until the player looks upward or leaves the room. This is intentional: while any of the continuous region remains visible it cannot safely change. The system does not claim to rebuild an entire room on every turn.

## Curated decorative slots

DecorativeRevision prepares inactive imported visual children once and uses the existing centralized ObservationTarget edge event and StateDeck. There is no per-prop Update, runtime model destruction, anchor interface, objective, or changing collision. Uniform scaling preserves proportions within a 1.6 × 2.4 × 1.6 metre shared envelope. A player-occupancy margin suppresses swaps near the player. The envelope includes a small floor tolerance.

| Area | Slots | Curated vocabulary |
|---|---:|---|
| Reception | 2 | Lounge chair, plant, cabinet, coffee table |
| Archive | 2 | Bookcase, records box, desk chair, plant |
| Winter atrium | 1 | Plant, lounge chair, low table |
| Workshop | 2 | Machine, cabinet, crate, piston |
| Reservoir | 1 | Valve equipment, crate, hopper |
| Observatory | 1 | Computer, chair, access terminal, cabinet |
| Return gallery | 2 | Chair, plant, column, cabinet |
| Continuity engine | 1 | Pipe machinery, piston, pipe column |

Twelve peripheral slots leave most furniture stable and preserve empty space. They use the existing asset catalog. No new imported packs or original-model claims. Later gallery columns provide a sparse cross-category change; the existing simplified late functional forms remain the principal loss-of-detail moments.

Every other swap is eligible for a very quiet material-specific movement cue, only within eight metres and subject to a global 2.5-second cooldown. It reuses carpet/stone/metal recordings through the pooled, occlusion-filtered sound system. It does not add twelve simultaneous shift sounds or claim bespoke recordings for every prop.

## Performance and limitations

Frustum rejection comes before physics. A potentially visible target needs at most one broad-phase ray plus eight direct collider rays; uncertain cases often exit earlier. Surface regions use no occlusion rays. Bounds/renderer lists and decorative variants are prepared/cached; region material variants are cached by the existing controller. No per-frame managed allocation is needed by the visibility decision.

Conservative AABBs and the single-box occlusion proof may keep fully hidden targets observed behind complex combinations of geometry or mesh stair blockers. This is a conscious safety tradeoff. Decorative objects are non-blocking and can be walked through; placement is peripheral and proximity prevents appearance on the player. No pixel-perfect visibility claim is made. Small material differences between separate architectural regions are possible and intentional.

Automated regression uses half-degree camera sweeps, solid walls, thin pillars and two-wall apertures, plus actual decorative transitions and atomic floor commits. Existing movement/puzzle/ending checks are retained. A human should still attempt slow vertical and diagonal sweeps at multiple FOVs, inspect all prop states in situ, listen on headphones/speakers, and assess the frequency of noticed changes. Performance on lower-end hardware and first-play pacing remain unmeasured.
