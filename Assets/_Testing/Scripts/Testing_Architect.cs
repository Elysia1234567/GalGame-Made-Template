using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;
        string[] lines = new string[5]
        {
            "This is a random line of dialogue.",
            "I want to say something,come over here.",
            "The world is a crazy place sometimes.",
            "Don't lose hope,things will get better.",
            "It's a bird?It's a plane? No! - It's Super Sheltie!"
        };

        // Start is called before the first frame update
        void Start()
        {
            ds=DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.fade;
            //architect.speed=0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            string longLine = "this is a very long line that makes no sense but i am just populating it with stuff.this is a very long line that makes no sense but i am just populating it with stuff.";
            if(bm!=architect.buildMethod)
            {
                architect.buildMethod = bm;
                architect.Stop();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                architect.Stop();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(architect.isBuilding)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                        architect.ForceComplete();
                }
                else
                    architect.Build(longLine);
                //architect.Build(lines[Random.Range(0, lines.Length)]);
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
                //architect.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }
}


