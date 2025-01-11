using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class VariableStoreTesting : MonoBehaviour
    {
        // Start is called before the first frame update
        public int var_int = 0;
        public float var_flt = 0;
        public bool var_bool = false;
        public string var_str = "";
        void Start()
        {
            VariableStore.CreateDatabase("DB_Links");
            VariableStore.CreateVariable("L_int",var_int,()=>var_int,value=>var_int=value);

            VariableStore.CreateDatabase("DB_Numbers");
            VariableStore.CreateDatabase("DB2");
            VariableStore.CreateDatabase("DB3");

            VariableStore.CreateVariable("DB_Numbers.num1", 1);
            VariableStore.CreateVariable("DB_Numbers.num2", 5);
            VariableStore.CreateVariable("lightIsOn", true);
            VariableStore.CreateVariable("float1", 7.5f);
            VariableStore.CreateVariable("str1", "Hello");
            VariableStore.CreateVariable("str2", "World");
            VariableStore.PrintAllDatabases();

            VariableStore.PrintAllVariables();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                VariableStore.PrintAllVariables();
            }

            if(Input.GetKeyDown(KeyCode.A))
            {
                string variable = "DB_Numbers.num1";
                VariableStore.TryGetValue(variable, out object v);
                VariableStore.TrySetValue(variable, (int)v + 5);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                
                VariableStore.TryGetValue("DB_Numbers.num1", out object num1);
                VariableStore.TryGetValue("DB_Numbers.num2", out object num2);

                Debug.Log($"num1+num2={(int)num1 + (int)num2}");
            }
            if(Input.GetKeyDown(KeyCode.Z))
            {
                VariableStore.TryGetValue("L_int", out object linked_integer);
                VariableStore.TrySetValue("L_int",(int)linked_integer+5);
            }
        }
    }
}