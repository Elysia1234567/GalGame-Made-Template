using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CommandTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    

    IEnumerator Running()
    {
        yield return CommandManager.instance.Execute("print");
        yield return CommandManager.instance.Execute("print_1p", "Hello World!");
        yield return CommandManager.instance.Execute("print_mp", "Line1", "Line2", "Line3");
        yield return CommandManager.instance.Execute("lambda");
        yield return CommandManager.instance.Execute("lambda_1p", "Hello World!");
        yield return CommandManager.instance.Execute("lambda_mp", "Line1", "Line2", "Line3");
        yield return CommandManager.instance.Execute("process");
        yield return CommandManager.instance.Execute("process_1p", "3");
        yield return CommandManager.instance.Execute("process_mp", "Line1", "Line2", "Line3");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CommandManager.instance.Execute("moveCharDemo", "left");
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            CommandManager.instance.Execute("moveCharDemo", "right");
    }
}
