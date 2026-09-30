using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour, IUpdatable
{
    [SerializeField] private RectTransform playerAppearanceParent;
    [SerializeField] private RectTransform playerAppearancePrefab;
    [SerializeField] private RectTransform selectionParent; 
    [SerializeField] private RectTransform selectionPrefab;

    private RectTransform selectedCharacter;

    private Action<CharacterSelectionClickEvent> characterSelectionObserver;

    private void Awake()
    {
        characterSelectionObserver = eventData => ShowAppearance(eventData.idSelection);
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);

        ObserverManager.Unregister<CharacterSelectionClickEvent>(characterSelectionObserver);
    }

    public void OnEnable()
    {
        GameManager.Instance.Register(this);

        string path = Path.Combine(Application.dataPath, "Resources/Database/Players.xlsx");

        if (!File.Exists(path))
        {
            Debug.LogError($"Không tìm thấy file: {path}");
            return;
        }

        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            Debug.Log($"Có file {path}");

            IWorkbook workbook = WorkbookFactory.Create(file);
            ISheet sheet = workbook.GetSheetAt(0);

            // Bắt đầu từ 1 để bỏ qua dòng tiêu đề
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (row == null)
                    continue;

                int idPlayer = (int)row.GetCell(0).NumericCellValue;
                string namePlayer = row.GetCell(1).StringCellValue;
                string appearance = row.GetCell(2).StringCellValue;
                int level = (int)row.GetCell(3).NumericCellValue;

                PlayerManager.characters.Add(idPlayer, (namePlayer, appearance, level));
            }
        }

        foreach (var character in PlayerManager.characters)
        {
            RectTransform characterSelection = PoolManager.Instance.Get(selectionPrefab, selectionParent);

            TMP_Text level = characterSelection.Find("Level").GetComponent<TMP_Text>();
            TMP_Text namePlayer = characterSelection.Find("NamePlayer").GetComponent<TMP_Text>();

            level.text = $"Level: {character.Value.Item3.ToString()}";
            namePlayer.text = character.Value.Item1;
        }

        ObserverManager.Register<CharacterSelectionClickEvent>(characterSelectionObserver);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate() { }

    private void ShowAppearance(int idSelection)
    {
        if (selectedCharacter != null)
            PoolManager.Instance.Release(selectedCharacter);

        PlayerManager.idPlayer = idSelection;

        selectedCharacter = PoolManager.Instance.Get(playerAppearancePrefab, playerAppearanceParent);

        Image image = selectedCharacter.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>($"Sprites/Appearance/{PlayerManager.characters[idSelection].Item2}");
        image.preserveAspect = true;
    }
}
