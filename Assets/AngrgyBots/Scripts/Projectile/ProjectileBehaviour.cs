using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour 
{
	[Header("Movement")]
	public float speed = 50f;
	
	[Header("Life Settings")]
	public float lifeTime = 0.5f;

	[Header("Damage")]
	public int damageToEnemy = 10;

	[Header("Hit Object AVFX")]
	public GameObject hitEnemyParticles;
	public GameObject hitWallParticles;

    private Rigidbody _rigidbody;
    private Vector3 movement = Vector3.zero;

    private void Start()
	{
        _rigidbody = GetComponent<Rigidbody>();
		Invoke("RemoveProjectile", lifeTime);
	}

    private void OnTriggerEnter(Collider theCollider)
	{	
		if(theCollider.CompareTag("Enemy"))
		{
			if(damageToEnemy > 0)
			{
                //theCollider.GetComponent<EnemyHealth>().RemoveHealth(damageToEnemy);
                theCollider.GetComponent<Health>().OnDamage(damageToEnemy, -this.transform.forward);
            }

			if(hitEnemyParticles != null) { Instantiate(hitEnemyParticles, transform.position, transform.rotation); }
			
			RemoveProjectile();

		} 
		else if (theCollider.CompareTag("Environment"))
		{
			if (hitWallParticles != null) { Instantiate(hitWallParticles, transform.position, transform.rotation); }
			RemoveProjectile();
		}
	}

    private void Update()
	{
		movement = transform.forward * speed * Time.deltaTime;
        _rigidbody.MovePosition(transform.position + movement);
	}

	private void RemoveProjectile()
	{
		Destroy(gameObject);
	}

}
