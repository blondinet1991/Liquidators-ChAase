using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;


using UnityEngine;

public class GameCore : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PortalPrefab;
    public GameObject CactiPrefab;

    public Transform Player;

    public string WalletAdd="";

    public string gotchiTID="19961";

    public List<string> gotchiIDS;

    List<Transform> PowerPositions;
    public Transform PowerPositionsHolder;

    public Transform PlayerStartPos;


    public Dictionary<string, string> Svgs;
    public Dictionary<string, string> Traits;


    public SpriteRenderer playerSprite;

    public List<GameObject> PlatformsList;
    public List<Transform> PlatformsTransformList;


    public GameObject PlatformsContainer;

    public int MaximumCactiPerPlatform = 2;
    public int lessChanceForNextCatcti = 50;

    public float summonPortalTime=30f;
    public float summonPortalCounter;

    public float summonRandomBuffTime=20f;
    public float summoneRandomBuffCounter;

    public int BaseMaximumChance = 100;
    private List<int> chanceForEachCacti;
    public float score;
    public float passedTime;

    private float ingameTime;

    public float countDown=4f;

    private bool started = false;

    public Text ScoreText;
    public Text EnergyText;
    public Text TimeText;


    public int currentEnergy = 5;
    public float energyRegenBaseTime=10f;
    public float energyRegenCounter;
    public float energyRegenModifer=1;


    public bool consumeEnergy()
    {
        if (currentEnergy >0) {
            currentEnergy--;
            EnergyText.text = currentEnergy.ToString();
            return true;
        }else return false;

    }

    private void Awake() {
        Svgs = new Dictionary<string, string>();
        Traits = new Dictionary<string, string>();
        PlatformsList = new List<GameObject>();
        PlatformsTransformList = new List<Transform>();
        //CactiList = new List<GameObject>();
        chanceForEachCacti = new List<int>();

        

        RefreshPlatformsList();
        CalcCactiAppearanceChance();
    }

