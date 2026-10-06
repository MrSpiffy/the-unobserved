# Polish audit — baseline 0.4.0

Reviewed before implementation: all runtime/editor C# scripts; authored WorldBuilder coordinates and eight wing routes; ScriptableObject assets and scene/bootstrap; surface/audio generation and import settings; shaders/postprocessing; UI, movement, observation and anchoring; package/build settings; documentation, player screenshots and the 128-check integration harness.

## Findings, in priority order

1. **Functional geometry has no environmental identity.** UnstableEntity.BuildShape renders five forms from one material and boxes; every top is tagged metal. A Pillar is a solid cuboid, Empty is a tiny aim marker, and architecture is added independently. Keep proven collision envelopes but separate their visual interpretation: institutional joinery/stone, structural concrete, service metalwork.
2. **Transformations compete rather than compose.** SpatialRevision independently cycles every ceiling bay through the same three versions. Original beams/rings remain beneath lowered ceilings. EnvironmentThemeController also rewrites most furnishings into unrelated materials. Replace ubiquitous ceiling cycling with fixed, room-specific composition. Use two authored, nonrepeating hero replacements, with quieter wall/floor changes elsewhere. Preserve furniture's material identity.
3. **Puzzle labels can substitute for physical reasoning.** Workshop Rise asks for Steps but its terminal is on the ground. Move that existing test onto the stair's raised landing. Do not increase station count. Observatory's three matching tasks remain a repetition risk, but rewriting several puzzle wings during polish would complicate pacing attribution; first test the focused change.
4. **Empty is harder to target than to understand.** Its 15 cm marker can make the intended held-empty solution feel unavailable. Allow a clear ray through the empty reserve to target that logical location, still respecting nearer occluders and the existing range/capacity rules.
5. **Visual hierarchy is too uniform.** Large neon edge rectangles, identical ceiling ornament and repeated free-standing cubes dominate every room. Reduce decorative emission and repeated fixtures; reserve brighter feedback for actual controls and held forms. Surfaces lack grounded material consistency despite 21 custom maps.
6. **Audio and game feel need restraint, not more systems.** The 0.4 fan removal is sound; retain its sparse beds. Footsteps ignore visual form materials, and full-object anchor tint obscures material identity. Fix those mismatches. Preserve reliable separate movement/gravity and continuous stair collision.
7. **Validation is useful but not a playthrough.** Current tests teleport between panels and tune states programmatically. They establish circuit logic, panel ray access and one stair traversal, not the complete human route, difficulty or 15–30 minute duration. Add targeted checks for the changed interaction, forms and hero safety. Keep duration explicitly unverified.

## Good existing structure to preserve

Central observation scheduler with frustum and occlusion tests; immediate visible-to-hidden edge per the latest user preference; bounded state deck; support protection; binary one/two-anchor progression; latched circuits; checkpoint recovery; 17 circuits plus two pickups; no enemies or conventional horror; original audio/maps; isolated editor/runtime verification.

## Bounded implementation

Separate functional collision from three architectural form interpretations; replace generic ceiling cycles with room-specific shell treatment; author atrium garden→service core and gallery shallow opening→deep threshold transformations; relocate the existing Rise test; improve empty targeting and contextual readout; retain the 8-wing footprint, objective count, speed and jump values. No additional major gameplay systems or content wings.

## Remaining playtest questions

Can a first-time player identify useful forms without abstract labels? Does the first major replacement read as a place changing rather than a prop swap? Can the player anchor an absence without instruction? Does Rise teach why Steps matters? Record per-wing time, wrong turns, release decisions and confusion; do not tune duration with waits or travel padding. Repetition in Observatory should be judged from this run before changing its objectives.

## Outcome — 0.5.0

Implemented the bounded plan: FormArchitecture separates function from construction; RoomArchitecture replaces universal cycling decoration; SpatialRevision now controls two observed, one-way hero replacements; material changes preserve furniture identity; Rise uses its stair physically; Empty supports occlusion-respecting targeting; E shows architecture plus its stable function. Sixteen filler reserves were removed. No objective, wing or movement expansion was added.

Final Windows build: 156 automated assertions passed, zero failed, plus the 10,000-transition deck check. Reviewed actual player renders, including matching-pose hero comparisons and all three visual interpretations. The remaining questions above are deliberately left for first-time playtesting; these checks do not establish portfolio readiness or duration by themselves.
