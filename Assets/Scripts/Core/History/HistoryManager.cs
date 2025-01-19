using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace History
{
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {
        public const int HISTORY_CACHE_LIMIT = 100;
        public static HistoryManager instance {  get; private set; }
        public List<HistoryState> history=new List<HistoryState>();

        private HistoryNavigation navigation;

        private void Awake()
        {
            instance = this;
            navigation = GetComponent<HistoryNavigation>();
        }

        private void Start()
        {
            DialogueSystem.instance.onClear += LogCurrentState;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);
            if(history.Count > HISTORY_CACHE_LIMIT )
                history.RemoveAt(0);
        }

        public void LoadState(HistoryState state)
        {
            state.Load();
        }

        public void GoForward()=>navigation.GoForward();
        public void GoBack()=>navigation.GoBack();  

    }
}