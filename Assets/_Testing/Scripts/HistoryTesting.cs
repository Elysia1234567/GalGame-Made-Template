using History;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class HistoryTesting : MonoBehaviour
    {
       public HistoryState state =new HistoryState();
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                state = HistoryState.Capture();
            if(Input.GetKeyDown(KeyCode.R))
                state.Load();
        }
    }
}