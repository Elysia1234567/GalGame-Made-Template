using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MonoBehaviour
{
    public static InputPanel instance { get; private set; } =null;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController cg;

    public string lastInput {  get; private set; } = string.Empty;

    public bool isWaitingOnUserInput {  get; private set; }

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cg=new CanvasGroupController(this,canvasGroup);

        canvasGroup.alpha = 0;
        SetCanvasState(active:false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);
        
    }

    // Update is called once per frame
    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        cg.Show();
        SetCanvasState(active:true);
        isWaitingOnUserInput = true;
    }

    public void OnAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;
        lastInput = inputField.text;
        Hide();
    }

    public void Hide()
    {
        cg.Hide();
        SetCanvasState(active:false);
        isWaitingOnUserInput=false;
    }

    private void SetCanvasState(bool active)
    {
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    public void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }

    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
}
