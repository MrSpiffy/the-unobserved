# Version 0.8 verification — 2026-10-06

Final Windows x64 build: **233 passed, 0 failed** in the standalone regression run. Unity 6000.3.12f1 reported build success; no failed assertions or exceptions were found in runtime-v8.log. Counts include repeated observation attempts, not 233 independent test cases.

The run connects all 17 circuits using actual optical conditions, opens the seven wing exits, and reaches the ending. It verifies the additional intake service gate exists, the 24 decorative envelopes remain collider-free, and all relay configurations have removed named-form passwords.

Actual CharacterController movement checks climb the workshop, observatory and engine stairs, cross the workshop and west observatory gaps, and pass through the transformed gallery. Downward ray checks confirm the new test-bed edges have no flat-floor shortcut; rays from below cannot use the raised Zenith/Return stations through their floors. Previous slow edge sweeps, wall/aperture occlusion, anchor preservation, hero transitions and coherent floor-region tests remain passing.

Two defects were found and corrected during this pass: Feed B lacked its required tall shielding state, and the workshop upper landing blocked the lower crossing. The latter now has a marked low side approach, which the traversal test exercises.

These are controlled progression and targeted movement checks, not a blind human beginning-to-end playtest. They do not establish that every unintended shortcut or softlock is impossible, that optical clues are understandable without coaching, or that performance is adequate on low-end hardware. See GAMEPLAY_REDESIGN.md and PLAYTEST.md for outstanding human checks.

Current route/upper-access captures and the result record are preserved in `Documentation/Images/V8`.

---

# Version 0.7 verification

Windows x64 builds successfully in Unity 6000.3.12f1. The observation regression adds slow 0.5-degree sweeps for all five functional visual forms and an actual decorative state transition, rather than relying only on 180-degree turns. It checks margin retention, eventual release, complete wall occlusion, a narrow blocker and a two-wall opening for each form.

All twelve decorative slots are checked for collider-free variants within their safe envelope. The run checks one change per observed/hidden edge, no unseen reroll, no partial visible floor commit, atomic hidden floor commits and three distinct production floor texture maps. Existing anchor, stair, hero, circuit, interlock and ending regressions remain included.

See OBSERVATION_POLISH.md for the conservative occlusion tradeoff and PLAYTEST.md for the outstanding human stress test. Automated geometry checks do not establish pixel-perfect visibility at every camera/FOV or subjective atmosphere. Captures are direct player Camera.Render output without HUD; the floor comparison camera is positioned below the reception ceiling.

Final standalone run on 2026-09-08: **209 passed, 0 failed**, with no failed assertions or exceptions found in runtime-v7.log. Counts include repeated observation attempts rather than 209 independent cases. Captures and the result record are preserved in `Documentation/Images/V7`; see [visual review](VISUAL_PREVIEW.md).

---

# Version 0.6 verification — 2026-09-08

Unity 6000.3.12f1 built the Windows x64 player successfully (`UNOBSERVED_BUILD_SUCCESS`). The final standalone run reported **173 passed, 0 failed**, with no exceptions or failed assertions found in `runtime-v6.log`. Counts include observation attempts, not 173 independent test cases.

The run verifies the 73-entry imported catalog, distinct imported content in all eight wings, collider-free imported prefabs, actual observation-driven transitions, held states, hero guards, panel reachability, every circuit/interlock and the ending. CharacterController movement checks cover the unstable staircase, atrium stairs and transformed gallery aperture. The architectural visual check now accepts a single detailed imported mesh instead of requiring multiple primitive renderers; it still rejects empty mesh content and enabled visual colliders.

Player captures are preserved in `Documentation/Images/V6`; see VISUAL_PREVIEW.md. The image review checked fixtures, platform edges, model materials, forms and the hero changes. It does not establish that every possible camera angle or decorative collision has been exhaustively inspected. No blind end-to-end human playtest or hardware performance profile was performed. First-time completion duration remains unverified.

The sparse audio implementation was preserved in this continuation; subjective headphone/speaker evaluation remains a playtest task.

---

# Version 0.5 verification

The Windows x64 player compiled successfully in Unity 6000.3.12f1. The final standalone run reported **156 passed, 0 failed**, with no runtime exceptions found in the log. Counts include variable observation-sampling attempts, not 156 independent test cases. Editor validation additionally checks 10,000 deck transitions for repeats/starvation.

## Verified behavior

- Eight authored wings, 19 interaction stations, seven interlocks, 25 audio clips and 21 surface maps.
- Immediate visible-to-hidden functional transitions, no background cycling, anchor preservation and pause.
- Actual camera observation reaches required forms; all panel rays reach their targets; all 17 circuits latch, all wing gates open and the ending is reachable in the automated sequence.
- The atrium starts as a garden, changes only after observation and looking away, and does not revert. An occluded unvisited gallery hero remains unchanged.
- The gallery transforms into its authored vestibule; a real CharacterController crosses the resulting held-empty aperture.
- An Empty reserve can be selected through its aperture. A deliberately inserted nearer solid wall blocks selection.
- The relocated Rise panel is on the 2.4m landing; a real CharacterController climbs the specimen stair to reach that elevation.
- Held forms preserve architectural interpretation through a room palette request.
- All 15 form/interpretation combinations create visual construction without enabling any additional collision. Existing UnstableEntity shapes retain gameplay collision responsibility.
- Visible surface patches preserve their materials until unseen, and architectural hero versions have only one active collision set.
- The original atrium stair regression still reaches the landing at approximately Y=2.43m. Checkpoint and outdoor sound/ending transitions pass.

## Render review

The compiled player renders matching-camera atrium and gallery before/after pairs, all eight wings, and three isolated form-comparison images. The comparison staging area exists only during `-smokeTest`; it is not playable content. Curated output is linked from VISUAL_PREVIEW.md. Captures exclude OnGUI overlays.

## Limits

The suite teleports between objectives and tunes forms through camera observation; it does not establish a full first-time human traversal, difficulty, readability, subjective audio quality, frame-rate targets or 15–30 minute pacing. PLAYTEST.md provides the next review protocol. Original sparse audio is retained; form footstep tags now distinguish stone from metal construction.

## Historical fixes retained

Continuous stair ramps, separate planar/gravity movement, zero CharacterController minimum movement cutoff, noncolliding decorative landing edges, workshop instrument clearance and the unobstructed Nadir terminal. No new movement constants or timing delays were introduced.

