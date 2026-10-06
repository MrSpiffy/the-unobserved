using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Unobserved
{
    // Opt-in player integration and authored-route checks. Uses real camera visibility,
    // collisions and puzzle conditions; automated positioning is not a blind playtest.
    public sealed class RuntimeVerification : MonoBehaviour
    {
        int passed,failed;
        string folder;
        GameFlow flow;
        WorldBuilder world;
        void Check(bool ok,string name) { if(ok) { passed++; Debug.Log("PASS: "+name); } else { failed++; Debug.LogError("FAIL: "+name); } }
        IEnumerator Start()
        {
            Application.targetFrameRate=60;
            folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../VerificationV2")); Directory.CreateDirectory(folder);
            yield return new WaitForSecondsRealtime(2);
            flow=GameFlow.Instance; world=GetComponent<WorldBuilder>(); flow.Begin(); flow.Player.enabled=false;
            Check(world.Wings.Count==8,"Eight distinct authored wings");
            Check(ImportedArt.Count==73,"Curated 73-model third-party catalog is bundled");
            foreach(var wingArt in world.Wings)
            {
                var imported=wingArt.GetComponentsInChildren<ImportedVisual>();
                Check(imported.Select(a=>a.assetKey).Distinct().Count()>=3,"Distinct imported architecture and furnishings in "+wingArt.name);
                Check(imported.All(a=>!a.GetComponentsInChildren<Collider>().Any(c=>c.enabled)),"Imported art preserves authored collision ownership in "+wingArt.name);
            }
            Check(FindObjectsByType<PuzzleRelay>(FindObjectsSortMode.None).Length==19,"Nineteen interaction stations including two instrument acquisitions");
            Check(FindObjectsByType<CircuitDoor>(FindObjectsSortMode.None).Length==8,"Seven exit interlocks and one intake-controlled service gate");
            Check(Resources.LoadAll<AudioClip>("Audio").Length==25,"25 designed sound assets available in player");
            yield return VerifyHero();
            var entity=FindObjectsByType<UnstableEntity>(FindObjectsSortMode.None).First(e=>e.label=="West ascent");
            yield return View(entity);
            int changes=entity.TransitionCount;
            Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8);
            yield return null; yield return null;
            Check(entity.TransitionCount==changes+1,"Looking away causes an immediate transition within one rendered frame");
            yield return new WaitForSeconds(.2f);
            Check(entity.TransitionCount==changes+1,"Staying unseen cannot keep cycling the state");
            yield return View(entity); changes=entity.TransitionCount;
            entity.SetAnchor(true); Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return null; yield return null;
            Check(entity.TransitionCount==changes,"Anchoring defeats the visibility edge"); entity.SetAnchor(false);
            flow.Pause(); yield return new WaitForSecondsRealtime(.1f); Check(Time.timeScale==0,"Pause stops simulation"); flow.Resume();
            Check(Resources.LoadAll<Texture2D>("Surfaces").Length==21,"Twenty-one authored surface maps are bundled");
            flow.Player.Teleport(world.PreviewPositions[2]); Look(world.PreviewTargets[2]); yield return new WaitForSeconds(.8f);
            var palette=world.Wings[2].GetComponent<EnvironmentThemeController>();
            var frustum=GeometryUtility.CalculateFrustumPlanes(flow.Player.eye);
            var surface=world.Wings[2].GetComponentsInChildren<Renderer>().Where(r=>r.enabled && !r.GetComponentInParent<SurfaceRevisionZone>() && r.GetComponentInParent<ImportedVisual>() && r.GetComponentInParent<ImportedVisual>().structural && GeometryUtility.TestPlanesAABB(frustum,r.bounds)).OrderByDescending(r=>Vector3.Dot(flow.Player.eye.transform.forward,r.bounds.center-flow.Player.eye.transform.position)).First();
            var bays=world.Wings[2].GetComponentsInChildren<SpatialRevision>();
            Check(bays.Length==1,"Atrium uses one authored transformation instead of repeated ceiling cycles");
            int architecturalChanges=bays.Sum(b=>b.AppliedChanges);
            var originalMaterial=surface.sharedMaterial; Capture("atrium-before");
            palette.RequestShift(); yield return new WaitForSeconds(.1f);
            Check(surface.sharedMaterial==originalMaterial,"A directly observed surface does not visibly reskin");
            int revisions=palette.AppliedSurfaceChanges;
            Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return new WaitForSeconds(.15f);
            Check(palette.AppliedSurfaceChanges>revisions && surface.sharedMaterial!=originalMaterial,"Looking away changes the surrounding surfaces as well as functional reserves");
            Check(bays.Sum(b=>b.AppliedChanges)==architecturalChanges,"Authored hero transformation does not revert on repeated turns");
            int heldChanges=bays.Sum(b=>b.AppliedChanges); yield return new WaitForSeconds(.2f);
            Check(bays.Sum(b=>b.AppliedChanges)==heldChanges,"Unseen architecture does not repeatedly cycle");
            Check(bays.All(b=>b.versions.Count(v=>v.activeSelf)==1),"Exactly one solid architectural version is active in every bay");
            Look(world.PreviewTargets[2]); yield return new WaitForSeconds(.1f); Capture("atrium-after");
            for(int i=0;i<world.Wings.Count;i++)
            {
                var wing=world.Wings[i];
                if(i==6) yield return VerifyGallery();
                flow.Player.Teleport(world.PreviewPositions[i]); Look(world.PreviewTargets[i]); yield return new WaitForSeconds(.15f); Capture("wing-"+(i+1));
                foreach(var relay in wing.GetComponentsInChildren<PuzzleRelay>())
                {
                    flow.Anchors.ReleaseAll();
                    if(relay.requirements.Length>0)
                    {
                        Check(!relay.Ready,"Unheld configuration rejected for "+relay.label);
                        foreach(var requirement in relay.requirements)
                        {
                            for(int attempt=0;attempt<7 && requirement.entity.CurrentForm!=requirement.form;attempt++)
                            {
                                yield return View(requirement.entity);
                                Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return null; yield return null;
                            }
                            Check(requirement.entity.CurrentForm==requirement.form,"Required form reachable: "+requirement.Description);
                            flow.Anchors.Toggle(requirement.entity);
                        }
                    }
                    foreach(var subject in relay.optics.Select(o=>o.subject).Distinct())
                    {
                        bool solved=relay.optics.Where(o=>o.subject==subject).All(o=>o.Met);
                        for(int attempt=0;attempt<10 && !solved;attempt++)
                        {
                            yield return View(subject);
                            Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return null; yield return null;
                            solved=relay.optics.Where(o=>o.subject==subject).All(o=>o.Met);
                        }
                        Check(solved,"Physical optical condition is reachable: "+relay.label+" / "+subject.label);
                        if(flow.Anchors.Capacity>flow.Anchors.Used) flow.Anchors.Toggle(subject);
                    }
                    flow.Player.Teleport(relay.transform.position-relay.transform.forward*2.2f+Vector3.up*.1f);
                    Look(relay.transform.position+Vector3.up*1.65f); yield return null; yield return null;
                    bool hit=Physics.Raycast(flow.Player.eye.transform.position,flow.Player.eye.transform.forward,out var target,3.3f,~(1<<8),QueryTriggerInteraction.Ignore);
                    Check(hit && target.collider.GetComponentInParent<PuzzleRelay>()==relay,"Interaction panel reachable: "+relay.label);
                    relay.Activate(); Check(relay.Active,"Circuit connected: "+relay.label);
                }
                yield return new WaitForSeconds(.1f);
                var door=wing.GetComponentsInChildren<CircuitDoor>().FirstOrDefault(d=>d.name=="Circuit interlock");
                if(door) { Check(door.Open,"Wing exit opens: "+wing.name); flow.Anchors.ReleaseAll(); yield return null; Check(door.Open,"Solved wing survives release"); }
            }
            var absence=FindObjectsByType<UnstableEntity>(FindObjectsSortMode.None).First(e=>e.label=="Door A");
            yield return Tune(absence,Form.Empty);
            flow.Player.Teleport(absence.transform.TransformPoint(new Vector3(0,.1f,-5))); Look(absence.transform.position+Vector3.up*1.6f); yield return null; yield return null;
            Check(flow.Anchors.FindTarget(flow.Player.eye)==absence,"Empty doorway can be targeted through its clear aperture");
            var blocker=WorldBuilder.Box("Temporary occlusion test",flow.Player.eye.transform.position+flow.Player.eye.transform.forward,new Vector3(2,3,.3f),world.Wings[0].GetComponent<EnvironmentThemeController>().wall,null);
            blocker.transform.rotation=flow.Player.eye.transform.rotation; Physics.SyncTransforms();
            Check(flow.Anchors.FindTarget(flow.Player.eye)!=absence,"A nearer solid wall blocks empty-state targeting"); Destroy(blocker); yield return null;
            var specimen=FindObjectsByType<UnstableEntity>(FindObjectsSortMode.None).First(e=>e.label=="Specimen A");
            yield return Tune(specimen,Form.Steps); specimen.SetAnchor(true);
            var mover=flow.Player.GetComponent<CharacterController>(); flow.Player.Teleport(specimen.transform.TransformPoint(new Vector3(0,.1f,-3.5f)));
            float climbTime=0; while(climbTime<2.1f) { mover.Move(specimen.transform.forward*3.7f*Time.deltaTime); mover.Move(Vector3.down*2*Time.deltaTime); climbTime+=Time.deltaTime; yield return null; }
            Check(flow.Player.transform.position.y>2.2f,"Rise test is physically reachable by climbing its required stair");
            var risePanel=world.Wings[3].GetComponentsInChildren<PuzzleRelay>().First(r=>r.label=="RISE TEST");
            Check(risePanel.transform.localPosition.y==2.4f,"Existing Rise objective uses the elevated landing");
            int interpretation=specimen.Interpretation; world.Wings[3].GetComponent<EnvironmentThemeController>().RequestShift(); yield return null;
            Check(specimen.Interpretation==interpretation,"Held form preserves its architectural identity across room revisions");
            Check(specimen.GetComponentsInChildren<Renderer>().Any(r=>r.enabled && r.name!="State geometry"),"Functional stair has a separate architectural visual model");
            specimen.SetAnchor(false);
            // Movement regression: real CharacterController climbs the new ascending form.
            yield return Tune(entity,Form.Steps); entity.SetAnchor(true);
            var local=entity.transform; var cc=flow.Player.GetComponent<CharacterController>();
            flow.Player.gameObject.AddComponent<MovementDiagnostics>();
            flow.Player.Teleport(local.TransformPoint(new Vector3(0,.1f,-entity.length/2-.7f)));
            float elapsed=0;
            while(elapsed<3.2f) { cc.Move(local.forward*3.7f*Time.deltaTime); cc.Move(Vector3.down*2*Time.deltaTime); elapsed+=Time.deltaTime; yield return null; }
            Check(flow.Player.transform.position.y>2.2f,"CharacterController climbs full 2.4 metre unstable staircase; local endpoint "+local.InverseTransformPoint(flow.Player.transform.position));
            flow.Player.Teleport(local.TransformPoint(new Vector3(-3,3,-7))); Look(local.position+Vector3.up); Capture("stair-inspection");
            flow.Player.Checkpoint=world.Wings[7].TransformPoint(new Vector3(0,.1f,3)); flow.Respawn();
            Check(Vector3.Distance(flow.Player.transform.position,flow.Player.Checkpoint)<.05f,"Checkpoint restores position");
            Check(flow.ConnectedCircuits==17,"All seventeen departure circuits recorded");
            yield return VerifyPhysicalRedesign();
            yield return CaptureInterpretations();
            yield return VerifyRevisionSafety();
            flow.Player.Teleport(new Vector3(2,.1f,-24)); Look(new Vector3(-12,3,-24)); yield return new WaitForSeconds(.2f);
            Check(Soundscape.Instance.Outside,"The outdoor soundscape begins at the daylight threshold"); Capture("outside");
            flow.Player.Teleport(new Vector3(-10,.1f,-24)); yield return new WaitForSecondsRealtime(.2f);
            Check(flow.State==FlowState.Ending,"Stable threshold triggers ending");
            File.WriteAllText(Path.Combine(folder,"results.txt"),passed+" passed; "+failed+" failed. Automated checks are not a blind human playthrough.\n");
            Debug.Log("UNOBSERVED_V2_RESULT: "+passed+" passed; "+failed+" failed"); Application.Quit(failed==0?0:1);
        }
        IEnumerator VerifyPhysicalRedesign()
        {
            flow.Anchors.ReleaseAll();
            foreach(string label in new[]{"Specimen A","Lens B","Return D"})
            {
                var e=FindObjectsByType<UnstableEntity>(FindObjectsSortMode.None).First(x=>x.label==label);
                yield return Tune(e,Form.Steps); e.SetAnchor(true);
                flow.Player.Teleport(e.transform.TransformPoint(new Vector3(0,.1f,-e.length/2-.6f)));
                Look(e.transform.TransformPoint(new Vector3(0,3,e.length/2)));
                var cc=flow.Player.GetComponent<CharacterController>(); float elapsed=0;
                while(elapsed<(e.length+1.8f)/3.7f)
                { cc.Move(e.transform.forward*3.7f*Time.deltaTime); cc.Move(Vector3.down*2*Time.deltaTime); elapsed+=Time.deltaTime; yield return null; }
                Check(flow.Player.transform.position.y>2.2f,"Physical stair access reaches redesigned upper station: "+label);
                Capture("access-"+label.Replace(" ","-")); e.SetAnchor(false);
            }
            foreach(string label in new[]{"Specimen A","Lens A"})
            {
                var e=FindObjectsByType<UnstableEntity>(FindObjectsSortMode.None).First(x=>x.label==label);
                yield return Tune(e,Form.Bridge); e.SetAnchor(true);
                var origin=e.transform.TransformPoint(new Vector3(e.width/2+1,1,0));
                Check(!Physics.Raycast(origin,Vector3.down,1.2f,~(1<<8),QueryTriggerInteraction.Ignore),"Test-bed edge is a real gap rather than a flat-floor shortcut: "+label);
                // The workshop has a low access shoulder beside its elevated inspection landing.
                flow.Player.Teleport(e.transform.TransformPoint(new Vector3(label=="Specimen A"?-2:0,.1f,-e.length/2-.6f)));
                var cc=flow.Player.GetComponent<CharacterController>(); float elapsed=0;
                while(elapsed<(e.length+.9f)/3.7f)
                { cc.Move(e.transform.forward*3.7f*Time.deltaTime); cc.Move(Vector3.down*2*Time.deltaTime); elapsed+=Time.deltaTime; yield return null; }
                Check(e.transform.InverseTransformPoint(flow.Player.transform.position).z>e.length/2-.25f && flow.Player.transform.position.y>-.2f,"Actual traversal crosses the redesigned gap: "+label); e.SetAnchor(false);
            }
            foreach(var relay in FindObjectsByType<PuzzleRelay>(FindObjectsSortMode.None).Where(r=>r.label=="ZENITH"||r.label=="RETURN"))
            {
                var origin=relay.transform.position+Vector3.down*2.4f+Vector3.up*1.64f-relay.transform.forward;
                var ray=relay.transform.position+Vector3.up*1.65f-origin;
                bool hit=Physics.Raycast(origin,ray.normalized,out var result,ray.magnitude,~(1<<8),QueryTriggerInteraction.Ignore);
                Check(hit && result.collider.GetComponentInParent<PuzzleRelay>()!=relay,"Upper station cannot be used through its floor: "+relay.label);
            }
            foreach(var relay in FindObjectsByType<PuzzleRelay>(FindObjectsSortMode.None))
                Check(relay.requirements.Length==0,"Relay uses physical access/signal outcomes rather than named-form passwords: "+relay.label);
        }
        IEnumerator VerifyRevisionSafety()
        {
            var observer=ObservationManager.Instance; var eye=flow.Player.eye;
            var stage=new GameObject("Visibility regression stage").transform; stage.position=new Vector3(1500,0,1500);
            for(int f=0;f<5;f++)
            {
                var model=new GameObject("Slow sweep / "+(Form)f); model.transform.SetParent(stage,false);
                FormArchitecture.Build(model.transform,(Form)f,8,12,2.4f,1);
                var target=model.AddComponent<ObservationTarget>(); target.center=new Vector3(0,1,0); target.size=new Vector3(8,5,12); target.RefreshVisualBounds();
                flow.Player.Teleport(stage.TransformPoint(new Vector3(0,.1f,-18)));
                bool protectedEdge=true,marginSeen=false,finallyHidden=false;
                var exact=new Plane[6];
                for(float yaw=25;yaw<=120;yaw+=.5f)
                {
                    eye.transform.rotation=Quaternion.Euler(0,yaw,0);
                    GeometryUtility.CalculateFrustumPlanes(eye,exact);
                    bool intersects=GeometryUtility.TestPlanesAABB(exact,target.WorldBounds);
                    bool retained=observer.MaySee(target.WorldBounds,target.transform);
                    if(intersects && !retained) protectedEdge=false;
                    if(!intersects && retained) marginSeen=true;
                    finallyHidden=!retained;
                    if(Mathf.Abs(yaw-65)<.1f) Capture("edge-"+(Form)f);
                    yield return null;
                }
                Check(protectedEdge && marginSeen && finallyHidden,"Slow half-degree sweep protects final edge and releases beyond margin: "+(Form)f);
                eye.transform.rotation=Quaternion.identity;
                var blocker=GameObject.CreatePrimitive(PrimitiveType.Cube); blocker.transform.position=stage.TransformPoint(new Vector3(0,2,-10)); blocker.transform.localScale=new Vector3(30,20,1); Physics.SyncTransforms();
                Check(!observer.MaySee(target.WorldBounds,target.transform),"Complete wall occlusion remains valid: "+(Form)f);
                blocker.transform.localScale=new Vector3(.6f,20,1); Physics.SyncTransforms();
                Check(observer.MaySee(target.WorldBounds,target.transform),"A narrow pillar cannot hide exposed target edges: "+(Form)f);
                blocker.transform.position=stage.TransformPoint(new Vector3(-8,2,-10)); blocker.transform.localScale=new Vector3(14,20,1);
                var right=GameObject.CreatePrimitive(PrimitiveType.Cube); right.transform.position=stage.TransformPoint(new Vector3(8,2,-10)); right.transform.localScale=new Vector3(14,20,1); Physics.SyncTransforms();
                Check(observer.MaySee(target.WorldBounds,target.transform),"An opening between walls preserves observation: "+(Form)f);
                Destroy(right); Destroy(blocker); Destroy(model); yield return null;
            }
            var decorations=FindObjectsByType<DecorativeRevision>(FindObjectsSortMode.None);
            Check(decorations.Length==24,"Twenty-four curated decorative slots across eight wings");
            foreach(var decor in decorations)
            {
                var envelope=decor.GetComponent<ObservationTarget>().WorldBounds; bool contained=true;
                foreach(var version in decor.versions) foreach(var r in version.GetComponentsInChildren<Renderer>(true))
                    contained &= envelope.Contains(r.bounds.min) && envelope.Contains(r.bounds.max);
                Check(contained && decor.GetComponentsInChildren<Collider>(true).Length==0,"Decor variants remain inside their collider-free envelope: "+decor.name);
            }
            var prop=new GameObject("Decor sweep"); prop.transform.SetParent(stage,false);
            var revision=prop.AddComponent<DecorativeRevision>(); revision.Initialize(new[]{"KenneyFurniture/loungeChair","KenneyFurniture/pottedPlant","KenneyFurniture/bookcaseClosedDoors"},17);
            flow.Player.Teleport(stage.TransformPoint(new Vector3(0,.1f,-5))); Look(stage.position+Vector3.up);
            yield return null; yield return null;
            Capture("decor-before");
            bool decorSafe=true; var clip=new Plane[6];
            for(float yaw=0;yaw<115;yaw+=.5f)
            {
                eye.transform.rotation=Quaternion.Euler(0,yaw,0); GeometryUtility.CalculateFrustumPlanes(eye,clip);
                if(GeometryUtility.TestPlanesAABB(clip,revision.GetComponent<ObservationTarget>().WorldBounds) && revision.Changes!=0) decorSafe=false;
                yield return null;
            }
            Check(decorSafe && revision.Changes==1,"Decor changes once after slow edge sweep, never while visible");
            int changes=revision.Changes; yield return new WaitForSeconds(.2f);
            Check(revision.Changes==changes,"Unseen decorative props do not reroll in the background");
            Look(stage.position+Vector3.up); Capture("decor-after");
            var zone=world.Wings[0].GetComponentInChildren<SurfaceRevisionZone>();
            flow.Player.Teleport(zone.WorldBounds.center+Vector3.up*2.6f); Look(zone.WorldBounds.center);
            Capture("floor-region-before");
            int next=(zone.Palette+1)%3; var finishes=world.Wings[0].GetComponent<EnvironmentThemeController>(); var finish=finishes.FloorFinish(next);
            Check(!zone.TryCommit(next,finish),"A visible floor region cannot partially commit");
            eye.transform.rotation=Quaternion.Euler(-90,0,0);
            Check(zone.TryCommit(next,finish) && zone.surfaces.All(r=>r.sharedMaterial==finish),"Entire floor region commits one finish atomically when safely unseen");
            Look(zone.WorldBounds.center); Capture("floor-region-after");
            for(int i=0;i<3;i++)
            {
                eye.transform.rotation=Quaternion.Euler(-90,0,0); zone.TryCommit(i,finishes.FloorFinish(i));
                Look(zone.WorldBounds.center); Capture("floor-finish-"+i);
            }
            Check(finishes.FloorFinish(0).GetTexture("_BaseMap")!=finishes.FloorFinish(1).GetTexture("_BaseMap") && finishes.FloorFinish(1).GetTexture("_BaseMap")!=finishes.FloorFinish(2).GetTexture("_BaseMap"),"Floor realities use categorical tile, concrete and metal maps");
            Destroy(stage.gameObject); yield return null;
        }
        IEnumerator VerifyGallery()
        {
            var wing=world.Wings[6]; var hero=wing.GetComponentInChildren<SpatialRevision>();
            flow.Player.Teleport(wing.TransformPoint(new Vector3(-8,.1f,4))); Look(wing.TransformPoint(new Vector3(-8,2,11))); yield return null; yield return null;
            Capture("hero-gallery-before");
            Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return null; yield return null;
            Check(hero.Current==1,"Gallery opening becomes the authored deep vestibule");
            Look(wing.TransformPoint(new Vector3(-8,2,11))); yield return null; Capture("hero-gallery-after");
            var door=wing.GetComponentsInChildren<UnstableEntity>().First(e=>e.label=="Door A"); yield return Tune(door,Form.Empty); door.SetAnchor(true);
            flow.Player.Teleport(wing.TransformPoint(new Vector3(-8,.1f,6))); var cc=flow.Player.GetComponent<CharacterController>();
            float seconds=0; while(seconds<2.5f) { cc.Move(wing.forward*3.7f*Time.deltaTime); cc.Move(Vector3.down*2*Time.deltaTime); seconds+=Time.deltaTime; yield return null; }
            Check(wing.InverseTransformPoint(flow.Player.transform.position).z>12.5f,"CharacterController crosses the transformed gallery aperture");
            door.SetAnchor(false);
        }
        IEnumerator CaptureInterpretations()
        {
            var stage=new GameObject("Verification-only architectural comparison"); stage.transform.position=new Vector3(250,0,250);
            var floorMaterial=WorldBuilder.Material(new Color(.35f,.36f,.34f));
            FormArchitecture.Part(stage.transform,new Vector3(0,-.45f,0),new Vector3(30,.2f,12),floorMaterial);
            var lamp=stage.AddComponent<Light>(); lamp.type=LightType.Point; lamp.range=35; lamp.intensity=4;
            var lighting=new GameObject("Comparison soft light"); lighting.transform.SetParent(stage.transform,false); lighting.transform.localPosition=new Vector3(0,8,-3); var light=lighting.AddComponent<Light>(); light.range=30; light.intensity=3;
            for(int reality=0;reality<3;reality++)
            {
                var set=new GameObject("Interpretation "+reality); set.transform.SetParent(stage.transform,false);
                for(int f=0;f<5;f++)
                {
                    var model=new GameObject(((Form)f).ToString()); model.transform.SetParent(set.transform,false); model.transform.localPosition=new Vector3((f-2)*5.5f,0,0);
                    FormArchitecture.Build(model.transform,(Form)f,4,5,2.4f,reality);
                    Check(model.GetComponentsInChildren<Renderer>().Length>=1 && model.GetComponentsInChildren<MeshFilter>().Any(m=>m.sharedMesh && m.sharedMesh.vertexCount>8) && !model.GetComponentsInChildren<Collider>().Any(c=>c.enabled),"Architectural interpretation "+reality+" / "+(Form)f+" preserves separate gameplay collision");
                }
                flow.Player.Teleport(stage.transform.TransformPoint(new Vector3(-5,5,-15))); Look(stage.transform.TransformPoint(new Vector3(0,1,0))); yield return null;
                Capture("forms-"+reality); Destroy(set); yield return null;
            }
            Destroy(stage); Destroy(floorMaterial); yield return null;
        }
        IEnumerator VerifyHero()
        {
            var hero=world.Wings[2].GetComponentInChildren<SpatialRevision>();
            flow.Player.Teleport(world.PreviewPositions[2]); Look(hero.WorldEnvelope.center); yield return null; yield return null;
            Check(hero.Current==0,"Atrium garden remains its authored initial state until observed and left behind"); Capture("hero-atrium-before");
            Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return null; yield return null;
            Check(hero.Current==1 && hero.AppliedChanges==1,"Unseen garden is replaced by the complete service core");
            Look(hero.WorldEnvelope.center); yield return null; Capture("hero-atrium-after");
            Check(world.Wings[6].GetComponentInChildren<SpatialRevision>().Current==0,"An occluded unvisited hero cannot change remotely");
        }
        IEnumerator Tune(UnstableEntity entity,Form form)
        {
            entity.SetAnchor(false);
            for(int i=0;i<7 && entity.CurrentForm!=form;i++) { yield return View(entity); Look(flow.Player.eye.transform.position-flow.Player.eye.transform.forward*8); yield return null; yield return null; }
        }
        IEnumerator View(UnstableEntity entity)
        {
            var obs=entity.GetComponent<ObservationTarget>(); bool found=false;
            for(int i=0;i<8 && !found;i++)
            {
                float angle=i*Mathf.PI/4;
                Vector3 offset=new Vector3(Mathf.Sin(angle)*(entity.width/2+3),.1f,Mathf.Cos(angle)*(entity.length/2+3));
                flow.Player.Teleport(entity.transform.TransformPoint(offset)); Look(entity.transform.position+Vector3.up*1.3f);
                yield return null; yield return null; found=obs.Visible;
            }
            Check(found,"Reserve visible from a valid sampled viewpoint: "+entity.label);
        }
        void Look(Vector3 p)=>flow.Player.eye.transform.rotation=Quaternion.LookRotation(p-flow.Player.eye.transform.position);
        void Capture(string name)
        {
            var camera=flow.Player.eye; var target=new RenderTexture(1280,720,24); var old=RenderTexture.active;
            camera.targetTexture=target; camera.Render(); RenderTexture.active=target;
            var image=new Texture2D(1280,720,TextureFormat.RGB24,false); image.ReadPixels(new Rect(0,0,1280,720),0,0); image.Apply();
            File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG()); camera.targetTexture=null; RenderTexture.active=old;
            target.Release(); Destroy(target); Destroy(image);
        }
    }
}
