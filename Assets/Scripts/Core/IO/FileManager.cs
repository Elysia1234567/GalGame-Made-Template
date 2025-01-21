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

    public static bool TryCreateDirectoryFromPath(string path)
    {
        Debug.LogWarning(path);
        if (Directory.Exists(path) || File.Exists(path))
            return true;
        if(path.Contains("."))
        {
            path=Path.GetDirectoryName(path);
            if(Directory.Exists(path))
                return true ;

        }
        if(path==string.Empty)
            return false;
        return false;
    }

    public static void Save(string filePath,string JSONData)
    {
        if(!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"FAILED TO SAVE FILE '{filePath}'");
            return;
        }

        StreamWriter sw=new StreamWriter(filePath);
        sw.Write(JSONData);
        sw.Close();

        Debug.Log($"保存文件到{filePath}");
    }

    public static T Load<T>(string filePath)
    {
        if(File.Exists(filePath))
        {
            string JSONData = File.ReadAllLines(filePath)[0];
            return JsonUtility.FromJson<T>(JSONData);
        }
        else
        {
            Debug.LogError($"文件{filePath}不存在");
            return default(T);
        }
    }
}
