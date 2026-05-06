using UnityEngine;
using TMPro; // for TMP text
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class SkillEquipmentUI : MonoBehaviour
{
    [Header("SkillAttachment Reference")]
    [SerializeField] private SkillAttachment skillAttachment; // reference to skill attachment

    [Header("Skill UI")]
    [SerializeField] private Button[] skillSlotButtons; // 4 active skills in total
    [SerializeField] private Button skillButtonPrefab;  // button prefab
    [SerializeField] private Button doneButton; // finished equipping skills
    [SerializeField] private Transform unlockedSkillContainer; // how many unlocked skills

    [Header("Skill Panel")]
    [SerializeField] private GameObject skillPanel; // for controlling the UI

    [Header("Done Key")]
    [SerializeField] private KeyCode doneKey; // for confrim skill setup function

    private SkillData skill; // accessor to the skill data

    private Coroutine displaySkillCoroutine; // for displaying the skill

    private void Start()
    {
        HidePanel(); // hides the panel in the beginning

        //LoadPlayer(); // load player

        //// only refresh if the skill attachment is found
        //if (skillAttachment != null)
        //    RefreshUI(); // refreshes the UI of skill select

        // check to see if the doneButton is working
        if (doneButton != null)
        {
            doneButton.onClick.RemoveAllListeners(); // stop all the ongoing UI/sound

            doneButton.onClick.AddListener(ConfrimSkillSetup); // start on the confrim setup
        }
    }

    private void Update()
    {
        // check if skillPanel is vaild and showing, if player pressed the dongKey close the UI
        if (skillPanel != null && skillPanel.activeSelf && Input.GetKeyDown(doneKey))
        {
            ConfrimSkillSetup(); // call confrimSkill Set up
        }
    }

    private void LoadPlayer()
    {
        CharacterInfo1 player = CharacterInfo1.Instance; // accessor for the playerInfo

        // if player not set up get out
        if (player == null) return;

        skillAttachment = player.GetComponent<SkillAttachment>(); // set up the skill attachment reference

        Debug.Log($"[SEUI] SkillAttachment found:{skillAttachment != null}"); // debug msg

        // display the instance ID to check if it matches other's
        if (skillAttachment != null)
            Debug.Log($"[SEUI] Attachment Instance:{skillAttachment.GetInstanceID()}"); // debug msg
    }

    // refresh current skills and the unlocked skills
    public void RefreshUI()
    {
        // check to see if skillAttackment is working
        if (skillAttachment == null)
            LoadPlayer();

        // check to see if skill attachment exist
        if (skillAttachment == null)
        {
            Debug.LogWarning($"[SEUI] SkillAttachment is null | RefreshUI failed!"); // debug msg
            return;
        }

        DisplayCurrentSkillSlots();

        DisplayUnlockedSkills();
    }

    // check for player and skill attackment before displaying UI
    public void LoadPlayerAndRefresh()
    {
        LoadPlayer(); // set up the player

        Debug.Log($"[SEUI] Attachment Instance:{skillAttachment.GetInstanceID()}"); // debug msg

        // refresh the UI if skill attachement is found
        if (skillAttachment != null)
            RefreshUI();
        else
            Debug.LogWarning("[SEUI] LoadPlayerAndRefresh failed (SkillAttachment is null!)"); // debug msg
    }

    private void DisplayCurrentSkillSlots()
    {
        // check to see if the button and skill attachment are set up
        if (skillSlotButtons == null || skillAttachment == null) return;

        // loop through the skill buttons
        for (int i = 0; i < skillSlotButtons.Length; i++)
        {
            // ignores the empty slots skip
            if (skillSlotButtons[i] == null) continue;

            int index = i; // set indext to i

            SkillData skill = skillAttachment.GetActiveSkill(index); // get the skill from the skill attachment by index

            string text = skill != null ? skill.skillDisplayName : "Empty"; // display skill name text

            TMP_Text lable = skillSlotButtons[i].GetComponentInChildren<TMP_Text>(); // get the text

            // check to see if the text exist
            if (lable != null)
                lable.text = text;

            skillSlotButtons[i].onClick.RemoveAllListeners(); // stop the function after clicking to ensure it doesn't mess up the out put

            skillSlotButtons[i].onClick.AddListener(() => PickSkillSlotEquip(index)); // calls the skill slot right away
        }
    }

    private void DisplayUnlockedSkills()
    {
        Debug.LogWarning($"[SEUI] RefreshUnlockedSklls start | attachment:{skillAttachment.GetInstanceID()}"); // debug msg

        // before refreshing unlocked skills check to see if it's missing any attachment references
        if (unlockedSkillContainer == null || skillButtonPrefab == null || skillAttachment == null)
        {
            Debug.LogWarning("[SEUI] ResreshUnlockedSkill failed! Missing refreences"); // debug msg
            return;
        }

        Debug.LogWarning($"[SEUI] Unlocked skill count:{skillAttachment.UnlockedActiveSkills.Count}"); // debug msg

        // loop through the unlocked skills and destroy them all, go backwards 
        for (int i = unlockedSkillContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(unlockedSkillContainer.GetChild(i).gameObject);
        }
         
        // loop through the skills in the unlocked active skills
        foreach (SkillData skill in skillAttachment.UnlockedActiveSkills)
        {
            // if skill doesn't exist skip
            if (skill == null) continue;

            Debug.Log($"[SEUI] Spawning unlocked skill button:{skill.skillDisplayName}"); // debug msg

            Button button = Instantiate(skillButtonPrefab, unlockedSkillContainer); // button start when prefab and skills are ready

            Debug.Log($"[SEUI] Spawned parent button:{button.transform.parent.name}"); // debug msg

            TMP_Text lable = button.GetComponentInChildren<TMP_Text>(); // get the skill name 

            // set up the skill name if it exist
            if (lable != null)
                lable.text = skill.skillDisplayName;

            //SkillData existSkill = skill; // which skill is in

            button.onClick.AddListener(() => SelectedSkill(skill)); // start on the function right away
        }
    }

    private void SelectedSkill(SkillData _skill)
    {
        skill = _skill; // set the skill equal to the _skill name passed

        Debug.Log($"[SEUI] Selected skill:{skill.skillDisplayName}"); // debug msg
    }

    private void PickSkillSlotEquip(int index)
    {
        // check to see if skill is selected
        if (skill == null || skillAttachment == null)
        {
            Debug.Log("[SEUI] No skill selected | No skillAttachment found"); // debug msg
            return;
        }

        bool setSlotSuccess = skillAttachment.EquipActiveSkillToSlot(skill, index); // try to equip the skill to slot, flag

        // check if it can be add it
        if (setSlotSuccess)
        {
            Debug.Log($"[SEUI] Equipped:{skill.skillDisplayName} to slot:{index + 1}"); // debug msg

            RefreshUI(); // update the skill lists
        }
        else
            Debug.Log("Failled to equip the selected skill!"); // debug msg

    }

    public void HilightSelectedSkillSlot(int selectedIndex)
    {
        // go through all the skill buttons
        for (int i = 0; i < skillSlotButtons.Length; i++)
        {
            // skill slot is null skip
            if (skillSlotButtons[i] == null) continue;

            Image img = skillSlotButtons[i].GetComponent<Image>(); // get the image of the sprite

            // change the images color if found
            if (img != null) 
                img.color = (i == selectedIndex) ? Color.red : Color.white;
        }
    }

    public void ConfrimSkillSetup()
    {
        Debug.Log("[SEUI] Skill setup completed!"); // debug msg

        HidePanel(); // finished set up hide the panel
    }

    public void ShowPanelTemporary(float duration)
    {
        // check to make sure panle is found
        if (skillPanel == null) return;

        // if the display is found stop it
        if (displaySkillCoroutine != null)
            StopCoroutine(displaySkillCoroutine);

        displaySkillCoroutine = StartCoroutine(ShowAndHide(duration)); // start the duration
    }

    private IEnumerator ShowAndHide(float duration)
    {
        skillPanel.SetActive(true); // display the skill UI panel

        yield return new WaitForSeconds(duration); // wait for 1.5 seconds

        skillPanel.SetActive(false); // disable the skill UI panel
    }
    public void ShowPanel()
    {
        // if the skill panel is found turn on for now
        if (skillPanel != null)
        {
            skillPanel.SetActive(true); // turn on the button from te scene

            Debug.Log($"[SEUI] ShowPanel being called | activeSelf:{skillPanel.activeSelf}"); // debug msg
        }
    }

    public void HidePanel()
    {
        // if the skill panel is found turn off for now
        if (skillPanel != null)
            skillPanel.SetActive(false); // turn of the button from te scene
    }
}
