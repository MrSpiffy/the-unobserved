# Version 0.7 — observation and surface review

The final Windows player reported **209 passed, 0 failed**. Half-degree sweep regressions cover all functional visual forms and a changing decorative prop, including viewport retention, complete wall occlusion, narrow blockers and openings. These are automated checks, not an exhaustive human playtest.

## One reception floor, three complete finishes

The camera and lighting are held constant. The entire authored floor region changes together; the partition remains stable. The comparison uses the production materials and is captured from below the ceiling.

![Institutional tile](Images/V7/floor-finish-0.png)

![Raw concrete](Images/V7/floor-finish-1.png)

![Industrial metal](Images/V7/floor-finish-2.png)

## Decorative identity change

This isolated regression slot changes from chair to cabinet after a slow camera sweep. Its variants are prepared once, constrained to the same safe envelope, and have no colliders. The dark test-stage captures verify distinct silhouettes, not final room lighting.

![Chair](Images/V7/decor-before.png)

![Cabinet](Images/V7/decor-after.png)

All eight route views, hero pairs and edge captures are preserved in `Images/V7`. The unchanged route capture positions do not show every decorative slot. See [implementation and limitations](OBSERVATION_POLISH.md) and [human stress-test protocol](PLAYTEST.md).

---

# Version 0.6 — imported environment art

These are direct camera renders from the final Windows player, without HUD. All eight wings are captured below, followed by the two authored transformations. Archive and gallery route captures are close to partitions; their framing does not show every furnishing. These are implementation review images, not proof of a complete visual walk-through.

## Reception

![Reception](Images/V6/wing-1.png)

## Archive

![Archive](Images/V6/wing-2.png)

## Atrium

![Atrium](Images/V6/wing-3.png)

## Workshop

![Workshop](Images/V6/wing-4.png)

## Reservoir

![Reservoir](Images/V6/wing-5.png)

## Observatory

![Observatory](Images/V6/wing-6.png)

## Gallery

![Gallery](Images/V6/wing-7.png)

## Engine

![Engine](Images/V6/wing-8.png)

![hero-atrium-before](Images/V6/hero-atrium-before.png)

![hero-atrium-after](Images/V6/hero-atrium-after.png)

![hero-gallery-before](Images/V6/hero-gallery-before.png)

![hero-gallery-after](Images/V6/hero-gallery-after.png)

![forms-0](Images/V6/forms-0.png)

![forms-1](Images/V6/forms-1.png)

![forms-2](Images/V6/forms-2.png)

---

# Version 0.5 — player-rendered polish review

See POLISH_AUDIT.md for findings that guided this pass. These images come from the compiled Unity player, with matching camera poses for each before/after pair. They exclude the HUD.

## Atrium: a different use of the space

![Garden](Images/HeroGardenV5.png)

![Service core](Images/HeroCoreV5.png)

The former open central garden becomes a sealed service structure with different sightlines. The side traversal routes remain available. This authored reveal occurs once, giving repeated functional changes a quieter background afterward.

## Gallery: a different threshold

![Shallow opening](Images/GalleryBeforeV5.png)

![Deep vestibule](Images/GalleryAfterV5.png)

The same puzzle aperture is set inside a recessed enclosure with physical side walls and ceiling. A CharacterController traversal checks the clear route.

## Same five functions, three interpretations

Each comparison shows Bridge, Steps, Pillar, Islands and Empty from left to right. These are visual construction examples rendered in an isolated verification area, not extra playable rooms. Traversal collision remains owned by UnstableEntity.

![Institutional](Images/FormsCivicV5.png)

![Structural](Images/FormsConcreteV5.png)

![Service](Images/FormsServiceV5.png)

## Workshop composition

![Workshop](Images/WorkshopV5.png)

The existing Rise test is on the landing behind the central specimen. The game remains stylized; these are specific composition and functional-art changes, not a claim of photorealism.
