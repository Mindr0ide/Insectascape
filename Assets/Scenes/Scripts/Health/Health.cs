using System.Collections;
using Scenes.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    private Animator anim;

    private void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0 , maxHealth);
        if (currentHealth > 0)
        {
            //player hurt
            anim.SetTrigger("hurt");
            StartCoroutine(HurtDelay());
        }
        else
        {
            //dead
            anim.SetTrigger("die");
            // CALL Respawn here
            StartCoroutine(RespawnAfterDelay()); 
            //FindObjectOfType<PlayerRespawn>().Respawn();
        }
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.E))
        //    TakeDamage(1);
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, maxHealth);
    }

    public void Respawn()
    {
        AddHealth(maxHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
    }
    private IEnumerator RespawnAfterDelay()
    {
        GetComponent<PlayerMovement>().freeze();
        yield return new WaitForSeconds(2.5f);
        FindObjectOfType<PlayerRespawn>().Respawn();
        GetComponent<PlayerMovement>().unfreeze();
    }
    private IEnumerator HurtDelay()
    {
        GetComponent<PlayerMovement>().freeze();
        yield return new WaitForSeconds(1.25f);
        GetComponent<PlayerMovement>().unfreeze();
    }
}
