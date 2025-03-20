using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandlers dataHandlers;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandlers = new FileDataHandlers(Application.persistentDataPath, fileName, encryptData);
        dataHandlers.Delete();
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;


        dataHandlers = new FileDataHandlers(Application.persistentDataPath, fileName, encryptData);
    }

    private void Start()
    {

        saveManagers = FindAllSaveManagers();

        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandlers.Load();

        if(this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
            return;
        }

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

        Debug.Log("Loaded currency " + gameData.currency);
        
    }

    public void SaveGame()
    {
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandlers.Save(gameData);
        Debug.Log("Saved currency " + gameData.currency);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        if(dataHandlers.Load() != null)
        {
            return true;
        }

        return false;
    }
}
