using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public long money;
    public long moneyIncreaseAmount;

    public Text textMoney;

    public GameObject prefabMoney;

    public long moneyIncreaseLevel; // 클릭 당 단가 업그레이드 레벨
    public long moneyIncreasePrice; // 업그레이드 가격

    public Text textPrice; // 표시할 텍스트

    public Button buttonPrice; // 단가 업그레이드 버튼

    public int employeeCount; // 직원 수 (레벨)
    public Text textRecruit; // 직원 고용 패널의 텍스트

    public Button buttonRecruit;

    public int width; // 가로 최대 직원 수
    public float space; // 직원 간격

    public GameObject prefabEmployee; // 직원 프리팹

    public Text textPerson;

    public float spaceFloor; // 바닥의 간격 
    public int floorCapacity; // 바닥이 수용 가능한 인원 수
    public int currentFloor; // 현재 바닥의 수

    public GameObject prefabFloor; // 바닥 프리팹

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/save.xml";
        if (System.IO.File.Exists(path))
        {
            Load();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowInfo();
        MoneyIncrease();

        UpdateUpgradePanel();

        ButtonActiveCheck();

        UpdateRecruitPanelText();
        CreateFloor();
    }

    // 소지금 증가.
    void MoneyIncrease()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 버튼 눌렀을 때.
        {
            if (EventSystem.current.IsPointerOverGameObject() == false) // UI 위에 있지 않을 때.
            {
                money += moneyIncreaseAmount; //'소지금'을 '소지금 증가량'만큼 증가시킴.
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Instantiate(prefabMoney, mousePosition, Quaternion.identity);
            }
        }
    }

    void ShowInfo()
    {
        if (money == 0)
            textMoney.text = "0원"; // 변수가 0일 때 텍스트표기를 안해 따로 사용.
        else
            textMoney.text = money.ToString("###,###") + "원";

        if (employeeCount == 0)
            textPerson.text = "0명";
        else
            textPerson.text = employeeCount + "명";
    }

    void UpdateUpgradePanel()
    {
        textPrice.text = "Lv." + moneyIncreaseLevel + " 단가 상승\n";
        textPrice.text += "외주 당 단가>\n";
        textPrice.text += moneyIncreaseAmount.ToString("###,###") + " 원\n";
        textPrice.text += moneyIncreasePrice.ToString("###,###") + " 원";
    }

    public void UpgradePrice() // 버튼과 연동할 때 public을 붙지 않으면 인스펙터 상에 노출 불가
    {
        if (money >= moneyIncreasePrice)
        {
            money -= moneyIncreasePrice;
            moneyIncreaseLevel += 1;
            moneyIncreaseAmount += moneyIncreaseLevel * 100;
            moneyIncreasePrice += moneyIncreaseLevel * 500;
        }
    }

    void ButtonActiveCheck()
    {
        if(money >= moneyIncreasePrice)
        {
            buttonPrice.interactable = true;
        }
        else
        {
            buttonPrice.interactable = false;
        }
    }

    void UpdateRecruitPanelText()
    {
        textRecruit.text = "Lv." + employeeCount + "직원 고용\n\n";
        textRecruit.text += "직원 1초 당 단가 > \n";
        textRecruit.text += AutoWork.autoMoneyIncreaseAmount.ToString("###,###") + " 원\n";
        textRecruit.text += "업그레이드 가격 > \n";
        textRecruit.text += AutoWork.autoIncreasePrice.ToString("###,###") + " 원";
    }

    void ButtonRecruitActiveCheck()
    {
        if (money >= AutoWork.autoIncreasePrice)
        {
            buttonRecruit.interactable = true;
        }
        else
        {
            buttonRecruit.interactable = false;
        }
    }

    void CreateEmployee()
    {
        Vector2 bossSpot = GameObject.Find("Boss").transform.position;
        float spotX = bossSpot.x + (employeeCount % width) * space;
        float spotY = bossSpot.y - (employeeCount / width) * space;

        Instantiate(prefabEmployee, new Vector2(spotX, spotY), Quaternion.identity);
    }

    public void Recruit()
    {
        if(money >= AutoWork.autoIncreasePrice)
        {
            money -= AutoWork.autoIncreasePrice;
            employeeCount += 1;
            AutoWork.autoMoneyIncreaseAmount += moneyIncreaseLevel * 10;
            AutoWork.autoIncreasePrice += employeeCount * 500;

            CreateEmployee();
        }
    }

    void CreateFloor()
    {
        Vector2 bgPosition = GameObject.Find("Background").transform.position;

        float nextFloor = (employeeCount + 1) / floorCapacity;

        float spotX = bgPosition.x;
        float spotY = bgPosition.y;

        spotY -= spaceFloor * nextFloor;

        if(nextFloor >= currentFloor)
        {
            Instantiate(prefabEmployee, new Vector2(spotX, spotY), Quaternion.identity);
            currentFloor += 1;
            Camera.main.GetComponent<CameraDrag>().limitMinY -= spaceFloor;
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    void Save()
    {
        SaveData saveData = new SaveData();

        saveData.money = money;
        saveData.moneyIncreaseAmount = moneyIncreaseAmount;
        saveData.moneyIncreaseLevel = moneyIncreaseLevel;
        saveData.moneyIncreasePrice = moneyIncreasePrice;
        saveData.employeeCount = employeeCount;
        saveData.autoMoneyIncreaseAmount = AutoWork.autoMoneyIncreaseAmount;
        saveData.autoIncreasePrice = AutoWork.autoIncreasePrice;

        string path = Application.persistentDataPath + "/save.xml";
        XmlManager.XmlSave<SaveData>(saveData, path);
    }

    void Load()
    {
        SaveData saveData = new SaveData();

        string path = Application.persistentDataPath + "/save.xml";
        saveData = XmlManager.XmlLoad<SaveData>(path);

        money = saveData.money;
        moneyIncreaseAmount = saveData.moneyIncreaseAmount;
        moneyIncreaseLevel = saveData.moneyIncreaseLevel;
        moneyIncreasePrice = saveData.moneyIncreasePrice;
        employeeCount = saveData.employeeCount;
        AutoWork.autoMoneyIncreaseAmount = saveData.autoMoneyIncreaseAmount;
        AutoWork.autoIncreasePrice = saveData.autoIncreasePrice;
        FileEmployee();
    }

    void FileEmployee()
    {
        GameObject[] employees = GameObject.FindGameObjectsWithTag("Employee");

        if(employeeCount != employees.Length)
        {
            for(int i = employees.Length; i <= employeeCount; i++)
            {
                Vector2 bossSpot = GameObject.Find("Boss").transform.position;
                float spotX = bossSpot.x + (i % width) * space;
                float spotY = bossSpot.y - (i / width) * space;

                GameObject obj = Instantiate(prefabEmployee, new Vector2(spotX, spotY), Quaternion.identity);
            }
        }
    }
}
