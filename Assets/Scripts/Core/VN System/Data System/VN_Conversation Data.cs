using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VISUALNOVEL
{
    [System.Serializable]
    public class VN_ConversationData 
    {
        public List<string> conversation =new List<string>();
        public int progress;
    }
}