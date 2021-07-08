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

    public long moneyIncreaseLevel; // Ŭ�� �� �ܰ� ���׷��̵� ����
    public long moneyIncreasePrice; // ���׷��̵� ����

    public Text textPrice; // ǥ���� �ؽ�Ʈ

    public Button buttonPrice; // �ܰ� ���׷��̵� ��ư

    public int employeeCount; // ���� �� (����)
    public Text textRecruit; // ���� ��� �г��� �ؽ�Ʈ

    public Button buttonRecruit;

    public int width; // ���� �ִ� ���� ��
    public float space; // ���� ����

    public GameObject prefabEmployee; // ���� ������

    public Text textPerson;

    public float spaceFloor; // �ٴ��� ���� 
    public int floorCapacity; // �ٴ��� ���� ������ �ο� ��
    public int currentFloor; // ���� �ٴ��� ��

    public GameObject prefabFloor; // �ٴ� ������

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

    // ������ ����.
    void MoneyIncrease()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ��ư ������ ��.
        {
            if (EventSystem.current.IsPointerOverGameObject() == false) // UI ���� ���� ���� ��.
            {
                money += moneyIncreaseAmount; //'������'�� '������ ������'��ŭ ������Ŵ.
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Instantiate(prefabMoney, mousePosition, Quaternion.identity);
            }
        }
    }

    void ShowInfo()
    {
        if (money == 0)
            textMoney.text = "0��"; // ������ 0�� �� �ؽ�Ʈǥ�⸦ ���� ���� ���.
        else
            textMoney.text = money.ToString("###,###") + "��";

        if (employeeCount == 0)
            textPerson.text = "0��";
        else
            textPerson.text = employeeCount + "��";
    }

    void UpdateUpgradePanel()
    {
        textPrice.text = "Lv." + moneyIncreaseLevel + " �ܰ� ���\n";
        textPrice.text += "���� �� �ܰ�>\n";
        textPrice.text += moneyIncreaseAmount.ToString("###,###") + " ��\n";
        textPrice.text += moneyIncreasePrice.ToString("###,###") + " ��";
    }

    public void UpgradePrice() // ��ư�� ������ �� public�� ���� ������ �ν����� �� ���� �Ұ�
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
        textRecruit.text = "Lv." + employeeCount + "���� ���\n\n";
        textRecruit.text += "���� 1�� �� �ܰ� > \n";
        textRecruit.text += AutoWork.autoMoneyIncreaseAmount.ToString("###,###") + " ��\n";
        textRecruit.text += "���׷��̵� ���� > \n";
        textRecruit.text += AutoWork.autoIncreasePrice.ToString("###,###") + " ��";
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
