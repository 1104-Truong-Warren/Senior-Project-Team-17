// Warren
// The purpose of this script is to manage enemy health bars using the same UI setup as CharacterInfo.
// It updates the health bar fill amount based on enemy's current health.

// Resource: https://docs.unity3d.com/ScriptReference/UI.Image-fillAmount.html - For health bar fill control
// Resource: https://docs.unity3d.com/ScriptReference/Transform.LookAt.html - For making bar face camera

using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Visual")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBarBackground; 
    
    [Header("Settings")]
    [SerializeField] private bool alwaysFaceCamera = true;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    
    private EnemyInfo enemyInfo;
    private Transform mainCamera;
    private int maxHealth;
    
    void Start()
    {
        // Get enemy info from parent
        enemyInfo = GetComponentInParent<EnemyInfo>();
        
        // Get camera reference
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
        
        // If hpBar not assigned, try to find it
        if (hpBar == null)
        {
            hpBar = GetComponentInChildren<Image>();
        }
        
        // Store max health
        if (enemyInfo != null)
        {
            maxHealth = enemyInfo.health;
        }
        
        // Set position offset (X, Y, Z)
        transform.localPosition = offset;
    }
    
    void Update()
    {
        if (enemyInfo == null || hpBar == null) 
        {
            return;
        }
        
        // Update health bar
        float hpPercentage = (float)enemyInfo.health / maxHealth;
        hpBar.fillAmount = hpPercentage;
        
        // Change color based on health 
        if (hpPercentage > 0.6f)
        {
            hpBar.color = Color.green;
        }
        else if (hpPercentage > 0.3f)
        {
            hpBar.color = Color.yellow;
        }
        else
        {
            hpBar.color = Color.red;
        }
        
        // Make health bar face camera
        if (alwaysFaceCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.forward);
        }
    }
}