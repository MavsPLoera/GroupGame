using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class Ranged_Attack_Controller : MonoBehaviour
{
    [Header("Stats.")]
    public float projectileDamage;
    public float projectileSpeed;
    public float attackCooldownTime;

    [Header("Ranged Attack Controller Misc.")]
    public GameObject projectile;

    private bool _onCooldown = false;
    private readonly bool _debug = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"{gameObject.transform.parent.name} Collision Enter Player");
            if(!_onCooldown)
            {
                Attack(collision);
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"{gameObject.transform.parent.name} Collision Stay Player");
            if(!_onCooldown)
            {
                Attack(collision);
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private void Attack(Collider2D collision)
    {
        // ** TODO: corr. SFX and particle systems **

        // Get vector in the direction of the collision. Add offset.
        Vector3 spawnDirection = (collision.transform.position - gameObject.transform.parent.position);
        Vector3 spawnPosition = gameObject.transform.parent.position + spawnDirection * .2f;

        // Spawn projectile and set its stats.
        GameObject spawnedProjectile = Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
        spawnedProjectile.transform.parent = transform;
        spawnedProjectile.GetComponent<Projectile_Controller>().speed = projectileSpeed;
        spawnedProjectile.GetComponent<Projectile_Controller>().damage = projectileDamage;
        spawnedProjectile.GetComponent<Projectile_Controller>().SetTarget(spawnDirection);
    }

    private IEnumerator AttackCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(attackCooldownTime);
        _onCooldown = false;
    }
}
