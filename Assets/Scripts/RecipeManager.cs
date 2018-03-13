using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Manages all recipes
public static class RecipeManager{

    //Master list of all recipes, discovered or not
    public static List<Recipe> MASTER_LIST = new List<Recipe>();
    static List<string> KNOWN_LIST = new List<string>();

    public static TextAsset ASSET;

    //Whether or not the list has been init
    static bool LIST_INIT;

    //Line count
    static int LINE_COUNT;

    //Location of our text file
    //Two different ones  for saving. 
    //NOTE: Text file format:
    //      Apple Pie
    //      0.1 10
    //      Produce/Apple Produce/Sugar
    //static 
    static string MASTER_TXT_LOCATION = Application.dataPath + "/SaveData/MasterRecipeList.txt";

    //Helper function that inits the master list
    public static void InitList()
    {
        Debug.Log("Don't use this anymore");
    }

    public static void LoadData(string DATA)
    {
        //Compare loaded strings to the master list and set them to discovered
        string[] INPUT = DATA.Split('/');

        for(int i = 0; i < INPUT.Length; i++)
        {
            KNOWN_LIST.Add(INPUT[i]);
        }

        //Load master list
        LoadItems(MASTER_TXT_LOCATION);
    }

    //Function that adds items to the master list of items.
    public static void LoadItems(string PATH)
    {

        Debug.Log("Not actually loading the right stuff. Check known list.");
        string TEMP_NAME, TEMP_WEIGHT_AND_PRICE, TEMP_LIST_TEXT;

        //Split weight and price
        string[] TEMP_ARRAY;
        float TEMP_WEIGHT;
        int TEMP_PRICE;
        bool DISCOVERED;

        //Stream read the text
        using (StreamReader READER = new StreamReader(PATH))
        {
            //While we haven't reached the end of this text file
            while (!READER.EndOfStream)
            {
                //Read in the text 
                TEMP_NAME = READER.ReadLine(); //i.e Apple Pie
                TEMP_WEIGHT_AND_PRICE = READER.ReadLine(); // 0.1
                TEMP_LIST_TEXT = READER.ReadLine(); //i.e. Produce/Tomatoes Produce/Apples

                //Split weight and price
                TEMP_ARRAY = TEMP_WEIGHT_AND_PRICE.Split();
                TEMP_WEIGHT = float.Parse(TEMP_ARRAY[0]);
                TEMP_PRICE = int.Parse(TEMP_ARRAY[1]);

                //Split the ingredients
                TEMP_ARRAY = TEMP_LIST_TEXT.Split(' ');

                //List we will be adding to
                List<BaseItem> TEMP_LIST = new List<BaseItem>();

                //Load in through path names
                for (int i = 0; i < TEMP_ARRAY.Length; i++)
                {
                    GameObject TEMP = Resources.Load(TEMP_ARRAY[i], typeof(GameObject)) as GameObject;
                    TEMP_LIST.Add(TEMP.GetComponent<BaseItem>());
                }

                DISCOVERED = true;
                for(int i = 0; i < KNOWN_LIST.Count; i++)
                {
                    if (KNOWN_LIST[i].Equals(TEMP_NAME))
                    {
                        DISCOVERED = true;
                        KNOWN_LIST.RemoveAt(i);
                         break;
                    }
                }

                //Add to master list.
                AddItem(TEMP_NAME, TEMP_LIST, DISCOVERED, TEMP_WEIGHT, TEMP_PRICE);
            }
        }
    }

    //Helper function that adds an item to the list
    public static void AddItem(string NAME, List<BaseItem> INGREDIENTS, bool DISCOVERED, float WEIGHT, int PRICE)
    {
        MASTER_LIST.Add(new Recipe(NAME, INGREDIENTS, DISCOVERED, WEIGHT, PRICE));
    }

    //Helper function that returns a list of all discovered
    //Since we loaded in true items first, as soon as its false we can return.
    public static List<Recipe> GetDiscovered()
    {
        List<Recipe> RETURN_LIST = new List<Recipe>();
        foreach(Recipe TEMP in MASTER_LIST)
        {
            //Debug.Log(TEMP.NAME + TEMP.DISCOVERED);
            if (TEMP.DISCOVERED)
            {
                RETURN_LIST.Add(TEMP);
            }
            else
            {
                return RETURN_LIST;
            }
        }
        return RETURN_LIST;
    }

    //Save data for recipes
    public static void SaveData()
    {
        string DATA = "";
        foreach(Recipe RECIPE in MASTER_LIST)
        {
            if (!RECIPE.DISCOVERED)
            {
                break;
            }
            else
            {
                DATA += RECIPE.NAME + "\n";   
            }
        }
        SaveSystem.SaveTo(SaveSystem.SaveType.Recipes, "/Recipe\n" + DATA + "/");
    }
}

//Recipe class. Stores important information
[System.Serializable]
public class Recipe
{
    //Name of the recipe
    public string NAME;

    //list of ingredients this recipe needs
    public List<BaseItem> INGREDIENTS;

    //Discovered
    public bool DISCOVERED;

    //Weight used when shuffling
    public float WEIGHT;

    //Price of the recipe
    public int PRICE;

    //Constructor
    public Recipe(string NAME_T, List<BaseItem> INGREDIENTS_T, bool DISCOVERED_T, float WEIGHT_T, int PRICE_T)
    {
        NAME = NAME_T;
        INGREDIENTS = INGREDIENTS_T;
        DISCOVERED = DISCOVERED_T;
        WEIGHT = WEIGHT_T;
        PRICE = PRICE_T;
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Recipe))]
public class RecipeInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //Indent 
        var indent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel++;

        //Calculate rects
        var NAME_RECT = new Rect(position.x, position.y, 200, position.height);
        var DISCOVERED_RECT = new Rect(position.x + 200, position.y, 30, position.height);
        var WEIGHT_RECT = new Rect(position.x + 230, position.y, 30, position.height);
        var PRICE_RECT = new Rect(position.x + 260, position.y, 30, position.height);

        //GUIContent COMPLETE_LABEL = new GUIContent("Complete", "Whether or not this tutorial has been done.");

        //Draw fields
        EditorGUI.PropertyField(NAME_RECT, property.FindPropertyRelative("NAME"), GUIContent.none);
        EditorGUI.PropertyField(DISCOVERED_RECT, property.FindPropertyRelative("DISCOVERED"), GUIContent.none);
        EditorGUI.PropertyField(WEIGHT_RECT, property.FindPropertyRelative("WEIGHT"), GUIContent.none);


        //Go back to previous indent
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif