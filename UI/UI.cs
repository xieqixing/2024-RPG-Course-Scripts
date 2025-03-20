using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] public DarkScreenUI fadeScreen;
    [SerializeField] public GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterMenu;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject ingameUI;

    public ItemTooltipUI itemTooltip;
    public StatTooltipUI statTooltip;
    public CraftWindowUI craftWindow;
    public SkillTooltipUI skillTooltip;

    private void Awake()
    {
        SwitchTo(skillTreeUI);

        fadeScreen.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchTo(ingameUI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
        //itemTooltip = GetComponentInChildren<ItemTooltipUI>();
        //SwitchTo(characterMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchWithKeyTo(characterMenu);
        }
    }

    public void SwitchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<DarkScreenUI>() != null;

            if(!fadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if(_menu != null)
            _menu.SetActive(true);

        if(GameManager.Instance != null)
        {
            if(_menu == ingameUI)
                GameManager.Instance.PauseGame(false);
            else
                GameManager.Instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();

            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<DarkScreenUI>() == null && transform.GetChild(i).gameObject.activeSelf)
                return;
        }

        SwitchTo(ingameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeIn();
        StartCoroutine(EndScreenCoroutine());
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(2);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.Instance.RestartScene();
}
