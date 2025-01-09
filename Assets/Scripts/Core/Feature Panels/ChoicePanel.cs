using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
   public static ChoicePanel instance { get; private set; }

    private const float BUTTON_MIN_WIDTH = 50;
    private const float BUTTON_MAX_WIDTH = 1000;
    private const float BUTTON_WIDTH_PADDING = 25;

    private const float BUTTON_HEIGHT_PER_LINE = 50f;
    private const float BUTTON_HEIGHT_PADDING = 20;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;

    private CanvasGroupController cg = null;
    private List<ChoiceButton> buttons=new List<ChoiceButton>();
    public ChoicePanelDecision lastDecision { get; private set; } = null;

    public bool isWaitingOnUserChoice { get; private set; } = false;

    private void Awake()
    {
        instance = this;
        cg = new CanvasGroupController(this, canvasGroup);
        cg.alpha = 0;
        cg.SetInteractableState(false);
    }

    private void Start()
    {
        //Debug.LogWarning("已经初始化");
        
    }

    public void Show(string question, string[] choices)
    {
        lastDecision=new ChoicePanelDecision(question, choices);

        isWaitingOnUserChoice = true;

        cg.Show();
        cg.SetInteractableState(active: true);

        titleText.text = question;
        StartCoroutine(GenerateChoices(choices));
    }

    private IEnumerator GenerateChoices(string[] choices)
    {
        float maxWidth = 0;

        for(int i=0;i<choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if(i<buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle =newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout=newButton.GetComponent<LayoutElement>();

                choiceButton =new ChoiceButton { button = newButton,title=newTitle, layout = newLayout };
                //Debug.LogWarning($"{choiceButton.title}");
                buttons.Add(choiceButton);
            }
            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(()=>AcceptAnswer(buttonIndex));
            //Debug.LogWarning($"{choiceButton.title}");
            choiceButton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choiceButton.title.preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_WIDTH);
            maxWidth=Mathf.Max(maxWidth, buttonWidth);
        }
        foreach(var button in buttons)
        {
            button.layout.preferredWidth=maxWidth;
        }
        for(int i=0;i<buttons.Count; i++)
        {
            bool show = i < choices.Length;
            buttons[i].button.gameObject.SetActive(show);

        }

        yield return new WaitForEndOfFrame();

        foreach(var button in buttons)
        {
            int lines = button.title.textInfo.lineCount;
            //Debug.Log(lines);
            button.layout.preferredHeight=BUTTON_HEIGHT_PADDING+(BUTTON_HEIGHT_PER_LINE*lines);

        }
    }

    public void Hide()
    {
        cg.Hide();
    }

    private void AcceptAnswer(int index)
    {
        if (index < 0 || index > lastDecision.choices.Length - 1)
            return;
        lastDecision.answerIndex = index;
        isWaitingOnUserChoice = false;
        Hide();
    }

    public class ChoicePanelDecision
    {
        public string question=string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string question, string[] choices)
        {
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton
    {
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }

}
