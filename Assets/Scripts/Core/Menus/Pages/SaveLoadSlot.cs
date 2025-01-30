using History;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VISUALNOVEL;

public class SaveLoadSlot : MonoBehaviour
{
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI titleText;
    public Button deleteButton;
    public Button saveButton;
    public Button loadButton;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath = "";

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;
    public void PopulateDetails(SaveAndLoadMenu.MenuFunction function)
    {
        if(File.Exists(filePath))
        {
            VNGameSave file=VNGameSave.Load(filePath);
            PopulateDetailsFromFile(function, file);    
        }
        else
        {
            PopulateDetailsFromFile(function,null);
        }
    }

    public void PopulateDetailsFromFile(SaveAndLoadMenu.MenuFunction function, VNGameSave file)
    {
        if(file == null)
        {
            titleText.text = $"{fileNumber}.Empty File";
            deleteButton.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(function==SaveAndLoadMenu.MenuFunction.save);
            previewImage.texture = SaveAndLoadMenu.Instance.emptyFileImage;
        }
        else
        {
            titleText.text = $"{fileNumber}.{file.timestamp}";
            deleteButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.load);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.save);

            byte[] data=File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, data);
            previewImage.texture = screenshotPreview;
            
        }
    }

    public void Delete()
    {
        uiChoiceMenu.Show("Delete this file?(<i>This cannot be undone!</i>)", 
            new UIConfirmationMenu.ConfirmationButton("Yes", () =>
            {
                uiChoiceMenu.Show(
                    "Are you sure?",
                    new UIConfirmationMenu.ConfirmationButton("I am sure", OnConfirmDelete),
                    new UIConfirmationMenu.ConfirmationButton("Never Mind", null)
                    ) ;
            },autoCloseOnQuit:false), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void OnConfirmDelete()
    {
        File.Delete(filePath);
        PopulateDetails(SaveAndLoadMenu.Instance.menuFunction);
    }


    public void Load()
    {
        VNGameSave file = VNGameSave.Load(filePath, false);
        SaveAndLoadMenu.Instance.Close(closeAllMenus: true);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == MainMenu.MAIN_MENU_SCENE)
        {
            MainMenu.instance.LoadGame(file);
        }
        else
        {
            file.Activate();
        }
       

        
    }

    public void Save()
    {
        if(HistoryManager.instance.isViewingHistory)
        {
            UIConfirmationMenu.instance.Show("You cannot save while viewing history.", new UIConfirmationMenu.ConfirmationButton("Okay", null));
            return;
        }
            
        var activeSave=VNGameSave.activeFile;

        activeSave.slotNumber = fileNumber;
        activeSave.Save();

        PopulateDetailsFromFile(SaveAndLoadMenu.Instance.menuFunction, activeSave);
    }
}
