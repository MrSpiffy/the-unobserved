using System.Collections.Generic;
using UnityEngine;
namespace Unobserved
{
    // Fixed wing plans. Every position is local to its wing, including puzzle geometry.
    public sealed class WorldBuilder : MonoBehaviour
    {
        public RealityState[] realityStates;
        public EnvironmentThemeData[] themes;
        public TransitionSettings transitions;
        public readonly List<Transform> Wings=new List<Transform>();
        public readonly List<Vector3> PreviewPositions=new List<Vector3>(), PreviewTargets=new List<Vector3>();
        Transform room;
        Material wall,floor,trim,teal,amber,wood,metal,leaf,glass,marking;
        float width,length,height;
        int wing;
        readonly List<PuzzleRelay> relays=new List<PuzzleRelay>();
        void Awake()
        {
            var flow=gameObject.AddComponent<GameFlow>(); gameObject.AddComponent<Soundscape>();
            trim=Material(new Color(.075f,.095f,.1f)); teal=Material(new Color(.18f,.8f,.67f),true); amber=Material(new Color(.98f,.54f,.21f),true);
            marking=Material(new Color(.5f,.42f,.25f)); SurfaceLibrary.Apply(marking,6);
            wood=Material(new Color(.28f,.15f,.085f)); SurfaceLibrary.Apply(wood,3); metal=Material(new Color(.22f,.28f,.29f)); SurfaceLibrary.Apply(metal,4);
            leaf=Material(new Color(.12f,.23f,.19f)); glass=Material(new Color(.07f,.18f,.19f)); SurfaceLibrary.Apply(glass,5); glass.SetFloat("_Glossiness",.9f);
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight=new Color(.19f,.22f,.24f);
            RenderSettings.fog=true; RenderSettings.fogMode=FogMode.ExponentialSquared; RenderSettings.fogDensity=.009f;
            BuildPlayer(flow); var observer=gameObject.AddComponent<ObservationManager>(); observer.eye=flow.Player.eye; observer.settings=transitions;
            BeginWing("Reception",Vector3.zero,0,24,32,6,0); Reception(); EndWing();
            BeginWing("Archive of absences",new Vector3(0,0,42),0,32,38,7,0); Archive(); EndWing();
            BeginWing("Winter atrium",new Vector3(0,0,90),0,42,46,12,1); Atrium(); EndWing();
            BeginWing("Calibration workshop",new Vector3(30,0,156),90,32,40,8,2); Workshop(); EndWing();
            BeginWing("The reservoir",new Vector3(86,0,136),180,36,44,10,1); Reservoir(); EndWing();
            BeginWing("Parallax observatory",new Vector3(86,0,80),180,34,40,14,2); Observatory(); EndWing();
            BeginWing("The return gallery",new Vector3(86,0,28),180,32,36,7,0); Gallery(); EndWing();
            BeginWing("Continuity engine",new Vector3(60,0,-24),270,38,40,11,2); Engine(); EndWing();
            Corridor(new Vector3(0,0,32),new Vector3(0,0,42)); Corridor(new Vector3(0,0,80),new Vector3(0,0,90));
            Corridor(new Vector3(0,0,136),new Vector3(0,0,156)); Corridor(new Vector3(0,0,156),new Vector3(30,0,156));
            Corridor(new Vector3(70,0,156),new Vector3(86,0,156)); Corridor(new Vector3(86,0,156),new Vector3(86,0,136));
            Corridor(new Vector3(86,0,92),new Vector3(86,0,80)); Corridor(new Vector3(86,0,40),new Vector3(86,0,28));
            Corridor(new Vector3(86,0,-8),new Vector3(86,0,-24)); Corridor(new Vector3(86,0,-24),new Vector3(60,0,-24));
            Corridor(new Vector3(20,0,-24),new Vector3(-2,0,-24));
            room=new GameObject("Stable threshold").transform; room.SetParent(transform); room.position=new Vector3(8,0,-24); room.rotation=Quaternion.Euler(0,270,0);
            B("Threshold",new Vector3(0,-.3f,13),new Vector3(18,.6f,26),Material(new Color(.65f,.65f,.54f)));
            RenderSettings.skybox=new Material(Resources.Load<Shader>("DaylightSky"));
            foreach(int side in new[]{-1,1}) B("Terrace parapet",new Vector3(side*9,.8f,14),new Vector3(.3f,1.6f,24),Material(new Color(.56f,.61f,.52f)));
            for(int i=0;i<7;i++) B("Distant roofs",new Vector3(-36+i*12,1.5f+(i%3),48),new Vector3(10,3+(i%3)*2,10),Material(new Color(.32f,.42f,.4f)));
            Plant(new Vector3(-6,0,15),3); Plant(new Vector3(7,0,22),3.5f); Bench(new Vector3(5,0,16));
            Light(new Vector3(0,10,17),new Color(1,.88f,.66f),4,28,true);
            Sign("OUTSIDE",new Vector3(0,3,5),.13f);
            Trigger("Fresh air",new Vector3(0,1,6),new Vector3(15,3,5)).AddComponent<OutsideAudio>();
            Trigger("Final threshold",new Vector3(0,1,18),new Vector3(17,3,2)).AddComponent<ExitTrigger>();
            gameObject.AddComponent<GameUI>(); if(GameFlow.Verifying) gameObject.AddComponent<RuntimeVerification>();
        }
        void BuildPlayer(GameFlow flow)
        {
            var go=new GameObject("Player"); go.layer=8; go.transform.position=new Vector3(0,.1f,3);
            var cc=go.AddComponent<CharacterController>(); cc.height=1.8f; cc.center=new Vector3(0,.9f,0); cc.radius=.3f; cc.stepOffset=.35f; cc.slopeLimit=48; cc.skinWidth=.03f; cc.minMoveDistance=0;
            var p=go.AddComponent<FirstPersonController>(); p.Checkpoint=go.transform.position;
            var cameraGo=new GameObject("Eye"); cameraGo.transform.SetParent(go.transform,false); cameraGo.transform.localPosition=new Vector3(0,1.64f,0);
            var eye=cameraGo.AddComponent<Camera>(); eye.tag="MainCamera"; eye.nearClipPlane=.05f; eye.farClipPlane=110; eye.fieldOfView=PlayerPrefs.GetFloat("fov",78); eye.clearFlags=CameraClearFlags.SolidColor; eye.allowHDR=true;
            cameraGo.AddComponent<AudioListener>(); cameraGo.AddComponent<AtmosphereEffect>(); p.eye=eye; p.sensitivity=PlayerPrefs.GetFloat("sensitivity",1.8f);
            flow.Player=p; flow.Anchors=go.AddComponent<AnchorManager>(); go.AddComponent<PlayerInteraction>();
        }
        void BeginWing(string name,Vector3 origin,float yaw,float w,float l,float h,int theme)
        {
            room=new GameObject((wing+1).ToString("00")+" / "+name).transform; room.SetParent(transform); room.position=origin; room.rotation=Quaternion.Euler(0,yaw,0);
            Wings.Add(room); width=w; length=l; height=h; relays.Clear();
            wall=Material(themes[theme].walls); SurfaceLibrary.Apply(wall,theme==0?6:0); floor=Material(themes[theme].floor); SurfaceLibrary.Apply(floor,wing==0||wing==1||wing==6?2:1);
            foreach(int side in new[]{-1,1})
            {
                var shell=B("Stable structural shell",new Vector3(side*(w/2+.25f),h/2,l/2),new Vector3(.5f,h,l),wall); shell.GetComponent<Renderer>().enabled=false;
                int panels=Mathf.CeilToInt(l/5);
                for(int i=0;i<panels;i++)
                {
                    var panel=B("Wall surface",new Vector3(side*(w/2+.245f),h/2,(i+.5f)*l/panels),new Vector3(.51f,h,l/panels),wall); panel.GetComponent<Collider>().enabled=false;
                }
            }
            foreach(float z in new[]{0f,l})
            {
                foreach(int side in new[]{-1,1}) B("Portal wall",new Vector3(side*(w/4+1.5f),h/2,z),new Vector3(w/2-3,h,.5f),wall);
                B("Portal lintel",new Vector3(0,(h+4.6f)/2,z),new Vector3(6,h-4.6f,.5f),wall);
                foreach(int side in new[]{-1,1}) B("Door jamb",new Vector3(side*2.93f,2.3f,z),new Vector3(.12f,4.6f,.6f),trim);
            }
            B("Roof",new Vector3(0,h+.2f,l/2),new Vector3(w+.5f,.4f,l),trim);
            foreach(int s in new[]{-1,1})
            {
                B("Skirting",new Vector3(s*(w/2-.07f),.16f,l/2),new Vector3(.13f,.32f,l),trim);
                B("Picture rail",new Vector3(s*(w/2-.08f),3.6f,l/2),new Vector3(.15f,.08f,l),wood);
                for(float z=4;z<l;z+=7)
                {
                    B("Pilaster",new Vector3(s*(w/2-.45f),h/2,z),new Vector3(.9f,h,.55f),trim);
                    B("Wall sconce",new Vector3(s*(w/2-.92f),2.7f,z),new Vector3(.12f,.6f,.22f),theme==2?amber:teal);
                }
            }
            for(float z=5;z<l;z+=9)
            {
                B("Ceiling beam",new Vector3(0,h-.35f,z),new Vector3(w,.65f,.4f),wall);
                B("Recessed luminaire",new Vector3(0,h-.73f,z),new Vector3(4,.09f,.4f),theme==2?amber:teal);
                Light(new Vector3(0,Mathf.Min(h-1,6.5f),z),themes[theme].accent,1.6f,14,z==5);
            }
            Sign((wing+1).ToString("00")+" / "+name.ToUpperInvariant(),new Vector3(0,3.5f,2),.13f);
            var obs=room.gameObject.AddComponent<ObservationTarget>(); obs.center=new Vector3(0,h/2,l/2); obs.size=new Vector3(w,h,l);
            var controller=room.gameObject.AddComponent<EnvironmentThemeController>(); controller.themes=themes; controller.wall=wall; controller.floor=floor; controller.accent=Material(themes[theme].accent,true); controller.initialTheme=theme; controller.props=new GameObject[0];
            var progress=Trigger("Arrival checkpoint",new Vector3(0,1,2.5f),new Vector3(5,3,2)).AddComponent<Progression>(); progress.number=wing+1; progress.section=name; progress.checkpoint=room.TransformPoint(new Vector3(0,.1f,3)); progress.checkpointYaw=yaw;
            PreviewPositions.Add(room.TransformPoint(new Vector3(-w*.19f,2,5))); PreviewTargets.Add(room.TransformPoint(new Vector3(0,2,l*.65f)));
        }
        void EndWing()
        {
            var emitter=new GameObject("Local ventilation / water source"); emitter.transform.SetParent(room,false); emitter.transform.localPosition=new Vector3(width/2-1,3,length*.5f);
            emitter.AddComponent<LocalAmbience>().sound=wing==2||wing==4?1:0;
            // Peripheral filler reserves were removed; contrast comes from authored transformations.
            if(relays.Count>0) Door(length-.6f,relays.ToArray());
            var theme=room.GetComponent<EnvironmentThemeController>();
            foreach(var light in room.GetComponentsInChildren<Light>()) if(light.shadows==LightShadows.None && light.intensity<=1.6f) theme.lights.Add(light);
            RoomArchitecture.Build(room,width,length,height,wing);
            FacilityArt.FinishRoom(room,width,length,height,wing);
            UnstableDecor.Dress(room,wing);
            wing++;
        }
        void Reception()
        {
            B("Sealed exterior door",new Vector3(0,2.3f,.55f),new Vector3(5.8f,4.6f,.3f),metal);
            Sign("EXTERIOR DOORS OFFLINE\nRESTORE THE DEPARTURE LINE",new Vector3(0,2.7f,4.5f),.085f);
            Deck(0,32); FacilityArt.ReceptionDesk(room);
            Sign("LAST VISITOR: STILL INSIDE\nDEPARTURE LINE: OFFLINE",new Vector3(-5,2.55f,12.1f),.085f);
            FacilityArt.WaitingRoom(room);
            Plant(new Vector3(-10,0,5),1); Plant(new Vector3(10,0,27),1.4f);
            Partition(18,3,5,3.5f); Partition(25,-5,5,3.5f); Entity(new Vector3(-6,0,21.5f),2,2,new[]{2,3},"Visitor furniture");
            B("Clock face",new Vector3(0,4,31.6f),new Vector3(1.2f,1.2f,.08f),wall); B("Clock hand",new Vector3(0,4.17f,31.5f),new Vector3(.04f,.38f,.04f),trim);
            Sign("ARCHIVE  →",new Vector3(-5,2.5f,24.5f),.1f);
        }
        void Archive()
        {
            Deck(0,38); float[] z={10,20,30},x={-9,9,-7};
            for(int i=0;i<3;i++)
            {
                Partition(z[i],x[i],4,5); var shutter=Entity(new Vector3(x[i],0,z[i]),3.9f,.7f,new[]{2,3},"Stack shutter "+(i+1));
                var record=Relay("Archive circuit "+(i+1),new Vector3(x[i]+3.5f,0,z[i]-2));
                record.clue="KEEP THE RECORDS SIGHTLINE OPEN"; record.optics=new[]{Optical(shutter,1.5f,false,"RECORDS PATH",true)};
                if(i>0) record.prerequisite=relays[i-1]; relays.Add(record);
                for(int j=0;j<3;j++) Shelf(new Vector3(-9+j*8,0,z[i]-4),j+i);
                Sign("STACK "+(i+1)+"   /   "+(x[i]<0?"←":"→"),new Vector3(0,3.3f,z[i]-.5f),.1f);
            }
        }
        void Atrium()
        {
            Deck(0,10); Deck(34,46); Water(23,28);
            Entity(new Vector3(-12,0,14),5,8,new[]{0,1,3},"West ascent"); Floor(new Vector3(-12,2.4f,22),new Vector2(9,8)); StableStairs(new Vector3(-12,0,30),5,8,2.4f,true);
            Entity(new Vector3(12,0,14),5,8,new[]{3,4,0},"East causeway"); Floor(new Vector3(12,0,22),new Vector2(9,8)); var eastReturn=Entity(new Vector3(12,0,30),5,8,new[]{2,0},"East return");
            var west=Relay("West ventilation",new Vector3(-14,2.4f,23)); west.clue="UPPER ACCESS / WEST DAMPER"; relays.Add(west);
            var east=Relay("East ventilation",new Vector3(14,0,23)); east.prerequisite=west; east.clue="CLEAR THE RETURN DUCT"; east.optics=new[]{Optical(eastReturn,.7f,false,"RETURN PATH")}; relays.Add(east);
            var atriumRoom=room; var garden=new GameObject("Winter garden"); garden.transform.SetParent(room,false); room=garden.transform; Tree(new Vector3(0,-2,23)); room=atriumRoom; RoomArchitecture.Atrium(room,garden);
            for(int i=0;i<7;i++) B("Skylight mullion",new Vector3(-18+i*6,11.65f,23),new Vector3(.15f,.2f,40),metal);
            B("Skylight glow",new Vector3(0,11.9f,23),new Vector3(38,.08f,36),Material(new Color(.24f,.39f,.44f),true));
            Ring(new Vector3(0,7,23),7,teal,.07f); Light(new Vector3(0,8,23),new Color(.42f,.65f,.8f),3,26,true);
            Sign("VENTILATION\nWEST + EAST",new Vector3(0,2.6f,8),.11f); Plant(new Vector3(-18,0,5),1.5f); Plant(new Vector3(18,0,39),1.8f);
        }
        void Workshop()
        {
            Deck(0,17); Deck(23,40); Water(20,6); Pickup(new Vector3(-4,0,5),1); var specimen=Entity(new Vector3(0,0,20),5,6,new[]{2,0,1},"Specimen A");
            Ring(new Vector3(0,.035f,20),6,amber,.045f); B("Calibration backdrop",new Vector3(0,3,27),new Vector3(11,6,.6f),trim);
            foreach(int s in new[]{-1,1}) B("Observation baffle",new Vector3(s*7,1.8f,20),new Vector3(.5f,3.6f,11),wall);
            var load=Relay("LOAD TEST",new Vector3(-8,0,28)); load.clue="SERVICE ACCESS / CROSS THE TEST BED"; relays.Add(load);
            Floor(new Vector3(0,2.4f,25),new Vector2(3,4));
            Sign("LOW ACCESS  ←    ↑  INSPECTION",new Vector3(0,2.15f,23),.043f);
            var riseTest=Relay("RISE TEST",new Vector3(0,2.4f,25)); riseTest.prerequisite=load; riseTest.clue="UPPER INSPECTION ACCESS"; relays.Add(riseTest);
            var mass=Relay("MASS TEST",new Vector3(0,0,32)); mass.prerequisite=riseTest; mass.clue="SHIELD THE UPPER CALIBRATION BEAM"; mass.optics=new[]{Optical(specimen,3.1f,true,"UPPER SHIELD")}; relays.Add(mass);
            foreach(int s in new[]{-1,1}) foreach(float z in new[]{6f,14f,32f}) Workbench(new Vector3(s*14,0,z));
            Sign("ACCESS → INSPECTION → SHIELD\nA HELD STRUCTURE SURVIVES YOUR DEPARTURE",new Vector3(0,3,10),.08f); Pipe(new Vector3(-14,6,4),new Vector3(-14,6,37),.15f,metal); Ring(new Vector3(0,6.5f,20),4,amber,.13f);
        }
        void Reservoir()
        {
            Deck(0,8); Deck(36,44); Water(22,30);
            var a=Entity(new Vector3(-10,0,13),4,10,new[]{3,0,4},"Intake span"); var b=Entity(new Vector3(10,0,13),4,10,new[]{3,4,0},"Filter span");
            Floor(new Vector3(-10,0,22),new Vector2(9,8)); Floor(new Vector3(10,0,22),new Vector2(9,8));
            Entity(new Vector3(-10,0,31),4,10,new[]{3,0},"Intake return"); Entity(new Vector3(10,0,31),4,10,new[]{2,0},"Filter return");
            var first=Relay("INTAKE",new Vector3(-12,0,23)); first.clue="INTAKE ENABLES FILTER ACCESS";
            var second=Relay("FILTER",new Vector3(12,0,23)); second.clue="FILTER ACCESS / SERVICE CHANNEL"; second.prerequisite=first; relays.Add(first); relays.Add(second);
            ServiceGate(new Vector3(10,0,7.8f),4.6f,first);
            foreach(int s in new[]{-1,1})
            {
                 Pipe(new Vector3(s*15,7,4),new Vector3(s*15,7,40),.2f,metal);
                // Imported hopper vessels are authored in FacilityArt.
            }
            Sign("01 INTAKE   →   02 FILTER\nA CONNECTED CIRCUIT REMAINS ON",new Vector3(0,2.7f,6),.085f); Light(new Vector3(0,4,22),new Color(.19f,.55f,.57f),2,23,false);
        }
        void Observatory()
        {
            Deck(0,17); Deck(23,40); Water(20,6); Pickup(new Vector3(-4,0,5),2);
            var a=Entity(new Vector3(-8,0,20),4,6,new[]{2,0,1},"Lens A"); var b=Entity(new Vector3(8,0,20),4,6,new[]{1,2,0},"Lens B");
            B("Central occluder",new Vector3(0,3.5f,20),new Vector3(2,7,10),trim); FacilityArt.ObservatoryCore(room);
            Floor(new Vector3(8,2.4f,26),new Vector2(6,6));
            var horizon=Relay("HORIZON",new Vector3(-8,0,28)); horizon.clue="LOW WEST PATH / HIGH EAST SHIELD";
            horizon.optics=new[]{Optical(a,.7f,false,"WEST LOW"),Optical(b,3.1f,true,"EAST SHIELD")}; relays.Add(horizon);
            var zenith=Relay("ZENITH",new Vector3(8,2.4f,26)); zenith.prerequisite=horizon; zenith.clue="UPPER ACCESS / HIGH WEST SHIELD";
            zenith.optics=new[]{Optical(a,3.1f,true,"WEST SHIELD"),Optical(b,3.1f,false,"EAST HIGH")}; relays.Add(zenith);
            var nadir=Relay("NADIR",new Vector3(0,0,35)); nadir.prerequisite=zenith; nadir.clue="REOPEN BOTH LOW RETURN PATHS";
            nadir.optics=new[]{Optical(a,.7f,false,"WEST LOW"),Optical(b,.7f,false,"EAST LOW")}; relays.Add(nadir);
            for(int i=0;i<4;i++) Ring(new Vector3(0,7+i*1.6f,20),11-i*2,i%2==0?amber:metal,.12f);
            Sphere("Suspended sun",new Vector3(0,9.3f,20),new Vector3(2,2,2),amber); Light(new Vector3(0,8,20),new Color(1,.51f,.23f),4,24,true);
            for(int i=0;i<12;i++) { if(i==3) continue; float angle=i*Mathf.PI*2/12; ImportedArt.Place(room,"Quaternius/Column_Round",new Vector3(Mathf.Cos(angle)*14,0,20+Mathf.Sin(angle)*14),new Vector3(.7f,6,.7f)); }
            Sign("HORIZON → ZENITH → NADIR\nAMBER: UNRESOLVED / GREEN: READY\nTHE CENTRAL HOUSING SEPARATES YOUR SIGHTLINES",new Vector3(0,3,9),.067f);
        }
        void Gallery()
        {
            Deck(0,36); RoomArchitecture.GalleryThreshold(room,width,11,-8,4,5); Partition(23,8,4,5);
            var a=Entity(new Vector3(-8,0,11),3.9f,.8f,new[]{2,3},"Door A"); var b=Entity(new Vector3(8,0,23),3.9f,.8f,new[]{2,3},"Door B");
            var first=Relay("PASSAGE A",new Vector3(-4.5f,0,6)); first.clue="EXPOSE THE FORWARD SIGNAL"; first.optics=new[]{Optical(a,1.5f,false,"PASSAGE SIGNAL",true)};
            var second=Relay("PASSAGE B",new Vector3(11.5f,0,21)); second.clue="KEEP THE RETURN SIGNAL IN VIEW"; second.optics=new[]{Optical(b,1.5f,false,"RETURN SIGNAL",true)};
            second.prerequisite=first; relays.Add(first); relays.Add(second);
            for(int i=0;i<6;i++)
            {
                B("Exhibit plinth",new Vector3(i%2==0?-13:13,.55f,4+i*5),new Vector3(1.7f,1.1f,1.7f),wall);
                // Familiar imported furniture occupies these deliberate display plinths.
            }
            Sign("AN ABSENCE CAN BE HELD",new Vector3(0,3.2f,7),.11f);
        }
        void Engine()
        {
            Deck(0,8); Deck(16,24); Deck(32,40); Water(20,27);
            var a=Entity(new Vector3(-11,0,12),4,8,new[]{2,0,4},"Feed A"); var b=Entity(new Vector3(11,0,12),4,8,new[]{3,0,1,2},"Feed B");
            var c=Entity(new Vector3(-11,0,28),4,8,new[]{3,0,4,2},"Return C"); var d=Entity(new Vector3(11,0,28),4,8,new[]{3,0,1},"Return D");
            Floor(new Vector3(11,2.4f,35),new Vector2(8,6));
            var first=Relay("FEED",new Vector3(-14,0,20)); first.clue="CROSS WEST / SHIELD EAST FEED"; first.optics=new[]{Optical(b,3.1f,true,"FEED SHIELD")};
            var second=Relay("RETURN",new Vector3(12,2.4f,35)); second.clue="UPPER RETURN / SHIELD WEST / CLEAR EAST HIGH";
            second.optics=new[]{Optical(c,3.1f,true,"RETURN SHIELD"),Optical(d,3.1f,false,"EAST HIGH")}; second.prerequisite=first; relays.Add(first); relays.Add(second);
            var core=B("Core housing collision",new Vector3(0,4,20),new Vector3(4,8,5),trim); core.GetComponent<Renderer>().enabled=false;
            foreach(int s in new[]{-1,1})
            {
                Pipe(new Vector3(s*2.2f,7,3),new Vector3(s*2.2f,7,38),.25f,metal);
                for(int j=0;j<5;j++) B("Engine filament",new Vector3(s*2.08f,1+j*1.3f,20),new Vector3(.09f,.35f,5.2f),teal);
            }
            Ring(new Vector3(0,9,20),10,teal,.18f); Light(new Vector3(0,7,20),new Color(.3f,.85f,.7f),3,25,true);
            Sign("FEED → RETURN → RELEASE\nCONNECTED CIRCUITS SURVIVE RELEASE",new Vector3(0,3.1f,5.5f),.09f);
        }
        void Pickup(Vector3 p,int capacity)
        {
            var relay=Relay(capacity==1?"Continuity instrument":"Second anchor channel",p); relay.upgrade=capacity; relays.Add(relay);
            var core=B("Instrument core",p+Vector3.up*2.1f,Vector3.one*.32f,teal); core.transform.localRotation=Quaternion.Euler(30,0,45); Destroy(core.GetComponent<Collider>());
        }
        FormRequirement Req(UnstableEntity e,Form f)=>new FormRequirement{entity=e,form=f,held=true};
        OpticalCondition Optical(UnstableEntity e,float y,bool blocked,string label,bool through=false)
        {
            Vector3 p=e.transform.localPosition;
            Vector3 offset=through?new Vector3(0,0,1.6f):new Vector3(e.width/2+.7f,0,0);
            Vector3 center=p+new Vector3(through?1.25f:0,y,0);
            foreach(int side in new[]{-1,1})
            {
                var point=center+offset*side;
                var marker=B("Optical emitter / receiver",point,new Vector3(.14f,.14f,.14f),teal); marker.GetComponent<Collider>().enabled=false;
                var stand=B("Instrument support",new Vector3(point.x,p.y+y/2-.15f,point.z),new Vector3(.06f,Mathf.Max(.1f,y-.3f),.06f),metal); stand.GetComponent<Collider>().enabled=false;
            }
            return new OpticalCondition{from=room.TransformPoint(center-offset),to=room.TransformPoint(center+offset),subject=e,interrupted=blocked,clue=label};
        }
        void ServiceGate(Vector3 p,float span,PuzzleRelay source)
        {
            var root=new GameObject("Filter service interlock"); root.transform.SetParent(room,false); root.transform.localPosition=p;
            var gate=root.AddComponent<CircuitDoor>(); gate.circuits=new[]{source};
            var slab=B("Filter shutter",p+Vector3.up*2,new Vector3(span,4,.25f),metal); slab.transform.SetParent(root.transform,true); gate.slab=slab.transform;
            gate.display=Sign("INTAKE RELEASES FILTER ACCESS",p+new Vector3(0,2.8f,-.2f),.05f);
        }
        PuzzleRelay Relay(string name,Vector3 p,params FormRequirement[] requirements)
        {
            var go=new GameObject(name); go.transform.SetParent(room,false); go.transform.localPosition=p;
            var relay=go.AddComponent<PuzzleRelay>(); relay.label=name; relay.requirements=requirements;
            var body=B("Control pedestal",p+Vector3.up*.7f,new Vector3(1.8f,1.4f,.8f),metal); body.transform.SetParent(go.transform,true);
            var screen=B("Control display",p+new Vector3(0,1.65f,.05f),new Vector3(3.3f,1.3f,.15f),trim); screen.transform.SetParent(go.transform,true);
            relay.display=Sign(name.ToUpperInvariant()+"\nF / CONNECT",p+new Vector3(0,1.65f,-.05f),.052f,false); relay.display.transform.SetParent(go.transform,true);
            var lamp=B("Circuit lamp",p+Vector3.up*2.4f,new Vector3(.32f,.14f,.16f),amber); lamp.transform.SetParent(go.transform,true); relay.lamp=lamp.GetComponent<Renderer>(); return relay;
        }
        void Door(float z,PuzzleRelay[] circuits)
        {
            var go=new GameObject("Circuit interlock"); go.transform.SetParent(room,false); go.transform.localPosition=new Vector3(0,0,z);
            var door=go.AddComponent<CircuitDoor>(); door.circuits=circuits;
            var slab=B("Interlock shutter",new Vector3(0,2.3f,z),new Vector3(5.8f,4.6f,.35f),metal); slab.transform.SetParent(go.transform,true); door.slab=slab.transform;
            for(int i=0;i<6;i++) { var rib=B("Shutter rib",new Vector3(0,.5f+i*.7f,z-.21f),new Vector3(5.6f,.045f,.08f),trim); rib.transform.SetParent(slab.transform,true); }
            door.display=Sign("CIRCUITS",new Vector3(0,3.6f,z-.4f),.09f);
        }
        UnstableEntity Entity(Vector3 p,float w,float l,int[] choices,string label)
        {
            var go=new GameObject(label); go.transform.SetParent(room,false); go.transform.localPosition=p;
            var obs=go.AddComponent<ObservationTarget>(); obs.size=new Vector3(w+.2f,5.8f,l+.2f); obs.center=new Vector3(0,1.4f,0);
            var e=go.AddComponent<UnstableEntity>(); e.allowForgotten=wing>=5; e.width=w; e.length=l; e.label=label; e.states=new RealityState[choices.Length]; for(int i=0;i<choices.Length;i++) e.states[i]=realityStates[choices[i]];
            foreach(int s in new[]{-1,1}) B("Reserve edge",p+new Vector3(s*(w/2+.12f),.025f,0),new Vector3(.05f,.025f,l),marking);
            return e; // Interface requirements keep logical names; scenery no longer needs floating object labels.
        }
        void Partition(float z,float opening,float gap,float h)
        {
            float left=opening-gap/2,right=opening+gap/2;
            var leftWall=B("Sightline wall collision",new Vector3((-width/2+left)/2,h/2,z),new Vector3(left+width/2,h,.6f),wall); leftWall.GetComponent<Renderer>().enabled=false;
            FacilityArt.Wall(room,leftWall.transform.localPosition,new Vector3(left+width/2,h,.6f),wing);
            var rightWall=B("Sightline wall collision",new Vector3((right+width/2)/2,h/2,z),new Vector3(width/2-right,h,.6f),wall); rightWall.GetComponent<Renderer>().enabled=false;
            FacilityArt.Wall(room,rightWall.transform.localPosition,new Vector3(width/2-right,h,.6f),wing);
            B("Opening lintel",new Vector3(opening,h-.25f,z),new Vector3(gap,.5f,.6f),wood);
        }
        void Deck(float a,float b)=>Floor(new Vector3(0,0,(a+b)/2),new Vector2(width,b-a));
        void Floor(Vector3 p,Vector2 size)
        {
            int firstChild=room.childCount;
            var f=B("Stable landing",p-Vector3.up*.3f,new Vector3(size.x,.6f,size.y),floor); f.AddComponent<SurfaceTag>().kind=wing==0||wing==1||wing==6?SurfaceKind.Carpet:SurfaceKind.Stone;
            f.GetComponent<Renderer>().enabled=wing>=2 && wing!=6;
            if(wing>=2 && wing!=6) FacilityArt.LandingFrame(room,p,size);
            int nx=Mathf.CeilToInt(size.x/4),nz=Mathf.CeilToInt(size.y/4);
            for(int x=0;x<nx;x++) for(int z=0;z<nz;z++)
            {
                var tile=B("Surface tile",p+new Vector3(-size.x/2+(x+.5f)*size.x/nx,-.06f,-size.y/2+(z+.5f)*size.y/nz),new Vector3(size.x/nx,.12f,size.y/nz),floor);
                tile.GetComponent<Collider>().enabled=false;
                if(wing>=2 && wing!=6) { tile.GetComponent<Renderer>().enabled=false; FacilityArt.DeckTile(room,p+new Vector3(-size.x/2+(x+.5f)*size.x/nx,0,-size.y/2+(z+.5f)*size.y/nz),new Vector2(size.x/nx,size.y/nz),wing); }
            }
            foreach(int s in new[]{-1,1})
            {
                var edge=B("Landing edge",p+new Vector3(0,.015f,s*size.y/2),new Vector3(size.x,.018f,.045f),marking); edge.GetComponent<Collider>().enabled=false;
            }
            int lastChild=room.childCount;
            var zone=new GameObject("Floor finish / authored landing").transform; zone.SetParent(room,false);
            var children=new List<Transform>(); for(int i=firstChild;i<lastChild;i++) children.Add(room.GetChild(i));
            foreach(var child in children) child.SetParent(zone,true);
            zone.gameObject.AddComponent<SurfaceRevisionZone>().Prepare();
        }
        void StableStairs(Vector3 p,float w,float l,float rise,bool descending)
        {
            ImportedArt.Place(room,"Quaternius/Platform_Stairs_4Wide",p,new Vector3(w,rise,l),descending?180:0);
            var ramp=new GameObject("Stable stair collision"); ramp.transform.SetParent(room,false); ramp.transform.localPosition=p; StairCollision.Build(ramp,w,l,rise,descending);
        }
        void Water(float z,float extent)
        {
            var surface=B("Still reservoir",new Vector3(0,-2.6f,z),new Vector3(width-.5f,.08f,extent),glass); Destroy(surface.GetComponent<Collider>());
        }
        void Shelf(Vector3 p,int seed)=>FacilityArt.Shelf(room,p,seed);
        void Bench(Vector3 p)=>FacilityArt.Bench(room,p);
        void Workbench(Vector3 p)=>FacilityArt.Workbench(room,p);
        void Plant(Vector3 p,float scale)=>FacilityArt.Plant(room,p,scale);
        void Tree(Vector3 p)
        {
            Cylinder("Atrium planter",p+Vector3.up*.3f,new Vector3(4,.3f,4),trim);
            Plant(p+new Vector3(-.9f,.6f,0),2.1f);
            Plant(p+new Vector3(.8f,.6f,.8f),1.7f);
            Plant(p+new Vector3(.5f,.6f,-.8f),1.35f);
        }

