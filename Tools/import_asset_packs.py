"""Curated Unity imports from user-supplied CC0 archives; originals remain outside Assets."""
from pathlib import Path
import zipfile,json
packs={
 'KenneyFurniture':('kenney_furniture-kit.zip','Models/FBX format/', '''benchCushion bench bookcaseClosed bookcaseClosedDoors bookcaseOpen books cardboardBoxClosed chairDesk computerScreen computerKeyboard desk kitchenBar kitchenBarEnd kitchenCabinetDrawer lampSquareCeiling lampWall loungeChair loungeSofa pottedPlant stairs stairsOpen tableCoffee trashcan doorway wall wallWindow paneling'''.split()),
 'KenneyFactory':('kenney_factory-kit_3.0.zip','Models/FBX format/', '''catwalk-straight catwalk-stairs catwalk-corner machine machine-fortified machine-window machine-connection-pipe pipe-large-long pipe-large-bend pipe-large-valve pipe-large-junction structure-wall structure-window structure-doorway-wide structure-tall hopper-high-round piston-round crane-lift screen-panel-small screen-small box-large floor'''.split()),
 'Quaternius':('Modular SciFi MegaKit[Standard].zip','Modular SciFi MegaKit[Standard]/FBX (Unity)/', '''Column_Round Column_Simple Column_Pipes Column_MetalSupport Door_Frame_Square Door_Simple Platform_Simple Platform_Metal Platform_Stairs_4 Platform_Stairs_4Wide Platform_Rails_4 Prop_AccessPoint Prop_Computer Prop_Fan_Small Prop_Light_Wide Prop_Rail_4 Prop_Vent_Wide Prop_Vent_Big Prop_Cable_3 WallBand_Straight WallAstra_Straight_Flat WallWindow_Straight BottomSimple_Straight TopSimple_Straight'''.split())}
manifest=[]
for pack,(archive,prefix,selected) in packs.items():
 root=Path('Assets/ThirdParty')/pack; (root/'Models').mkdir(parents=True,exist_ok=True); (root/'Textures').mkdir(exist_ok=True)
 with zipfile.ZipFile(archive) as z:
  for stem in selected:
   matches=[n for n in z.namelist() if n.startswith(prefix) and n.endswith('/'+stem+'.fbx')]
   assert len(matches)==1,(pack,stem,matches)
   dst=root/'Models'/(stem+'.fbx'); dst.write_bytes(z.read(matches[0])); manifest.append({'pack':pack,'model':stem,'source':matches[0],'path':dst.as_posix()})
  for n in z.namelist():
   if 'license' in Path(n).name.lower() and n.endswith('.txt'): (root/Path(n).name).write_bytes(z.read(n))
   if n.startswith(prefix+'Textures/') and n.endswith('.png'): (root/'Textures'/Path(n).name).write_bytes(z.read(n))
   if pack=='Quaternius' and '/Textures/' in n and n.endswith('.png') and any(x in Path(n).name for x in ['T_Trim_01_','T_Trim_02_','T_Trim_03_']):
    if any(x in Path(n).name for x in ['_BaseColor.png','_Normal.png','_ORM.png','_Emissive.png']): (root/'Textures'/Path(n).name).write_bytes(z.read(n))
Path('Documentation/AssetReview/import-manifest.json').write_text(json.dumps(manifest,indent=2))
print('Selected',len(manifest),'FBX models; no redundant formats imported.')
