using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class SwordSkill : Skill
{
    public SwordType swordType;

    [Header("Bounce info")]
    [SerializeField] private SkillTreeSlotUI bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;

    [Header("Pierce info")]
    [SerializeField] private SkillTreeSlotUI pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private SkillTreeSlotUI spinUnlockButton;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;
    [SerializeField] private float hitCooldown;

    [Header("Sword info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;
    private GameObject[] dots;

    [Header("Unlock sword")]
    [SerializeField] private SkillTreeSlotUI swordUnlockButton;
    public bool swordUnlocked { get; private set; }

    [Header("passive skills")]
    [SerializeField] private SkillTreeSlotUI timeStopUnlockButton;
    [SerializeField] private SkillTreeSlotUI vulunerableUnlockButton;
    public bool timeStopUnlocked;
    public bool vulunerableUnlocked;


    protected override void Start()
    {
        base.Start();

        SetupGravity();

        GenerateDots();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulunerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulunerable);

    }

    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockSword();
        UnlockBounceSword();
        UnlockSpinSword();
        UnlockPierceSword();
        UnlockTimeStop();
        UnlockVulunerable();
    }

    private void SetupGravity()
    {
        switch (swordType)
        {
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                break;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController newSwordScript = newSword.GetComponent<SwordSkillController>();

        switch (swordType)
        {
            case SwordType.Bounce:
                newSwordScript.SetupBounce(true, bounceAmount);
                break;
            case SwordType.Pierce:
                newSwordScript.SetupPierce(pierceAmount);
                break;
            case SwordType.Spin:
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
                break;
        }


        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }


    #region Unlock region

    private void UnlockTimeStop()
    {
        if(timeStopUnlockButton.unlock)
            timeStopUnlocked = true;
    }

    private void UnlockVulunerable()
    {
        if(vulunerableUnlockButton.unlock)
            vulunerableUnlocked = true;
    }

    private void UnlockSword()
    {
        if (swordUnlockButton.unlock)
        {
            swordUnlocked = true;
            swordType = SwordType.Regular;
        }
    }

    private void UnlockBounceSword()
    {
        if(bounceUnlockButton.unlock)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if(pierceUnlockButton.unlock)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlock)
            swordType= SwordType.Spin;
    }

    #endregion

    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float _time)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * _time + .5f * (Physics2D.gravity * swordGravity) * _time * _time;

        return position;
    }
    #endregion
}
