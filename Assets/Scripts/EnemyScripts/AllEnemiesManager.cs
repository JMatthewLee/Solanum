using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//https://www.youtube.com/watch?v=anHxFtiVuiE for connecting this to UI

public class AllEnemiesManager : MonoBehaviour
{

    List<EnemyValues> listOfEnemyValueScript = new List<EnemyValues>();
    List<EnemyController> listOfEnemyControllerScript = new List<EnemyController>();

    public int totalEnemies;

    private TMP_Text TotalEnemyTxt;

    private void OnEnable()
    {
        EnemyValues.EnemyHasDied += enemyDeathHandlerValues; //Grab the event from enemy and add it to the death handler
        EnemyController.EnemyHasDied += enemyDeathHandlerController;
    }

    private void OnDisable()
    {
        EnemyValues.EnemyHasDied -= enemyDeathHandlerValues; //Grab the event from enemy and remove it from the death handler
        EnemyController.EnemyHasDied -= enemyDeathHandlerController;
    }

    private void Awake()
    {
        TotalEnemyTxt = GameObject.Find("EnemyCountTxt").GetComponent<TMP_Text>();

        listOfEnemyValueScript = GameObject.FindObjectsOfType<EnemyValues>().ToList();
        listOfEnemyControllerScript = GameObject.FindObjectsOfType<EnemyController>().ToList();
    
        totalEnemies = listOfEnemyValueScript.Count + listOfEnemyControllerScript.Count;
        Debug.Log("Total Enemies: " + totalEnemies);
        //Update UI HERE
        UpdateUI();
    }

    void enemyDeathHandlerValues(EnemyValues enemy)
    {
        listOfEnemyValueScript.Remove(enemy);

        totalEnemies = listOfEnemyValueScript.Count + listOfEnemyControllerScript.Count;
        //Update UI HERE
        UpdateUI();
    }

    void enemyDeathHandlerController(EnemyController enemy)
    {
        listOfEnemyControllerScript.Remove(enemy);

        totalEnemies = listOfEnemyValueScript.Count + listOfEnemyControllerScript.Count;
        //Update UI HERE
        UpdateUI();
    }
    void UpdateUI()
    {
        Debug.Log("UI UPDATED");
        if (TotalEnemyTxt != null)
        {
            TotalEnemyTxt.text = totalEnemies.ToString();
        }
    }
}
