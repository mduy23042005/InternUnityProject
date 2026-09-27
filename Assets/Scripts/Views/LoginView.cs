using System.IO;
using UnityEngine;
using NPOI.SS.UserModel;

public class LoginView : MonoBehaviour, IUpdatable
{
    public void OnDisable()
    {
        GameManager.Instance.Unregister(this);
    }

    public void OnEnable()
    {
        GameManager.Instance.Register(this);

        string path = Path.Combine(Application.dataPath, "Database/Players.xlsx");

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

            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (row == null)
                    continue;

                for (int j = 0; j < row.LastCellNum; j++)
                {
                    ICell cell = row.GetCell(j);

                    if (cell != null)
                    {
                        Debug.Log(cell.ToString());
                    }
                }
            }
        }
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate() { }
}
