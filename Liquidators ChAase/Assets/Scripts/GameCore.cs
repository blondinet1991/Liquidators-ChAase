using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PortalPrefab;
    public GameObject CactiPrefab;

    public List<GameObject> PlatformsList;


    public GameObject PlatformsContainer;

    public int MaximumCactiPerPlatform = 2;
    public int lessChanceForNextCatcti = 50;

    public int BaseMaximumChance = 100;
    private List<int> chanceForEachCacti;
    public float score;

    private void Awake() {
        PlatformsList = new List<GameObject>();
        //CactiList = new List<GameObject>();
        chanceForEachCacti = new List<int>();


        RefreshPlatformsList();
        CalcCactiAppearanceChance();
    }

    void Start()
    {
        summonRandomTrap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenPortalAt(Transform Position) {

    }


    public void summonRandomTrap() {
        int Roll = 0;
        foreach (var platform in PlatformsList)
        {
           for (int i = 0; i < MaximumCactiPerPlatform; i++) {
                Roll = Random.Range(0, BaseMaximumChance);
                if (Roll <= chanceForEachCacti[i]) {
                    platform.GetComponent<Platforms>().SummonOnTop(CactiPrefab);
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

        for (int i = 0; i < PlatformsContainer.transform.childCount; i++)
        {
            PlatformsList.Add(PlatformsContainer.transform.GetChild(i).gameObject);
        }
    }
}
