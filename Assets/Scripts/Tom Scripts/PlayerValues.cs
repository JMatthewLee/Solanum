using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerValues : MonoBehaviour
{
    [SerializeField] private DammageFlash dammageFlash;

    public int maxHp= 5;
    public int hp;
    public int noBonusAc = 10;
    public int ac;
    public int minCoins = 0;
    public int coins;

    public TMP_Text hpText;
    public TMP_Text acText; 
    public TMP_Text coinText; 

    void Start()
    {
        hp = maxHp;
        ac = noBonusAc;
        coins = minCoins;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpText != null)
        {
            hpText.text = hp.ToString();
        }
        if (acText != null)
        {
            acText.text = ac.ToString();
        }
        if (coinText != null)
        {
            coinText.text = coins.ToString()+"g";
        }
    }

    //----------------------------HP Functions----------------------------------

    public void RecieveDamage(int damageDelt)
    {
        dammageFlash.CallHitFlash();//Calls the dammage flash script

        hp -= damageDelt;

        if (hp <= 0)
        {
            hp= 0;
            Destroy(gameObject);
        }
        UpdateUI();
    }
    public void Heal(int healAmount)
    {
        hp += healAmount;
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        UpdateUI();
    }

    //-----------------------------AC Functions-----------------------------------

    public void BonusAC(int bonusAc)
    {
        ac += bonusAc;
        UpdateUI();
    }


    //----------------------------Coins Functions---------------------------------

    public void GainCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

}