# Third-party environment art — version 0.6

The environment uses selected third-party models and textures. These models were created by Kenney and Quaternius, not by the game developer. Selection, placement, material adaptation, functional collision, puzzles and environmental transformations are project integration work.

| Pack / creator | Supplied source archive | License | Uses |
|---|---|---|---|
| Furniture Kit 2.0 / Kenney (www.kenney.nl) | `kenney_furniture-kit.zip` | CC0 | Reception counter/seating, archive cabinetry, desks, plants, civic walls and stairs, gallery exhibits |
| Factory Kit 3.0 / Kenney (www.kenney.nl) | `kenney_factory-kit_3.0.zip` | CC0 | Workshop machinery, reservoir hoppers/pipes, engine housing, functional blockers |
| Modular SciFi MegaKit Standard FREE / Quaternius (https://quaternius.com) | `Modular SciFi MegaKit[Standard].zip` | CC0 1.0 Universal | Modular elevations, doorframes, platforms, stairs, columns, rails, fixtures and service-core details |

License evidence is the original supplied `License.txt` / `License_Standard.txt` in each `Assets/ThirdParty` pack directory. The original archives remain outside Assets. Only the Standard FREE content was used from Quaternius; no PRO/source-version entitlement is assumed.

`Documentation/AssetReview/import-manifest.json` records all 73 selected models and their exact source-archive paths. `Tools/import_asset_packs.py` reproduces extraction. Duplicate interchange formats, demonstrations and unrelated assets are excluded. Some selected models are retained as reusable alternatives rather than placed everywhere.

Vendor sources live under `Assets/ThirdParty`. Normalized floor-pivot prefabs live under `Assets/Art/Prefabs`; adapted materials under `Assets/Art/Materials`. `AssetPreparation.Prepare` regenerates these and `Resources/ImportedCatalog.asset` and runs before a build. Model axes and bounds were inspected through `AssetPackReview.Inspect`; the resulting report is in AssetReview.

Imported textures retain their original UV layout. The custom surface shader applies subdued palettes and maps normal/ORM data. Imported model prefabs contain no collision: authored shell, functional collision and separate furniture proxies own movement and observation. Do not add vendor mesh colliders indiscriminately.

Original procedural sound and project surface maps remain separate from these third-party assets. No third-party audio was imported.
