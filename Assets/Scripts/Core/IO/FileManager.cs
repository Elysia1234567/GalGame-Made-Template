using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
   public static List<string> ReadTextFile(string filePath,bool includeBlankLines=true)
   {
        if(!filePath.StartsWith('/'))
            filePath=FilePaths.root+filePath;
        List<string > lines=new List<string>();
        try
        {
            using(StreamReader sr=new StreamReader(filePath))
            {
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
        }
        catch(FileNotFoundException ex)
        {
            Debug.LogError($"没找到文件:{ex.FileName}");
        }
        return lines;
   }

    public static List<string> ReadTextAsset(string filePath,bool includeBlankLines=true)
    { 
        TextAsset asset=Resources.Load<TextAsset>(filePath);
        if(asset==null)
        {
            Debug.LogError($"没找到资源:{filePath}");
            return null;
        }
        return ReadTextAsset(asset,includeBlankLines);
    }

    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {
        List<string> lines=new List<string>();
        using(StringReader sr=new StringReader(asset.text))
        {
            while(sr.Peek()>-1)//如果到了文件末尾就会变成-1
            {
                string line = sr.ReadLine();
                if(includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }
        }
        return lines;
    }
}
