using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [SerializeField] private float movementSpeed;
    [SerializeField] private float damageToBuildings;
    
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Health _health;
    private int _collisionDebounce;

    void Start() {
        _health = GetComponent<Health>();
        
        GameObject visual = transform.Find("Visual").gameObject;
        _animator = visual.GetComponent<Animator>();
        _spriteRenderer = visual.GetComponent<SpriteRenderer>();
    }

    void Update() {
        UpdateAttack();
        UpdateMovement();
        UpdateHealth();
    }

    private void UpdateAttack() {
        _animator.SetBool("Attacking", _collisionDebounce > 0);
        _collisionDebounce--;
    }

    private void UpdateMovement() {
        GameObject nearestBuildingMesh = GetNearestBuildingMesh();

        // Update animation
        if (!nearestBuildingMesh) {
            _animator.SetBool("Moving", false);
            return;
        }
        
        _animator.SetBool("Moving", true);
        
        // Move
        Vector3 direction = (nearestBuildingMesh.transform.position - transform.position).normalized;
        direction = new(direction.x, 0, direction.z);
        
        transform.Translate(movementSpeed * Time.deltaTime * direction);
        
        // Flip sprite
        _spriteRenderer.flipX = direction.x >= 0; 
    }

    private void UpdateHealth() {
        if (GameManager.Instance.CurrentState == GameManager.State.Night) return;
        
        float damage = _health.MaximumHealth / 3f - _health.MaximumHealth * 0.1f;
        _health.TakeDamage(damage * Time.deltaTime);
    }

    private GameObject GetNearestBuildingMesh(bool excludeWalls = true) {
        List<GameObject> buildings = GameObject.FindGameObjectsWithTag("Building").ToList();

        if (excludeWalls) buildings = buildings.Where(e => !e.GetComponent<Building>().BuildingSO.isWall).ToList();
        
        List<GameObject> meshes = buildings.Select(e => e.transform.Find("Mesh").gameObject).ToList();

        GameObject nearestBuildingMesh = meshes
            .OrderBy(e => (transform.position - e.transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (!nearestBuildingMesh && excludeWalls) return GetNearestBuildingMesh(false);

        return nearestBuildingMesh;
    }

    void OnCollisionStay(Collision other) {
        if (!other.collider.CompareTag("Building")) return;
        
        Health buildingHealth = other.gameObject.GetComponent<Health>();
        buildingHealth?.TakeDamage(damageToBuildings * Time.deltaTime);

        _collisionDebounce = 2;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Bullet")) return;
        
        _health.TakeDamage(other.GetComponent<BulletController>().damage);
        Destroy(other.gameObject);
    }

}