void DisplayTime(float timeToDisplay)
{
  float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
  float seconds = Mathf.FloorToInt(timeToDisplay % 60);

  TimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
}
    void Start()
    {
        Player.transform.position = PlayerStartPos.position;
        Player.transform.GetComponent<Player>().gameCore = this;
        passedTime = 0f;
        ingameTime = 0f;
        started = false;

        summonRandomTrap();
        StartCoroutine(GetTokenIDS(WalletAdd));

        // foreach (var gotchiID in gotchiIDS)
        // {
        //   StartCoroutine(downloadSVG(gotchiID));  
        //   StartCoroutine(GetTraitsByTID(gotchiID)); 
        // }

        //setSvg();
    }


    public void finish(){

    }

    public void loadGotchiByID(int ID){
        string tokenID = gotchiIDS[ID];
        LoadSVGFromString(Svgs[tokenID]);
    }

    public void loadGotchiByTokenID(string TID){
        LoadSVGFromString(Svgs[TID]);
    }

    public void summonRandomTrap() {
        int Roll = 0;
        foreach (var platform in PlatformsList)
        {
           for (int i = 0; i < MaximumCactiPerPlatform; i++) {
                Roll = UnityEngine.Random.Range(0, BaseMaximumChance);
                if (Roll <= chanceForEachCacti[i]) {
                    GameObject tmpCacti = platform.GetComponent<Platforms>().SummonOnTop(CactiPrefab);
                    if (tmpCacti != null)
                        tmpCacti.GetComponent<Cacti>().gameCore = this;
                }
           } 
        }
    }


    public void summonRandomCoin() {
        summoneRandomBuffCounter = 0f;
        int Roll = 0;
        // foreach (var platform in PlatformsList)
        // {
        //    for (int i = 0; i < MaximumCactiPerPlatform; i++) {
        //         Roll = UnityEngine.Random.Range(0, BaseMaximumChance);
        //         if (Roll <= chanceForEachCacti[i]) {
        //             GameObject tmpCacti = platform.GetComponent<Platforms>().SummonOnTop(CactiPrefab);
        //             if (tmpCacti != null)
        //                 tmpCacti.GetComponent<Cacti>().gameCore = this;
        //         }
        //    } 
        // }
    }

    private void Update() {
        if (started) {
            summonPortalCounter += Time.deltaTime;

            ingameTime += Time.deltaTime;
            DisplayTime(ingameTime);

            if (currentEnergy < 5) {
                energyRegenCounter += Time.deltaTime*energyRegenModifer;
                if (energyRegenCounter >= energyRegenBaseTime) {
                    currentEnergy++;
                    EnergyText.text = currentEnergy.ToString();
                }
            } else {
                energyRegenCounter = 0;
            }

            if (summonPortalCounter >= summonPortalTime)
                summonRandomLikidator();
                
            summoneRandomBuffCounter += Time.deltaTime;

            if (summoneRandomBuffCounter >= summonRandomBuffTime)
                summonRandomCoin();
        }
        

        passedTime += Time.deltaTime;

        if (passedTime >= countDown && !started) {
            started = true;
        }
    }
    public void summonRandomLikidator() {
        summonPortalCounter = 0f;
        int Roll = 0;
        foreach (var platform in PlatformsList)
        {
           for (int i = 0; i < MaximumCactiPerPlatform; i++) {
                Roll = UnityEngine.Random.Range(0, BaseMaximumChance);
                if (Roll <= chanceForEachCacti[i]) {
                    GameObject tmpPortal = platform.GetComponent<Platforms>().SummonOnTop(PortalPrefab);
                    if (tmpPortal != null)
                        tmpPortal.GetComponent<Portals>().gameCore = this;
                    return;
                }
           } 
        }
    }

    public void CalcCactiAppearanceChance(){
        chanceForEachCacti.Clear();
        int Chance = BaseMaximumChance;
            for (int i = 0; i < MaximumCactiPerPlatform; i++)
            {
                Chance = Mathf.RoundToInt(Chance *(lessChanceForNextCatcti/100.0f));
                chanceForEachCacti.Add(Chance);
            }
        
    }

    void RefreshPlatformsList() {
        PlatformsList.Clear();
        PlatformsTransformList.Clear();

        for (int i = 0; i < PlatformsContainer.transform.childCount; i++)
        {
            PlatformsList.Add(PlatformsContainer.transform.GetChild(i).gameObject);
            PlatformsTransformList.Add(PlatformsContainer.transform.GetChild(i));
        }
    }

    private IEnumerator downloadSVG(string TokenID)
    {
        var url = "https://eyzbivxgm7j8.bigmoralis.com:2053/server/functions/getAavegotchiSVG?_ApplicationId=a4AT7fFVOlC09aOqPxyniupsWUPqC6tBTu5WXC7u&tokenid="+TokenID;
        SVGImage svgimg;
        UnityWebRequest www = UnityWebRequest.Get(url);
 
        yield return www.SendWebRequest();

        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log("Error while Receiving: " + www.error);
        }
        else
        {
            //Convert byte[] data of svg into string
            string bitString = parse_json(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));
            Svgs.Add(TokenID, bitString);
        } 
    }


    private IEnumerator GetTokenIDS(string WalletAddress)
    {
        var url = "https://eyzbivxgm7j8.bigmoralis.com:2053/server/functions/getAavegotchiTokenIDS?_ApplicationId=a4AT7fFVOlC09aOqPxyniupsWUPqC6tBTu5WXC7u&address=" + WalletAddress;

        UnityWebRequest www = UnityWebRequest.Get(url);
 
        yield return www.SendWebRequest();

        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log("Error while Receiving: " + www.error);
        }
        else
        {
            //Convert byte[] data of svg into string
           string bitString = parse_json(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data).Replace("\",\"","|").Replace("[","").Replace("]",""));
           gotchiIDS.AddRange(bitString.Split('|'));
        } 
    }

    private IEnumerator GetTraitsByTID(string TokenID)
    {
        var url = "https://eyzbivxgm7j8.bigmoralis.com:2053/server/functions/getAavegotchiTraits?_ApplicationId=a4AT7fFVOlC09aOqPxyniupsWUPqC6tBTu5WXC7u&tokenid=" + TokenID;

        UnityWebRequest www = UnityWebRequest.Get(url);
 
        yield return www.SendWebRequest();

        if (www.isHttpError || www.isNetworkError)
        {
            Debug.Log("Error while Receiving: " + www.error);
        }
        else
        {
            //Convert byte[] data of svg into string
           string bitString = parse_json(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data).Replace("\",\"","|").Replace("[","").Replace("]",""));
           Traits.Add(TokenID, bitString);
        } 
    }
    private void LoadSVGFromString(string DataString)
    {
       SVGImage svgimg;
       var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 100.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };
 
            // Dynamically import the SVG data, and tessellate the resulting vector scene.
            var sceneInfo = SVGParser.ImportSVG(new StringReader(DataString));
            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);
 
            // Build a sprite with the tessellated geometry
            Sprite sprite = VectorUtils.BuildSprite(geoms, 100f, VectorUtils.Alignment.Center, Vector2.zero, 64, true);
            playerSprite.sprite = sprite;
    } 


       public string parse_json(string raw_message){
        string result="";
        object items = null;
        try {
            items = Parser.JsonToDict(raw_message);
        } catch (Exception e) {
             Debug.Log("Parsing Failed at object num:  " +  Parser.c + " | eleName: " + Parser.lastEleName);
             Debug.Log("Sequence: " + Parser.eleSequence);
             Debug.Log(e.ToString());
        }

        if (Parser.isDict(items)) {
            var dict = Parser.toDict(items);
            if (dict.ContainsKey("result")) {
                result = dict["result"].ToString();
            }
        }

        return result;
    }
}

