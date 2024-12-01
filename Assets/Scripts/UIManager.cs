using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get; private set;}
    private Dictionary<string, string> inputComponent;
    private Dictionary<string, string> inputData;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("UIManager already exists. This instance will be destroyed.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSaveButtonClicked()
    {
        ;
    }

    public string getInputData()
    {
        return "0";
    }
}
