using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base script for all plants
public class PlantBase : MonoBehaviour {

    //Name of plant
    string NAME = "Base";

    //Quality value
    [Range(0f,3f)]
    float QUALITY = 0.0f;

    //Next stage prefab
    public GameObject NEXT_STAGE;

    //Produce gained upon harvest
    public GameObject PRODUCE;

    //Dead plant prefab
    public GameObject DEAD_STAGE;

    //Time to next stage
    public float TIME_TO_NEXT;

    //Day counter
    float TIME_PLANTED;

    //Number of produce gained
    public int PRODUCE_NUMBER = 0;

    //Day's time decrement (growth for today)
    [HideInInspector]
    public float GROWTH_TODAY = 1f;

    //Bool for whether or not plant dies on harvest
    public bool DEATH_ON_HARVEST = true;

    //Bool for whether or not the plant has produce at this stage
    public bool BIRTHS_PRODUCE = true;
    public int MIN_PRODUCE, MAX_PRODUCE;

    //Bool for whether or not the plant has to inherit produce positions
    public bool INHERIT_PRODUCE = false;

    [SerializeField]
    bool isDead = false;

    //Days since watered
    int DAYS_SINCE_WATER = 0;

    //How long can plant go without water
    public int DAYS_TO_DIE;

    //Rotation offset of plant
    Vector3 rotOffset;

    //Position offset of plant
    Vector3 posOffset;

    //Randomizer factor for offsets. Height will randomize crop heights on init, width randomizes X and Z for bigger circumferences.
    public float heightFactor = 0.5f;
    public float widthFactor = 0.5f;

    //Convert quality to a text
    public string GetQualityString()
    {
        if(QUALITY <= 1)
        {
            return "Simple";
        }
        else if(QUALITY <= 2)
        {
            return "Super";
        }
        else
        {
            return "Spectacular";
        }
    }

    //The transform with spawn points
    public Transform SpawnPoints;

    //When getting planted, init script
    public void Init(GameObject PREV)
    {
        //Set timer
        TIME_PLANTED = TIME_TO_NEXT;

        //Use randomizer on models
        float randomY = 1 + Random.Range(-heightFactor, heightFactor);
        float randomX = 1 + Random.Range(-widthFactor, widthFactor);

        //Apply to local scale
        transform.localScale = new Vector3(randomX, randomY, randomX);
       

        //Spawn produce
        if (BIRTHS_PRODUCE)
        {
            //Generate number of produce
            PRODUCE_NUMBER = Random.Range(MIN_PRODUCE, MAX_PRODUCE);

            //Spawn produce in children
            BirthProduce();
        }

        if (INHERIT_PRODUCE)
        {
            //Write function to map produce to previous stage.
        }
       
    }

    //When getting planted init script BUT without a previous.
    public void Init()
    {
        //Set timer
        TIME_PLANTED = TIME_TO_NEXT;

        //Use randomizer on models
        float randomY = 1 + Random.Range(-heightFactor, heightFactor);
        float randomX = 1 + Random.Range(-widthFactor, widthFactor);

        //Apply to local scale
        transform.localScale = new Vector3(randomX, randomY, randomX);

        //Spawn produce
        if (BIRTHS_PRODUCE)
        {
            //Generate number of produce
            PRODUCE_NUMBER = Random.Range(MIN_PRODUCE, MAX_PRODUCE);

            //Spawn produce
            BirthProduce();
        }

        if (INHERIT_PRODUCE)
        {
            //Write function to map produce to previous stage.
        }
    }

    //Spawns produce on plant in given spawn points
    public void BirthProduce()
    {
        //Debug.Log("Birth");
        int counter = PRODUCE_NUMBER;
        List<GameObject> SpawnList = new List<GameObject>();

        //Retrieve all spawn points.
        for(int i = 0; i < SpawnPoints.childCount; i++)
        {
            SpawnList.Add(SpawnPoints.GetChild(i).gameObject);
        }

        //Extract from the list at random.
        List<GameObject> ChosenList = new List<GameObject> ();
        for(int i = 0; i < PRODUCE_NUMBER; i++)
        {
            //Never exceeds index because it goes up to count. Max exclusive.
            int index = Random.Range(0, SpawnList.Count);
            ChosenList.Add(SpawnList[index]);
            SpawnList.Remove(SpawnList[index]);
        }

        //SPawn at the chosen points
        for(int i = 0; i < ChosenList.Count; i++)
        {
            //Debug.Log("Spawn!");
            GameObject obj = Instantiate(PRODUCE, ChosenList[i].transform.position, ChosenList[i].transform.rotation, ChosenList[i].transform);
        }

    }

    //Harvest from plant. Returns quality of plant
    public void Harvest(GameObject produce)
    {
        //Assign quality to produce
        //asdaasdasdlhsajkfhsalkjdfhkjasdhflkjashdfjklhsakdjlf
        Debug.Log("Quality not being assigned to produce. Not written yet.");

        //If plant dies on harvest, transition to next stage.
        if (DEATH_ON_HARVEST && PRODUCE_NUMBER == 0)
        {
            //Set to dead
            isDead = true;
        }
    }

    //End of day function. Tallies growth and resets for a new day.
    public virtual void DayEnd()
    {
        //If we're dead, be dead.
        if (isDead)
        {
            BeDead();
        }
        else
        {
            //Decrements the day counter by the growth for today
            TIME_PLANTED -= CalculateGrowth();
            if (TIME_PLANTED <= 0)
            {
                //Move onto next stage since we've reached the end for this stage
                NextStage();
            }
            else if(DAYS_SINCE_WATER >= DAYS_TO_DIE)
            {
                BeDead();
            }

            GROWTH_TODAY = 1f;
        }     

    }

    //Growth function. Takes into account water and fertilized.
    public virtual float CalculateGrowth()
    {
        //Retrieve the value of growth today
        float totalGrowth = GROWTH_TODAY;

        //Apply any factors
        if (transform.parent.GetComponent<FarmBlockInfo>().WATERED)
        {
            totalGrowth *= 1;
        }
        else
        {
            DAYS_SINCE_WATER += 1;
            totalGrowth *= 0;
        }

        return totalGrowth;


    }
	
    //Go next function.
    public virtual void NextStage()
    {
        GameObject next;

        //If no next stage, go straight to death.
        if (NEXT_STAGE == null)
        {
            next = Instantiate(DEAD_STAGE, transform.position, transform.rotation, transform.parent);
        }
        else
        {
            //Spawn replacement plant.
            next = Instantiate(NEXT_STAGE, transform.position, transform.rotation, transform.parent);
        }

        //Transfer relevant details to new plant.
        next.GetComponent<PlantBase>().QUALITY = QUALITY;
        next.GetComponent<PlantBase>().PRODUCE_NUMBER = PRODUCE_NUMBER;

        //Change values in the farmBlockInfo
        transform.parent.GetComponent<FarmBlockInfo>().PLANT = next.transform;

        //Init new plant
        next.GetComponent<PlantBase>().Init(gameObject);

        //Remove this stage.
        Destroy(gameObject);
    }

    //Go to dead function
    public virtual void BeDead()
    {
        //Spawn replacement plant.
        GameObject next = Instantiate(DEAD_STAGE, transform.position, transform.rotation, transform.parent);

        //Transfer relevant details to new plant.
        next.GetComponent<PlantBase>().QUALITY = QUALITY;
        next.GetComponent<PlantBase>().PRODUCE_NUMBER = PRODUCE_NUMBER;

        //Init new plant
        next.GetComponent<PlantBase>().Init(gameObject);

        //Remove this stage.
        Destroy(gameObject);
    }
}
