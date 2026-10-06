using UnityEngine;
namespace Unobserved
{
    // Prepared visual children only: no Update, puzzle interface or changing collision.
    [RequireComponent(typeof(ObservationTarget))]
    public sealed class DecorativeRevision:MonoBehaviour
    {
        public GameObject[] versions;
        public int Current {get;private set;}
        public int Changes {get;private set;}
        StateDeck deck;
        ObservationTarget observation;
        public void Initialize(string[] keys,int seed,bool composition=false)
        {
            deck=new StateDeck(seed); observation=GetComponent<ObservationTarget>();
            observation.center=new Vector3(0,1.1f,0); observation.size=new Vector3(1.6f,2.4f,1.6f);
            if(composition) observation.size=new Vector3(2.8f,2.4f,2);
            versions=new GameObject[keys.Length];
            for(int i=0;i<keys.Length;i++)
            {
                // Preserve model proportions, constrained to a shared decorative envelope.
                if(composition)
                {
                    versions[i]=new GameObject(keys[i]+" arrangement"); versions[i].transform.SetParent(transform,false);
                    var main=ImportedArt.PlaceFitted(versions[i].transform,keys[i],new Vector3(1.1f,2.1f,1.3f)); main.transform.localPosition=Vector3.left*.65f;
                    string accessory=i%2==0?"KenneyFurniture/tableCoffee":"KenneyFactory/box-large";
                    var small=ImportedArt.PlaceFitted(versions[i].transform,accessory,new Vector3(.85f,.85f,.8f)); small.transform.localPosition=new Vector3(.7f,0,.25f);
                }
                else versions[i]=ImportedArt.PlaceFitted(transform,keys[i],new Vector3(1.35f,2.1f,1.35f));
                versions[i].SetActive(i==0);
            }
            observation.FullyUnobserved+=Change;
        }
        void Change()
        {
            if(!GameFlow.Playing) return;
            var safety=observation.WorldBounds; safety.Expand(1.2f);
            if(safety.Contains(GameFlow.Instance.Player.transform.position+Vector3.up)) return;
            versions[Current].SetActive(false); Current=deck.Next(versions.Length,Current);
            versions[Current].SetActive(true); Changes++;
            if(Changes%2==0) Soundscape.Instance.Decor(transform.position,versions[Current].name);
        }
    }
    public static class UnstableDecor
    {
        const string F="KenneyFurniture/",K="KenneyFactory/",Q="Quaternius/";
        static readonly string[][] pools={
            new[]{F+"loungeChair",F+"pottedPlant",F+"bookcaseClosedDoors",F+"tableCoffee"},
            new[]{F+"bookcaseOpen",F+"cardboardBoxClosed",F+"chairDesk",F+"pottedPlant"},
            new[]{F+"pottedPlant",F+"loungeChair",F+"tableCoffee"},
            new[]{K+"machine-window",F+"kitchenCabinetDrawer",K+"box-large",K+"piston-round"},
            new[]{K+"pipe-large-valve",K+"box-large",K+"hopper-high-round"},
            new[]{Q+"Prop_Computer",F+"chairDesk",Q+"Prop_AccessPoint",F+"bookcaseClosed"},
            new[]{F+"loungeChair",F+"pottedPlant",Q+"Column_Round",F+"bookcaseClosedDoors"},
            new[]{K+"machine-connection-pipe",K+"piston-round",Q+"Column_Pipes"}
        };
        public static void Dress(Transform room,int wing)
        {
            Vector3[][] positions={
                new[]{new Vector3(-10,0,15),new Vector3(10,0,22),new Vector3(-10,0,28)},
                new[]{new Vector3(-14,0,15),new Vector3(14,0,25),new Vector3(-13,0,34)},
                new[]{new Vector3(-18,0,40),new Vector3(18,0,5),new Vector3(-18,0,8)},
                new[]{new Vector3(-11,0,9),new Vector3(11,0,29),new Vector3(-11,0,34)},
                new[]{new Vector3(14,0,39),new Vector3(-15,0,4),new Vector3(15,0,4)},
                new[]{new Vector3(-14,0,4),new Vector3(14,0,34),new Vector3(-14,0,35)},
                new[]{new Vector3(-13,0,32),new Vector3(13,0,14),new Vector3(-6,0,32)},
                new[]{new Vector3(-16,0,35),new Vector3(16,0,4),new Vector3(-16,0,4)}
            };
            int slot=0;
            foreach(var p in positions[wing])
            {
                var go=new GameObject("Unstable decor / "+wing+" / "+slot); go.transform.SetParent(room,false); go.transform.localPosition=p;
                go.AddComponent<DecorativeRevision>().Initialize(pools[wing],wing*137+slot*29,slot==2); slot++;
            }
        }
    }
}