        void Corridor(Vector3 a,Vector3 b)
        {
            var old=room; room=new GameObject("Turning service gallery").transform; room.SetParent(transform); room.position=a; room.rotation=Quaternion.LookRotation(b-a); float distance=Vector3.Distance(a,b);
            var deck=B("Grated service floor",new Vector3(0,-.25f,distance/2),new Vector3(6,.5f,distance+6),metal); deck.AddComponent<SurfaceTag>().kind=SurfaceKind.Metal;
            foreach(int s in new[]{-1,1})
            {
                if(distance>6) { B("Guard rail",new Vector3(s*2.8f,1.1f,distance/2),new Vector3(.07f,.1f,distance-6),metal); B("Gallery wall",new Vector3(s*3.05f,2.5f,distance/2),new Vector3(.2f,5,distance-6),trim); }
                for(float z=3;z<distance-2;z+=3) B("Rail post",new Vector3(s*2.8f,.55f,z),new Vector3(.08f,1.1f,.08f),metal);
            }
            B("Service ceiling",new Vector3(0,5.1f,distance/2),new Vector3(6,.2f,distance+6),trim);
            for(float z=3;z<distance;z+=6) { B("Service tube",new Vector3(0,4.8f,z),new Vector3(2,.08f,.1f),amber); Light(new Vector3(0,3.8f,z),new Color(1,.62f,.31f),1.2f,9,false); }
            foreach(var r in room.GetComponentsInChildren<Renderer>()) if(r.name=="Guard rail"||r.name=="Rail post") r.enabled=false;
            foreach(int side in new[]{-1,1}) for(float z=3;z<distance-3;z+=3)
                ImportedArt.Place(room,"Quaternius/Prop_Rail_4",new Vector3(side*2.8f,0,z+Mathf.Min(3,distance-3-z)/2),new Vector3(.07f,1.15f,Mathf.Min(3,distance-3-z)));
            room=old;
        }
        void Light(Vector3 p,Color color,float intensity,float range,bool shadows)
        {
            var go=new GameObject("Architectural light"); go.transform.SetParent(room,false); go.transform.localPosition=p;
            var l=go.AddComponent<Light>(); l.color=color; l.intensity=intensity; l.range=range; l.type=LightType.Point; l.shadows=shadows?LightShadows.Soft:LightShadows.None; l.shadowStrength=.65f; l.shadowBias=.03f;
        }
        void Ring(Vector3 p,float radius,Material material,float thickness)
        {
            var go=new GameObject("Suspended ring"); go.transform.SetParent(room,false); go.transform.localPosition=p;
            var line=go.AddComponent<LineRenderer>(); line.useWorldSpace=false; line.loop=true; line.positionCount=80; line.startWidth=line.endWidth=thickness; line.sharedMaterial=material;
            for(int i=0;i<80;i++) { float a=i*Mathf.PI*2/80; line.SetPosition(i,new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius)); }
        }
        void Pipe(Vector3 a,Vector3 b,float radius,Material material)
        {
            var go=Cylinder("Pipe / branch",(a+b)/2,new Vector3(radius,Vector3.Distance(a,b)/2,radius),material); go.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);
        }
        GameObject Cylinder(string name,Vector3 p,Vector3 size,Material m)=>Primitive(PrimitiveType.Cylinder,name,p,size,m);
        GameObject Sphere(string name,Vector3 p,Vector3 size,Material m)=>Primitive(PrimitiveType.Sphere,name,p,size,m);
        GameObject Primitive(PrimitiveType type,string name,Vector3 p,Vector3 size,Material m)
        {
            var go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(room,false); go.transform.localPosition=p; go.transform.localScale=size; go.GetComponent<Renderer>().sharedMaterial=m; return go;
        }
        GameObject B(string name,Vector3 p,Vector3 size,Material m)=>Primitive(PrimitiveType.Cube,name,p,size,m);
        GameObject Trigger(string name,Vector3 p,Vector3 size)
        {
            var go=new GameObject(name); go.transform.SetParent(room,false); go.transform.localPosition=p; var c=go.AddComponent<BoxCollider>(); c.size=size; c.isTrigger=true; return go;
        }
        TextMesh Sign(string text,Vector3 p,float size,bool panel=true)
        {
            var go=new GameObject("Wayfinding / "+text.Replace('\n',' ')); go.transform.SetParent(room,false); go.transform.localPosition=p;
            var mesh=go.AddComponent<TextMesh>(); mesh.text=text; mesh.fontSize=64; mesh.characterSize=size*.28f; mesh.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            mesh.anchor=TextAnchor.MiddleCenter; mesh.alignment=TextAlignment.Center; mesh.color=new Color(.81f,.9f,.85f); go.AddComponent<WorldSign>();
            if(panel)
            {
                string[] lines=text.Split('\n'); int longest=0; foreach(string line in lines) longest=Mathf.Max(longest,line.Length);
                var board=B("Sign backing",p+Vector3.forward*.04f,new Vector3(Mathf.Max(.6f,longest*size*1.15f),Mathf.Max(.35f,lines.Length*size*2.5f),.05f),trim); Destroy(board.GetComponent<Collider>());
            }
            return mesh;
        }
        public static Material Material(Color color,bool glow=false)
        {
            var m=new Material(Resources.Load<Material>("WorldSurface")); m.shader=Resources.Load<Shader>("ArchitecturalSurface"); m.color=color; m.SetFloat("_Glossiness",.24f); SurfaceLibrary.Apply(m,0); if(glow) m.SetColor("_EmissionColor",color*1.15f); return m;
        }
        public static GameObject Box(string name,Vector3 p,Vector3 scale,Material material,Transform parent)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube); go.name=name; go.transform.SetParent(parent); go.transform.position=p; go.transform.localScale=scale; go.GetComponent<Renderer>().sharedMaterial=material; return go;
        }
    }
}

