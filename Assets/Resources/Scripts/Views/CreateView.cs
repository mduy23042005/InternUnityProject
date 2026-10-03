using NPOI.SS.UserModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateView : MonoBehaviour, IUpdatable
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text statusMessage;

    [SerializeField] private List<RectTransform> clazzSelectionParents;
    [SerializeField] private RectTransform clazzSelectionPrefab;

    [SerializeField] private RectTransform createButton;

    private Action<ClazzSelectionClickEvent> clazzSelectionObserver;
    private Action<BackClickEvent> backClickObserver;
    private Action<CreateCharacterClickEvent> createClickObserver;

    public Dictionary<int, (int, string, string)> selections = new Dictionary<int, (int, string, string)>();

    private int selectedIDClazz;

    private void Awake()
    {
        clazzSelectionObserver = eventData => ShowInfoClazz(eventData.idSelection);
        backClickObserver = eventData => SceneManager.LoadScene("Login");
        createClickObserver = eventData => CreateCharacter();

        createButton.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);

        ObserverManager.Unregister<ClazzSelectionClickEvent>(clazzSelectionObserver);
        ObserverManager.Unregister<BackClickEvent>(backClickObserver);
        ObserverManager.Unregister<CreateCharacterClickEvent>(createClickObserver);
    }
    public void OnEnable()
    {
        GameManager.Instance.Register(this);

        try
        {
            int idSelection = 0;

            foreach (var clazz in CacheManager.clazzes)
            {
                RectTransform clazzSelection = PoolManager.Instance.Get(clazzSelectionPrefab, clazzSelectionParents[idSelection]);

                selections.Add(idSelection, (clazz.Key, clazz.Value.nameClazz, clazz.Value.appearance));
                idSelection++;

                Image image = clazzSelection.GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>($"Sprites/Appearance/{CacheManager.clazzes[clazz.Key].appearance}");
                image.preserveAspect = true;
            }
        } catch (Exception ex)
        {
            Debug.LogError($"Error: Load Clazzes from CacheManager: {ex.Message}");
        }


        ObserverManager.Register<ClazzSelectionClickEvent>(clazzSelectionObserver);
        ObserverManager.Register<BackClickEvent>(backClickObserver);
        ObserverManager.Register<CreateCharacterClickEvent>(createClickObserver);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate() { }

    private void ShowInfoClazz(int idSelection)
    {
        int idClazz = idSelection + 1;

        if (!CacheManager.clazzes.ContainsKey(idClazz))
            return;

        selectedIDClazz = idClazz;
        createButton.gameObject.SetActive(true);

        title.text = CacheManager.clazzes[idClazz].nameClazz;
        description.text = $"Description of {CacheManager.clazzes[idClazz].nameClazz}";
    }
    private void CreateCharacter()
    {
        string namePlayer = nameInput.text.Trim();

        if (string.IsNullOrEmpty(namePlayer))
        {
            statusMessage.text = "Please enter your name!";
            statusMessage.color = Color.yellow;
            return;
        }

        try
        {
            string pathPlayers = Path.Combine(Application.dataPath, "Resources/Database/Characters.xlsx");

            IWorkbook workbook;

            // Đọc và load Workbook vào bộ nhớ
            using (FileStream file = new FileStream(pathPlayers, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = WorkbookFactory.Create(file);
            }

            ISheet sheet = workbook.GetSheetAt(0);
            int maxID = 0;

            // Kiểm tra tên nhân vật đã tồn tại chưa
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                ICell cellName = row.GetCell(1);
                if (cellName != null && cellName.StringCellValue.Equals(namePlayer, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogWarning($"Character name '{namePlayer}' already exists!");
                    workbook.Close();
                    return;
                }

                ICell cellID = row.GetCell(0);
                if (cellID != null)
                {
                    int idPlayer = (int)cellID.NumericCellValue;
                    if (idPlayer > maxID) maxID = idPlayer;
                }
            }

            // Thêm dòng mới
            int newIDPlayer = maxID + 1;
            int newRowIndex = sheet.LastRowNum + 1;
            IRow newRow = sheet.CreateRow(newRowIndex);

            newRow.CreateCell(0).SetCellValue(newIDPlayer);
            newRow.CreateCell(1).SetCellValue(namePlayer);
            newRow.CreateCell(2).SetCellValue(CacheManager.clazzes[selectedIDClazz].appearance);
            newRow.CreateCell(3).SetCellValue(1);
            newRow.CreateCell(4).SetCellValue(selectedIDClazz);

            // Ghi dữ liệu ra file an toàn
            using (FileStream outputFile = new FileStream(pathPlayers, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                workbook.Write(outputFile);
            }

            workbook.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: Create character: {ex.Message}");
            return;
        }

        SceneManager.LoadScene("Login");
    }
}
