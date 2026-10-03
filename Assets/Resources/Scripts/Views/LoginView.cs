using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginView : MonoBehaviour, IUpdatable
{
    [SerializeField] private RectTransform playerAppearanceParent;
    [SerializeField] private RectTransform playerAppearancePrefab;
    [SerializeField] private RectTransform selectionParent; 
    [SerializeField] private RectTransform selectionPrefab;

    [SerializeField] private RectTransform playButton;

    // idSelection -> (idPlayer, namePlayer, appearance, level, clazz)
    public Dictionary<int, (int, string, string, int, int)> selections = new Dictionary<int, (int, string, string, int, int)>();

    private RectTransform selectedCharacter;

    private Action<CharacterSelectionClickEvent> characterSelectionObserver;
    private Action<CreateClickEvent> createClickObserver;

    private void Awake()
    {
        characterSelectionObserver = eventData => ShowAppearance(eventData.idSelection);
        createClickObserver = eventData => SceneManager.LoadScene("Create");
        playButton.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);

        ObserverManager.Unregister<CharacterSelectionClickEvent>(characterSelectionObserver);
        ObserverManager.Unregister<CreateClickEvent>(createClickObserver);
    }
    public void OnEnable()
    {
        GameManager.Instance.Register(this);

        CacheManager.characters.Clear();
        CacheManager.clazzes.Clear();
        selections.Clear();

        LoadDatabase();

        try
        {
            int idSelection = 0;

            foreach (var character in CacheManager.characters)
            {
                RectTransform characterSelection = PoolManager.Instance.Get(selectionPrefab, selectionParent);

                selections.Add(idSelection, (character.Key, character.Value.nameCharacter, character.Value.appearance, character.Value.level, character.Value.clazz));
                idSelection++;

                TMP_Text level = characterSelection.Find("Level").GetComponent<TMP_Text>();
                TMP_Text namePlayer = characterSelection.Find("NamePlayer").GetComponent<TMP_Text>();

                string clazzName = CacheManager.clazzes[character.Value.clazz].nameClazz;

                level.text = $"Level: {character.Value.level} {clazzName}";
                namePlayer.text = character.Value.nameCharacter;
            }
        } catch (Exception ex)
        {
            Debug.LogError($"Error: Load Characters from CacheManager: {ex.Message}");
        }

        ObserverManager.Register<CharacterSelectionClickEvent>(characterSelectionObserver);
        ObserverManager.Register<CreateClickEvent>(createClickObserver);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate() { }

    private void LoadDatabase()
    {
        try
        {
            string pathClazzes = Path.Combine(Application.dataPath, "Resources/Database/Classes.xlsx");
            string pathPlayers = Path.Combine(Application.dataPath, "Resources/Database/Characters.xlsx");

            // Load data from Excel files
            using (FileStream file = new FileStream(pathPlayers, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(file);
                ISheet sheet = workbook.GetSheetAt(0);

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    if (row == null)
                        continue;

                    int idCharacter = (int)row.GetCell(0).NumericCellValue;
                    string nameCharacter = row.GetCell(1).StringCellValue;
                    string appearance = row.GetCell(2).StringCellValue;
                    int level = (int)row.GetCell(3).NumericCellValue;
                    int clazz = (int)row.GetCell(4).NumericCellValue;

                    int idSelection = i - 1; // i - 1 để bắt đầu từ 0 giúp tránh lệch index

                    CacheManager.characters.Add(idCharacter, new Characters { nameCharacter = nameCharacter, appearance = appearance, level = level, clazz = clazz });
                }

                Debug.Log($"Load characters data successfully!");
            }
            using (FileStream file = new FileStream(pathClazzes, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(file);
                ISheet sheet = workbook.GetSheetAt(0);

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    if (row == null)
                        continue;

                    int idClazz = (int)row.GetCell(0).NumericCellValue;
                    string nameClazz = row.GetCell(1).StringCellValue;
                    string appearance = row.GetCell(2).StringCellValue;

                    CacheManager.clazzes.Add(idClazz, new Clazzes { nameClazz = nameClazz, appearance = appearance });
                }

                Debug.Log($"Load clazzes data successfully!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: Load file Excel: {ex.Message}");
        }
    }

    private void ShowAppearance(int selectedID)
    {
        if (selectedCharacter != null)
            PoolManager.Instance.Release(selectedCharacter);

        PlayerManager.idPlayer = selections[selectedID].Item1;

        selectedCharacter = PoolManager.Instance.Get(playerAppearancePrefab, playerAppearanceParent);

        Image image = selectedCharacter.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>($"Sprites/Appearance/{CacheManager.characters[PlayerManager.idPlayer].appearance}");
        image.preserveAspect = true;

        playButton.gameObject.SetActive(true);
    }
}
