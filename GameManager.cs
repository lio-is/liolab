using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Game values:
    [Header("------Game Values------")]
    public int pesos;                               // Currency
    public int experience;                          // Experience
    public List<int> weaponPrices;                  // Weapon price table
    public List<int> xpTable;                       // Level-up experience table    

    // Resources:
    [Header("------Resources------")]
    public List<Sprite> playerSprites;              // Player sprites
    public List<Sprite> weaponSprites;              // Weapon sprites


    // References:
    [Header("------References------")]
    public Player player;                           // Player
    public Weapon weapon;                           // Weapon
    public UIManager UIManager;                     // UI Manager
    public SaveManager SaveManager;                 // Save Manager


    private void Awake()
    {
        // Prevents duplicate objects from being created on scene reload
        if (GameManager.instance != null)
        {
            Destroy(player.gameObject);
            Destroy(gameObject);
            Destroy(UIManager.gameObject);
            Destroy(SaveManager.gameObject);
            return;
        }

        instance = this;

        // Load save state on scene loaded
        SceneManager.sceneLoaded += LoadState;

        // Preserve these objects
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("UIManager"));
        DontDestroyOnLoad(GameObject.Find("SaveManager"));
    }

    // Update various UI information function:
    public void OnUIChange()
    {
        UIManager.UIUpdate();
    }

    // Show short text message function:
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        UIManager.ShowText(msg, fontSize, color, position, motion, duration);
    }

    // Check if weapon can be upgraded function:
    public bool TryUpgradeWeapon()
    {
        // If the weapon level is already at the maximum, it cannot be upgraded further
        if (weaponPrices.Count <= weapon.weaponLevel)
            return false;

        // If there is enough currency, perform the upgrade
        if (pesos >= weaponPrices[weapon.weaponLevel])
        {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();

            return true;
        }
        return false;
    }

    // XP leveling system:
    public int GetCurrentLevel()
    {
        // Get the current level
        int l = 0, add = 0;
        while (experience >= add)
        {
            add += xpTable[l];
            l++;

            if (l == xpTable.Count)
                return l;
        }
        return l;
    }
    public int GetXPToLevel(int level)
    {
        // Get the XP required for the given level
        int l = 0, xp = 0;
        while (l < level)
        {
            xp += xpTable[l];
            l++;
        }
        return xp;
    }
    public void GrantXP(int xp)
    {
        // Gain experience points
        int currentLevel = GetCurrentLevel();
        experience += xp;

        OnUIChange();

        if (currentLevel < GetCurrentLevel())
            OnLevelUp();
    }
    public void OnLevelUp()
    {
        ShowText("LEVEL UP!", 30, Color.yellow, player.transform.position, Vector3.up * 30, 2.0f);
        player.OnLevelUp();
        OnUIChange();
    }

    // Respawn function:
    public void Respawn()
    {
        // Hide death UI, reload the main scene
        SceneManager.LoadScene(1);
        UIManager.HideDeathAnimation();

        // Configure respawn information
        player.Respawn();
    }

    // Save game state function:
    public void SaveState()
    {
        // JSON saving
        SaveManager.SaveGame();
    }

    // Load game state function:
    public void LoadState(Scene s, LoadSceneMode sceneMode)
    {
        SaveManager.LoadGame();

        // Note: It is Save, not Sava, be careful not to write it wrong, otherwise it won't be found
        //if (!PlayerPrefs.HasKey("SaveState"))
        //    return;

        // Retrieve various game information into data[]
        //string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        //    s: "10|20|30|5"   => "10" "20" "30" "5"
        //Debug.Log("data:" + data[0] + "|" + data[1] + "|" + data[2] + "|" + data[3]);

        // Load currency
        //pesos = int.Parse(data[1]);

        // Load experience and player level
        //experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());

        // Load weapon
        //weapon.SetWeaponLevel(int.Parse(data[3]));

        // Set spawn point
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;

        OnUIChange();
    }
}


