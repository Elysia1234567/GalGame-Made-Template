using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

public class SaveAndLoadMenu : MenuPage
{
    public static SaveAndLoadMenu Instance { get; private set; }    

    public const int MAX_FILES = 99;
   
    private int currentPage = 1;
    private bool loadedFilesForFirstTime=false;

    public SaveLoadSlot[] saveSlots;
    public int slotsPerPage=>saveSlosts.Length;

    public enum MenuFunction { save,load }  
    public MenuFunction menuFunction=MenuFunction.save;

    public SaveLoadSlot[] saveSlosts;

    public Texture emptyFileImage;
    private void Awake()
    {
        Instance = this;
    }
    public override void Open()
    {
        base.Open();
        if(!loadedFilesForFirstTime )
            PopulateSaveSlotsForPage(currentPage);
    }

    public void PopulateSaveSlotsForPage(int pageNumber)
    {
        currentPage = pageNumber;
        int startingFile=((currentPage-1)*slotsPerPage)+1;
        int endingFile = startingFile + slotsPerPage - 1;

        for(int i=0;i<slotsPerPage;i++)
        {
            int fileNum = startingFile + i;
            SaveLoadSlot slot = saveSlosts[i];

            if(fileNum<MAX_FILES)
            {
                slot.root.SetActive(true);
                string filePath = $"{FilePaths.gameSaves}{fileNum}{VNGameSave.FILE_TYPE}";
                slot.fileNumber=fileNum;
                slot.filePath=filePath;
                slot.PopulateDetails(menuFunction);
            }
            else
            {
                slot.root.SetActive(false);
            }
        }
    }
}
